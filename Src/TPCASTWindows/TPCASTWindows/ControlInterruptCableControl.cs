using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TPCASTWindows.Properties;

namespace TPCASTWindows
{
	public class ControlInterruptCableControl : UserControl
	{
		public delegate void OnRetryDelegate();

		public ControlInterruptCableControl.OnRetryDelegate OnRetry;

		private IContainer components;

		private CustomLabel customLabel2;

		private Label label1;

		private Button retryButton;

		private PictureBox pictureBox1;

		public ControlInterruptCableControl()
		{
			this.InitializeComponent();
		}

		private void retryButton_Click(object sender, EventArgs e)
		{
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
			ComponentResourceManager arg_5D_0 = new ComponentResourceManager(typeof(ControlInterruptCableControl));
			this.customLabel2 = new CustomLabel(this.components);
			this.label1 = new Label();
			this.retryButton = new Button();
			this.pictureBox1 = new PictureBox();
			((ISupportInitialize)this.pictureBox1).BeginInit();
			base.SuspendLayout();
			arg_5D_0.ApplyResources(this.customLabel2, "customLabel2");
			this.customLabel2.LineDistance = 1;
			this.customLabel2.Name = "customLabel2";
			arg_5D_0.ApplyResources(this.label1, "label1");
			this.label1.BackColor = Color.FromArgb(216, 216, 216);
			this.label1.Name = "label1";
			arg_5D_0.ApplyResources(this.retryButton, "retryButton");
			this.retryButton.BackgroundImage = Resources.blue_background_1;
			this.retryButton.FlatAppearance.BorderSize = 0;
			this.retryButton.ForeColor = Color.White;
			this.retryButton.Name = "retryButton";
			this.retryButton.UseVisualStyleBackColor = false;
			this.retryButton.Click += new EventHandler(this.retryButton_Click);
			arg_5D_0.ApplyResources(this.pictureBox1, "pictureBox1");
			this.pictureBox1.Image = Resources.exception;
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.TabStop = false;
			arg_5D_0.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.White;
			base.Controls.Add(this.customLabel2);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.retryButton);
			base.Controls.Add(this.pictureBox1);
			base.Name = "ControlInterruptCableControl";
			((ISupportInitialize)this.pictureBox1).EndInit();
			base.ResumeLayout(false);
		}
	}
}
