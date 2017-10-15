using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using TPCASTWindows.Properties;

namespace TPCASTWindows
{
	public class AboutDialog : BaseDialogForm, SocketConnectCallback, SocketExceptonCallback
	{
		private SocketModel socketModel;

		private IContainer components;

		private Label label3;

		private CustomLabel customLabel1;

		private Label versionLabel;

		private Label firmwareVersionLabel;

		private Button refresh;

		private Panel panel1;

		private Label titleLabel;

		public AboutDialog()
		{
			this.InitializeComponent();
			this.socketModel = new SocketModel(this);
			this.socketModel.addSocketConnectCallback(this);
			this.socketModel.addSocketExceptionCallback(this);
			this.socketModel.GetVersion();
			Version v = Assembly.GetExecutingAssembly().GetName().Version;
			this.versionLabel.Text = string.Concat(new object[]
			{
				Resources.currentVersion,
				v.Major,
				".",
				v.Minor,
				".",
				v.Build
			});
			this.firmwareVersionLabel.Text = Resources.firmwareCurrentVersion + "---";
		}

		public void OnConnected(bool success)
		{
			if (success)
			{
				if (this.socketModel != null)
				{
					this.socketModel.GetVersion();
					return;
				}
			}
			else
			{
				this.firmwareVersionLabel.Text = Resources.firmwareCurrentVersion + Resources.firmwareUnconnected;
				this.refresh.Visible = true;
			}
		}

		public void OnMacReceive(string mac)
		{
		}

		public void OnReceiveTimeout()
		{
			this.firmwareVersionLabel.Text = Resources.firmwareCurrentVersion + Resources.firmwareUnconnected;
			this.refresh.Visible = true;
		}

		public void OnSendFail()
		{
			this.firmwareVersionLabel.Text = Resources.firmwareCurrentVersion + Resources.firmwareUnconnected;
			this.refresh.Visible = true;
		}

		public void OnVersionReceive(string version)
		{
			this.firmwareVersionLabel.Text = Resources.firmwareCurrentVersion + "v" + version;
			this.refresh.Visible = true;
		}

		private void AboutDialog_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (this.socketModel != null)
			{
				this.socketModel.removeSocketConnectCallback(this);
				this.socketModel.removeSocketExceptionCallback(this);
			}
		}

		private void refresh_Click(object sender, EventArgs e)
		{
			this.firmwareVersionLabel.Text = Resources.firmwareCurrentVersion + "---";
			this.refresh.Visible = false;
			if (this.socketModel != null)
			{
				this.socketModel.GetVersion();
				return;
			}
			this.refresh.Visible = true;
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
			ComponentResourceManager arg_7E_0 = new ComponentResourceManager(typeof(AboutDialog));
			this.label3 = new Label();
			this.customLabel1 = new CustomLabel(this.components);
			this.versionLabel = new Label();
			this.firmwareVersionLabel = new Label();
			this.refresh = new Button();
			this.panel1 = new Panel();
			this.titleLabel = new Label();
			this.panel1.SuspendLayout();
			base.SuspendLayout();
			arg_7E_0.ApplyResources(this.closeButton, "closeButton");
			this.closeButton.FlatAppearance.BorderSize = 0;
			arg_7E_0.ApplyResources(this.label3, "label3");
			this.label3.BackColor = Color.FromArgb(216, 216, 216);
			this.label3.Name = "label3";
			arg_7E_0.ApplyResources(this.customLabel1, "customLabel1");
			this.customLabel1.LineDistance = 3;
			this.customLabel1.Name = "customLabel1";
			arg_7E_0.ApplyResources(this.versionLabel, "versionLabel");
			this.versionLabel.Name = "versionLabel";
			arg_7E_0.ApplyResources(this.firmwareVersionLabel, "firmwareVersionLabel");
			this.firmwareVersionLabel.Name = "firmwareVersionLabel";
			arg_7E_0.ApplyResources(this.refresh, "refresh");
			this.refresh.BackgroundImage = Resources.refresh;
			this.refresh.FlatAppearance.BorderSize = 0;
			this.refresh.Name = "refresh";
			this.refresh.UseVisualStyleBackColor = true;
			this.refresh.Click += new EventHandler(this.refresh_Click);
			arg_7E_0.ApplyResources(this.panel1, "panel1");
			this.panel1.Controls.Add(this.titleLabel);
			this.panel1.Name = "panel1";
			arg_7E_0.ApplyResources(this.titleLabel, "titleLabel");
			this.titleLabel.Name = "titleLabel";
			arg_7E_0.ApplyResources(this, "$this");
			base.Controls.Add(this.panel1);
			base.Controls.Add(this.refresh);
			base.Controls.Add(this.firmwareVersionLabel);
			base.Controls.Add(this.versionLabel);
			base.Controls.Add(this.customLabel1);
			base.Controls.Add(this.label3);
			base.Name = "AboutDialog";
			base.FormClosing += new FormClosingEventHandler(this.AboutDialog_FormClosing);
			base.Controls.SetChildIndex(this.label3, 0);
			base.Controls.SetChildIndex(this.customLabel1, 0);
			base.Controls.SetChildIndex(this.versionLabel, 0);
			base.Controls.SetChildIndex(this.firmwareVersionLabel, 0);
			base.Controls.SetChildIndex(this.refresh, 0);
			base.Controls.SetChildIndex(this.panel1, 0);
			base.Controls.SetChildIndex(this.closeButton, 0);
			this.panel1.ResumeLayout(false);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
