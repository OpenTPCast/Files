using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TPCASTWindows.Properties;

namespace TPCASTWindows
{
	public class BaseDialogForm : Form
	{
		public delegate void OnCloseClickDelegate();

		private bool moving;

		private Point oldMousePosition;

		public BaseDialogForm.OnCloseClickDelegate OnCloseClick;

		private IContainer components;

		public Button closeButton;

		public BaseDialogForm()
		{
			base.StartPosition = FormStartPosition.CenterParent;
			base.ShowInTaskbar = false;
			this.InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			base.Close();
			base.Dispose();
			if (this.OnCloseClick != null)
			{
				this.OnCloseClick();
			}
		}

		private void Panel_MouseDown(object sender, MouseEventArgs e)
		{
			if (base.WindowState == FormWindowState.Maximized)
			{
				return;
			}
			this.oldMousePosition = e.Location;
			this.moving = true;
		}

		private void Panel_MouseUp(object sender, MouseEventArgs e)
		{
			this.moving = false;
		}

		private void Panel_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left && this.moving)
			{
				Point pt = new Point(e.Location.X - this.oldMousePosition.X, e.Location.Y - this.oldMousePosition.Y);
				if (base.Location.Y + pt.Y > SystemInformation.WorkingArea.Height - 20)
				{
					pt.Y = SystemInformation.WorkingArea.Height - 20 - base.Location.Y;
				}
				base.Location += new Size(pt);
			}
		}

		private void BaseDialogForm_Load(object sender, EventArgs e)
		{
		}

		private void BaseDialogForm_FormClosed(object sender, FormClosedEventArgs e)
		{
		}

		private void BaseDialogForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			Util.HideGrayBackground();
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
			this.closeButton = new Button();
			base.SuspendLayout();
			this.closeButton.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.closeButton.BackgroundImage = Resources.dialog_close;
			this.closeButton.BackgroundImageLayout = ImageLayout.Center;
			this.closeButton.FlatAppearance.BorderSize = 0;
			this.closeButton.FlatStyle = FlatStyle.Flat;
			this.closeButton.Location = new Point(484, 6);
			this.closeButton.Name = "closeButton";
			this.closeButton.Padding = new Padding(14);
			this.closeButton.Size = new Size(10, 10);
			this.closeButton.TabIndex = 0;
			this.closeButton.UseVisualStyleBackColor = true;
			this.closeButton.Click += new EventHandler(this.button1_Click);
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.White;
			base.ClientSize = new Size(500, 264);
			base.Controls.Add(this.closeButton);
			base.FormBorderStyle = FormBorderStyle.None;
			base.Name = "BaseDialogForm";
			this.Text = "BaseDialogForm";
			base.FormClosing += new FormClosingEventHandler(this.BaseDialogForm_FormClosing);
			base.FormClosed += new FormClosedEventHandler(this.BaseDialogForm_FormClosed);
			base.Load += new EventHandler(this.BaseDialogForm_Load);
			base.MouseDown += new MouseEventHandler(this.Panel_MouseDown);
			base.MouseMove += new MouseEventHandler(this.Panel_MouseMove);
			base.MouseUp += new MouseEventHandler(this.Panel_MouseUp);
			base.ResumeLayout(false);
		}
	}
}
