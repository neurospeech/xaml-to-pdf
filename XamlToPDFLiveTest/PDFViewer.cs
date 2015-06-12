using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace XamlToPDFLiveTest
{
	public partial class PDFViewer : UserControl
	{
		public PDFViewer()
		{
			InitializeComponent();


			this.axAcroPDF1.setShowToolbar(true);
			
		}



		#region internal void LoadPDF(string p)
		internal void LoadPDF(string fileName)
		{
			this.axAcroPDF1.LoadFile(fileName);
		}
		#endregion
}
}
