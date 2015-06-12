using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuroSpeech.XamlToPDF.Objects
{
	public class PDFInfo : PDFObject
	{

		#region protected internal override void  Initialize()
		protected internal override void Initialize()
		{
			base.Initialize();

			CreationDate = DateTime.Now;
		}
		#endregion


		#region public override string  Type
		public override string Type
		{
			get
			{
				return "";
			}
		}
		#endregion

		public string Title
		{
			get
			{
				return GetValue<string>("Title");
			}
			set
			{
				SetValue("Title", value);
			}
		}

		public string Author
		{
			get
			{
				return GetValue<string>("Author");
			}
			set
			{
				SetValue("Author",value);
			}
		}


		public string Subject
		{
			get
			{
				return GetValue<string>("Subject");
			}
			set
			{
				SetValue("Subject", value);
			}
		}

		public string Keywords
		{
			get
			{
				return GetValue<string>("Keywords");
			}
			set
			{
				SetValue("Keywords", value);
			}
		}

		public string Producer
		{
			get
			{
				return GetValue<string>("Producer");
			}
			set
			{
				SetValue("Producer", value);
			}
		}

		public string Creator
		{
			get
			{
				return GetValue<string>("Creator");
			}
			set
			{
				SetValue("Creator", value);
			}
		}

		public DateTime CreationDate {
			get {
				return GetValue<DateTime>("CreationDate");
			}
			set {
				SetValue<DateTime>("CreationDate", value);
			}
		}

		public DateTime ModDate {
			get {
				return GetValue<DateTime>("ModDate");
			}
			set {
				SetValue<DateTime>("ModDate", value);
			}
		}		
		
		#region protected override void  WriteProperty(System.IO.TextWriter writer, object value, string key)
		protected override void WriteProperty(System.IO.TextWriter writer, object value, string key)
		{
			//base.WriteProperty(writer, value, key);
			string val = value as string;
			
			if (val == null) {
				base.WriteProperty(writer, value, key);
				return;
			}

			writer.WriteLine("/{1} {0}" , "(" + Encode(val) + ")", key);
		}
		#endregion

		private string Encode(string p)
		{
			p = p.Replace("\\", "\\\\");
			p = p.Replace("(", "\\(");
			p = p.Replace(")", "\\)");
			p = p.Replace("\n", "\\n");
			p = p.Replace("\r", "\\r");
			p = p.Replace("\t", "\\t");
			p = p.Replace("\b", "\\b");
			p = p.Replace("\f", "\\f");
			return p;
		}

	}
}
