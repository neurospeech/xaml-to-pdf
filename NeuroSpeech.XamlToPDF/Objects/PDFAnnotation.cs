using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuroSpeech.XamlToPDF.Objects
{

    public interface IPDFInlineObject { 
    }

    public class PDFAnnotationAction : PDFObject , IPDFInlineObject
    {
        public override string Type
        {
            get
            {
                return "/Action";
            }
        }

        public string Action {
            get {
                return GetValue<string>("S");
            }
            set {
                SetValue<string>("S", value);
            }
        }

    }

    public class PDFAnnotationURIAction : PDFAnnotationAction {

        public PDFAnnotationURIAction()
        {
            Action = "URI";
        }

        public Uri URI {
            get {
                return GetValue<Uri>("URI");
            }
            set {
                SetValue<Uri>("URI", value);
            }
        }
    }

    public class PDFAnnotation : PDFXObject
    {

        public override string Type
        {
            get
            {
                return "/Annots";
            }
        }
        
    }


    public class PDFLinkAnnotation : PDFAnnotation {

        private PDFAnnotationURIAction action;

        protected internal override void Initialize()
        {
            base.Initialize();

            Subtype = "Link";

            action = new PDFAnnotationURIAction();
            SetValue<PDFAnnotationURIAction>("A", action);
            // empty border...
            SetValue<PDFRect>("Border", new PDFRect {});
        }

        public PDFRect Rect {
            get {
                return GetValue<PDFRect>("Rect");
            }
            set {
                SetValue<PDFRect>("Rect", value);
            }
        }

        public Uri URI {
            get {
                return action.URI;
            }
            set {
                action.URI = value;
            }
        }
    }


    public class PDFAnnotations : PDFObject
    {

        private List<PDFAnnotation> _annots = new List<PDFAnnotation>();

        public void AddLink(Uri link, PDFRect rect) {
            var l =  Document.CreateObject<PDFLinkAnnotation>();
            l.URI = link;
            l.Rect = rect;
            _annots.Add(l);
        }

        protected override void WriteHeader(System.IO.TextWriter writer)
        {
            // nothing to write....
        }

        protected override void WriteContents(System.IO.TextWriter writer)
        {
            writer.WriteLine("[");
            foreach (var item in _annots)
            {
                writer.WriteLine( "{0} 0 R", item.ID);
            }
            writer.WriteLine("]");
        }

    }
}
