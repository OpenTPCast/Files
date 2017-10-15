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
			Rectangle rectangle = new Rectangle(0, 0, base.Width, base.Height);
			e.Graphics.FillRectangle(new SolidBrush(this.BackColor), 0, 0, rectangle.Width, rectangle.Height);
			rectangle.Width = (int)((double)rectangle.Width * ((double)base.Value / (double)base.Maximum));
			e.Graphics.FillRectangle(new SolidBrush(this.ForeColor), 0, 0, rectangle.Width, rectangle.Height);
		}
	}
}
