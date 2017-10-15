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
			ComponentResourceManager arg_5D_0 = new ComponentResourceManager(typeof(ControlDialogTimeOutControl));
			this.customLabel1 = new CustomLabel(this.components);
			this.retryButton = new Button();
			this.pictureBox1 = new PictureBox();
			this.label1 = new Label();
			((ISupportInitialize)this.pictureBox1).BeginInit();
			base.SuspendLayout();
			arg_5D_0.ApplyResources(this.customLabel1, "customLabel1");
			this.customLabel1.LineDistance = 3;
			this.customLabel1.Name = "customLabel1";
			this.retryButton.BackgroundImage = Resources.blue_background_0;
			arg_5D_0.ApplyResources(this.retryButton, "retryButton");
			this.retryButton.FlatAppearance.BorderSize = 0;
			this.retryButton.ForeColor = Color.White;
			this.retryButton.Name = "retryButton";
			this.retryButton.UseVisualStyleBackColor = false;
			this.pictureBox1.Image = Resources.control_exception;
			arg_5D_0.ApplyResources(this.pictureBox1, "pictureBox1");
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.TabStop = false;
			this.label1.BackColor = Color.FromArgb(216, 216, 216);
			arg_5D_0.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			arg_5D_0.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.White;
			base.Controls.Add(this.label1);
			base.Controls.Add(this.customLabel1);
			base.Controls.Add(this.retryButton);
			base.Controls.Add(this.pictureBox1);
			base.Name = "ControlDialogTimeOutControl";
			((ISupportInitialize)this.pictureBox1).EndInit();
			base.ResumeLayout(false);
		}
	}
}
