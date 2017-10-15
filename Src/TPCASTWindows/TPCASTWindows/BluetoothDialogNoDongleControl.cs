using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TPCASTWindows.Properties;

namespace TPCASTWindows
{
	public class BluetoothDialogNoDongleControl : UserControl
	{
		public delegate void OnRetryButtonClickDelegate();

		public BluetoothDialogNoDongleControl.OnRetryButtonClickDelegate OnRetryClickListener;

		private IContainer components;

		private CustomLabel customLabel1;

		private Button retryButton;

		private PictureBox pictureBox1;

		private Label label1;

		public BluetoothDialogNoDongleControl()
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
			this.retryButton = new Button();
			this.pictureBox1 = new PictureBox();
			this.label1 = new Label();
			((ISupportInitialize)this.pictureBox1).BeginInit();
			base.SuspendLayout();
			this.customLabel1.Font = new Font("微软雅黑", 16f, FontStyle.Regular, GraphicsUnit.Pixel);
			this.customLabel1.LineDistance = 3;
			this.customLabel1.Location = new Point(134, 37);
			this.customLabel1.Name = "customLabel1";
			this.customLabel1.Size = new Size(249, 25);
			this.customLabel1.TabIndex = 17;
			this.customLabel1.Text = "请插入包装内的蓝牙适配器。";
			this.retryButton.BackgroundImage = Resources.blue_background_1;
			this.retryButton.BackgroundImageLayout = ImageLayout.Stretch;
			this.retryButton.FlatAppearance.BorderSize = 0;
			this.retryButton.FlatStyle = FlatStyle.Flat;
			this.retryButton.Font = new Font("微软雅黑", 20f, FontStyle.Regular, GraphicsUnit.Pixel);
			this.retryButton.ForeColor = Color.White;
			this.retryButton.Location = new Point(150, 169);
			this.retryButton.Name = "retryButton";
			this.retryButton.Size = new Size(200, 40);
			this.retryButton.TabIndex = 16;
			this.retryButton.Text = "重试";
			this.retryButton.UseVisualStyleBackColor = false;
			this.retryButton.Click += new EventHandler(this.retryButton_Click);
			this.pictureBox1.BackgroundImage = Resources.exception;
			this.pictureBox1.Location = new Point(66, 24);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new Size(46, 46);
			this.pictureBox1.TabIndex = 15;
			this.pictureBox1.TabStop = false;
			this.label1.BackColor = Color.FromArgb(216, 216, 216);
			this.label1.Location = new Point(50, 99);
			this.label1.Name = "label1";
			this.label1.Size = new Size(400, 1);
			this.label1.TabIndex = 21;
			this.label1.Text = "label1";
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.White;
			base.Controls.Add(this.label1);
			base.Controls.Add(this.customLabel1);
			base.Controls.Add(this.retryButton);
			base.Controls.Add(this.pictureBox1);
			base.Name = "BluetoothDialogNoDongleControl";
			base.Size = new Size(500, 244);
			((ISupportInitialize)this.pictureBox1).EndInit();
			base.ResumeLayout(false);
		}
	}
}
