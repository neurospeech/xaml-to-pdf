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
	public class WriterLibrary
	{

		public static WriterLibrary Library = new WriterLibrary();

		CompositionContainer container;

		private WriterLibrary()
		{
			AggregateCatalog cat = new AggregateCatalog();
			cat.Catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
			container = new CompositionContainer(cat);
		}

		private Dictionary<Type, BasePDFPageWriter> values = new Dictionary<Type, BasePDFPageWriter>();

		public BasePDFPageWriter GetWriter(Type type)
		{

			BasePDFPageWriter val = null;
			if (values.TryGetValue(type, out val))
				return val;

			val = container.GetExportedValues<BasePDFPageWriter>().FirstOrDefault(x => x.VisualType == type);
			if (val == null)
			{
				val = new PDFPageDefaultWriter();
			}
			values[type] = val;
			return val;
		}

		public void Write(PDFPage page, object context, object value) {
			BasePDFPageWriter write = GetWriter(value.GetType());
			write.Write(page, context, value);
		}


		#region internal void GetPDFValue(PDFPage page,Xaml.XamlPdfContext context,Brush brush)
		internal string GetPDFValue(PDFPage page, Xaml.XamlPdfContext context, object value)
		{
			BasePDFPageWriter write = GetWriter(value.GetType());
			return write.GetValue(page, context, value);
		}
		#endregion
}
}
