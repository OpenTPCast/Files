using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TPCASTWindows.Properties;

namespace TPCASTWindows.UI.Update
{
	public class UpdateFirmwareSuccessControl : UserControl
	{
		public delegate void OnOkClickDelegate();

		public UpdateFirmwareSuccessControl.OnOkClickDelegate OnOkClick;

		private IContainer components;

		private CustomLabel customLabel2;

		private Label label1;

		private Button okButton;

		public UpdateFirmwareSuccessControl()
		{
			this.InitializeComponent();
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			UpdateFirmwareSuccessControl.OnOkClickDelegate expr_06 = this.OnOkClick;
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
			ComponentResourceManager arg_47_0 = new ComponentResourceManager(typeof(UpdateFirmwareSuccessControl));
			this.customLabel2 = new CustomLabel(this.components);
			this.label1 = new Label();
			this.okButton = new Button();
			base.SuspendLayout();
			arg_47_0.ApplyResources(this.customLabel2, "customLabel2");
			this.customLabel2.LineDistance = 1;
			this.customLabel2.Name = "customLabel2";
			arg_47_0.ApplyResources(this.label1, "label1");
			this.label1.BackColor = Color.FromArgb(216, 216, 216);
			this.label1.Name = "label1";
			arg_47_0.ApplyResources(this.okButton, "okButton");
			this.okButton.BackgroundImage = Resources.blue_background_1;
			this.okButton.FlatAppearance.BorderSize = 0;
			this.okButton.ForeColor = Color.White;
			this.okButton.Name = "okButton";
			this.okButton.UseVisualStyleBackColor = false;
			this.okButton.Click += new EventHandler(this.okButton_Click);
			arg_47_0.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.White;
			base.Controls.Add(this.customLabel2);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.okButton);
			base.Name = "UpdateFirmwareSuccessControl";
			base.ResumeLayout(false);
		}
	}
}
