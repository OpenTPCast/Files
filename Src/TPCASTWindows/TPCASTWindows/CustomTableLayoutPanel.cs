using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TPCASTWindows
{
	internal class CustomTableLayoutPanel : TableLayoutPanel
	{
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern int ShowScrollBar(IntPtr hWnd, int bar, int show);

		protected override void WndProc(ref Message m)
		{
			CustomTableLayoutPanel.ShowScrollBar(m.HWnd, 1, 0);
			base.WndProc(ref m);
		}
	}
}
