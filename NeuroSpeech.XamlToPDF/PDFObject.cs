using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NeuroSpeech.XamlToPDF.Objects;

namespace NeuroSpeech.XamlToPDF
{

	public class PDFObject : IDisposable
	{

		public virtual string Type
		{
			get
			{
				string name = GetType().Name;
				if (name.ToLower().StartsWith("pdf"))
					name = name.Substring(3);
				return "/" + name;
			}
		}

		public string Name { 
			get{
				return GetValue<string>("Name");
			}
			set {
				SetValue("Name", value);
			}
		}

		public int ID { get; internal set; }

		public int Offset { get; internal set; }

		public virtual string Ref {
			get {
				return string.Format("{0} 0 R",ID);
			}
		}

		public PDFDocument Document
		{
			get;
			internal set;
		}

		internal protected virtual void Initialize() { 
		}

		public virtual void Write(TextWriter writer) {

			Stream s = ((StreamWriter)writer).BaseStream;

			Offset = (int)s.Length;

			writer.WriteLine("{0} 0 obj",ID);
            WriteHeader(writer);
			WriteContents(writer);
			writer.WriteLine("endobj");
		}

        protected virtual void WriteHeader(TextWriter writer)
        {
            writer.Write("<<");
            string type = Type;
            if (!string.IsNullOrEmpty(type))
            {
                writer.WriteLine("/Type {0}", type);
            }
            WriteProperties(writer);
            writer.WriteLine(">>");
        }

		#region private void WriteProperties(TextWriter writer)
		protected virtual void WriteProperties(TextWriter writer)
		{
			foreach (var item in Values)
			{
				object value = item.Value;
				string key = item.Key;
				WriteProperty(writer, value, key);
			}
		}

		protected virtual void WriteProperty(TextWriter writer, object value, string key)
		{
			if (value is string[]) {
				writer.WriteLine("/{0} [/{1}]",key, string.Join(" /", ((string[])value)));
				return;
			}
			if (value is IPDFDictionary)
			{
				IPDFDictionary d = value as IPDFDictionary;
				if (d.HasValues)
				{
					writer.Write("/{0} ", key);
					d.Write(writer);
				}
				return;
			}
            if (value is IPDFInlineObject) {
                writer.Write("/{0} ", key);
                PDFObject pdfobj = value as PDFObject;
                pdfobj.WriteHeader(writer);
                return;
            }
			if (value is PDFObject)
			{
				writer.WriteLine("/{0} {1}", key, ((PDFObject)value).Ref);
				return;
			}
			if (value is DateTime) {
				writer.WriteLine("/{0} (D:{1:yyyyMMddHHmmss})", key, value);
				return;
			}
			if (value.GetType().IsEnum) {
				Type t = value.GetType();
				if (t.GetCustomAttributes(typeof(FlagsAttribute), true).Length > 0)
				{
					int val = (int)value;
					writer.WriteLine("/{0} {1}",key,val	);
					return;
				}
				writer.WriteLine("/{0} /{1}", key, value.ToString()); 
				return;
			}
			if (value is string) {
				writer.WriteLine("/{0} /{1}", key, value);
                return;
			}

            if (value is Uri) {
                writer.WriteLine("/{0} ({1})", key, value);
                return;
            }
			
			writer.WriteLine("/{0} {1}", key, value);
			
		}
		#endregion


		#region private void WriteContents(TextWriter writer)
		protected virtual void WriteContents(TextWriter writer)
		{
			
		}
		#endregion


		private Dictionary<string, object> Values = new Dictionary<string, object>();

		public T GetValue<T>(string name, T val = default(T)) {
			object v = null;
			if (!Values.TryGetValue(name, out v)) {
				Values[name] = val;
			}else{
				val = (T)v;
			}
			return val;
		}

		public void SetValue<T>(string name, T val) {
			Values[name] = val;
		}

		public TC GetCollection<TC>(string name)
			where TC: PDFObject
		{
			TC val = null;
			object obj = null;
			if (!Values.TryGetValue(name, out obj))
			{
				val = Document.CreateObject<TC>();
				Values[name] = val;
			}
			else {
				val = (TC)obj;
			}
			return val;
		}

		#region public void  Dispose()
		public virtual void Dispose()
		{
			
		}
		#endregion

	}


	public interface IPDFObjectCollection { 
	}
}