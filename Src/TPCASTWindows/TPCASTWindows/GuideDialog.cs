using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using TPCASTWindows.Properties;
using TPCASTWindows.Resources;

namespace TPCASTWindows
{
	public class GuideDialog : BaseDialogForm
	{
		private IContainer components;

		private Button okButton;

		private CustomLabel customLabel1;

		private LinkLabel tutorialLinkLabel;

		private CustomLabel customLabel2;

		public GuideDialog()
		{
			this.InitializeComponent();
		}

		private void tutorialLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("iexplore", Localization.tutorialLink);
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			base.Close();
			base.Dispose();
			BaseDialogForm.OnCloseClickDelegate expr_12 = this.OnCloseClick;
			if (expr_12 == null)
			{
				return;
			}
			expr_12();
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
			ComponentResourceManager arg_79_0 = new ComponentResourceManager(typeof(GuideDialog));
			this.okButton = new Button();
			this.customLabel1 = new CustomLabel(this.components);
			this.tutorialLinkLabel = new LinkLabel();
			this.customLabel2 = new CustomLabel(this.components);
			base.SuspendLayout();
			this.closeButton.FlatAppearance.BorderSize = 0;
			this.okButton.BackgroundImage = Resources.blue_background_1;
			arg_79_0.ApplyResources(this.okButton, "okButton");
			this.okButton.FlatAppearance.BorderSize = 0;
			this.okButton.ForeColor = Color.White;
			this.okButton.Name = "okButton";
			this.okButton.UseVisualStyleBackColor = false;
			this.okButton.Click += new EventHandler(this.okButton_Click);
			arg_79_0.ApplyResources(this.customLabel1, "customLabel1");
			this.customLabel1.LineDistance = 2;
			this.customLabel1.Name = "customLabel1";
			arg_79_0.ApplyResources(this.tutorialLinkLabel, "tutorialLinkLabel");
			this.tutorialLinkLabel.LinkBehavior = LinkBehavior.NeverUnderline;
			this.tutorialLinkLabel.LinkColor = Color.FromArgb(42, 173, 223);
			this.tutorialLinkLabel.Name = "tutorialLinkLabel";
			this.tutorialLinkLabel.TabStop = true;
			this.tutorialLinkLabel.LinkClicked += new LinkLabelLinkClickedEventHandler(this.tutorialLinkLabel_LinkClicked);
			arg_79_0.ApplyResources(this.customLabel2, "customLabel2");
			this.customLabel2.LineDistance = 2;
			this.customLabel2.Name = "customLabel2";
			arg_79_0.ApplyResources(this, "$this");
			base.Controls.Add(this.customLabel2);
			base.Controls.Add(this.tutorialLinkLabel);
			base.Controls.Add(this.customLabel1);
			base.Controls.Add(this.okButton);
			base.Name = "GuideDialog";
			base.Controls.SetChildIndex(this.closeButton, 0);
			base.Controls.SetChildIndex(this.okButton, 0);
			base.Controls.SetChildIndex(this.customLabel1, 0);
			base.Controls.SetChildIndex(this.tutorialLinkLabel, 0);
			base.Controls.SetChildIndex(this.customLabel2, 0);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
