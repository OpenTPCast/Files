using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TPCASTWindows.Properties;

namespace TPCASTWindows
{
	public class BluetoothDialogFlashingFastControl : UserControl
	{
		public delegate void OnRetryButtonClickDelegate();

		public delegate void OnSkipBluetoothClickDelegate();

		public bool showSkip;

		public BluetoothDialogFlashingFastControl.OnRetryButtonClickDelegate OnRetryClickListener;

		public BluetoothDialogFlashingFastControl.OnSkipBluetoothClickDelegate OnSkipBluetoothClick;

		private IContainer components;

		private CustomLabel customLabel1;

		private PictureBox pictureBox1;

		private Button retryButton;

		private Label label1;

		private PictureBox pictureBox2;

		private Label skipUnderline;

		private Label skipBluetooth;

		public BluetoothDialogFlashingFastControl()
		{
			this.InitializeComponent();
			if (this.showSkip)
			{
				this.skipBluetooth.Visible = true;
				this.skipUnderline.Visible = true;
			}
		}

		private void retryButton_Click(object sender, EventArgs e)
		{
			if (this.OnRetryClickListener != null)
			{
				this.OnRetryClickListener();
			}
		}

		private void skipBluetooth_Click(object sender, EventArgs e)
		{
			if (this.OnSkipBluetoothClick != null)
			{
				this.OnSkipBluetoothClick();
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
			this.label1 = new Label();
			this.pictureBox2 = new PictureBox();
			this.retryButton = new Button();
			this.pictureBox1 = new PictureBox();
			this.skipUnderline = new Label();
			this.skipBluetooth = new Label();
			((ISupportInitialize)this.pictureBox2).BeginInit();
			((ISupportInitialize)this.pictureBox1).BeginInit();
			base.SuspendLayout();
			this.customLabel1.Font = new Font("微软雅黑", 16f, FontStyle.Regular, GraphicsUnit.Pixel);
			this.customLabel1.LineDistance = 3;
			this.customLabel1.Location = new Point(130, 14);
			this.customLabel1.Name = "customLabel1";
			this.customLabel1.Size = new Size(320, 75);
			this.customLabel1.TabIndex = 17;
			this.customLabel1.Text = "电脑未能正常连接头盔端蓝牙音频，请屏幕右下角的蓝牙功能中选择TPCAST耳机，进 行连接。";
			this.label1.BackColor = Color.FromArgb(216, 216, 216);
			this.label1.Location = new Point(50, 99);
			this.label1.Name = "label1";
			this.label1.Size = new Size(400, 1);
			this.label1.TabIndex = 19;
			this.label1.Text = "label1";
			this.pictureBox2.Image = Resources.bluetooth_guide;
			this.pictureBox2.Location = new Point(60, 126);
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new Size(259, 92);
			this.pictureBox2.TabIndex = 20;
			this.pictureBox2.TabStop = false;
			this.retryButton.BackgroundImage = Resources.blue_background_0;
			this.retryButton.BackgroundImageLayout = ImageLayout.Stretch;
			this.retryButton.FlatAppearance.BorderSize = 0;
			this.retryButton.FlatStyle = FlatStyle.Flat;
			this.retryButton.Font = new Font("微软雅黑", 20f, FontStyle.Regular, GraphicsUnit.Pixel);
			this.retryButton.ForeColor = Color.White;
			this.retryButton.Location = new Point(346, 155);
			this.retryButton.Name = "retryButton";
			this.retryButton.Size = new Size(126, 40);
			this.retryButton.TabIndex = 18;
			this.retryButton.Text = "重试";
			this.retryButton.UseVisualStyleBackColor = false;
			this.retryButton.Click += new EventHandler(this.retryButton_Click);
			this.pictureBox1.Image = Resources.bluetooth_exception;
			this.pictureBox1.Location = new Point(66, 24);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new Size(45, 45);
			this.pictureBox1.TabIndex = 15;
			this.pictureBox1.TabStop = false;
			this.skipUnderline.BackColor = Color.FromArgb(42, 173, 223);
			this.skipUnderline.Font = new Font("微软雅黑", 10.5f, FontStyle.Regular, GraphicsUnit.Point, 134);
			this.skipUnderline.Location = new Point(346, 230);
			this.skipUnderline.Name = "skipUnderline";
			this.skipUnderline.Size = new Size(145, 1);
			this.skipUnderline.TabIndex = 24;
			this.skipUnderline.Text = "暂不使用无线音频 >>";
			this.skipBluetooth.AutoSize = true;
			this.skipBluetooth.Font = new Font("微软雅黑", 14f, FontStyle.Regular, GraphicsUnit.Pixel, 134);
			this.skipBluetooth.Location = new Point(346, 210);
			this.skipBluetooth.Name = "skipBluetooth";
			this.skipBluetooth.Size = new Size(145, 20);
			this.skipBluetooth.TabIndex = 23;
			this.skipBluetooth.Text = "暂不使用无线音频 >>";
			this.skipBluetooth.Click += new EventHandler(this.skipBluetooth_Click);
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.White;
			base.Controls.Add(this.skipUnderline);
			base.Controls.Add(this.skipBluetooth);
			base.Controls.Add(this.pictureBox2);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.retryButton);
			base.Controls.Add(this.customLabel1);
			base.Controls.Add(this.pictureBox1);
			base.Name = "BluetoothDialogFlashingFastControl";
			base.Size = new Size(500, 244);
			((ISupportInitialize)this.pictureBox2).EndInit();
			((ISupportInitialize)this.pictureBox1).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
