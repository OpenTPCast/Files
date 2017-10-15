using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TPCASTWindows.Properties;

namespace TPCASTWindows
{
	public class BluetoothDialogControl : UserControl
	{
		public delegate void OnOffButtonDelegate();

		public delegate void OnSlowFlashingButtonDelegate();

		public delegate void OnFastFlashingButtonDelegate();

		public BluetoothDialogControl.OnOffButtonDelegate OnOffButtonClick;

		public BluetoothDialogControl.OnSlowFlashingButtonDelegate OnSlowFlashingClick;

		public BluetoothDialogControl.OnFastFlashingButtonDelegate OnFastFlashingClick;

		private IContainer components;

		private Button offButton;

		private Button slowFlashingbutton;

		private PictureBox pictureBox1;

		private PictureBox pictureBox2;

		private Label label2;

		private Button fastFlashingbutton;

		private CustomLabel customLabel1;

		public BluetoothDialogControl()
		{
			this.InitializeComponent();
		}

		private void offButton_Click(object sender, EventArgs e)
		{
			if (this.OnOffButtonClick != null)
			{
				this.OnOffButtonClick();
			}
		}

		private void slowFlashingbutton_Click(object sender, EventArgs e)
		{
			if (this.OnSlowFlashingClick != null)
			{
				this.OnSlowFlashingClick();
			}
		}

		private void fastFlashingbutton_Click(object sender, EventArgs e)
		{
			if (this.OnFastFlashingClick != null)
			{
				this.OnFastFlashingClick();
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
			this.label2 = new Label();
			this.customLabel1 = new CustomLabel(this.components);
			this.fastFlashingbutton = new Button();
			this.pictureBox2 = new PictureBox();
			this.pictureBox1 = new PictureBox();
			this.slowFlashingbutton = new Button();
			this.offButton = new Button();
			((ISupportInitialize)this.pictureBox2).BeginInit();
			((ISupportInitialize)this.pictureBox1).BeginInit();
			base.SuspendLayout();
			this.label2.BackColor = Color.FromArgb(216, 216, 216);
			this.label2.Location = new Point(50, 99);
			this.label2.Name = "label2";
			this.label2.Size = new Size(400, 1);
			this.label2.TabIndex = 5;
			this.label2.Text = "label2";
			this.customLabel1.Font = new Font("微软雅黑", 16f, FontStyle.Regular, GraphicsUnit.Pixel);
			this.customLabel1.LineDistance = 3;
			this.customLabel1.Location = new Point(130, 25);
			this.customLabel1.Name = "customLabel1";
			this.customLabel1.Size = new Size(302, 50);
			this.customLabel1.TabIndex = 7;
			this.customLabel1.Text = "声音连接未能正常工作，请查看头盔端的蓝色信号指示灯当前状态。";
			this.fastFlashingbutton.BackgroundImage = Resources.flashing;
			this.fastFlashingbutton.BackgroundImageLayout = ImageLayout.Center;
			this.fastFlashingbutton.FlatAppearance.BorderSize = 0;
			this.fastFlashingbutton.FlatStyle = FlatStyle.Flat;
			this.fastFlashingbutton.Font = new Font("微软雅黑", 12f, FontStyle.Regular, GraphicsUnit.Pixel);
			this.fastFlashingbutton.ForeColor = Color.FromArgb(102, 102, 102);
			this.fastFlashingbutton.Location = new Point(329, 128);
			this.fastFlashingbutton.Name = "fastFlashingbutton";
			this.fastFlashingbutton.Size = new Size(70, 70);
			this.fastFlashingbutton.TabIndex = 6;
			this.fastFlashingbutton.Text = "快闪烁";
			this.fastFlashingbutton.TextAlign = ContentAlignment.BottomCenter;
			this.fastFlashingbutton.UseVisualStyleBackColor = true;
			this.fastFlashingbutton.Click += new EventHandler(this.fastFlashingbutton_Click);
			this.pictureBox2.Image = Resources.bluetooth_head;
			this.pictureBox2.Location = new Point(54, 125);
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new Size(115, 73);
			this.pictureBox2.TabIndex = 4;
			this.pictureBox2.TabStop = false;
			this.pictureBox1.Image = Resources.bluetooth_exception;
			this.pictureBox1.Location = new Point(66, 24);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new Size(45, 45);
			this.pictureBox1.TabIndex = 3;
			this.pictureBox1.TabStop = false;
			this.slowFlashingbutton.BackgroundImage = Resources.flashing;
			this.slowFlashingbutton.BackgroundImageLayout = ImageLayout.Center;
			this.slowFlashingbutton.FlatAppearance.BorderSize = 0;
			this.slowFlashingbutton.FlatStyle = FlatStyle.Flat;
			this.slowFlashingbutton.Font = new Font("微软雅黑", 12f, FontStyle.Regular, GraphicsUnit.Pixel);
			this.slowFlashingbutton.ForeColor = Color.FromArgb(102, 102, 102);
			this.slowFlashingbutton.Location = new Point(281, 194);
			this.slowFlashingbutton.Name = "slowFlashingbutton";
			this.slowFlashingbutton.Size = new Size(50, 42);
			this.slowFlashingbutton.TabIndex = 1;
			this.slowFlashingbutton.Text = "慢闪烁";
			this.slowFlashingbutton.TextAlign = ContentAlignment.BottomCenter;
			this.slowFlashingbutton.UseVisualStyleBackColor = true;
			this.slowFlashingbutton.Visible = false;
			this.slowFlashingbutton.Click += new EventHandler(this.slowFlashingbutton_Click);
			this.offButton.BackgroundImage = Resources.off;
			this.offButton.BackgroundImageLayout = ImageLayout.Center;
			this.offButton.FlatAppearance.BorderSize = 0;
			this.offButton.FlatStyle = FlatStyle.Flat;
			this.offButton.Font = new Font("微软雅黑", 12f, FontStyle.Regular, GraphicsUnit.Pixel);
			this.offButton.ForeColor = Color.FromArgb(102, 102, 102);
			this.offButton.Location = new Point(205, 128);
			this.offButton.Name = "offButton";
			this.offButton.Size = new Size(70, 70);
			this.offButton.TabIndex = 0;
			this.offButton.Text = "熄灭";
			this.offButton.TextAlign = ContentAlignment.BottomCenter;
			this.offButton.UseVisualStyleBackColor = true;
			this.offButton.Click += new EventHandler(this.offButton_Click);
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.White;
			base.Controls.Add(this.customLabel1);
			base.Controls.Add(this.fastFlashingbutton);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.pictureBox2);
			base.Controls.Add(this.pictureBox1);
			base.Controls.Add(this.slowFlashingbutton);
			base.Controls.Add(this.offButton);
			base.Name = "BluetoothDialogControl";
			base.Size = new Size(500, 244);
			((ISupportInitialize)this.pictureBox2).EndInit();
			((ISupportInitialize)this.pictureBox1).EndInit();
			base.ResumeLayout(false);
		}
	}
}
