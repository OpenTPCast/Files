using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace boost.exception_detail
{
	[NativeCppClass]
	[StructLayout(LayoutKind.Sequential, Size = 8)]
	internal struct refcount_ptr<boost::exception_detail::error_info_container>
	{
		private long <alignment member>;
	}
}
