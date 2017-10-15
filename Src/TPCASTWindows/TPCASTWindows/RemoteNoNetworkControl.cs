using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TPCASTWindows.Properties;

namespace TPCASTWindows
{
	public class RemoteNoNetworkControl : UserControl
	{
		public delegate void OnOkClickDelegate();

		public RemoteNoNetworkControl.OnOkClickDelegate OnOkClick;

		private IContainer components;

		private Label label2;

		private CustomLabel customLabel1;

		private Button okButton;

		private PictureBox pictureBox1;

		public RemoteNoNetworkControl()
		{
			this.InitializeComponent();
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			if (this.OnOkClick != null)
			{
				this.OnOkClick();
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
			this.okButton = new Button();
			this.pictureBox1 = new PictureBox();
			this.customLabel1 = new CustomLabel(this.components);
			((ISupportInitialize)this.pictureBox1).BeginInit();
			base.SuspendLayout();
			this.label2.BackColor = Color.FromArgb(216, 216, 216);
			this.label2.Location = new Point(50, 105);
			this.label2.Name = "label2";
			this.label2.Size = new Size(400, 1);
			this.label2.TabIndex = 23;
			this.label2.Text = "label2";
			this.okButton.BackgroundImage = Resources.blue_background_0;
			this.okButton.BackgroundImageLayout = ImageLayout.Stretch;
			this.okButton.FlatAppearance.BorderSize = 0;
			this.okButton.FlatStyle = FlatStyle.Flat;
			this.okButton.Font = new Font("微软雅黑", 20f, FontStyle.Regular, GraphicsUnit.Pixel);
			this.okButton.ForeColor = Color.White;
			this.okButton.Location = new Point(150, 175);
			this.okButton.Name = "okButton";
			this.okButton.Size = new Size(200, 40);
			this.okButton.TabIndex = 21;
			this.okButton.Text = "确定";
			this.okButton.UseVisualStyleBackColor = false;
			this.okButton.Click += new EventHandler(this.okButton_Click);
			this.pictureBox1.Image = Resources.exception;
			this.pictureBox1.Location = new Point(66, 30);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new Size(46, 46);
			this.pictureBox1.TabIndex = 20;
			this.pictureBox1.TabStop = false;
			this.customLabel1.Font = new Font("微软雅黑", 16f, FontStyle.Regular, GraphicsUnit.Pixel);
			this.customLabel1.LineDistance = 3;
			this.customLabel1.Location = new Point(130, 30);
			this.customLabel1.Name = "customLabel1";
			this.customLabel1.Size = new Size(304, 50);
			this.customLabel1.TabIndex = 22;
			this.customLabel1.Text = "检测到无线套件未连接到网络，请稍后再试或重新插拔电源盒。";
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.Controls.Add(this.label2);
			base.Controls.Add(this.customLabel1);
			base.Controls.Add(this.okButton);
			base.Controls.Add(this.pictureBox1);
			base.Name = "RemoteNoNetworkControl";
			base.Size = new Size(500, 244);
			((ISupportInitialize)this.pictureBox1).EndInit();
			base.ResumeLayout(false);
		}
	}
}
