using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TPCASTWindows.Properties;

namespace TPCASTWindows
{
	public class NetworkDialogWaitControl : UserControl
	{
		private IContainer components;

		private PictureBox pictureBox1;

		private Label routerLabel;

		private PictureBox blue0;

		private PictureBox blue1;

		private PictureBox blue2;

		private PictureBox blue3;

		private PictureBox blue4;

		private PictureBox blue5;

		private PictureBox blue6;

		private PictureBox blue7;

		private PictureBox pictureBox2;

		private Label raspberryLabel;

		private Label controlLabel;

		public NetworkDialogWaitControl()
		{
			this.InitializeComponent();
			this.routerLabel.Visible = true;
			this.raspberryLabel.Visible = false;
			this.controlLabel.Visible = false;
		}

		public void setRouterLabelText(string msg)
		{
			this.routerLabel.Visible = true;
			this.routerLabel.Text = msg;
		}

		public void setRaspberryLabelText(string msg)
		{
			this.raspberryLabel.Text = msg;
			this.raspberryLabel.Visible = true;
		}

		public void setControlLabelText(string msg)
		{
			this.controlLabel.Text = msg;
			this.controlLabel.Visible = true;
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
			ComponentResourceManager arg_112_0 = new ComponentResourceManager(typeof(NetworkDialogWaitControl));
			this.routerLabel = new Label();
			this.raspberryLabel = new Label();
			this.controlLabel = new Label();
			this.pictureBox2 = new PictureBox();
			this.blue7 = new PictureBox();
			this.blue6 = new PictureBox();
			this.blue5 = new PictureBox();
			this.blue4 = new PictureBox();
			this.blue3 = new PictureBox();
			this.blue2 = new PictureBox();
			this.blue1 = new PictureBox();
			this.blue0 = new PictureBox();
			this.pictureBox1 = new PictureBox();
			((ISupportInitialize)this.pictureBox2).BeginInit();
			((ISupportInitialize)this.blue7).BeginInit();
			((ISupportInitialize)this.blue6).BeginInit();
			((ISupportInitialize)this.blue5).BeginInit();
			((ISupportInitialize)this.blue4).BeginInit();
			((ISupportInitialize)this.blue3).BeginInit();
			((ISupportInitialize)this.blue2).BeginInit();
			((ISupportInitialize)this.blue1).BeginInit();
			((ISupportInitialize)this.blue0).BeginInit();
			((ISupportInitialize)this.pictureBox1).BeginInit();
			base.SuspendLayout();
			arg_112_0.ApplyResources(this.routerLabel, "routerLabel");
			this.routerLabel.Name = "routerLabel";
			arg_112_0.ApplyResources(this.raspberryLabel, "raspberryLabel");
			this.raspberryLabel.Name = "raspberryLabel";
			arg_112_0.ApplyResources(this.controlLabel, "controlLabel");
			this.controlLabel.Name = "controlLabel";
			this.pictureBox2.Image = Resources.waiting;
			arg_112_0.ApplyResources(this.pictureBox2, "pictureBox2");
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.TabStop = false;
			this.blue7.BackgroundImage = Resources.network_reset_blue_7;
			arg_112_0.ApplyResources(this.blue7, "blue7");
			this.blue7.Name = "blue7";
			this.blue7.TabStop = false;
			this.blue6.BackgroundImage = Resources.network_reset_blue_6;
			arg_112_0.ApplyResources(this.blue6, "blue6");
			this.blue6.Name = "blue6";
			this.blue6.TabStop = false;
			this.blue5.BackgroundImage = Resources.network_reset_blue_5;
			arg_112_0.ApplyResources(this.blue5, "blue5");
			this.blue5.Name = "blue5";
			this.blue5.TabStop = false;
			this.blue4.BackgroundImage = Resources.network_reset_blue_4;
			arg_112_0.ApplyResources(this.blue4, "blue4");
			this.blue4.Name = "blue4";
			this.blue4.TabStop = false;
			this.blue3.BackgroundImage = Resources.network_reset_blue_3;
			arg_112_0.ApplyResources(this.blue3, "blue3");
			this.blue3.Name = "blue3";
			this.blue3.TabStop = false;
			this.blue2.BackgroundImage = Resources.network_reset_blue_2;
			arg_112_0.ApplyResources(this.blue2, "blue2");
			this.blue2.Name = "blue2";
			this.blue2.TabStop = false;
			this.blue1.BackgroundImage = Resources.network_reset_blue_1;
			arg_112_0.ApplyResources(this.blue1, "blue1");
			this.blue1.Name = "blue1";
			this.blue1.TabStop = false;
			this.blue0.BackgroundImage = Resources.network_reset_blue_0;
			arg_112_0.ApplyResources(this.blue0, "blue0");
			this.blue0.Name = "blue0";
			this.blue0.TabStop = false;
			this.pictureBox1.BackgroundImage = Resources.network_reset_dark;
			arg_112_0.ApplyResources(this.pictureBox1, "pictureBox1");
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.TabStop = false;
			arg_112_0.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.White;
			base.Controls.Add(this.controlLabel);
			base.Controls.Add(this.raspberryLabel);
			base.Controls.Add(this.pictureBox2);
			base.Controls.Add(this.blue7);
			base.Controls.Add(this.blue6);
			base.Controls.Add(this.blue5);
			base.Controls.Add(this.blue4);
			base.Controls.Add(this.blue3);
			base.Controls.Add(this.blue2);
			base.Controls.Add(this.blue1);
			base.Controls.Add(this.blue0);
			base.Controls.Add(this.routerLabel);
			base.Controls.Add(this.pictureBox1);
			base.Name = "NetworkDialogWaitControl";
			((ISupportInitialize)this.pictureBox2).EndInit();
			((ISupportInitialize)this.blue7).EndInit();
			((ISupportInitialize)this.blue6).EndInit();
			((ISupportInitialize)this.blue5).EndInit();
			((ISupportInitialize)this.blue4).EndInit();
			((ISupportInitialize)this.blue3).EndInit();
			((ISupportInitialize)this.blue2).EndInit();
			((ISupportInitialize)this.blue1).EndInit();
			((ISupportInitialize)this.blue0).EndInit();
			((ISupportInitialize)this.pictureBox1).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
