using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TPCASTWindows.Properties;

namespace TPCASTWindows
{
	public class RouterSSIDPasswordControl : UserControl
	{
		public delegate void OnOkClickDelegate(string SSID, string password);

		public RouterSSIDPasswordControl.OnOkClickDelegate OnOkClick;

		private IContainer components;

		private CustomLabel customLabel2;

		private Label label1;

		private Button okButton;

		private PictureBox pictureBox1;

		private Label label2;

		private Label label3;

		private WaterTextBox SSIDTextBox;

		private WaterTextBox passwordTextBox;

		public RouterSSIDPasswordControl()
		{
			this.InitializeComponent();
			this.SSIDTextBox.WaterText = Resources.defaultWater;
			this.passwordTextBox.WaterText = Resources.defaultWater;
		}

		public GraphicsPath getroundrectpath(RectangleF rect, float radius)
		{
			return this.getroundrectpath(rect.X, rect.Y, rect.Width, rect.Height, radius);
		}

		public GraphicsPath getroundrectpath(float x, float y, float width, float height, float radius)
		{
			GraphicsPath expr_05 = new GraphicsPath();
			expr_05.AddLine(x + radius, y, x + width - radius * 2f, y);
			expr_05.AddArc(x + width - radius * 2f, y, radius * 2f, radius * 2f, 270f, 90f);
			expr_05.AddLine(x + width, y + radius, x + width, y + height - radius * 2f);
			expr_05.AddArc(x + width - radius * 2f, y + height - radius * 2f, radius * 2f, radius * 2f, 0f, 90f);
			expr_05.AddLine(x + width - radius * 2f, y + height, x + radius, y + height);
			expr_05.AddArc(x, y + height - radius * 2f, radius * 2f, radius * 2f, 90f, 90f);
			expr_05.AddLine(x, y + height - radius * 2f, x, y + radius);
			expr_05.AddArc(x, y, radius * 2f, radius * 2f, 180f, 90f);
			expr_05.CloseFigure();
			return expr_05;
		}

		[DllImport("user32.dll")]
		private static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);

		[DllImport("gdi32.dll")]
		private static extern IntPtr CreateRoundRectRgn(int x1, int y1, int x2, int y2, int cx, int cy);

		private void okButton_Click(object sender, EventArgs e)
		{
			string ssid = this.SSIDTextBox.Text;
			string password = this.passwordTextBox.Text;
			if (string.IsNullOrEmpty(ssid))
			{
				this.SSIDTextBox.Text = "";
				this.SSIDTextBox.WaterText = Resources.ssidError;
				this.SSIDTextBox.WaterTextColor = Color.Red;
				return;
			}
			if (string.IsNullOrEmpty(password) || password.Count<char>() < 8 || password.Count<char>() > 16)
			{
				this.passwordTextBox.Text = "";
				this.passwordTextBox.WaterText = Resources.passwordError;
				this.passwordTextBox.WaterTextColor = Color.Red;
				return;
			}
			RouterSSIDPasswordControl.OnOkClickDelegate expr_A3 = this.OnOkClick;
			if (expr_A3 == null)
			{
				return;
			}
			expr_A3(ssid, password);
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
			this.components = new Container();
			ComponentResourceManager arg_A8_0 = new ComponentResourceManager(typeof(RouterSSIDPasswordControl));
			this.label1 = new Label();
			this.label2 = new Label();
			this.label3 = new Label();
			this.passwordTextBox = new WaterTextBox();
			this.SSIDTextBox = new WaterTextBox();
			this.customLabel2 = new CustomLabel(this.components);
			this.okButton = new Button();
			this.pictureBox1 = new PictureBox();
			((ISupportInitialize)this.pictureBox1).BeginInit();
			base.SuspendLayout();
			this.label1.BackColor = Color.FromArgb(216, 216, 216);
			arg_A8_0.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			arg_A8_0.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			arg_A8_0.ApplyResources(this.label3, "label3");
			this.label3.Name = "label3";
			this.passwordTextBox.BackColor = Color.White;
			arg_A8_0.ApplyResources(this.passwordTextBox, "passwordTextBox");
			this.passwordTextBox.Name = "passwordTextBox";
			this.passwordTextBox.WaterText = "请区分大小写";
			this.passwordTextBox.WaterTextColor = SystemColors.ControlDark;
			this.passwordTextBox.WaterTextFont = new Font("微软雅黑", 12f, FontStyle.Regular, GraphicsUnit.Pixel);
			this.SSIDTextBox.BackColor = Color.White;
			arg_A8_0.ApplyResources(this.SSIDTextBox, "SSIDTextBox");
			this.SSIDTextBox.Name = "SSIDTextBox";
			this.SSIDTextBox.WaterText = "Case sensitive";
			this.SSIDTextBox.WaterTextColor = SystemColors.ControlDark;
			this.SSIDTextBox.WaterTextFont = new Font("微软雅黑", 12f, FontStyle.Regular, GraphicsUnit.Pixel);
			arg_A8_0.ApplyResources(this.customLabel2, "customLabel2");
			this.customLabel2.LineDistance = 3;
			this.customLabel2.Name = "customLabel2";
			this.okButton.BackgroundImage = Resources.blue_background_1;
			arg_A8_0.ApplyResources(this.okButton, "okButton");
			this.okButton.FlatAppearance.BorderSize = 0;
			this.okButton.ForeColor = Color.White;
			this.okButton.Name = "okButton";
			this.okButton.UseVisualStyleBackColor = false;
			this.okButton.Click += new EventHandler(this.okButton_Click);
			this.pictureBox1.Image = Resources.router_ssid;
			arg_A8_0.ApplyResources(this.pictureBox1, "pictureBox1");
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.TabStop = false;
			arg_A8_0.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.White;
			base.Controls.Add(this.passwordTextBox);
			base.Controls.Add(this.SSIDTextBox);
			base.Controls.Add(this.label3);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.customLabel2);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.okButton);
			base.Controls.Add(this.pictureBox1);
			base.Name = "RouterSSIDPasswordControl";
			((ISupportInitialize)this.pictureBox1).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
