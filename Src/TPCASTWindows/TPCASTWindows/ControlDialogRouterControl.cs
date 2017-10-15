using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TPCASTWindows.Properties;

namespace TPCASTWindows
{
	public class ControlDialogRouterControl : UserControl
	{
		public delegate void RetryButtonClickDelegate();

		public ControlDialogRouterControl.RetryButtonClickDelegate OnRetryClick;

		private IContainer components;

		private PictureBox pictureBox1;

		private Button retryButton;

		private PictureBox pictureBox2;

		private CustomLabel customLabel1;

		private Label label1;

		public ControlDialogRouterControl()
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
			ComponentResourceManager arg_73_0 = new ComponentResourceManager(typeof(ControlDialogRouterControl));
			this.customLabel1 = new CustomLabel(this.components);
			this.pictureBox2 = new PictureBox();
			this.retryButton = new Button();
			this.pictureBox1 = new PictureBox();
			this.label1 = new Label();
			((ISupportInitialize)this.pictureBox2).BeginInit();
			((ISupportInitialize)this.pictureBox1).BeginInit();
			base.SuspendLayout();
			arg_73_0.ApplyResources(this.customLabel1, "customLabel1");
			this.customLabel1.LineDistance = 1;
			this.customLabel1.Name = "customLabel1";
			this.pictureBox2.Image = Resources.control_router;
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
			this.pictureBox1.Image = Resources.exception;
			arg_73_0.ApplyResources(this.pictureBox1, "pictureBox1");
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.TabStop = false;
			this.label1.BackColor = Color.FromArgb(216, 216, 216);
			arg_73_0.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			arg_73_0.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.White;
			base.Controls.Add(this.label1);
			base.Controls.Add(this.customLabel1);
			base.Controls.Add(this.pictureBox2);
			base.Controls.Add(this.retryButton);
			base.Controls.Add(this.pictureBox1);
			base.Name = "ControlDialogRouterControl";
			((ISupportInitialize)this.pictureBox2).EndInit();
			((ISupportInitialize)this.pictureBox1).EndInit();
			base.ResumeLayout(false);
		}
	}
}
