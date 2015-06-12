using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuroSpeech.XamlToPDF.Objects
{
	public class PDFFont : PDFObject
	{
		public string Subtype {
			get {
				return GetValue<string>("Subtype");
			}
			set {
				SetValue<string>("Subtype", value);
			}
		}

		public string BaseFont
		{
			get
			{
				return GetValue<string>("BaseFont");
			}
			set
			{
				SetValue<string>("BaseFont", value);
			}
		}

		public string Encoding {
			get {
				return GetValue<string>("Encoding");
			}
			set {
				SetValue<string>("Encoding", value);
			}
		}

		public PDFFontDescriptor FontDescriptor {
			get {
				return GetValue<PDFFontDescriptor>("FontDescriptor");
			}
			set {
				SetValue<PDFFontDescriptor>("FontDescriptor", value);
			}
		}

	}

	public class PDFFonts : PDFDictionary<PDFFont>
	{
	}


}

