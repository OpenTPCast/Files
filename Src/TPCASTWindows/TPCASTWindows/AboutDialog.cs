using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using TPCASTWindows.Resources;

namespace TPCASTWindows
{
	public class AboutDialog : BaseDialogForm, SocketConnectCallback
	{
		private SocketModel socketModel;

		private IContainer components;

		private Label label1;

		private Label label3;

		private CustomLabel customLabel1;

		private Label versionLabel;

		private Label firmwareVersionLabel;

		public AboutDialog()
		{
			this.InitializeComponent();
			this.socketModel = new SocketModel(this);
			this.socketModel.addSocketConnectCallback(this);
			this.socketModel.connect();
			Version version = Assembly.GetExecutingAssembly().GetName().Version;
			this.versionLabel.Text = string.Concat(new object[]
			{
				Localization.currentVersion,
				version.Major,
				".",
				version.Minor,
				".",
				version.Build
			});
			this.firmwareVersionLabel.Text = Localization.firmwareCurrentVersion + "---";
		}

		public void OnConnected(bool success)
		{
			if (success)
			{
				if (this.socketModel != null)
				{
					this.socketModel.getVerion();
					return;
				}
			}
			else
			{
				this.firmwareVersionLabel.Text = Localization.firmwareCurrentVersion + Localization.firmwareUnconnected;
			}
		}

		public void OnMacReceive(string mac)
		{
		}

		public void OnVersionReceive(string version)
		{
			this.firmwareVersionLabel.Text = Localization.firmwareCurrentVersion + "V" + version;
		}

		private void AboutDialog_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (this.socketModel != null)
			{
				this.socketModel.removeSocketConnectCallback(this);
				this.socketModel.disconnect();
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
			ComponentResourceManager arg_6E_0 = new ComponentResourceManager(typeof(AboutDialog));
			this.label1 = new Label();
			this.label3 = new Label();
			this.customLabel1 = new CustomLabel(this.components);
			this.versionLabel = new Label();
			this.firmwareVersionLabel = new Label();
			base.SuspendLayout();
			this.closeButton.FlatAppearance.BorderSize = 0;
			arg_6E_0.ApplyResources(this.label1, "label1");
			this.label1.ForeColor = Color.FromArgb(25, 25, 25);
			this.label1.Name = "label1";
			this.label3.BackColor = Color.FromArgb(216, 216, 216);
			arg_6E_0.ApplyResources(this.label3, "label3");
			this.label3.Name = "label3";
			arg_6E_0.ApplyResources(this.customLabel1, "customLabel1");
			this.customLabel1.LineDistance = 3;
			this.customLabel1.Name = "customLabel1";
			arg_6E_0.ApplyResources(this.versionLabel, "versionLabel");
			this.versionLabel.Name = "versionLabel";
			arg_6E_0.ApplyResources(this.firmwareVersionLabel, "firmwareVersionLabel");
			this.firmwareVersionLabel.Name = "firmwareVersionLabel";
			arg_6E_0.ApplyResources(this, "$this");
			base.Controls.Add(this.firmwareVersionLabel);
			base.Controls.Add(this.versionLabel);
			base.Controls.Add(this.customLabel1);
			base.Controls.Add(this.label3);
			base.Controls.Add(this.label1);
			base.Name = "AboutDialog";
			base.FormClosing += new FormClosingEventHandler(this.AboutDialog_FormClosing);
			base.Controls.SetChildIndex(this.closeButton, 0);
			base.Controls.SetChildIndex(this.label1, 0);
			base.Controls.SetChildIndex(this.label3, 0);
			base.Controls.SetChildIndex(this.customLabel1, 0);
			base.Controls.SetChildIndex(this.versionLabel, 0);
			base.Controls.SetChildIndex(this.firmwareVersionLabel, 0);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
