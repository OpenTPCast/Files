using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TPCASTWindows.Properties;

namespace TPCASTWindows
{
	public class RouterWaitControl : UserControl
	{
		private IContainer components;

		private PictureBox pictureBox2;

		private CustomLabel customLabel1;

		public RouterWaitControl()
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
			ComponentResourceManager arg_47_0 = new ComponentResourceManager(typeof(RouterWaitControl));
			this.pictureBox2 = new PictureBox();
			this.customLabel1 = new CustomLabel(this.components);
			((ISupportInitialize)this.pictureBox2).BeginInit();
			base.SuspendLayout();
			arg_47_0.ApplyResources(this.pictureBox2, "pictureBox2");
			this.pictureBox2.Image = Resources.waiting;
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.TabStop = false;
			arg_47_0.ApplyResources(this.customLabel1, "customLabel1");
			this.customLabel1.LineDistance = 3;
			this.customLabel1.Name = "customLabel1";
			arg_47_0.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.White;
			base.Controls.Add(this.pictureBox2);
			base.Controls.Add(this.customLabel1);
			base.Name = "RouterWaitControl";
			((ISupportInitialize)this.pictureBox2).EndInit();
			base.ResumeLayout(false);
		}
	}
}
