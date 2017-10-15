using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TPCASTWindows.Properties;

namespace TPCASTWindows.UI.Update
{
	public class UpdateFirmwareFailControl : UserControl
	{
		public delegate void OnRetryClickDelegate();

		public delegate void OnCancelClickDelegate();

		public UpdateFirmwareFailControl.OnRetryClickDelegate OnRetryClick;

		public UpdateFirmwareFailControl.OnCancelClickDelegate OnCancelClick;

		private IContainer components;

		private Label label1;

		private Button cancelButton;

		private CustomLabel customLabel1;

		private Button retryButton;

		private PictureBox pictureBox1;

		public UpdateFirmwareFailControl()
		{
			this.InitializeComponent();
		}

		private void retryButton_Click(object sender, EventArgs e)
		{
			UpdateFirmwareFailControl.OnRetryClickDelegate expr_06 = this.OnRetryClick;
			if (expr_06 == null)
			{
				return;
			}
			expr_06();
		}

		private void cancelButton_Click(object sender, EventArgs e)
		{
			UpdateFirmwareFailControl.OnCancelClickDelegate expr_06 = this.OnCancelClick;
			if (expr_06 == null)
			{
				return;
			}
			expr_06();
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
			ComponentResourceManager arg_87_0 = new ComponentResourceManager(typeof(UpdateFirmwareFailControl));
			this.label1 = new Label();
			this.cancelButton = new Button();
			this.customLabel1 = new CustomLabel(this.components);
			this.retryButton = new Button();
			this.pictureBox1 = new PictureBox();
			((ISupportInitialize)this.pictureBox1).BeginInit();
			base.SuspendLayout();
			this.label1.BackColor = Color.FromArgb(216, 216, 216);
			arg_87_0.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			this.cancelButton.BackgroundImage = Resources.blue_background_1;
			arg_87_0.ApplyResources(this.cancelButton, "cancelButton");
			this.cancelButton.FlatAppearance.BorderSize = 0;
			this.cancelButton.ForeColor = Color.White;
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.UseVisualStyleBackColor = false;
			this.cancelButton.Click += new EventHandler(this.cancelButton_Click);
			arg_87_0.ApplyResources(this.customLabel1, "customLabel1");
			this.customLabel1.LineDistance = 1;
			this.customLabel1.Name = "customLabel1";
			this.retryButton.BackgroundImage = Resources.blue_background_1;
			arg_87_0.ApplyResources(this.retryButton, "retryButton");
			this.retryButton.FlatAppearance.BorderSize = 0;
			this.retryButton.ForeColor = Color.White;
			this.retryButton.Name = "retryButton";
			this.retryButton.UseVisualStyleBackColor = false;
			this.retryButton.Click += new EventHandler(this.retryButton_Click);
			this.pictureBox1.Image = Resources.exception;
			arg_87_0.ApplyResources(this.pictureBox1, "pictureBox1");
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.TabStop = false;
			arg_87_0.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.White;
			base.Controls.Add(this.label1);
			base.Controls.Add(this.cancelButton);
			base.Controls.Add(this.customLabel1);
			base.Controls.Add(this.retryButton);
			base.Controls.Add(this.pictureBox1);
			base.Name = "UpdateFirmwareFailControl";
			((ISupportInitialize)this.pictureBox1).EndInit();
			base.ResumeLayout(false);
		}
	}
}
