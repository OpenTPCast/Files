using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TPCASTWindows
{
	public class GrayForm : Form
	{
		private IContainer components;

		public GrayForm()
		{
			this.InitializeComponent();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			base.SuspendLayout();
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.FromArgb(25, 25, 25);
			base.ClientSize = new Size(600, 364);
			base.FormBorderStyle = FormBorderStyle.None;
			base.Name = "GrayForm";
			base.Opacity = 0.8;
			this.Text = "GrayForm";
			base.ResumeLayout(false);
		}
	}
}
