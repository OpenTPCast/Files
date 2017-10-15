using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TPCASTWindows.Properties;

namespace TPCASTWindows
{
	public class BluetoothDialogFlashingSlowControl : UserControl
	{
		public delegate void OnRetryButtonClickDelegate();

		public BluetoothDialogFlashingSlowControl.OnRetryButtonClickDelegate OnRetryClickListener;

		private IContainer components;

		private PictureBox pictureBox2;

		private Button retryButton;

		private PictureBox pictureBox1;

		private CustomLabel customLabel1;

		public BluetoothDialogFlashingSlowControl()
		{
			this.InitializeComponent();
		}

		private void retryButton_Click(object sender, EventArgs e)
		{
			if (this.OnRetryClickListener != null)
			{
				this.OnRetryClickListener();
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
			this.components = new Container();
			this.customLabel1 = new CustomLabel(this.components);
			this.pictureBox2 = new PictureBox();
			this.retryButton = new Button();
			this.pictureBox1 = new PictureBox();
			((ISupportInitialize)this.pictureBox2).BeginInit();
			((ISupportInitialize)this.pictureBox1).BeginInit();
			base.SuspendLayout();
			this.customLabel1.Font = new Font("微软雅黑", 9f);
			this.customLabel1.LineDistance = 3;
			this.customLabel1.Location = new Point(75, 0);
			this.customLabel1.Name = "customLabel1";
			this.customLabel1.Size = new Size(242, 57);
			this.customLabel1.TabIndex = 21;
			this.customLabel1.Text = "请在通电状态下，使用回形针等针状物按住复位孔，直至蓝色指示灯变为慢闪烁后，点击重试。";
			this.pictureBox2.Image = Resources.bluetooth_flashing_slow;
			this.pictureBox2.Location = new Point(75, 76);
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new Size(137, 47);
			this.pictureBox2.TabIndex = 20;
			this.pictureBox2.TabStop = false;
			this.retryButton.BackColor = Color.White;
			this.retryButton.BackgroundImage = Resources.blue_background_0;
			this.retryButton.BackgroundImageLayout = ImageLayout.Stretch;
			this.retryButton.FlatAppearance.BorderSize = 0;
			this.retryButton.FlatStyle = FlatStyle.Flat;
			this.retryButton.Font = new Font("微软雅黑", 12f, FontStyle.Regular, GraphicsUnit.Pixel);
			this.retryButton.ForeColor = Color.White;
			this.retryButton.Location = new Point(260, 125);
			this.retryButton.Name = "retryButton";
			this.retryButton.Size = new Size(50, 23);
			this.retryButton.TabIndex = 19;
			this.retryButton.Text = "重试";
			this.retryButton.UseVisualStyleBackColor = false;
			this.retryButton.Click += new EventHandler(this.retryButton_Click);
			this.pictureBox1.Image = Resources.bluetooth_exception;
			this.pictureBox1.Location = new Point(66, 24);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new Size(45, 45);
			this.pictureBox1.TabIndex = 18;
			this.pictureBox1.TabStop = false;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.Controls.Add(this.customLabel1);
			base.Controls.Add(this.pictureBox2);
			base.Controls.Add(this.retryButton);
			base.Controls.Add(this.pictureBox1);
			base.Name = "BluetoothDialogFlashingSlowControl";
			base.Size = new Size(500, 244);
			((ISupportInitialize)this.pictureBox2).EndInit();
			((ISupportInitialize)this.pictureBox1).EndInit();
			base.ResumeLayout(false);
		}
	}
}
