using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TPCASTWindows.Properties;

namespace TPCASTWindows
{
	public class NetworkDialogSwitchControl : UserControl
	{
		public delegate void OnSwitchClickDelegate();

		public NetworkDialogSwitchControl.OnSwitchClickDelegate OnSwitchClick;

		private IContainer components;

		private Button switchButton;

		private PictureBox pictureBox1;

		private Label label2;

		private CustomLabel customLabel1;

		public NetworkDialogSwitchControl()
		{
			this.InitializeComponent();
		}

		private void switchButton_Click(object sender, EventArgs e)
		{
			if (this.OnSwitchClick != null)
			{
				this.OnSwitchClick();
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
			ComponentResourceManager arg_5D_0 = new ComponentResourceManager(typeof(NetworkDialogSwitchControl));
			this.switchButton = new Button();
			this.pictureBox1 = new PictureBox();
			this.label2 = new Label();
			this.customLabel1 = new CustomLabel(this.components);
			((ISupportInitialize)this.pictureBox1).BeginInit();
			base.SuspendLayout();
			arg_5D_0.ApplyResources(this.switchButton, "switchButton");
			this.switchButton.BackgroundImage = Resources.blue_background_1;
			this.switchButton.FlatAppearance.BorderSize = 0;
			this.switchButton.ForeColor = Color.White;
			this.switchButton.Name = "switchButton";
			this.switchButton.UseVisualStyleBackColor = false;
			this.switchButton.Click += new EventHandler(this.switchButton_Click);
			arg_5D_0.ApplyResources(this.pictureBox1, "pictureBox1");
			this.pictureBox1.Image = Resources.exception;
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.TabStop = false;
			arg_5D_0.ApplyResources(this.label2, "label2");
			this.label2.BackColor = Color.FromArgb(216, 216, 216);
			this.label2.Name = "label2";
			arg_5D_0.ApplyResources(this.customLabel1, "customLabel1");
			this.customLabel1.LineDistance = 1;
			this.customLabel1.Name = "customLabel1";
			arg_5D_0.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.White;
			base.Controls.Add(this.customLabel1);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.switchButton);
			base.Controls.Add(this.pictureBox1);
			base.Name = "NetworkDialogSwitchControl";
			((ISupportInitialize)this.pictureBox1).EndInit();
			base.ResumeLayout(false);
		}
	}
}
