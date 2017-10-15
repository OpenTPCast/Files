using NLog;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TPCASTWindows.Properties;
using TPCASTWindows.Utils;

namespace TPCASTWindows
{
	public class BaseDialogForm : Form
	{
		public delegate void OnCloseClickDelegate();

		private static Logger log = LogManager.GetCurrentClassLogger();

		private bool moving;

		private Point oldMousePosition;

		public BaseDialogForm.OnCloseClickDelegate OnCloseClick;

		private IContainer components;

		public Button closeButton;

		public BaseDialogForm()
		{
			this.InitializeComponent();
			base.StartPosition = FormStartPosition.Manual;
			base.Location = Util.sContext.Location + new Size((Util.sContext.Width - base.Width) / 2, (Util.sContext.Height - base.Height) / 2);
			base.ShowInTaskbar = false;
			WindowsMessage.OnBaseFormTaskbarCloseClick += new WindowsMessage.OnBaseFormTaskbarCloseClickDelegate(this.OnBaseFormTaskbarCloseClick);
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
				Point newPosition = new Point(e.Location.X - this.oldMousePosition.X, e.Location.Y - this.oldMousePosition.Y);
				if (base.Location.Y + newPosition.Y > SystemInformation.WorkingArea.Height - 20)
				{
					newPosition.Y = SystemInformation.WorkingArea.Height - 20 - base.Location.Y;
				}
				base.Location += new Size(newPosition);
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
			WindowsMessage.OnBaseFormTaskbarCloseClick -= new WindowsMessage.OnBaseFormTaskbarCloseClickDelegate(this.OnBaseFormTaskbarCloseClick);
			Util.HideGrayBackground();
		}

		private void OnBaseFormTaskbarCloseClick()
		{
			base.Close();
		}

		private void BaseDialogForm_KeyDown(object sender, KeyEventArgs e)
		{
			BaseDialogForm.log.Trace("BaseDialogForm_KeyDown kc = " + e.KeyCode);
			BaseDialogForm.log.Trace("BaseDialogForm_KeyDown m = " + e.Modifiers);
			if (e.Alt && e.KeyCode == Keys.F4)
			{
				e.Handled = true;
				this.closeButton.PerformClick();
			}
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
			base.KeyPreview = true;
			base.Name = "BaseDialogForm";
			this.Text = "BaseDialogForm";
			base.FormClosing += new FormClosingEventHandler(this.BaseDialogForm_FormClosing);
			base.FormClosed += new FormClosedEventHandler(this.BaseDialogForm_FormClosed);
			base.Load += new EventHandler(this.BaseDialogForm_Load);
			base.KeyDown += new KeyEventHandler(this.BaseDialogForm_KeyDown);
			base.MouseDown += new MouseEventHandler(this.Panel_MouseDown);
			base.MouseMove += new MouseEventHandler(this.Panel_MouseMove);
			base.MouseUp += new MouseEventHandler(this.Panel_MouseUp);
			base.ResumeLayout(false);
		}
	}
}
