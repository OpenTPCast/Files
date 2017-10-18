using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ATL
{
	[NativeCppClass, UnsafeValueType]
	[StructLayout(LayoutKind.Sequential, Size = 32)]
	internal struct CNilStringData
	{
		private long <alignment member>;
	}
}
