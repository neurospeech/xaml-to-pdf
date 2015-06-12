using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NeuroSpeech.XamlToPDF.Objects
{
	public interface IPDFDictionary {

		bool HasValues { get; }

		void Write(TextWriter writer);

	}

	public class PDFDictionary<T> : IPDFDictionary
			where T : PDFObject
	{


		private Dictionary<string, PDFObject> References = new Dictionary<string, PDFObject>();

		public T this[string name]
		{
			get
			{
				PDFObject obj = null;
				References.TryGetValue(name, out obj);
				return (T)obj;
			}
			set
			{
				References[name] = value;
				value.Name = name;
			}
		}


		#region public void  Write(TextWriter writer)
		public void Write(TextWriter writer)
		{
			writer.WriteLine("<<");
			foreach (var item in References)
			{
				writer.WriteLine("/{0} {1}", item.Key, item.Value.Ref);
			}
			writer.WriteLine(">>");
		}
		#endregion


		public bool HasValues
		{
			get {
				return References.Count > 0;
			}
		}
	}
}
