using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ATL
{
	[NativeCppClass, UnsafeValueType]
	[StructLayout(LayoutKind.Sequential, Size = 48)]
	internal struct CAtlStringMgr
	{
		private long <alignment member>;
	}
}
