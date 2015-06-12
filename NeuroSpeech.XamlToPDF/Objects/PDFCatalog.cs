using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuroSpeech.XamlToPDF.Objects
{
	public class PDFCatalog : PDFObject
	{

		#region protected internal override void  Initialize()
		protected internal override void Initialize()
		{
			SetValue<PDFViewerPreferences>("ViewerPreferences", Document.CreateObject<PDFViewerPreferences>());

			Outlines.Clear();
			Pages.Clear();

			ViewerPreferences.Duplex = PDFViewerPreferences.PDFDuplex.Simplex;
			ViewerPreferences.NonFullScreenPageMode = PDFViewerPreferences.PDFNonFullScreenPageMode.UseOutlines;

			
		}
		#endregion

		public PDFResources Resources {
			get {
				return Document.Resources;
			}
		}

		public PDFPages Pages {
			get {
				return GetCollection<PDFPages>("Pages");
			}
		}

		public PDFOutlines Outlines{
			get {
				return GetCollection<PDFOutlines>("Outlines");
			}
		}

		public PDFViewerPreferences ViewerPreferences {
			get {
				return GetValue<PDFViewerPreferences>("ViewerPreferences");
			}
		}

		public PDFPageMode PageMode {
			get {
				return GetValue<PDFPageMode>("PageMode", PDFPageMode.UseNone);
			}
			set {
				SetValue("PageMode", value);
			}
		}

		public enum PDFPageMode { 
			UseNone,
			UseOutlines,
			UseThumbs,
			FullScreen,
			UseOC,
			UseAttachments
		}

	}

}
