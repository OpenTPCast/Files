using System;
using System.Drawing;
using System.Windows.Forms;

namespace TPCASTWindows
{
	internal class DropButton : Button
	{
		private ContextMenuStrip contextMenuStrip;

		private Point point;

		private int x;

		private int y;

		private int workSize_x;

		private int workSize_y;

		public int WorkSizeX
		{
			get
			{
				return this.workSize_x;
			}
			set
			{
				this.workSize_x = value;
			}
		}

		public int WorkSizeY
		{
			get
			{
				return this.workSize_y;
			}
			set
			{
				this.workSize_y = value - 55;
			}
		}

		public new ContextMenuStrip ContextMenuStrip
		{
			get
			{
				return this.contextMenuStrip;
			}
			set
			{
				if (this.contextMenuStrip != null)
				{
					this.contextMenuStrip = value;
				}
			}
		}

		public DropButton()
		{
			this.x = base.Size.Width;
			this.y = 0;
		}

		protected override void OnClick(EventArgs e)
		{
			base.OnClick(e);
			int arg_63_0 = base.Location.X + base.Size.Width + this.contextMenuStrip.Size.Width;
			int _y = base.Location.Y + this.contextMenuStrip.Size.Height;
			if (arg_63_0 < this.WorkSizeX - 8)
			{
				this.x = base.Size.Width;
			}
			else
			{
				this.x = 0 - this.contextMenuStrip.Size.Width;
			}
			if (_y < this.WorkSizeY)
			{
				this.y = 0;
			}
			else
			{
				this.y = 0 - this.contextMenuStrip.Size.Height + base.Size.Height;
			}
			this.point = new Point(this.x, this.y);
			this.contextMenuStrip.Show(this, this.point);
		}

		protected override void OnMouseDown(MouseEventArgs mevent)
		{
			base.OnMouseDown(mevent);
			mevent.Button.ToString() != "Right";
		}
	}
}
