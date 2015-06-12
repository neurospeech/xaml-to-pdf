using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuroSpeech.XamlToPDF.Objects
{
	public class PDFFontDescriptor : PDFObject
	{


		#region protected internal override void  Initialize()
		protected internal override void Initialize()
		{
			base.Initialize();



		}
		#endregion


		public int FontWeight {
			get {
				return GetValue<int>("FontWeight");
			}
			set {
				SetValue<int>("FontWeight", value);
			}
		}

		public string FontName {
			get {
				return GetValue<string>("FontName");
			}
			set {
				SetValue<string>("FontName",value);
			}
		}

		public string FontFamily {
			get {
				return GetValue<string>("FontFamily");
			}
			set
			{
				SetValue<string>("FontFamily", value);
			}
		}

		public string FontStretch {
			get {
				return GetValue<string>("FontStretch");
			}
			set {
				SetValue<string>("FontStretch", value);
			}
		}

		public PDFFontFlags Flags {
			get {
				return GetValue<PDFFontFlags>("Flags");
			}
			set {
				SetValue("Flags", value);
			}
		}

		public PDFRect FontBBox {
			get {
				return GetValue<PDFRect>("FontBBox");
			}
			set {
				SetValue<PDFRect>("FontBBox", value);
			}
		}

		public int ItalicAngle {
			get {
				return GetValue<int>("ItalicAngle");
			}
			set {
				SetValue<int>("ItalicAngle", value);
			}
		}

		public double Ascent {
			get {
				return GetValue<double>("Ascent");
			}
			set {
				SetValue("Ascent", value);
			}
		}

		public double Descent {
			get {
				return GetValue<double>("Descent");
			}
			set {
				SetValue("Descent", value);
			}
		}

		public double CapHeight {
			get {
				return GetValue<double>("CapHeight");
			}
			set
			{
				SetValue<double>("CapHeight", value);
			}
		}

		public double XHeight {
			get {
				return GetValue<double>("XHeight");
			}
			set {
				SetValue<double>("XHeight", value);
			}
		}

		public double StemV {
			get {
				return GetValue<double>("StemV");
			}
			set {
				SetValue("StemV", value);
			}
		}


		public int StemH {
			get {
				return GetValue<int>("StemH");
			}
			set {
				SetValue<int>("StemH", value);
			}
		}


	}
}
