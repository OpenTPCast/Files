using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TPCASTWindows.Properties;

namespace TPCASTWindows
{
	public class ControlDialogOffControl : UserControl
	{
		public delegate void RetryButtonClickDelegate();

		public ControlDialogOffControl.RetryButtonClickDelegate OnRetryClick;

		private IContainer components;

		private Button retryButton;

		private PictureBox pictureBox1;

		private PictureBox pictureBox2;

		private CustomLabel customLabel1;

		private Label label2;

		public ControlDialogOffControl()
		{
			this.InitializeComponent();
		}

		private void retryButton_Click(object sender, EventArgs e)
		{
			if (this.OnRetryClick != null)
			{
				this.OnRetryClick();
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
			ComponentResourceManager arg_73_0 = new ComponentResourceManager(typeof(ControlDialogOffControl));
			this.customLabel1 = new CustomLabel(this.components);
			this.label2 = new Label();
			this.pictureBox2 = new PictureBox();
			this.retryButton = new Button();
			this.pictureBox1 = new PictureBox();
			((ISupportInitialize)this.pictureBox2).BeginInit();
			((ISupportInitialize)this.pictureBox1).BeginInit();
			base.SuspendLayout();
			arg_73_0.ApplyResources(this.customLabel1, "customLabel1");
			this.customLabel1.LineDistance = 1;
			this.customLabel1.Name = "customLabel1";
			this.label2.BackColor = Color.FromArgb(216, 216, 216);
			arg_73_0.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			this.pictureBox2.Image = Resources.control_off;
			arg_73_0.ApplyResources(this.pictureBox2, "pictureBox2");
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.TabStop = false;
			this.retryButton.BackgroundImage = Resources.blue_background_1;
			arg_73_0.ApplyResources(this.retryButton, "retryButton");
			this.retryButton.FlatAppearance.BorderSize = 0;
			this.retryButton.ForeColor = Color.White;
			this.retryButton.Name = "retryButton";
			this.retryButton.UseVisualStyleBackColor = false;
			this.retryButton.Click += new EventHandler(this.retryButton_Click);
			this.pictureBox1.Image = Resources.control_exception;
			arg_73_0.ApplyResources(this.pictureBox1, "pictureBox1");
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.TabStop = false;
			arg_73_0.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.White;
			base.Controls.Add(this.label2);
			base.Controls.Add(this.customLabel1);
			base.Controls.Add(this.pictureBox2);
			base.Controls.Add(this.retryButton);
			base.Controls.Add(this.pictureBox1);
			base.Name = "ControlDialogOffControl";
			((ISupportInitialize)this.pictureBox2).EndInit();
			((ISupportInitialize)this.pictureBox1).EndInit();
			base.ResumeLayout(false);
		}
	}
}
