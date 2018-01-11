using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ATL
{
	[NativeCppClass]
	[StructLayout(LayoutKind.Sequential, Size = 8)]
	internal struct CStringT<wchar_t,ATL::StrTraitATL<wchar_t,ATL::ChTraitsCRT<wchar_t> > >
	{
		private long <alignment member>;
	}
}
