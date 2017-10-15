using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TPCASTWindows.Properties;

namespace TPCASTWindows.UI.Other
{
	public class GuideForm : Form
	{
		private IContainer components;

		private PictureBox guideImage;

		public GuideForm()
		{
			this.InitializeComponent();
			this.guideImage.Image = Resources.guide_image;
		}

		private void guideImage_Click(object sender, EventArgs e)
		{
			this.click();
		}

		private void GuideForm_Click(object sender, EventArgs e)
		{
			this.click();
		}

		private void click()
		{
			Settings.Default.displayGuide = false;
			Settings.Default.Save();
			base.Close();
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
			ComponentResourceManager arg_2B_0 = new ComponentResourceManager(typeof(GuideForm));
			this.guideImage = new PictureBox();
			((ISupportInitialize)this.guideImage).BeginInit();
			base.SuspendLayout();
			arg_2B_0.ApplyResources(this.guideImage, "guideImage");
			this.guideImage.Name = "guideImage";
			this.guideImage.TabStop = false;
			this.guideImage.Click += new EventHandler(this.guideImage_Click);
			arg_2B_0.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.FromArgb(25, 25, 25);
			base.Controls.Add(this.guideImage);
			base.FormBorderStyle = FormBorderStyle.None;
			base.Name = "GuideForm";
			base.Opacity = 0.8;
			base.Click += new EventHandler(this.GuideForm_Click);
			((ISupportInitialize)this.guideImage).EndInit();
			base.ResumeLayout(false);
		}
	}
}
