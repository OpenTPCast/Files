using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TPCASTWindows.Properties;

namespace TPCASTWindows
{
	public class SteamDialog : BaseDialogForm
	{
		public delegate void OnRetryDelegate();

		public SteamDialog.OnRetryDelegate OnRetry;

		private IContainer components;

		private Button retryButton;

		private PictureBox pictureBox1;

		private CustomLabel customLabel1;

		private Label label1;

		public SteamDialog()
		{
			this.InitializeComponent();
		}

		private void retryButton_Click(object sender, EventArgs e)
		{
			base.Close();
			base.Dispose();
			if (this.OnRetry != null)
			{
				this.OnRetry();
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
			this.retryButton = new Button();
			this.pictureBox1 = new PictureBox();
			this.customLabel1 = new CustomLabel(this.components);
			this.label1 = new Label();
			((ISupportInitialize)this.pictureBox1).BeginInit();
			base.SuspendLayout();
			this.closeButton.FlatAppearance.BorderSize = 0;
			this.retryButton.BackgroundImage = Resources.blue_background_0;
			this.retryButton.BackgroundImageLayout = ImageLayout.Stretch;
			this.retryButton.FlatAppearance.BorderSize = 0;
			this.retryButton.FlatStyle = FlatStyle.Flat;
			this.retryButton.Font = new Font("微软雅黑", 20f, FontStyle.Regular, GraphicsUnit.Pixel);
			this.retryButton.ForeColor = Color.White;
			this.retryButton.Location = new Point(150, 189);
			this.retryButton.Name = "retryButton";
			this.retryButton.Size = new Size(200, 40);
			this.retryButton.TabIndex = 10;
			this.retryButton.Text = "重试";
			this.retryButton.UseVisualStyleBackColor = false;
			this.retryButton.Click += new EventHandler(this.retryButton_Click);
			this.pictureBox1.Image = Resources.exception;
			this.pictureBox1.Location = new Point(66, 44);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new Size(46, 46);
			this.pictureBox1.TabIndex = 8;
			this.pictureBox1.TabStop = false;
			this.customLabel1.Font = new Font("微软雅黑", 16f, FontStyle.Regular, GraphicsUnit.Pixel);
			this.customLabel1.LineDistance = 3;
			this.customLabel1.Location = new Point(130, 45);
			this.customLabel1.Name = "customLabel1";
			this.customLabel1.Size = new Size(320, 50);
			this.customLabel1.TabIndex = 11;
			this.customLabel1.Text = "未能正常关联到您的Steam VR，请确认安装正常。";
			this.label1.BackColor = Color.FromArgb(216, 216, 216);
			this.label1.Location = new Point(50, 119);
			this.label1.Name = "label1";
			this.label1.Size = new Size(400, 1);
			this.label1.TabIndex = 20;
			this.label1.Text = "label1";
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			this.BackColor = Color.White;
			base.ClientSize = new Size(500, 264);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.customLabel1);
			base.Controls.Add(this.retryButton);
			base.Controls.Add(this.pictureBox1);
			base.Name = "SteamDialog";
			base.Controls.SetChildIndex(this.closeButton, 0);
			base.Controls.SetChildIndex(this.pictureBox1, 0);
			base.Controls.SetChildIndex(this.retryButton, 0);
			base.Controls.SetChildIndex(this.customLabel1, 0);
			base.Controls.SetChildIndex(this.label1, 0);
			((ISupportInitialize)this.pictureBox1).EndInit();
			base.ResumeLayout(false);
		}
	}
}
