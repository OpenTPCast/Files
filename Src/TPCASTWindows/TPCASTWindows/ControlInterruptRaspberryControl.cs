using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TPCASTWindows.Properties;

namespace TPCASTWindows
{
	public class ControlInterruptRaspberryControl : UserControl
	{
		public delegate void OnWaitClickDelegate();

		public delegate void OnBackClickDelegate();

		public ControlInterruptRaspberryControl.OnWaitClickDelegate OnWaitClick;

		public ControlInterruptRaspberryControl.OnBackClickDelegate OnBackClick;

		private IContainer components;

		private Label label1;

		private Button backButton;

		private CustomLabel customLabel1;

		private Button waitButton;

		private PictureBox pictureBox1;

		private PictureBox pictureBox2;

		public ControlInterruptRaspberryControl()
		{
			this.InitializeComponent();
			this.pictureBox2.Image = Resources.interrupt_battery;
		}

		private void waitButton_Click(object sender, EventArgs e)
		{
			if (this.OnWaitClick != null)
			{
				this.OnWaitClick();
			}
		}

		private void backButton_Click(object sender, EventArgs e)
		{
			if (this.OnBackClick != null)
			{
				this.OnBackClick();
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
			ComponentResourceManager arg_9D_0 = new ComponentResourceManager(typeof(ControlInterruptRaspberryControl));
			this.label1 = new Label();
			this.backButton = new Button();
			this.waitButton = new Button();
			this.pictureBox1 = new PictureBox();
			this.pictureBox2 = new PictureBox();
			this.customLabel1 = new CustomLabel(this.components);
			((ISupportInitialize)this.pictureBox1).BeginInit();
			((ISupportInitialize)this.pictureBox2).BeginInit();
			base.SuspendLayout();
			this.label1.BackColor = Color.FromArgb(216, 216, 216);
			arg_9D_0.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			this.backButton.BackgroundImage = Resources.blue_background_1;
			arg_9D_0.ApplyResources(this.backButton, "backButton");
			this.backButton.FlatAppearance.BorderSize = 0;
			this.backButton.ForeColor = Color.White;
			this.backButton.Name = "backButton";
			this.backButton.UseVisualStyleBackColor = false;
			this.backButton.Click += new EventHandler(this.backButton_Click);
			this.waitButton.BackgroundImage = Resources.blue_background_1;
			arg_9D_0.ApplyResources(this.waitButton, "waitButton");
			this.waitButton.FlatAppearance.BorderSize = 0;
			this.waitButton.ForeColor = Color.White;
			this.waitButton.Name = "waitButton";
			this.waitButton.UseVisualStyleBackColor = false;
			this.waitButton.Click += new EventHandler(this.waitButton_Click);
			this.pictureBox1.Image = Resources.exception;
			arg_9D_0.ApplyResources(this.pictureBox1, "pictureBox1");
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.TabStop = false;
			this.pictureBox2.Image = Resources.interrupt_battery;
			arg_9D_0.ApplyResources(this.pictureBox2, "pictureBox2");
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.TabStop = false;
			arg_9D_0.ApplyResources(this.customLabel1, "customLabel1");
			this.customLabel1.LineDistance = 1;
			this.customLabel1.Name = "customLabel1";
			arg_9D_0.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.White;
			base.Controls.Add(this.pictureBox2);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.backButton);
			base.Controls.Add(this.customLabel1);
			base.Controls.Add(this.waitButton);
			base.Controls.Add(this.pictureBox1);
			base.Name = "ControlInterruptRaspberryControl";
			((ISupportInitialize)this.pictureBox1).EndInit();
			((ISupportInitialize)this.pictureBox2).EndInit();
			base.ResumeLayout(false);
		}
	}
}
