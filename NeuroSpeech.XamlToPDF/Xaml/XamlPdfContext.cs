using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using NeuroSpeech.XamlToPDF.Objects;
using System.Windows.Media;
using System.Windows;
using System.Diagnostics;
using System.IO;
using System.IO.Packaging;
using System.Windows.Xps.Packaging;
using System.Windows.Xps.Serialization;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.ComponentModel.Composition;
using NeuroSpeech.XamlToPDF.Writer;
using System.Windows.Controls;

namespace NeuroSpeech.XamlToPDF.Xaml
{
	public class XamlPdfContext
	{
		public PDFDocument PDFDocument { get; set; }
		public FlowDocument FlowDocument { get; set; }
		public XpsDocument XpsDocument { get; set; }
		public FixedDocument FixedDocument { get; set; }

		public FixedPage FixedPage { get; set; }

		public PDFPage PDFPage { get; set; }

		public Package Package { get; set; }

		public byte[] GetResource(Uri partUri)
		{
			var p = Package.GetPart(partUri);
			MemoryStream ms = new MemoryStream();
			using (Stream s = p.GetStream())
			{
				s.CopyTo(ms);
			}

			return ms.ToArray();
		}

		public System.Windows.Media.FontFamily GetFont(Uri partUri) {
			byte[] data = GetResource(partUri);

			XpsFont xf = null;

			foreach (var item in XpsDocument.FixedDocumentSequenceReader.FixedDocuments)
			{
				foreach (var fp in item.FixedPages)
				{
					foreach (var f in fp.Fonts)
					{
						if (f.Uri == partUri)
							xf = f;
					}
				}
			}



			MemoryStream reader = new MemoryStream(data);
			MemoryStream writer = new MemoryStream();

			if (xf.IsObfuscated)
			{
				ObfuscationSwitcher(partUri.ToString(), reader, writer);
			}
			else 
			{
				reader.CopyTo(writer);
			}

			string tmp = Path.GetTempFileName();

			File.WriteAllBytes(tmp,writer.ToArray());

			return new FontFamily("file:///" + tmp);
		}

		public static void ObfuscationSwitcher(string uri, MemoryStream font, Stream streamOut)
		{
			if (font == null || streamOut == null)
				throw new ArgumentNullException();
			int count1 = 4096;
			int length1 = 16;
			int count2 = 32;
			string originalString = uri;
			int startIndex = originalString.LastIndexOf('/') + 1;
			int length2 = originalString.LastIndexOf('.') - startIndex;
			string str = new Guid(originalString.Substring(startIndex, length2)).ToString("N");
			byte[] numArray = new byte[length1];
			for (int index = 0; index < numArray.Length; ++index)
				numArray[index] = Convert.ToByte(str.Substring(index * 2, 2), 16);
			byte[] buffer1 = new byte[count2];
			font.Read(buffer1, 0, count2);
			for (int index1 = 0; index1 < count2; ++index1)
			{
				int index2 = numArray.Length - index1 % numArray.Length - 1;
				buffer1[index1] ^= numArray[index2];
			}
			streamOut.Write(buffer1, 0, count2);
			byte[] buffer2 = new byte[count1];
			int count3;
			while ((count3 = font.Read(buffer2, 0, count1)) > 0)
				streamOut.Write(buffer2, 0, count3);
			streamOut.Position = 0L;
		}

		private Visual lastVisual = null;

		#region internal object TransformY(LineSegment visual,double p)
		internal Point TransformPoint(Visual visual, Point p)
		{
			lastVisual = visual;
			p = Transform(visual,p);
			p.Y = PDFPage.MediaBox.Height - p.Y;
			return p;
		}

		#region private Point Transform(Visual visual,Point p)
		private Point Transform(Visual visual, Point p)
		{
			if (visual == FixedPage)
				return p;

			Point r = p;

			UIElement e = visual as UIElement;
			if (e!=null)
			{
				Transform t = e.RenderTransform;
				Matrix m = t.Value;

				if (m != null && !m.IsIdentity)
				{
					r = t.Transform(p);
				}
			}
			return Transform((Visual) VisualTreeHelper.GetParent(visual),r) ;
		}
		#endregion

		#endregion

		#region internal object TransformY(double p)
		internal Point TransformPoint(Point p)
		{
			return TransformPoint(lastVisual, p);
		}
		#endregion


		Dictionary<GlyphTypeface, string> fonts = new Dictionary<GlyphTypeface, string>();

		#region internal object GetFont(System.Windows.Media.GlyphTypeface glyphTypeface)
		internal string GetFont(PDFResources resources, System.Windows.Media.GlyphTypeface typeFace)
		{
			string familyName = typeFace.FamilyNames.Values.FirstOrDefault();

			string key = null;
			if (!fonts.TryGetValue(typeFace, out key))
			{
				key = "R" + resources.ID + "F" + fonts.Count;
				PDFFont pf = PDFDocument.CreateObject<PDFFont>();
				pf.BaseFont = familyName;
				pf.Subtype = "Type1";
				pf.Encoding = "MacRomanEncoding";
				resources.Font[key] = pf;

				var pd = PDFDocument.CreateObject<PDFFontDescriptor>();

				pf.FontDescriptor = pd;

				pd.FontName = familyName;
				pd.FontFamily = familyName;
				pd.FontWeight = typeFace.Weight.ToOpenTypeWeight();


				pd.XHeight = typeFace.XHeight;
				pd.CapHeight = typeFace.CapsHeight;
				pd.StemV = typeFace.StrikethroughThickness;

				pd.Flags = PDFFontFlags.None;

				if (typeFace.Weight == FontWeights.Bold)
				{
					pd.Flags |= PDFFontFlags.ForceBold;
				}

				if (typeFace.Symbol)
				{
					pd.Flags |= PDFFontFlags.Symbolic;
				}
				else {
					pd.Flags |= PDFFontFlags.Nonsymbolic;
				}
				pd.Ascent = typeFace.AdvanceHeights.Select(x=>x.Value).Max() - typeFace.Baseline;
				pd.Descent = - ( typeFace.DistancesFromHorizontalBaselineToBlackBoxBottom.Select(x => x.Value).Max() );

				pd.FontBBox = new PDFRect();
				pd.FontBBox.Width = (int)typeFace.AdvanceWidths.Select(x => x.Value).Sum();
				pd.FontBBox.Height = (int)typeFace.AdvanceHeights.Select(x => x.Value).Sum();

				fonts[typeFace] = key;
			}
			return key;
		}
		#endregion

	}
}
