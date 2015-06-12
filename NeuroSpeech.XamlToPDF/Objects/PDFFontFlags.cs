using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuroSpeech.XamlToPDF.Objects
{
	[Flags]
	public enum PDFFontFlags : int
	{
		FixedPitch = 1,
		Serif = 2,
		Symbolic = 4,
		Script = 8,
		Nonsymbolic = 32,
		Italic = 64,
		AllCap = 0x20000,
		SmallCap = 0x4000,
		ForceBold = 0x8000,
		None = 0
	}
}
