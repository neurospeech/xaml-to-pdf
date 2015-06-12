using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NeuroSpeech.XamlToPDF.Objects
{
	public class PDFContents : PDFObject
	{


		#region public override void  Dispose()
		public override void Dispose()
		{
			base.Dispose();

			if (_contentWriter != null) {
				_contentWriter.Dispose();
				_contentWriter = null;
			}
		}
		#endregion


		StringWriter _contentWriter = new StringWriter();
		public StringWriter ContentWriter {
			get {
				return _contentWriter;
			}
		}

		#region public override string  Type
		public override string Type
		{
			get
			{
				return null;
			}
		}
		#endregion

		public void Compress() {
			string content = _contentWriter.ToString();
			_contentWriter = new StringWriter();
			MemoryStream ms = new MemoryStream();
			System.IO.Compression.GZipStream d = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Compress);

			byte[] data = System.Text.Encoding.Default.GetBytes(content);

			d.Write(data, 0, data.Length);
			d.Flush();
			d.Close();

			data = ms.ToArray();

			_contentWriter.Write(System.Text.Encoding.Default.GetString(data));

			SetValue<string[]>("Filter", new string[] { "FlateDecode" });

		}

		#region public override void  Write(System.IO.TextWriter writer)
		public override void Write(System.IO.TextWriter writer)
		{
			//Compress();
			SetValue<int>("Length", _contentWriter.ToString().Length);
			base.Write(writer);
		}
		#endregion

		public string Content { get; set; }

		#region protected override void  WriteContents(TextWriter writer)
		protected override void WriteContents(TextWriter writer)
		{
			writer.WriteLine("stream");
			writer.WriteLine(_contentWriter.ToString());
			writer.WriteLine("endstream");
		}
		#endregion

	}
}
