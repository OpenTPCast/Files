using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TPCASTWindows.Properties;

namespace TPCASTWindows
{
	public class ControlInterruptRebootControl : UserControl
	{
		public delegate void OnOkClickDelegate();

		public ControlInterruptRebootControl.OnOkClickDelegate OnOkClick;

		private IContainer components;

		private Label label2;

		private CustomLabel customLabel1;

		private PictureBox pictureBox1;

		private Button okButton;

		public ControlInterruptRebootControl()
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
			this.components = new Container();
			ComponentResourceManager arg_7C_0 = new ComponentResourceManager(typeof(ControlInterruptRebootControl));
			this.label2 = new Label();
			this.customLabel1 = new CustomLabel(this.components);
			this.pictureBox1 = new PictureBox();
			this.okButton = new Button();
			((ISupportInitialize)this.pictureBox1).BeginInit();
			base.SuspendLayout();
			this.label2.BackColor = Color.FromArgb(216, 216, 216);
			arg_7C_0.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			arg_7C_0.ApplyResources(this.customLabel1, "customLabel1");
			this.customLabel1.LineDistance = 3;
			this.customLabel1.Name = "customLabel1";
			this.pictureBox1.Image = Resources.exception;
			arg_7C_0.ApplyResources(this.pictureBox1, "pictureBox1");
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.TabStop = false;
			this.okButton.BackgroundImage = Resources.blue_background_0;
			arg_7C_0.ApplyResources(this.okButton, "okButton");
			this.okButton.FlatAppearance.BorderSize = 0;
			this.okButton.ForeColor = Color.White;
			this.okButton.Name = "okButton";
			this.okButton.UseVisualStyleBackColor = false;
			this.okButton.Click += new EventHandler(this.okButton_Click);
			arg_7C_0.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.White;
			base.Controls.Add(this.label2);
			base.Controls.Add(this.customLabel1);
			base.Controls.Add(this.okButton);
			base.Controls.Add(this.pictureBox1);
			base.Name = "ControlInterruptRebootControl";
			((ISupportInitialize)this.pictureBox1).EndInit();
			base.ResumeLayout(false);
		}
	}
}
