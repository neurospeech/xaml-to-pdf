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
	public class PDFPageDefaultWriter : PDFPageWriter<object,object>
	{
		#region public override void  WriteVisual(PDFPage page, object visual)
		public override void WriteVisual(PDFPage page, object context, object visual)
		{
			Trace.WriteLine("Warning !! Writer not found for " + visual.GetType().FullName);
		}
		#endregion

	}
}
