using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ATL
{
	[NativeCppClass]
	[StructLayout(LayoutKind.Sequential, Size = 8)]
	internal static struct IAtlStringMgr
	{
		private long <alignment member>;
	}
}
