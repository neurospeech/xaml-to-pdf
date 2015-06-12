using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuroSpeech.XamlToPDF.Objects
{
	public class PDFViewerPreferences : PDFObject
	{

		#region public override string  Type
		public override string Type
		{
			get
			{
				return "";
			}
		}
		#endregion


		public bool HideToolbar {
			get {
				return GetValue<bool>("HideToolbar");
			}
			set {
				SetValue<bool>("HideToolbar", value);
			}
		}

		public bool HideMenubar
		{
			get
			{
				return GetValue<bool>("HideMenubar");
			}
			set
			{
				SetValue<bool>("HideMenubar", value);
			}
		}

		public bool HideWindowUI
		{
			get
			{
				return GetValue<bool>("HideWindowUI");
			}
			set
			{
				SetValue<bool>("HideWindowUI", value);
			}
		}

		public bool FitWindow
		{
			get
			{
				return GetValue<bool>("FitWindow");
			}
			set
			{
				SetValue<bool>("FitWindow", value);
			}
		}
	
		public bool CenterWindow
		{
			get
			{
				return GetValue<bool>("CenterWindow");
			}
			set
			{
				SetValue<bool>("CenterWindow", value);
			}
		}	

		public bool DisplayDocTitle
		{
			get
			{
				return GetValue<bool>("DisplayDocTitle");
			}
			set
			{
				SetValue<bool>("DisplayDocTitle", value);
			}
		}

		public PDFNonFullScreenPageMode NonFullScreenPageMode
		{
			get {
				return GetValue<PDFNonFullScreenPageMode>("NonFullScreenPageMode", PDFNonFullScreenPageMode.UseNone);
			}
			set {
				SetValue("NonFullScreenPageMode", value);
			}
		}

		public PDFDirection Direction {
			get {
				return GetValue<PDFDirection>("Direction", PDFDirection.L2R);
			}
			set {
				SetValue("Direction", value);
			}
		}

		public PDFPrintScaling PrintScaling {
			get {
				return GetValue<PDFPrintScaling>("PrintScaling", PDFPrintScaling.None);
			}
			set {
				SetValue("PrintScaling", value);
			}
		}

		public PDFDuplex Duplex {
			get {
				return GetValue<PDFDuplex>("Duplex", PDFDuplex.Simplex);
			}
			set {
				SetValue("Duplex", value);
			}
		}

		public enum PDFDuplex { 
			None,
			Simplex,
			DuplexFlipShortEdge,
			DuplexFlipLongEdge
		}

		public enum PDFPrintScaling { 
			None,
			AppDefault
		}

		public enum PDFNonFullScreenPageMode
		{
			UseNone,
			UseOutlines,
			UseThumbs,
			UseOC
		}

		public enum PDFDirection
		{ 
			L2R,
			R2L
		}
	}

}
