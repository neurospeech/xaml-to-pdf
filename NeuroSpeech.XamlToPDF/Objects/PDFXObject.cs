using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NeuroSpeech.XamlToPDF.Objects
{
	public class PDFXObject : PDFObject
	{
		public string Subtype {
			get {
				return GetValue<string>("Subtype");
			}
			set {
				SetValue<string>("Subtype", value);
			}
		}
	}

	public class PDFXObjects : PDFDictionary<PDFXObject>
	{
	}



}
