using NLog;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace TPCASTWindows
{
	public class RouterDialog : BaseDialogForm
	{
		private class WifiConfig
		{
			public string SSID;

			public string password;
		}

		private delegate void OnSetWifiFinishDelegate(bool success);

		public delegate void OnRouterDialogCloseDelegate();

		private static Logger log = LogManager.GetCurrentClassLogger();

		private RouterDialog.OnSetWifiFinishDelegate OnSetWifiFinishListener;

		public RouterDialog.OnRouterDialogCloseDelegate OnRouterDialogClose;

		private IContainer components;

		private GroupBox dialogGroup;

		public RouterDialog()
		{
			this.InitializeComponent();
			this.OnSetWifiFinishListener = new RouterDialog.OnSetWifiFinishDelegate(this.OnSetWifiFinish);
		}

		public void setCloseButtonVisibility(bool visibility)
		{
			this.closeButton.Visible = visibility;
		}

		private void RouterDialog_Load(object sender, EventArgs e)
		{
			this.ShowSSIDPasswordControl();
		}

		private void ShowSSIDPasswordControl()
		{
			RouterSSIDPasswordControl ssidControl = new RouterSSIDPasswordControl();
			ssidControl.OnOkClick = new RouterSSIDPasswordControl.OnOkClickDelegate(this.OnSSIDOkClick);
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(ssidControl);
		}

		private void OnSSIDOkClick(string SSID, string password)
		{
			RouterDialog.log.Trace("OnSSIDOkClick");
			this.ShowWaitControl();
			RouterDialog.WifiConfig wifiConfig = new RouterDialog.WifiConfig();
			wifiConfig.SSID = SSID;
			wifiConfig.password = password;
			new Thread(new ParameterizedThreadStart(this.setWifiThreadStart)).Start(wifiConfig);
		}

		private void setWifiThreadStart(object obj)
		{
			if (obj is RouterDialog.WifiConfig)
			{
				RouterDialog.log.Trace("setWifiThreadStart");
				RouterDialog.WifiConfig expr_20 = (RouterDialog.WifiConfig)obj;
				string SSID = expr_20.SSID;
				string password = expr_20.password;
				RouterDialog.log.Trace("before set");
				ChannelUtil.setWifi(SSID, password);
				RouterDialog.log.Trace("after set");
				RouterDialog.log.Trace("before getssid");
				string newSSID = ChannelUtil.getWifiSSID();
				RouterDialog.log.Trace("after getssid");
				string newPassword = ChannelUtil.getWifiPassword();
				RouterDialog.log.Trace("old ssid = " + SSID);
				RouterDialog.log.Trace("old pass = " + password);
				RouterDialog.log.Trace("new ssid = " + newSSID);
				RouterDialog.log.Trace("new pass = " + newPassword);
				if (!string.IsNullOrEmpty(SSID) && !string.IsNullOrEmpty(newPassword) && SSID.Equals(newSSID) && password.Equals(newPassword))
				{
					this.setWifiFinish(true);
					return;
				}
				this.setWifiFinish(false);
			}
		}

		private void setWifiFinish(bool success)
		{
			if (this.OnSetWifiFinishListener != null)
			{
				if (base.InvokeRequired)
				{
					base.Invoke(this.OnSetWifiFinishListener, new object[]
					{
						success
					});
					return;
				}
				this.OnSetWifiFinishListener(success);
			}
		}

		private void OnSetWifiFinish(bool success)
		{
			if (success)
			{
				this.ShowSuccessControl();
				return;
			}
			this.ShowFailControl();
		}

		private void ShowWaitControl()
		{
			RouterWaitControl waitConrol = new RouterWaitControl();
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(waitConrol);
		}

		private void ShowSuccessControl()
		{
			RouterSuccessControl successControl = new RouterSuccessControl();
			successControl.OnOkClick = new RouterSuccessControl.OnOkClickDelegate(this.closeClick);
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(successControl);
		}

		private void ShowFailControl()
		{
			RouterFailControl failConrol = new RouterFailControl();
			failConrol.OnRetryClick = new RouterFailControl.OnRetryClickDelegate(this.ShowSSIDPasswordControl);
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(failConrol);
		}

		private void closeClick()
		{
			base.Close();
			base.Dispose();
		}

		private void RouterDialog_FormClosing(object sender, FormClosingEventArgs e)
		{
			RouterDialog.log.Trace("RouterDialog_FormClosing");
			RouterDialog.OnRouterDialogCloseDelegate expr_15 = this.OnRouterDialogClose;
			if (expr_15 == null)
			{
				return;
			}
			expr_15();
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
			this.dialogGroup = new GroupBox();
			base.SuspendLayout();
			this.closeButton.FlatAppearance.BorderSize = 0;
			this.dialogGroup.Location = new Point(0, 20);
			this.dialogGroup.Name = "dialogGroup";
			this.dialogGroup.Size = new Size(500, 244);
			this.dialogGroup.TabIndex = 4;
			this.dialogGroup.TabStop = false;
			this.dialogGroup.Text = "groupBox1";
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.ClientSize = new Size(500, 264);
			base.Controls.Add(this.dialogGroup);
			base.Name = "RouterDialog";
			base.FormClosing += new FormClosingEventHandler(this.RouterDialog_FormClosing);
			base.Load += new EventHandler(this.RouterDialog_Load);
			base.Controls.SetChildIndex(this.closeButton, 0);
			base.Controls.SetChildIndex(this.dialogGroup, 0);
			base.ResumeLayout(false);
		}
	}
}
