using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NeuroSpeech.XamlToPDF
{
	public abstract class BasePDFWriter
	{
		StringWriter _sw = new StringWriter();
		TextWriter _writer;

		public BasePDFWriter(TextWriter writer)
		{
			_writer = writer;
		}

		public BasePDFWriter()
		{
			
		}


		public TextWriter Writer {
			get {
				return _sw;
			}
		}

		#region public override string  ToString()
		public override string ToString()
		{
			return _sw.GetStringBuilder().ToString();
		}
		#endregion

		public void Write(string text) {
			Writer.Write(text);
		}

		public void WriteLine(string line,params object[] args){
			Writer.WriteLine(line, args);
		}

	}
}
