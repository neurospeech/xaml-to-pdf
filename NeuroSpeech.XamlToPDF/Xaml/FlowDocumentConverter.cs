using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.IO;
using System.IO.Packaging;
using System.Windows.Xps.Packaging;
using System.Windows.Xps.Serialization;
using System.Xml.Linq;
using System.Xaml;
using System.Threading.Tasks;
using System.Net;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows;

namespace NeuroSpeech.XamlToPDF.Xaml
{
	public class FlowDocumentPackage : IDisposable
	{

        private static string _TempPath = null;
        public static string TempPath {
            get {
                return _TempPath ?? Path.GetTempPath();
            }
        }

        private FlowDocument flowDoc;
        //private XpsSerializationManager XSrm;
        //private XpsPackagingPolicy XPolicy;
        private XpsDocument XpsDoc;
        //private Package Package;
        //private Stream Stream;
		private DirectoryInfo TempFolder;
		//private string pack;

        public List<IDisposable> toDispose;

		public FlowDocumentPackage(string xamlFlowDocument)
		{

            toDispose = new List<IDisposable>();

			TempFolder = new DirectoryInfo(TempPath + "\\XamlToPDF\\" + DateTime.Now.Ticks.ToString());
			if (!TempFolder.Exists)
				TempFolder.Create();

            /*Stream = File.Create(TempFolder.FullName + "\\a.xps");
            toDispose.Add(Stream);

            Package = Package.Open(Stream, FileMode.Create, FileAccess.ReadWrite);
            toDispose.Add(Package);

			pack = "pack://" + DateTime.Now.Ticks + ".xps";
			PackageStore.AddPackage(new Uri(pack), Package);

            

            XpsDoc = new XpsDocument(Package, CompressionOption.NotCompressed, pack);*/
            XpsDoc = new XpsDocument(TempFolder.FullName + "\\a.xps", FileAccess.ReadWrite,  CompressionOption.NotCompressed);
            toDispose.Add(new InlineDisposable(()=> XpsDoc.Close()));

            System.Windows.Xps.XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(XpsDoc);

            xamlFlowDocument = ReplaceImages(writer, xamlFlowDocument);

            flowDoc = XamlServices.Parse(xamlFlowDocument) as FlowDocument;

			IDocumentPaginatorSource src = flowDoc as IDocumentPaginatorSource;

            if (double.IsNaN(flowDoc.PageHeight))
            {
                flowDoc.PageHeight = 792;
            }
            if (double.IsNaN(flowDoc.PageWidth))
            {
                flowDoc.PageWidth = 612;
            }




            /*XPolicy = new XpsPackagingPolicy(XpsDoc);
            toDispose.Add(XPolicy);

            XSrm = new XpsSerializationManager(XPolicy, false);
            toDispose.Add(XSrm);*/

			
			DocumentPaginator pgn = src.DocumentPaginator;


			//XSrm.SaveAsXaml(pgn);


            writer.Write(pgn);

			var seq = XpsDoc.GetFixedDocumentSequence();

            DocumentReference reff = seq.References.First();

			Document = reff.GetDocument(true);

            

		}

		private Dictionary<string, DownloadItem> Cache = new Dictionary<string, DownloadItem>();

		#region private string ReplaceImages(string xamlFlowDocument)
        private string ReplaceImages(System.Windows.Xps.XpsDocumentWriter writer, string xamlFlowDocument)
		{
			XDocument doc = XDocument.Parse(xamlFlowDocument);

            

			foreach (XElement img in doc.Descendants().Where(x => x.Name.LocalName == "Image" && x.Attributes().Any(a=>a.Name.LocalName == "Source") )) {
				var at = img.Attributes().FirstOrDefault(x => x.Name.LocalName == "Source");
				if (at == null)
					continue;
				string url = at.Value;
				//if (url.StartsWith("http://") || url.StartsWith("https://")) 
                {

					string urlKey = at.Value.ToLower();
					DownloadItem file = null;

					if (!Cache.TryGetValue(urlKey, out file))
                    {
						file = new DownloadItem { 
                            FilePath = TempFolder + "\\" + Cache.Count + ".dat", 
                            Url = url
                        };
						Cache[urlKey] = file;
					}
                    file.Nodes.Add(img);
				}
			}

            

			Parallel.ForEach(Cache.Values, DownloadFile);

            foreach (var item in Cache.Values)
            {
                //var pp = Package.CreatePart(new Uri(item.PackgeUri, UriKind.Relative), "image/jpeg", CompressionOption.NotCompressed);
                //using (Stream ss = pp.GetStream(FileMode.OpenOrCreate, FileAccess.ReadWrite)) {
                //    using (FileStream fs = File.OpenRead(item.FilePath)) {
                //        fs.CopyTo(ss);
                //    }
                //}
                //Trace.WriteLine(pack + item.PackgeUri);
                ////Package.CreateRelationship(new Uri(pack + item.PackgeUri), TargetMode.External, "http://schemas.microsoft.com/xps/2005/06/required-resource");

                //foreach (var node in item.Nodes)
                //{
                //    node.Value = pack + ",,," + item.PackgeUri;
                //}

                foreach (var node in item.Nodes)
                {
                    node.Name = XName.Get("InlineImage",node.Name.NamespaceName);
                    XCData d = new XCData(Convert.ToBase64String(File.ReadAllBytes(item.FilePath)));
                    node.Add(d);

                    XAttribute a = node.Attributes().FirstOrDefault(x => x.Name.LocalName == "Source");
                    a.Remove();
                }

            }


			using(StringWriter sw = new StringWriter()){
				doc.Save(sw, SaveOptions.OmitDuplicateNamespaces);
				xamlFlowDocument = sw.ToString();
			};
			return xamlFlowDocument;
		}
		#endregion

		#region private void DownloadFile(string p,string p_2)
		private void DownloadFile(DownloadItem item)
		{
			try {
                string url = item.Url;
                if (url.StartsWith("//"))
                {
                    url = "http:" + url;
                }
                if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    using (WebClient client = new WebClient())
                    {
                        client.DownloadFile(item.Url, item.FilePath);
                    }
                }
			}
			catch { 
				
			}

			// check if zero sized file...
			FileInfo info = new FileInfo(item.FilePath);
			if (!info.Exists || info.Length == 0) { 
				// write empty file....
				File.WriteAllBytes(item.FilePath, XResources.EmptyImageData);
			}

		}
		#endregion



		public FixedDocument Document { get; private set; }

		#region public void  Dispose()
		public void Dispose()
		{

            if (toDispose.Count > 0)
			{

                //PackageStore.RemovePackage(new Uri(pack));


                foreach (var item in toDispose.ToArray().Reverse())
                {
                    item.Dispose();
                }


                try
                {

					TempFolder.Delete(true);
                }
                catch { }
			}

		}
		#endregion

	}

	public class DownloadItem {

        public DownloadItem()
        {
            Nodes = new List<XElement>();
        }

        public string PackgeUri { get; set; }

        public List<XElement> Nodes { get; private set; }

		public string Url { get; set; }
		public string FilePath { get; set; }
	}
}
