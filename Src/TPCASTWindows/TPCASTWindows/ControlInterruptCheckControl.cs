using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TPCASTWindows.Properties;

namespace TPCASTWindows
{
	public class ControlInterruptCheckControl : UserControl
	{
		public delegate void OnWaitClickDelegate();

		public ControlInterruptCheckControl.OnWaitClickDelegate OnWaitClick;

		private IContainer components;

		private Button waitButton;

		private PictureBox pictureBox1;

		private Label label1;

		private CustomLabel customLabel2;

		public ControlInterruptCheckControl()
		{
			this.InitializeComponent();
		}

		private void waitButton_Click(object sender, EventArgs e)
		{
			if (this.OnWaitClick != null)
			{
				this.OnWaitClick();
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
			ComponentResourceManager arg_7C_0 = new ComponentResourceManager(typeof(ControlInterruptCheckControl));
			this.label1 = new Label();
			this.waitButton = new Button();
			this.pictureBox1 = new PictureBox();
			this.customLabel2 = new CustomLabel(this.components);
			((ISupportInitialize)this.pictureBox1).BeginInit();
			base.SuspendLayout();
			this.label1.BackColor = Color.FromArgb(216, 216, 216);
			arg_7C_0.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			this.waitButton.BackgroundImage = Resources.blue_background_1;
			arg_7C_0.ApplyResources(this.waitButton, "waitButton");
			this.waitButton.FlatAppearance.BorderSize = 0;
			this.waitButton.ForeColor = Color.White;
			this.waitButton.Name = "waitButton";
			this.waitButton.UseVisualStyleBackColor = false;
			this.waitButton.Click += new EventHandler(this.waitButton_Click);
			this.pictureBox1.Image = Resources.exception;
			arg_7C_0.ApplyResources(this.pictureBox1, "pictureBox1");
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.TabStop = false;
			arg_7C_0.ApplyResources(this.customLabel2, "customLabel2");
			this.customLabel2.LineDistance = 3;
			this.customLabel2.Name = "customLabel2";
			arg_7C_0.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.White;
			base.Controls.Add(this.customLabel2);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.waitButton);
			base.Controls.Add(this.pictureBox1);
			base.Name = "ControlInterruptCheckControl";
			((ISupportInitialize)this.pictureBox1).EndInit();
			base.ResumeLayout(false);
		}
	}
}
