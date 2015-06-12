using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NeuroSpeech.XamlToPDF;
using System.IO;
using System.Windows.Documents;

namespace XamlToPDFLiveTest
{
	public class PDFTest
	{


		public FlowDocument FlowDocument { get; set; }

		public virtual string Name { 
			get {
				return this.GetType().Name;
			}
		}

		public virtual void Generate(string file) {
		}


	}
}
