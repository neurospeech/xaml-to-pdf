using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NeuroSpeech.XamlToPDF.Objects
{
	public class PDFPage : PDFChildObject
	{


		#region protected internal override void  Initialize()
		protected internal override void Initialize()
		{
			MediaBox = new PDFRect { Width = 612, Height=792 };
			SetValue<PDFResources>("Resources", Document.Resources);
			SetValue<PDFContents>("Contents", Document.CreateObject<PDFContents>());
		}
		#endregion


		public PDFRect MediaBox {
			get {
				return GetValue<PDFRect>("MediaBox");
			}
			set {
				SetValue<PDFRect>("MediaBox", value);
			}
		}

		public PDFResources Resources {
			get {
				return GetValue<PDFResources>("Resources");
			}
		}

		public PDFContents Contents {
			get {
				return GetValue<PDFContents>("Contents");
			}
		}

        public PDFAnnotations Annotations {
            get {
                var pa = GetValue<PDFAnnotations>("Annots");
                if (pa == null) {
                    pa = Document.CreateObject<PDFAnnotations>();
                    SetValue<PDFAnnotations>("Annots", pa);
                }
                return pa;
            }
        }

		public TextWriter ContentStream {
			get {
				return Contents.ContentWriter;
			}
		}

		//public void DrawString(int x, int y, string text)
		//{
		//    PDFFont font = Document.CreateObject<PDFFont>();
		//    Resources.Font["F1"] = font;
		//    font.Subtype = "Type1";
		//    font.BaseFont = "Helvetica";
		//    font.Encoding = "MacRomanEncoding";
		//    Resources.HasText = true;

		//    ContentStream.WriteLine("BT");
		//    ContentStream.WriteLine("/F1 12 Tf");
		//    ContentStream.WriteLine("{0} {1} Td", x, y);
		//    ContentStream.WriteLine("({0}) Tj",text);
		//    ContentStream.WriteLine("ET");
		//}

		private Stack<int> YCutOff = new Stack<int>();

		public bool IsTransformOn {
			get {
				return YCutOff.Count > 0;
			}
		}

		public int ViewPortHeight {
			get {
				return YCutOff.Count > 0 ? YCutOff.Peek() : MediaBox.Height;
			}
		}

		public void StartTransform(int yCutOff) {
			//if (IsTransformOn) {
			//    ContentStream.WriteLine("Q");
			//}
			ContentStream.WriteLine("q");
			if (!IsTransformOn)
			{
				YCutOff.Push(ViewPortHeight - yCutOff);
			}
			else {
				YCutOff.Push(yCutOff);
			}
		}

		public void EndTransform() {
			//if (IsTransformOn)
			ContentStream.WriteLine("Q");
			YCutOff.Pop();
		}


	}

	public class PDFPages : PDFObjectCollection<PDFPage>
	{
	}

}