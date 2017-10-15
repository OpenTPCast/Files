using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TPCASTWindows.Properties;

namespace TPCASTWindows
{
	public class ControlDialogControl : UserControl
	{
		public delegate void OffButtonDelegate();

		public delegate void FlashingButtonDelegate();

		public ControlDialogControl.OffButtonDelegate OnOffButtonClick;

		public ControlDialogControl.FlashingButtonDelegate OnFlashingButtonClick;

		private IContainer components;

		private Label label2;

		private PictureBox pictureBox2;

		private PictureBox pictureBox1;

		private Button flashingButton;

		private Button offButton;

		private CustomLabel customLabel1;

		public ControlDialogControl()
		{
			this.InitializeComponent();
			this.pictureBox2.Image = Resources.battery;
		}

		private void offButton_Click(object sender, EventArgs e)
		{
			if (this.OnOffButtonClick != null)
			{
				this.OnOffButtonClick();
			}
		}

		private void flashingButton_Click(object sender, EventArgs e)
		{
			if (this.OnFlashingButtonClick != null)
			{
				this.OnFlashingButtonClick();
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
			ComponentResourceManager arg_9D_0 = new ComponentResourceManager(typeof(ControlDialogControl));
			this.label2 = new Label();
			this.customLabel1 = new CustomLabel(this.components);
			this.pictureBox2 = new PictureBox();
			this.pictureBox1 = new PictureBox();
			this.flashingButton = new Button();
			this.offButton = new Button();
			((ISupportInitialize)this.pictureBox2).BeginInit();
			((ISupportInitialize)this.pictureBox1).BeginInit();
			base.SuspendLayout();
			this.label2.BackColor = Color.FromArgb(216, 216, 216);
			arg_9D_0.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			arg_9D_0.ApplyResources(this.customLabel1, "customLabel1");
			this.customLabel1.LineDistance = 1;
			this.customLabel1.Name = "customLabel1";
			arg_9D_0.ApplyResources(this.pictureBox2, "pictureBox2");
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.TabStop = false;
			this.pictureBox1.Image = Resources.control_exception;
			arg_9D_0.ApplyResources(this.pictureBox1, "pictureBox1");
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.TabStop = false;
			this.flashingButton.BackgroundImage = Resources.flashing;
			arg_9D_0.ApplyResources(this.flashingButton, "flashingButton");
			this.flashingButton.FlatAppearance.BorderSize = 0;
			this.flashingButton.ForeColor = Color.FromArgb(102, 102, 102);
			this.flashingButton.Name = "flashingButton";
			this.flashingButton.UseVisualStyleBackColor = true;
			this.flashingButton.Click += new EventHandler(this.flashingButton_Click);
			this.offButton.BackgroundImage = Resources.off;
			arg_9D_0.ApplyResources(this.offButton, "offButton");
			this.offButton.FlatAppearance.BorderSize = 0;
			this.offButton.ForeColor = Color.FromArgb(102, 102, 102);
			this.offButton.Name = "offButton";
			this.offButton.UseVisualStyleBackColor = true;
			this.offButton.Click += new EventHandler(this.offButton_Click);
			arg_9D_0.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.White;
			base.Controls.Add(this.customLabel1);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.pictureBox2);
			base.Controls.Add(this.pictureBox1);
			base.Controls.Add(this.flashingButton);
			base.Controls.Add(this.offButton);
			base.Name = "ControlDialogControl";
			((ISupportInitialize)this.pictureBox2).EndInit();
			((ISupportInitialize)this.pictureBox1).EndInit();
			base.ResumeLayout(false);
		}
	}
}
