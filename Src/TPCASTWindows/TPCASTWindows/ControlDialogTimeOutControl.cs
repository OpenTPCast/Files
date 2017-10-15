using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TPCASTWindows.Properties;

namespace TPCASTWindows
{
	public class ControlDialogTimeOutControl : UserControl
	{
		private IContainer components;

		private Button retryButton;

		private PictureBox pictureBox1;

		private CustomLabel customLabel1;

		private Label label1;

		public ControlDialogTimeOutControl()
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
			this.components = new Container();
			this.customLabel1 = new CustomLabel(this.components);
			this.retryButton = new Button();
			this.pictureBox1 = new PictureBox();
			this.label1 = new Label();
			((ISupportInitialize)this.pictureBox1).BeginInit();
			base.SuspendLayout();
			this.customLabel1.Font = new Font("微软雅黑", 16f, FontStyle.Regular, GraphicsUnit.Pixel);
			this.customLabel1.LineDistance = 3;
			this.customLabel1.Location = new Point(130, 25);
			this.customLabel1.Name = "customLabel1";
			this.customLabel1.Size = new Size(353, 50);
			this.customLabel1.TabIndex = 14;
			this.customLabel1.Text = "系统连接超时，请将移动电源断开再重新接入，信号指示灯再次变为闪烁后点击“重试”。";
			this.retryButton.BackgroundImage = Resources.blue_background_0;
			this.retryButton.BackgroundImageLayout = ImageLayout.Stretch;
			this.retryButton.FlatAppearance.BorderSize = 0;
			this.retryButton.FlatStyle = FlatStyle.Flat;
			this.retryButton.Font = new Font("微软雅黑", 20f, FontStyle.Regular, GraphicsUnit.Pixel);
			this.retryButton.ForeColor = Color.White;
			this.retryButton.Location = new Point(150, 169);
			this.retryButton.Name = "retryButton";
			this.retryButton.Size = new Size(200, 40);
			this.retryButton.TabIndex = 13;
			this.retryButton.Text = "重试";
			this.retryButton.UseVisualStyleBackColor = false;
			this.pictureBox1.Image = Resources.control_exception;
			this.pictureBox1.Location = new Point(66, 24);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new Size(46, 46);
			this.pictureBox1.TabIndex = 11;
			this.pictureBox1.TabStop = false;
			this.label1.BackColor = Color.FromArgb(216, 216, 216);
			this.label1.Location = new Point(50, 99);
			this.label1.Name = "label1";
			this.label1.Size = new Size(400, 1);
			this.label1.TabIndex = 23;
			this.label1.Text = "label1";
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.White;
			base.Controls.Add(this.label1);
			base.Controls.Add(this.customLabel1);
			base.Controls.Add(this.retryButton);
			base.Controls.Add(this.pictureBox1);
			base.Name = "ControlDialogTimeOutControl";
			base.Size = new Size(500, 244);
			((ISupportInitialize)this.pictureBox1).EndInit();
			base.ResumeLayout(false);
		}
	}
}
