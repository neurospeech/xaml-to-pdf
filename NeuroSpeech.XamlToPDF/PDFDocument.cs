using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NeuroSpeech.XamlToPDF.Objects;

namespace NeuroSpeech.XamlToPDF
{
	public class PDFDocument : IDisposable
	{

		public PDFDocument()
		{
			_catalog = CreateObject<PDFCatalog>();
			_info = CreateObject<PDFInfo>();
			_info.Creator = "XamlToPDF Converter";
			_info.Producer = "NeuroSpeech XAML to PDF Converter";
			_resources = CreateObject<PDFResources>();
		}

		private PDFInfo _info = null;
		public PDFInfo Info {
			get {
				return _info;
			}
		}

		private PDFResources _resources = null;
		public PDFResources Resources {
			get {
				return _resources;
			}
		}

		private PDFCatalog _catalog = null;
		public PDFCatalog Catalog {
			get {
				return _catalog;
			}
		}

		private List<PDFObject> _Objects = new List<PDFObject>();

		public IEnumerable<PDFObject> Objects
		{
			get
			{
				return _Objects;
			}
		}

		public T CreateObject<T>() where T : PDFObject
		{
			T obj = Activator.CreateInstance<T>();
			obj.ID = _Objects.Count + 1;
			obj.Document = this;
			_Objects.Add(obj);
			obj.Initialize();
			return obj;
		}

		public void Write(Stream stream)
		{
			using (StreamWriter writer = new StreamWriter(stream))
			{
				writer.AutoFlush = true;

				writer.WriteLine("%PDF-1.6");
				foreach (var item in _Objects.OrderBy(x => x.ID))
				{
					item.Write(writer);
				}

				int start = (int)stream.Length;

				writer.WriteLine("xref");
				writer.WriteLine("0 {0}", _Objects.Count + 1);
				writer.WriteLine("0000000000 65535 f");

				foreach (var item in _Objects.OrderBy(x => x.ID))
				{
					writer.WriteLine("{0:D10} 00000 n", item.Offset);
				}

				writer.WriteLine();

				writer.WriteLine("trailer");
				writer.WriteLine("<<");
				writer.WriteLine("/Size {0}", _Objects.Count + 1);
				writer.WriteLine("/Root 1 0 R");
				writer.WriteLine("/Info {0} 0 R", _info.ID);
				writer.WriteLine(">>");
				writer.WriteLine("startxref");
				writer.WriteLine("{0}", start);
				writer.WriteLine("%%EOF");
			}
			
		}


		#region public void  Dispose()
		public void Dispose()
		{
			foreach (var item in Objects)
			{
				item.Dispose();
			}
		}
		#endregion

	}
}
