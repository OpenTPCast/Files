using NLog;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace TPCASTWindows
{
	public class ControlDialog : BaseDialogForm
	{
		private class WifiConfig
		{
			public string SSID;

			public string password;
		}

		private delegate void OnSetWifiFinishDelegate(bool success);

		public delegate void OnRetryDelegate();

		private static Logger log = LogManager.GetCurrentClassLogger();

		public bool hasRouter = true;

		public bool cableProblem;

		private ControlDialog.OnSetWifiFinishDelegate OnSetWifiFinishListener;

		public ControlDialog.OnRetryDelegate OnRetry;

		private IContainer components;

		private GroupBox dialogGroup;

		public ControlDialog()
		{
			this.InitializeComponent();
			this.OnSetWifiFinishListener = new ControlDialog.OnSetWifiFinishDelegate(this.OnSetWifiFinish);
		}

		private void ControlDialog_Load(object sender, EventArgs e)
		{
			if (!this.hasRouter)
			{
				ControlDialogRouterControl routerControl = new ControlDialogRouterControl();
				routerControl.OnRetryClick = new ControlDialogRouterControl.RetryButtonClickDelegate(this.retry);
				this.dialogGroup.Controls.Clear();
				this.dialogGroup.Controls.Add(routerControl);
				return;
			}
			if (this.cableProblem)
			{
				this.ShowCableControl();
				return;
			}
			ControlDialogControl controlControl = new ControlDialogControl();
			ControlDialogControl controlDialogControl = controlControl;
			controlDialogControl.OnOffButtonClick = (ControlDialogControl.OffButtonDelegate)Delegate.Combine(controlDialogControl.OnOffButtonClick, new ControlDialogControl.OffButtonDelegate(this.OffButtonClick));
			controlDialogControl = controlControl;
			controlDialogControl.OnFlashingButtonClick = (ControlDialogControl.FlashingButtonDelegate)Delegate.Combine(controlDialogControl.OnFlashingButtonClick, new ControlDialogControl.FlashingButtonDelegate(this.FlashingButtonClick));
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(controlControl);
		}

		public void OffButtonClick()
		{
			ControlDialogOffControl offControl = new ControlDialogOffControl();
			offControl.OnRetryClick = new ControlDialogOffControl.RetryButtonClickDelegate(this.retry);
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(offControl);
		}

		public void FlashingButtonClick()
		{
			RouterCheckControl routerControl = new RouterCheckControl();
			routerControl.OnOkButtonClick = new RouterCheckControl.OnOkButtonClickDelegate(this.ShowFlashingControl);
			routerControl.OnNoButtonClick = new RouterCheckControl.OnNoButtonClickDelegate(this.ShowSSIDPasswordControl);
			routerControl.OnNoSSIDPasswordClick = new RouterCheckControl.OnNoSSIDPasswordClickDelegate(this.retry);
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(routerControl);
		}

		private void ShowFlashingControl()
		{
			ControlDialogFlashingControl flashingControl = new ControlDialogFlashingControl();
			flashingControl.OnRetryClick = new ControlDialogFlashingControl.RetryButtonClickDelegate(this.retry);
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(flashingControl);
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
			this.ShowWaitControl();
			ControlDialog.WifiConfig wifiConfig = new ControlDialog.WifiConfig();
			wifiConfig.SSID = SSID;
			wifiConfig.password = password;
			new Thread(new ParameterizedThreadStart(this.setWifiThreadStart)).Start(wifiConfig);
		}

		private void setWifiThreadStart(object obj)
		{
			if (obj is ControlDialog.WifiConfig)
			{
				ControlDialog.WifiConfig expr_11 = (ControlDialog.WifiConfig)obj;
				string SSID = expr_11.SSID;
				string password = expr_11.password;
				ChannelUtil.setWifi(SSID, password);
				string newSSID = ChannelUtil.getWifiSSID();
				string newPassword = ChannelUtil.getWifiPassword();
				ControlDialog.log.Trace("old ssid = " + SSID);
				ControlDialog.log.Trace("old pass = " + password);
				ControlDialog.log.Trace("new ssid = " + newSSID);
				ControlDialog.log.Trace("new pass = " + newPassword);
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

		private void ShowSuccessControl()
		{
			RouterSuccessControl successControl = new RouterSuccessControl();
			successControl.OnOkClick = new RouterSuccessControl.OnOkClickDelegate(this.retry);
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

		private void ShowWaitControl()
		{
			RouterWaitControl waitConrol = new RouterWaitControl();
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(waitConrol);
		}

		private void ShowCableControl()
		{
			ControlInterruptCableControl cableControl = new ControlInterruptCableControl();
			cableControl.OnRetry = new ControlInterruptCableControl.OnRetryDelegate(this.retry);
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(cableControl);
		}

		public void retry()
		{
			base.Close();
			if (this.OnRetry != null)
			{
				this.OnRetry();
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
			this.dialogGroup = new GroupBox();
			base.SuspendLayout();
			this.closeButton.FlatAppearance.BorderSize = 0;
			this.dialogGroup.Location = new Point(0, 20);
			this.dialogGroup.Name = "dialogGroup";
			this.dialogGroup.Size = new Size(500, 244);
			this.dialogGroup.TabIndex = 2;
			this.dialogGroup.TabStop = false;
			this.dialogGroup.Text = "groupBox1";
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			this.BackColor = Color.White;
			base.ClientSize = new Size(500, 264);
			base.Controls.Add(this.dialogGroup);
			base.Name = "ControlDialog";
			base.Load += new EventHandler(this.ControlDialog_Load);
			base.Controls.SetChildIndex(this.closeButton, 0);
			base.Controls.SetChildIndex(this.dialogGroup, 0);
			base.ResumeLayout(false);
		}
	}
}
