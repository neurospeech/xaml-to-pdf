using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NeuroSpeech.XamlToPDF;
using System.ComponentModel.Composition;
using NeuroSpeech.XamlToPDF.Objects;
using System.Windows.Documents;
using System.Xaml;
using NeuroSpeech.XamlToPDF.Xaml;

namespace XamlToPDFLiveTest.Tests
{

	[Export(typeof(PDFTest))]
	public class Test1 : PDFTest
	{


		#region public override void  Generate(string file)
		public override void Generate(string file)
		{
			base.Generate(file);

			Uri uri = new Uri("/Documents/SamplePDF.xaml", UriKind.Relative);


			XamlPdfWriter writer = new XamlPdfWriter();

			using (System.IO.FileStream fs = System.IO.File.OpenWrite(file))
			{
				writer.Write( System.IO.File.ReadAllText("Documents/SamplePDF.xaml") , fs);
			}

			FlowDocument doc = App.LoadComponent(uri) as FlowDocument;

			this.FlowDocument = doc;


		}
		#endregion



	}
}
