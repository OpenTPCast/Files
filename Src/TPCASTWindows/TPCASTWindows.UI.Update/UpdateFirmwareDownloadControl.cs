using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TPCASTWindows.Custom;
using TPCASTWindows.Properties;

namespace TPCASTWindows.UI.Update
{
	public class UpdateFirmwareDownloadControl : UserControl
	{
		public delegate void OnContinueClickDelegate();

		public UpdateFirmwareDownloadControl.OnContinueClickDelegate OnContinueClick;

		private IContainer components;

		private Button continueButton;

		private CustomProgressBar customProgressBar;

		private CustomLabel messageLabel;

		public UpdateFirmwareDownloadControl()
		{
			this.InitializeComponent();
		}

		public void setProgress(int value)
		{
			this.customProgressBar.Value = value;
		}

		public void setContinueButtonVisble(bool visibility)
		{
			this.continueButton.Visible = visibility;
		}

		public void setMessage(string message)
		{
			this.messageLabel.Text = message;
		}

		private void continueButton_Click(object sender, EventArgs e)
		{
			UpdateFirmwareDownloadControl.OnContinueClickDelegate expr_06 = this.OnContinueClick;
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
			ComponentResourceManager arg_57_0 = new ComponentResourceManager(typeof(UpdateFirmwareDownloadControl));
			this.continueButton = new Button();
			this.customProgressBar = new CustomProgressBar();
			this.messageLabel = new CustomLabel(this.components);
			base.SuspendLayout();
			this.continueButton.BackgroundImage = Resources.blue_background_1;
			arg_57_0.ApplyResources(this.continueButton, "continueButton");
			this.continueButton.FlatAppearance.BorderSize = 0;
			this.continueButton.ForeColor = Color.White;
			this.continueButton.Name = "continueButton";
			this.continueButton.UseVisualStyleBackColor = false;
			this.continueButton.Click += new EventHandler(this.continueButton_Click);
			this.customProgressBar.BackColor = Color.FromArgb(216, 216, 216);
			this.customProgressBar.ForeColor = Color.FromArgb(42, 173, 223);
			arg_57_0.ApplyResources(this.customProgressBar, "customProgressBar");
			this.customProgressBar.Name = "customProgressBar";
			arg_57_0.ApplyResources(this.messageLabel, "messageLabel");
			this.messageLabel.LineDistance = 1;
			this.messageLabel.Name = "messageLabel";
			arg_57_0.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.White;
			base.Controls.Add(this.continueButton);
			base.Controls.Add(this.customProgressBar);
			base.Controls.Add(this.messageLabel);
			base.Name = "UpdateFirmwareDownloadControl";
			base.ResumeLayout(false);
		}
	}
}
