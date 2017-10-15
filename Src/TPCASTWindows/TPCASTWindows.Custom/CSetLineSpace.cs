using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TPCASTWindows.Custom
{
	internal class CSetLineSpace
	{
		private struct PARAFORMAT2
		{
			public int cbSize;

			public uint dwMask;

			public short wNumbering;

			public short wReserved;

			public int dxStartIndent;

			public int dxRightIndent;

			public int dxOffset;

			public short wAlignment;

			public short cTabCount;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
			public int[] rgxTabs;

			public int dySpaceBefore;

			public int dySpaceAfter;

			public int dyLineSpacing;

			public short sStyle;

			public byte bLineSpacingRule;

			public byte bOutlineLevel;

			public short wShadingWeight;

			public short wShadingStyle;

			public short wNumberingStart;

			public short wNumberingStyle;

			public short wNumberingTab;

			public short wBorderSpace;

			public short wBorderWidth;

			public short wBorders;
		}

		public const int WM_USER = 1024;

		public const int EM_GETPARAFORMAT = 1085;

		public const int EM_SETPARAFORMAT = 1095;

		public const long MAX_TAB_STOPS = 32L;

		public const uint PFM_LINESPACING = 256u;

		[DllImport("user32", CharSet = CharSet.Auto)]
		private static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, ref CSetLineSpace.PARAFORMAT2 lParam);

		public static void SetLineSpace(Control ctl, int dyLineSpacing)
		{
			CSetLineSpace.PARAFORMAT2 fmt = default(CSetLineSpace.PARAFORMAT2);
			fmt.cbSize = Marshal.SizeOf(fmt);
			fmt.bLineSpacingRule = 4;
			fmt.dyLineSpacing = dyLineSpacing;
			fmt.dwMask = 256u;
			try
			{
				CSetLineSpace.SendMessage(new HandleRef(ctl, ctl.Handle), 1095, 0, ref fmt);
			}
			catch
			{
			}
		}
	}
}
