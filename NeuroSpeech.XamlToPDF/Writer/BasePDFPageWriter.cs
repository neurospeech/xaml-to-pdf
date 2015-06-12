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

namespace NeuroSpeech.XamlToPDF.Writer
{
	public class BasePDFPageWriter
	{
		public virtual string GetValue(PDFPage page, Object context, Object val) {
			return null;
		}

		public virtual void Write(PDFPage page, Object context, Object val)
		{
		}

		public virtual Type VisualType
		{
			get
			{
				return null;
			}
		}

		#region internal void EndWrite(PDFPage page,Xaml.XamlPdfContext Context,Visual fp)
		public virtual void EndWrite(PDFPage page, object context, object fp)
		{
			
		}
		#endregion
}
}
