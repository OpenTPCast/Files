using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TPCASTWindows.Properties;

namespace TPCASTWindows.UI.Update
{
	public class UpdateCheckControl : UserControl
	{
		public delegate void OnSoftwareClickDelegate();

		public delegate void OnFirmwareClickDelegate();

		public delegate void OnOkClickDelegate();

		public UpdateCheckControl.OnSoftwareClickDelegate OnSoftwareClick;

		public UpdateCheckControl.OnFirmwareClickDelegate OnFirmwareClick;

		public UpdateCheckControl.OnOkClickDelegate OnOkClick;

		private IContainer components;

		private Label softwareLabel;

		private Label firmwareLabel;

		private Button softwareButton;

		private Button firmwareButton;

		private Button okButton;

		public UpdateCheckControl()
		{
			this.InitializeComponent();
		}

		public void setSoftwareLabel(string message)
		{
			this.softwareLabel.Text = message;
		}

		public string getSoftwareLabel()
		{
			return this.softwareLabel.Text;
		}

		public void setFirmwareLabel(string message)
		{
			this.firmwareLabel.Text = message;
		}

		public string getFirmwareLabel()
		{
			return this.firmwareLabel.Text;
		}

		public void setSoftwareButtonVisibility(bool visibility)
		{
			this.softwareButton.Visible = visibility;
		}

		public void setFirmwareButtonVisibility(bool visibility)
		{
			this.firmwareButton.Visible = visibility;
		}

		private void softwareButton_Click(object sender, EventArgs e)
		{
			UpdateCheckControl.OnSoftwareClickDelegate expr_06 = this.OnSoftwareClick;
			if (expr_06 == null)
			{
				return;
			}
			expr_06();
		}

		private void firmwareButton_Click(object sender, EventArgs e)
		{
			UpdateCheckControl.OnFirmwareClickDelegate expr_06 = this.OnFirmwareClick;
			if (expr_06 == null)
			{
				return;
			}
			expr_06();
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			UpdateCheckControl.OnOkClickDelegate expr_06 = this.OnOkClick;
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
			ComponentResourceManager arg_4C_0 = new ComponentResourceManager(typeof(UpdateCheckControl));
			this.softwareLabel = new Label();
			this.firmwareLabel = new Label();
			this.firmwareButton = new Button();
			this.softwareButton = new Button();
			this.okButton = new Button();
			base.SuspendLayout();
			arg_4C_0.ApplyResources(this.softwareLabel, "softwareLabel");
			this.softwareLabel.Name = "softwareLabel";
			arg_4C_0.ApplyResources(this.firmwareLabel, "firmwareLabel");
			this.firmwareLabel.Name = "firmwareLabel";
			arg_4C_0.ApplyResources(this.firmwareButton, "firmwareButton");
			this.firmwareButton.BackgroundImage = Resources.blue_background_5;
			this.firmwareButton.FlatAppearance.BorderSize = 0;
			this.firmwareButton.ForeColor = Color.White;
			this.firmwareButton.Name = "firmwareButton";
			this.firmwareButton.UseVisualStyleBackColor = true;
			this.firmwareButton.Click += new EventHandler(this.firmwareButton_Click);
			arg_4C_0.ApplyResources(this.softwareButton, "softwareButton");
			this.softwareButton.BackgroundImage = Resources.blue_background_5;
			this.softwareButton.FlatAppearance.BorderSize = 0;
			this.softwareButton.ForeColor = Color.White;
			this.softwareButton.Name = "softwareButton";
			this.softwareButton.UseVisualStyleBackColor = true;
			this.softwareButton.Click += new EventHandler(this.softwareButton_Click);
			arg_4C_0.ApplyResources(this.okButton, "okButton");
			this.okButton.BackgroundImage = Resources.blue_background_1;
			this.okButton.FlatAppearance.BorderSize = 0;
			this.okButton.ForeColor = Color.White;
			this.okButton.Name = "okButton";
			this.okButton.UseVisualStyleBackColor = false;
			this.okButton.Click += new EventHandler(this.okButton_Click);
			arg_4C_0.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.White;
			base.Controls.Add(this.okButton);
			base.Controls.Add(this.firmwareButton);
			base.Controls.Add(this.softwareButton);
			base.Controls.Add(this.firmwareLabel);
			base.Controls.Add(this.softwareLabel);
			base.Name = "UpdateCheckControl";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
