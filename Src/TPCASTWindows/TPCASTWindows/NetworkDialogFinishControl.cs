using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TPCASTWindows.Properties;

namespace TPCASTWindows
{
	public class NetworkDialogFinishControl : UserControl
	{
		public delegate void OnOkClickDelegate();

		public NetworkDialogFinishControl.OnOkClickDelegate OnOkClick;

		private IContainer components;

		private Button okButton;

		private Label label1;

		public NetworkDialogFinishControl()
		{
			this.InitializeComponent();
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			if (this.OnOkClick != null)
			{
				this.OnOkClick();
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
			ComponentResourceManager arg_2B_0 = new ComponentResourceManager(typeof(NetworkDialogFinishControl));
			this.label1 = new Label();
			this.okButton = new Button();
			base.SuspendLayout();
			arg_2B_0.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			this.okButton.BackgroundImage = Resources.blue_background_1;
			arg_2B_0.ApplyResources(this.okButton, "okButton");
			this.okButton.FlatAppearance.BorderSize = 0;
			this.okButton.ForeColor = Color.White;
			this.okButton.Name = "okButton";
			this.okButton.UseVisualStyleBackColor = false;
			this.okButton.Click += new EventHandler(this.okButton_Click);
			arg_2B_0.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.White;
			base.Controls.Add(this.label1);
			base.Controls.Add(this.okButton);
			base.Name = "NetworkDialogFinishControl";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
