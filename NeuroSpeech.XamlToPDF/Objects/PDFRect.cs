using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuroSpeech.XamlToPDF.Objects
{
	public class PDFRect
	{

		public PDFRect()
		{

		}

		public int Left { get; set; }

		public int Top { get; set; }

		public int Width { get; set; }

		public int Height { get; set; }

		#region public override string  ToString()
		public override string ToString()
		{
			return string.Format("[{0} {1} {2} {3}]", Left, Top, Width, Height);
		}
		#endregion

	}
}
