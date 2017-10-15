using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TPCASTWindows
{
	public class ChannelSwitchDialog : BaseForm
	{
		private IContainer components;

		private Button button1;

		private PictureBox pictureBox1;

		private Label label1;

		private ListView channelListView;

		public ChannelSwitchDialog()
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
			this.button1 = new Button();
			this.label1 = new Label();
			this.channelListView = new ListView();
			this.pictureBox1 = new PictureBox();
			((ISupportInitialize)this.pictureBox1).BeginInit();
			base.SuspendLayout();
			this.button1.Location = new Point(317, 228);
			this.button1.Name = "button1";
			this.button1.Size = new Size(75, 23);
			this.button1.TabIndex = 0;
			this.button1.Text = "button1";
			this.button1.UseVisualStyleBackColor = true;
			this.label1.AutoSize = true;
			this.label1.Location = new Point(121, 47);
			this.label1.Name = "label1";
			this.label1.Size = new Size(41, 12);
			this.label1.TabIndex = 2;
			this.label1.Text = "label1";
			this.channelListView.Location = new Point(16, 127);
			this.channelListView.Name = "channelListView";
			this.channelListView.Size = new Size(379, 81);
			this.channelListView.TabIndex = 3;
			this.channelListView.UseCompatibleStateImageBehavior = false;
			this.pictureBox1.Location = new Point(24, 47);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new Size(78, 74);
			this.pictureBox1.TabIndex = 1;
			this.pictureBox1.TabStop = false;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(404, 263);
			base.Controls.Add(this.channelListView);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.pictureBox1);
			base.Controls.Add(this.button1);
			base.Name = "ChannelSwitchDialog";
			this.Text = "ChannelSwitchDialog";
			base.Controls.SetChildIndex(this.button1, 0);
			base.Controls.SetChildIndex(this.pictureBox1, 0);
			base.Controls.SetChildIndex(this.label1, 0);
			base.Controls.SetChildIndex(this.channelListView, 0);
			((ISupportInitialize)this.pictureBox1).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
