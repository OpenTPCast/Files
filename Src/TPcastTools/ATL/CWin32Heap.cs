using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ATL
{
	[NativeCppClass]
	[StructLayout(LayoutKind.Sequential, Size = 24)]
	internal struct CWin32Heap
	{
		private long <alignment member>;
	}
}
