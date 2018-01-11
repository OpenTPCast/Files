using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[NativeCppClass, UnsafeValueType]
[StructLayout(LayoutKind.Sequential, Size = 64)]
internal struct _IMAGE_DOS_HEADER
{
	private short <alignment member>;
}
