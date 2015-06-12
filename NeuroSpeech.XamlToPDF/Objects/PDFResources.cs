using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace NeuroSpeech.XamlToPDF.Objects
{
	public class PDFResources : PDFObject
	{

		public bool HasText { get; set; }

		public bool HasImage { get; set; }

		#region protected override void  WriteProperties(System.IO.TextWriter writer)
		protected override void WriteProperties(System.IO.TextWriter writer)
		{
			string ps = "/PDF ";
			if (HasText)
				ps += "/Text";
			if (HasImage)
				ps += "/Image";
			writer.WriteLine("/ProcSet [{0}]",ps);
			base.WriteProperties(writer);
		}
		#endregion


		#region public override string  Type
		public override string Type
		{
			get
			{
				return null;
			}
		}
		#endregion

		public PDFFonts Font {
			get {
				return GetValue<PDFFonts>("Font", new PDFFonts());
			}
		}

		public PDFXObjects XObject {
			get {
				return GetValue<PDFXObjects>("XObject", new PDFXObjects());
			}
		}

	}

}