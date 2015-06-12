using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using NeuroSpeech.XamlToPDF;
using System.Windows.Xps.Packaging;
using System.Diagnostics;

namespace XamlToPDFLiveTest
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			this.Loaded += new RoutedEventHandler(MainWindow_Loaded);

			TempFolder = new System.IO.DirectoryInfo(System.IO.Path.GetTempPath() + "\\XamlToPDFLive");
			if (!TempFolder.Exists)
				TempFolder.Create();

		}

		#region void  MainWindow_Loaded(object sender, RoutedEventArgs e)
		void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			CompositionContainer cc;
			AggregateCatalog cat = new AggregateCatalog();
			cat.Catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly()));

			cc = new CompositionContainer(cat);

			pdfList.DisplayMemberPath = "Name";
			var list = cc.GetExportedValues<PDFTest>();
			pdfList.ItemsSource = list;


			if (list.Any()) {
				pdfList.SelectedIndex = 0;
			}


		}
		#endregion


		private System.IO.DirectoryInfo TempFolder;

		private void pdfList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			PDFTest w = pdfList.SelectedItem as PDFTest;
			if (w == null) {

			} 
			else
			{

				string file = TempFolder.FullName + "\\" + DateTime.Now.Ticks.ToString() + ".pdf";
				w.Generate(file);
				pdfViewer.LoadPDF(file);
				source.Text = System.IO.File.ReadAllText(file);

				documentViewer1.Document = w.FlowDocument;
			}
		}
	}
}
