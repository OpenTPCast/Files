using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TPCASTWindows.Properties;

namespace TPCASTWindows.UI.Update
{
	public class UpdateFirmwareWaitingControl : UserControl
	{
		private IContainer components;

		private CustomLabel customLabel1;

		private PictureBox pictureBox2;

		public UpdateFirmwareWaitingControl()
		{
			this.InitializeComponent();
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
			ComponentResourceManager arg_47_0 = new ComponentResourceManager(typeof(UpdateFirmwareWaitingControl));
			this.customLabel1 = new CustomLabel(this.components);
			this.pictureBox2 = new PictureBox();
			((ISupportInitialize)this.pictureBox2).BeginInit();
			base.SuspendLayout();
			arg_47_0.ApplyResources(this.customLabel1, "customLabel1");
			this.customLabel1.LineDistance = 3;
			this.customLabel1.Name = "customLabel1";
			this.pictureBox2.Image = Resources.waiting;
			arg_47_0.ApplyResources(this.pictureBox2, "pictureBox2");
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.TabStop = false;
			arg_47_0.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.White;
			base.Controls.Add(this.pictureBox2);
			base.Controls.Add(this.customLabel1);
			base.Name = "UpdateFirmwareWaitingControl";
			((ISupportInitialize)this.pictureBox2).EndInit();
			base.ResumeLayout(false);
		}
	}
}
