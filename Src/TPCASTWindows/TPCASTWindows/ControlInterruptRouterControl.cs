using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TPCASTWindows.Properties;
using TPCASTWindows.Resources;

namespace TPCASTWindows
{
	public class ControlInterruptRouterControl : UserControl
	{
		public delegate void OnWaitClickDelegate();

		public delegate void OnBackClickDelegate();

		public ControlInterruptRouterControl.OnWaitClickDelegate OnWaitClick;

		public ControlInterruptRouterControl.OnBackClickDelegate OnBackClick;

		private IContainer components;

		private Label label1;

		private Button backButton;

		private Button waitButton;

		private PictureBox pictureBox1;

		private PictureBox pictureBox2;

		private CustomLabel customLabel2;

		public ControlInterruptRouterControl()
		{
			this.InitializeComponent();
			this.pictureBox2.Image = LocalizeUtil.getImageFromResource(Localization.interrupt_router);
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
			ComponentResourceManager arg_7E_0 = new ComponentResourceManager(typeof(ControlInterruptRouterControl));
			this.label1 = new Label();
			this.pictureBox2 = new PictureBox();
			this.backButton = new Button();
			this.waitButton = new Button();
			this.pictureBox1 = new PictureBox();
			this.customLabel2 = new CustomLabel(this.components);
			((ISupportInitialize)this.pictureBox2).BeginInit();
			((ISupportInitialize)this.pictureBox1).BeginInit();
			base.SuspendLayout();
			arg_7E_0.ApplyResources(this.label1, "label1");
			this.label1.BackColor = Color.FromArgb(216, 216, 216);
			this.label1.Name = "label1";
			arg_7E_0.ApplyResources(this.pictureBox2, "pictureBox2");
			this.pictureBox2.Image = Resources.interrupt_router;
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.TabStop = false;
			arg_7E_0.ApplyResources(this.backButton, "backButton");
			this.backButton.BackgroundImage = Resources.blue_background_1;
			this.backButton.FlatAppearance.BorderSize = 0;
			this.backButton.ForeColor = Color.White;
			this.backButton.Name = "backButton";
			this.backButton.UseVisualStyleBackColor = false;
			this.backButton.Click += new EventHandler(this.backButton_Click);
			arg_7E_0.ApplyResources(this.waitButton, "waitButton");
			this.waitButton.BackgroundImage = Resources.blue_background_1;
			this.waitButton.FlatAppearance.BorderSize = 0;
			this.waitButton.ForeColor = Color.White;
			this.waitButton.Name = "waitButton";
			this.waitButton.UseVisualStyleBackColor = false;
			this.waitButton.Click += new EventHandler(this.waitButton_Click);
			arg_7E_0.ApplyResources(this.pictureBox1, "pictureBox1");
			this.pictureBox1.Image = Resources.exception;
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.TabStop = false;
			arg_7E_0.ApplyResources(this.customLabel2, "customLabel2");
			this.customLabel2.LineDistance = 1;
			this.customLabel2.Name = "customLabel2";
			arg_7E_0.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.White;
			base.Controls.Add(this.customLabel2);
			base.Controls.Add(this.pictureBox2);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.backButton);
			base.Controls.Add(this.waitButton);
			base.Controls.Add(this.pictureBox1);
			base.Name = "ControlInterruptRouterControl";
			((ISupportInitialize)this.pictureBox2).EndInit();
			((ISupportInitialize)this.pictureBox1).EndInit();
			base.ResumeLayout(false);
		}
	}
}
