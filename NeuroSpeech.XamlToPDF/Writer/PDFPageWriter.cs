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
	public abstract class PDFPageWriter<T,TX> : BasePDFPageWriter
	{

		#region public override Type  VisualType
		public sealed override Type VisualType
		{
			get
			{
				return typeof(T);
			}
		}
		#endregion


		#region public override void  Write(PDFPage page, object val)
		public sealed override void Write(PDFPage page, object context , object val)
		{
			WriteVisual(page, (TX)context, (T)val);
		}
		#endregion


		#region public override void  EndWrite(PDFPage page, object context, object fp)
		public sealed override void EndWrite(PDFPage page, object context, object fp)
		{
			EndWriteVisual(page, (TX)context, (T)fp);
		}
		#endregion


		#region public override string  GetValue(PDFPage page, object context, object val)
		public sealed override string GetValue(PDFPage page, object context, object val)
		{
			return GetPDFValue(page, (TX)context, (T)val);
		}
		#endregion

		#region private string GetPDFValue(PDFPage page,TX context,T value)
		protected virtual string GetPDFValue(PDFPage page, TX context, T value)
		{
			return "";
		}
		#endregion




		public abstract void WriteVisual(PDFPage page, TX context, T visual);

		public virtual void EndWriteVisual(PDFPage page, TX context, T visual) { 
		}

	}
}
