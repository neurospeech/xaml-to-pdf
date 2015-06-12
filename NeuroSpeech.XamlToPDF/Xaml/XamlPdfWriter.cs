using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using NeuroSpeech.XamlToPDF.Objects;
using System.Windows.Media;
using System.Windows;
using System.Diagnostics;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.ComponentModel.Composition;
using NeuroSpeech.XamlToPDF.Writer;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Windows.Threading;

namespace NeuroSpeech.XamlToPDF.Xaml
{

	public class XamlPdfWriter
	{

		public XamlPdfWriter()
		{

		}
		
		public void Write(string xamlFlowDocument, System.IO.Stream writer)
		{
			Exception error = null;

			Thread t = new Thread(x =>
			{
				try
				{
					using (PDFDocument = new PDFDocument())
					{

						using (fdp = new FlowDocumentPackage(xamlFlowDocument))
						{

							FixedDocument fd = fdp.Document;

							foreach (PageContent page in fd.Pages)
							{
								page.UpdateLayout();
								FixedPage fp = page.GetPageRoot(true);
                                fp.UpdateLayout();
								CreatePage(fp);

							}

							PDFDocument.Write(writer);

                            System.GC.Collect();
                            System.GC.WaitForPendingFinalizers();

                            //foreach (var item in fd.Pages.Select(p => p.Child))
                            //{
                            //    item.Children.Clear();
                            //    item.UpdateLayout();
                            //}

                            //Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.SystemIdle, new DispatcherOperationCallback(delegate { return null; }), null);
                        }
					}

				}
				catch (Exception ex)
				{
					error = ex;
				}
			});

			t.SetApartmentState(ApartmentState.STA);
			t.Start();
			t.Join();

            if (error != null)
                throw new InvalidOperationException("Exection Error", error);

		}

		private FlowDocumentPackage fdp;

		public PDFDocument PDFDocument { get; private set; }

		public PDFPage Page { get; private set; }

		public FixedPage FixedPage { get; set; }


		#region private void CreatePage(FixedPage fp)
		private void CreatePage(FixedPage fp)
		{
			FixedPage = fp;
			PDFPage page = PDFDocument.Catalog.Pages.Create<PDFPage>();
			page.MediaBox.Width = (int)fp.Width;
			page.MediaBox.Height = (int)fp.Height;
			CreateVisual(page, fp);

		}
		#endregion

		#region private void CreateVisual(PDFPage page,FixedPage fp)
		private void CreateVisual(PDFPage page, Visual fp)
		{
			// get converter...
			this.Page = page;

			WriteVisual(fp);

			int count = VisualTreeHelper.GetChildrenCount(fp);
			for (int i = 0; i < count; i++)
			{
				Visual v = VisualTreeHelper.GetChild(fp, i) as Visual;
				if (v == null)
					continue;
				CreateVisual(page, v);
			}

            FinishVisual(fp);

		}

        private void FinishVisual(Visual fp)
        {
            Canvas c = fp as Canvas;
            if (c != null) {
                if (c.Clip != null) {
                    Page.ContentStream.WriteLine("Q");
                }
            }
        }
		#endregion

		private Visual lastVisual = null;

		#region internal object TransformY(LineSegment visual,double p)
		internal Point TransformPoint(Visual visual, Point p)
		{
			p = Transform(visual, p);
			p.Y = Page.MediaBox.Height - p.Y;
			return p;
		}

		#region private Point Transform(Visual visual,Point p)
		private Point Transform(Visual visual, Point p)
		{
			if (visual == FixedPage)
				return p;

			Point r = p;

			UIElement e = visual as UIElement;
			if (e != null)
			{
				Transform t = e.RenderTransform;
				Matrix m = t.Value;

				if (m != null && !m.IsIdentity)
				{
					r = t.Transform(p);
				}
			}
			return Transform((Visual)VisualTreeHelper.GetParent(visual), r);
		}
		internal Point TransformPoint(Point p)
		{
			return TransformPoint(lastVisual, p);
		}
		#endregion

		#endregion

		#region private void WriteVisual(Visual fp)
		private void WriteVisual(Visual fp)
		{
			lastVisual = fp;



			FixedPage f = fp as FixedPage;
			if (f != null)
				return;

			Canvas canvas = fp as Canvas;
            if (canvas != null)
            {
                // clip...
                Geometry g = canvas.Clip;
                if (g != null) {

                    Page.ContentStream.WriteLine("q");
                    PathGeometry pg = PathGeometry.CreateFromGeometry(g);
                    foreach (var item in pg.Figures)
                    {
                        Point p = TransformPoint(item.StartPoint);
                        Page.ContentStream.WriteLine("{0} {1} m",p.X,p.Y);
                        foreach (var segment in item.Segments)
                        {
                            WriteSegment(segment);
                        }
                        Page.ContentStream.WriteLine("h");
                        Page.ContentStream.WriteLine("W");
                        Page.ContentStream.WriteLine("n");
                    }
                }
                return;
            }

			Path path = fp as Path;
			if (path != null)
			{
				Write(path);
				return;
			}

			Glyphs glyphs = fp as Glyphs;
			if (glyphs != null) {
				Write(glyphs);
				return;
			}

			throw new NotImplementedException(fp.GetType() + " not supported.");
		}


		#endregion

		#region private void Write(Glyphs glyphs)
		private void Write(Glyphs visual)
		{
			var gr = visual.ToGlyphRun();


			Point p = TransformPoint(visual, new Point(visual.OriginX, visual.OriginY));



			//PDFFont font = page.Document.CreateObject<PDFFont>();
			//page.Resources.Font["F1"] = font;
			//font.Subtype = "Type1";
			//font.BaseFont = "Helvetica";
			//font.Encoding = "MacRomanEncoding";
			Page.Resources.HasText = true;


			Page.ContentStream.WriteLine("BT");


			Page.ContentStream.WriteLine("/{0} {1} Tf", GetFont(Page.Resources, gr.GlyphTypeface), gr.FontRenderingEmSize);
			Page.ContentStream.WriteLine("{0} {1} Td", p.X, p.Y);

			if (visual.Fill != null)
			{
				WriteFill(visual.Fill);
			}

			Page.ContentStream.WriteLine("({0}) Tj", Encode(visual.UnicodeString));
			Page.ContentStream.WriteLine("ET");

		}

		#region private object[] Encode(string p)
		private string Encode(string p)
		{
			p = p.Replace("\\","\\\\");
			p = p.Replace("(", "\\(");
			p = p.Replace(")", "\\)");
			p = p.Replace("\n", "\\n");
			p = p.Replace("\r", "\\r");
			p = p.Replace("\t", "\\t");
			p = p.Replace("\b", "\\b");
			p = p.Replace("\f", "\\f");
			return p;
		}
		#endregion


		Dictionary<string, string> fonts = new Dictionary<string, string>();

		#region internal object GetFont(System.Windows.Media.GlyphTypeface glyphTypeface)
		internal string GetFont(PDFResources resources, System.Windows.Media.GlyphTypeface typeFace)
		{
			string familyName = typeFace.FamilyNames.Values.FirstOrDefault();

			string faceName = familyName;

			var fn = typeFace.FaceNames.Where(x => x.Key.LCID == System.Globalization.CultureInfo.CurrentCulture.LCID).Select(x=>x.Value).FirstOrDefault();
			if (!string.IsNullOrWhiteSpace(fn))
			{
				if(fn!="Regular")
					faceName += "," + fn;
			}

			string key = null;
			if (!fonts.TryGetValue(faceName, out key))
			{
				key = "R" + resources.ID + "F" + fonts.Count;
				PDFFont pf = this.PDFDocument.CreateObject<PDFFont>();
				pf.BaseFont = faceName;
				pf.Subtype = "Type1";
				pf.Encoding = "WinAnsiEncoding";
				resources.Font[key] = pf;

				//var pd = PDFDocument.CreateObject<PDFFontDescriptor>();

				//pf.FontDescriptor = pd;

				//pd.FontName = familyName;
				//pd.FontFamily = familyName;
				//pd.FontWeight = typeFace.Weight.ToOpenTypeWeight();


				//pd.XHeight = typeFace.XHeight;
				//pd.CapHeight = typeFace.CapsHeight;
				//pd.StemV = typeFace.StrikethroughThickness;

				//pd.Flags = PDFFontFlags.None;

				//if (typeFace.Weight == FontWeights.Bold)
				//{
				//    pd.Flags |= PDFFontFlags.ForceBold;
				//}

				//if (typeFace.Symbol)
				//{
				//    pd.Flags |= PDFFontFlags.Symbolic;
				//}
				//else
				//{
				//    pd.Flags |= PDFFontFlags.Nonsymbolic;
				//}
				//pd.Ascent = typeFace.AdvanceHeights.Select(x => x.Value).Max() - typeFace.Baseline;
				//pd.Descent = -(typeFace.DistancesFromHorizontalBaselineToBlackBoxBottom.Select(x => x.Value).Max());

				//pd.FontBBox = new PDFRect();
				//pd.FontBBox.Width = (int)typeFace.AdvanceWidths.Select(x => x.Value).Sum();
				//pd.FontBBox.Height = (int)typeFace.AdvanceHeights.Select(x => x.Value).Sum();

				fonts[faceName] = key;
			}
			return key;
		}
		#endregion

		#endregion

		#region private void Write(Path path)
		private void Write(Path visual)
		{

            PathGeometry pg = PathGeometry.CreateFromGeometry(visual.Data);

            Uri url = FixedPage.GetNavigateUri(visual as UIElement);

            if (url != null) {

                var pa = Page.Annotations;
                var bounds = pg.Bounds;

                Point topLeft = TransformPoint(visual, bounds.TopLeft);
                Point bottomRight = TransformPoint(visual, bounds.BottomRight);

                pa.AddLink(url, new PDFRect { Left = (int)topLeft.X , Top = (int)topLeft.Y, Width = (int)bottomRight.X, Height = (int)bottomRight.Y });
                return;                
            }


			if (visual.Fill is ImageBrush) {
				WriteImage(visual,visual.Fill as ImageBrush);
				return;
			}

			if (visual.Stroke != null)
			{
				WriteStroke(visual.Stroke);
			}

			if (visual.Fill != null)
			{
				WriteFill(visual.Fill);
			}


			foreach (var item in pg.Figures)
			{
				var start = TransformPoint(visual, item.StartPoint);

				Page.ContentStream.WriteLine("{0} {1} m", start.X, start.Y);

				foreach (var s in item.Segments)
				{
					WriteSegment(s);
				}

				if (item.IsClosed)
				{
					Page.ContentStream.WriteLine("{0} {1} l", start.X, start.Y);
				}

				string op = "S";
				if (visual.Stroke != null && visual.Fill != null)
				{
					op = "B";
				}
				else {
					if (visual.Stroke != null)
						op = "S";
					if (visual.Fill != null)
						op = "f";
				}

				Page.ContentStream.WriteLine(op);
				

			}



		}

		#region private void WriteImage(Path visual,ImageBrush imageBrush)
		private void WriteImage(Path visual, ImageBrush brush)
		{

			

			System.Windows.Media.Imaging.BitmapFrame frame = brush.ImageSource as System.Windows.Media.Imaging.BitmapFrame;


			PDFImage img = PDFDocument.CreateObject<PDFImage>();

			string key ="R" + Page.Resources.ID + "I" + img.ID;

			Page.Resources.XObject[key] = img;

			img.ColorSpace = "DeviceRGB";
			img.BitsPerComponent = 8;

			System.Windows.Media.Imaging.JpegBitmapEncoder enc = new JpegBitmapEncoder();


			/*TransformedBitmap tb = new TransformedBitmap(frame, new ScaleTransform { 
				ScaleX = brush.Viewport.Width / frame.Width ,
				ScaleY = brush.Viewport.Height / frame.Height
			});*/

			var tb = frame;

			img.Width = tb.PixelWidth;
			img.Height = tb.PixelHeight;

            enc.Frames.Add(BitmapFrame.Create(tb));
            enc.QualityLevel = 100;
            enc.Save(img.Stream);

			Page.ContentStream.WriteLine("q");

			PathGeometry pg = PathGeometry.CreateFromGeometry(visual.Data);
			Point p = pg.Figures.First().StartPoint;
			p = TransformPoint(visual, p);



			Page.ContentStream.WriteLine("{0} 0 0 {1} {2} {3} cm", brush.Viewport.Width, brush.Viewport.Height, p.X, p.Y - brush.Viewport.Height);

			Page.ContentStream.WriteLine("/" + key + " Do");
			Page.ContentStream.WriteLine("Q");
		}
		#endregion


		#endregion

		#region private void WriteSegment(PathSegment s)
		private void WriteSegment(PathSegment s)
		{
			LineSegment ls = s as LineSegment;
			if (ls != null) {
				Write(ls);
				return;
			}

			PolyLineSegment ps = s as PolyLineSegment;
			if (ps != null) {
				Write(ps);
				return;
			}

			throw new NotImplementedException(s.GetType() + " not supported.");
		}

		#region private void Write(PolyLineSegment ps)
		private void Write(PolyLineSegment visual)
		{
			foreach (var item in visual.Points)
			{
				Point p = TransformPoint(item);
				Page.ContentStream.WriteLine("{0} {1} l", p.X, p.Y);
			}
		}
		#endregion


		#region private void Write(LineSegment ls)
		private void Write(LineSegment visual)
		{
			Point p = TransformPoint(visual.Point);
			Page.ContentStream.WriteLine("{0} {1} l", p.X, p.Y);
		}
		#endregion

		#endregion

		#region private void WriteFill(Brush brush)
		private void WriteFill(Brush brush)
		{
			SolidColorBrush sb = brush as SolidColorBrush;
			if (sb != null) {
				WriteFillBrush(sb);
				return;
			}

			ImageBrush ib = brush as ImageBrush;
			if (ib != null) {
				WriteFillBrush(ib);
				return;
			}

			throw new NotImplementedException(brush.GetType() + " not supported.");
		}

		#region private void WriteFillBrush(ImageBrush ib)
		private void WriteFillBrush(ImageBrush ib)
		{
			Trace.WriteLine("Image Brush..");
		}
		#endregion

		#endregion

		#region private void WriteFillBrush(SolidColorBrush sb)
		private void WriteFillBrush(SolidColorBrush value)
		{
			Page.ContentStream.WriteLine("{0} {1} {2} rg", ((double)value.Color.R / 255.0), ((double)value.Color.G / 255.0), ((double)value.Color.B / 255.0));
		}
		#endregion



		#region private void WriteStroke(Brush brush)
		private void WriteStroke(Brush brush)
		{
			SolidColorBrush sb = brush as SolidColorBrush;
			if (sb != null) {
				WriteStrokeBrush(sb);
				return;
			}
			throw new NotImplementedException(brush.GetType() + " not supported.");
		}

		#region private void WriteStrokeBrush(SolidColorBrush sb)
		private void WriteStrokeBrush(SolidColorBrush value)
		{
			Page.ContentStream.WriteLine("{0} {1} {2} RG", ((double)value.Color.R / 255.0), ((double)value.Color.G / 255.0), ((double)value.Color.B / 255.0));
		}
		#endregion

		#endregion


	}


}
