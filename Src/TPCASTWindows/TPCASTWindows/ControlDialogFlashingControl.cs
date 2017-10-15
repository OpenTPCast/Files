using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TPCASTWindows.Properties;

namespace TPCASTWindows
{
	public class ControlDialogFlashingControl : UserControl
	{
		public delegate void RetryButtonClickDelegate();

		public ControlDialogFlashingControl.RetryButtonClickDelegate OnRetryClick;

		private IContainer components;

		private PictureBox pictureBox1;

		private Label label1;

		private Button retryButton;

		private Label label2;

		public ControlDialogFlashingControl()
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
			ComponentResourceManager arg_4C_0 = new ComponentResourceManager(typeof(ControlDialogFlashingControl));
			this.label1 = new Label();
			this.label2 = new Label();
			this.retryButton = new Button();
			this.pictureBox1 = new PictureBox();
			((ISupportInitialize)this.pictureBox1).BeginInit();
			base.SuspendLayout();
			arg_4C_0.ApplyResources(this.label1, "label1");
			this.label1.ForeColor = Color.FromArgb(25, 25, 25);
			this.label1.Name = "label1";
			arg_4C_0.ApplyResources(this.label2, "label2");
			this.label2.BackColor = Color.FromArgb(216, 216, 216);
			this.label2.Name = "label2";
			arg_4C_0.ApplyResources(this.retryButton, "retryButton");
			this.retryButton.BackgroundImage = Resources.blue_background_0;
			this.retryButton.FlatAppearance.BorderSize = 0;
			this.retryButton.ForeColor = Color.White;
			this.retryButton.Name = "retryButton";
			this.retryButton.UseVisualStyleBackColor = false;
			this.retryButton.Click += new EventHandler(this.retryButton_Click);
			arg_4C_0.ApplyResources(this.pictureBox1, "pictureBox1");
			this.pictureBox1.Image = Resources.control_exception;
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.TabStop = false;
			arg_4C_0.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.White;
			base.Controls.Add(this.label2);
			base.Controls.Add(this.retryButton);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.pictureBox1);
			base.Name = "ControlDialogFlashingControl";
			((ISupportInitialize)this.pictureBox1).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
