using System;
using System.Drawing;
using System.Windows.Forms;

namespace TPCASTWindows.Custom
{
	internal class CustomProgressBar : ProgressBar
	{
		public CustomProgressBar()
		{
			base.SetStyle(ControlStyles.UserPaint, true);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Rectangle bounds = new Rectangle(0, 0, base.Width, base.Height);
			e.Graphics.FillRectangle(new SolidBrush(this.BackColor), 0, 0, bounds.Width, bounds.Height);
			bounds.Width = (int)((double)bounds.Width * ((double)base.Value / (double)base.Maximum));
			e.Graphics.FillRectangle(new SolidBrush(this.ForeColor), 0, 0, bounds.Width, bounds.Height);
		}
	}
}
