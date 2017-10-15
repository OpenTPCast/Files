using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using TPCASTWindows.Properties;

namespace TPCASTWindows
{
	public class BluetoothDialogDriverControl : UserControl
	{
		private IContainer components;

		private Button installDriver;

		private PictureBox pictureBox1;

		private CustomLabel customLabel1;

		private Label label2;

		public BluetoothDialogDriverControl()
		{
			this.InitializeComponent();
		}

		private void installDriver_Click(object sender, EventArgs e)
		{
			Process.Start("http://www.tpcast.cn/index.php?s=/Front/Public/download/?p1=3&p2=0");
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
			this.label2 = new Label();
			this.installDriver = new Button();
			this.pictureBox1 = new PictureBox();
			((ISupportInitialize)this.pictureBox1).BeginInit();
			base.SuspendLayout();
			this.customLabel1.Font = new Font("微软雅黑", 16f, FontStyle.Regular, GraphicsUnit.Pixel);
			this.customLabel1.LineDistance = 3;
			this.customLabel1.Location = new Point(130, 25);
			this.customLabel1.Name = "customLabel1";
			this.customLabel1.Size = new Size(304, 50);
			this.customLabel1.TabIndex = 14;
			this.customLabel1.Text = "检测到VR声音连接驱动启动异常或存在冲突，请重新安装驱动。";
			this.label2.BackColor = Color.FromArgb(216, 216, 216);
			this.label2.Location = new Point(50, 99);
			this.label2.Name = "label2";
			this.label2.Size = new Size(400, 1);
			this.label2.TabIndex = 15;
			this.label2.Text = "label2";
			this.installDriver.BackgroundImage = Resources.blue_background_0;
			this.installDriver.BackgroundImageLayout = ImageLayout.Stretch;
			this.installDriver.FlatAppearance.BorderSize = 0;
			this.installDriver.FlatStyle = FlatStyle.Flat;
			this.installDriver.Font = new Font("微软雅黑", 20f, FontStyle.Regular, GraphicsUnit.Pixel);
			this.installDriver.ForeColor = Color.White;
			this.installDriver.Location = new Point(150, 169);
			this.installDriver.Name = "installDriver";
			this.installDriver.Size = new Size(200, 40);
			this.installDriver.TabIndex = 13;
			this.installDriver.Text = "安装驱动";
			this.installDriver.UseVisualStyleBackColor = false;
			this.installDriver.Click += new EventHandler(this.installDriver_Click);
			this.pictureBox1.Image = Resources.exception;
			this.pictureBox1.Location = new Point(66, 24);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new Size(46, 46);
			this.pictureBox1.TabIndex = 11;
			this.pictureBox1.TabStop = false;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.White;
			base.Controls.Add(this.label2);
			base.Controls.Add(this.customLabel1);
			base.Controls.Add(this.installDriver);
			base.Controls.Add(this.pictureBox1);
			base.Name = "BluetoothDialogDriverControl";
			base.Size = new Size(500, 244);
			((ISupportInitialize)this.pictureBox1).EndInit();
			base.ResumeLayout(false);
		}
	}
}
