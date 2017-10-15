using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using TPCASTWindows.Resources;

namespace TPCASTWindows.UI.Update
{
	public class ForceUpdateDialog : BaseDialogForm, DownloadCallbackInterface, SocketUpdateCallback, SocketExceptonCallback
	{
		public delegate void OnRetryDelegate();

		public string adapterUrl = "";

		public string adapterMd5 = "";

		public string updateAdapterVersionString = "";

		public bool isAdapterDownloaded;

		public string softwareUrl = "";

		public string softwareMd5 = "";

		public bool isSoftwareDownloaded;

		private SocketModel socketModel;

		private bool isAdapterDownloading;

		private bool isSoftwareDownloading;

		private UpdateSoftwareDownloadControl softwareControl;

		private UpdateFirmwareDownloadControl firmwareControl;

		public ForceUpdateDialog.OnRetryDelegate OnRetry;

		private IContainer components;

		private GroupBox dialogGroup;

		public ForceUpdateDialog()
		{
			this.InitializeComponent();
			this.closeButton.Visible = false;
			this.socketModel = new SocketModel(this);
			this.socketModel.addSocketUpdateCallback(this);
			this.socketModel.addSocketExceptionCallback(this);
		}

		private void ForceUpdateDialog_Load(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(this.adapterUrl))
			{
				if (!string.IsNullOrEmpty(this.softwareUrl))
				{
					if (this.isSoftwareDownloaded)
					{
						this.showDoanloadSoftwareFinish();
						return;
					}
					this.startDownloadSoftware();
				}
				return;
			}
			if (this.isAdapterDownloaded)
			{
				this.showDownloadFirmwareFinish();
				return;
			}
			this.startDownloadFirmware();
		}

		private void startDownloadFirmware()
		{
			this.ShowFirmwareDownloadControl();
			new Thread(new ThreadStart(this.downloadAdapterThreadStart)).Start();
		}

		private void showDownloadFirmwareFinish()
		{
			this.ShowFirmwareDownloadControl();
			this.firmwareControl.setMessage(Localization.firmwareDonloadFinish);
			this.firmwareControl.setProgress(100);
			this.firmwareControl.setContinueButtonVisble(true);
		}

		private void downloadAdapterThreadStart()
		{
			DownloadModel expr_06 = new DownloadModel(this);
			expr_06.setDownloadCallback(this);
			expr_06.DownloadFile(this.adapterUrl, Constants.updateAdapterFilePath, "adapter");
			this.isAdapterDownloading = true;
		}

		private void startDownloadSoftware()
		{
			this.ShowSoftwareDownloadControl();
			FileStream expr_11 = new FileStream(Constants.updateSoftwareMd5Path, FileMode.Create);
			StreamWriter expr_17 = new StreamWriter(expr_11);
			expr_17.Write(this.softwareMd5);
			expr_17.Flush();
			expr_17.Close();
			expr_11.Close();
			new Thread(new ThreadStart(this.downloadSoftwareThreadStart)).Start();
		}

		private void downloadSoftwareThreadStart()
		{
			DownloadModel expr_06 = new DownloadModel(this);
			expr_06.setDownloadCallback(this);
			expr_06.DownloadFile(this.softwareUrl, Constants.updateSoftwareFilePath, "software");
			this.isSoftwareDownloading = true;
		}

		private void showDoanloadSoftwareFinish()
		{
			this.ShowSoftwareDownloadControl();
			FileStream expr_11 = new FileStream(Constants.updateSoftwareMd5Path, FileMode.Create);
			StreamWriter expr_17 = new StreamWriter(expr_11);
			expr_17.Write(this.softwareMd5);
			expr_17.Flush();
			expr_17.Close();
			expr_11.Close();
			this.softwareControl.setMessage(Localization.softwareDownloadFinish);
			this.softwareControl.setProgress(100);
			this.softwareControl.setContinueButtonVisble(true);
		}

		public void OnUpdateVersionReceive(bool success)
		{
			if (success)
			{
				if (this.socketModel != null)
				{
					this.socketModel.transUpdateMd5(this.adapterMd5);
					return;
				}
			}
			else
			{
				this.ShowFirmwareFailControl();
			}
		}

		public void OnUpdateMd5Receive(bool success)
		{
			if (success)
			{
				if (this.socketModel != null)
				{
					this.socketModel.prepareUpdate();
					return;
				}
			}
			else
			{
				this.ShowFirmwareFailControl();
			}
		}

		public void OnUpdateReady(bool success)
		{
			if (success)
			{
				if (this.socketModel != null)
				{
					this.socketModel.transFile(Constants.updateAdapterFilePath);
					return;
				}
			}
			else
			{
				this.ShowFirmwareFailControl();
			}
		}

		public void OnMd5CheckResult(bool success)
		{
			if (!success)
			{
				this.ShowFirmwareFailControl();
			}
		}

		public void OnUpdateFinish(bool success)
		{
			if (success)
			{
				this.ShowFirmwareSuccessControl();
				return;
			}
			this.ShowFirmwareFailControl();
		}

		public void OnSendFail()
		{
			this.ShowFirmwareFailControl();
		}

		public void OnReceiveTimeout()
		{
			this.ShowFirmwareFailControl();
		}

		public void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
		{
			Console.WriteLine("percent = " + e.ProgressPercentage);
			if ("adapter".Equals(e.UserState))
			{
				if (this.firmwareControl != null && this.isAdapterDownloading)
				{
					this.firmwareControl.setProgress(e.ProgressPercentage);
					return;
				}
			}
			else if ("software".Equals(e.UserState) && this.softwareControl != null && this.isSoftwareDownloading)
			{
				this.softwareControl.setProgress(e.ProgressPercentage);
			}
		}

		public void DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
		{
			Console.WriteLine("sender = " + sender);
			if ("adapter".Equals(e.UserState))
			{
				this.isAdapterDownloading = false;
				if (File.Exists(Constants.updateAdapterFilePath))
				{
					string mD5HashFromFile = CryptoUtil.GetMD5HashFromFile(Constants.updateAdapterFilePath);
					if (!this.adapterMd5.Equals(mD5HashFromFile))
					{
						this.ShowFirmwareDownloadExceptionControl();
						return;
					}
					if (this.firmwareControl != null)
					{
						this.firmwareControl.setMessage(Localization.firmwareDonloadFinish);
						this.firmwareControl.setContinueButtonVisble(true);
						this.firmwareControl.OnContinueClick = new UpdateFirmwareDownloadControl.OnContinueClickDelegate(this.transUpdateFile);
						return;
					}
				}
			}
			else if ("software".Equals(e.UserState))
			{
				this.isSoftwareDownloading = false;
				if (File.Exists(Constants.updateSoftwareFilePath))
				{
					string mD5HashFromFile2 = CryptoUtil.GetMD5HashFromFile(Constants.updateSoftwareFilePath);
					if (this.softwareMd5.Equals(mD5HashFromFile2))
					{
						if (this.softwareControl != null)
						{
							this.softwareControl.setMessage(Localization.softwareDownloadFinish);
							this.softwareControl.setContinueButtonVisble(true);
							return;
						}
					}
					else
					{
						this.ShowSoftwareDownloadExceptionControl();
					}
				}
			}
		}

		public void DownloadTimeout(object userToken)
		{
			if ("adapter".Equals(userToken))
			{
				this.ShowFirmwareDownloadExceptionControl();
				return;
			}
			if ("software".Equals(userToken))
			{
				this.ShowSoftwareDownloadExceptionControl();
			}
		}

		private void transUpdateFile()
		{
			this.ShowFirmwareWaitingControl();
			if (!string.IsNullOrEmpty(this.updateAdapterVersionString) && this.socketModel != null)
			{
				this.socketModel.transUpdateVersion(this.updateAdapterVersionString);
			}
		}

		private void uninstallTPCAST()
		{
			RegistryUtil.UninstallTPCAST();
		}

		private void ShowSoftwareDownloadControl()
		{
			this.softwareControl = new UpdateSoftwareDownloadControl();
			this.softwareControl.OnContinueClick = new UpdateSoftwareDownloadControl.OnContinueClickDelegate(this.uninstallTPCAST);
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(this.softwareControl);
		}

		private void ShowSoftwareDownloadExceptionControl()
		{
			UpdateSoftwareDownloadExceptionControl updateSoftwareDownloadExceptionControl = new UpdateSoftwareDownloadExceptionControl();
			updateSoftwareDownloadExceptionControl.OnRetryClick = new UpdateSoftwareDownloadExceptionControl.OnRetryClickDelegate(this.startDownloadSoftware);
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(updateSoftwareDownloadExceptionControl);
		}

		private void ShowFirmwareDownloadControl()
		{
			this.firmwareControl = new UpdateFirmwareDownloadControl();
			this.firmwareControl.OnContinueClick = new UpdateFirmwareDownloadControl.OnContinueClickDelegate(this.transUpdateFile);
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(this.firmwareControl);
		}

		private void ShowFirmwareDownloadExceptionControl()
		{
			UpdateFirmwareDownloadExceptionControl updateFirmwareDownloadExceptionControl = new UpdateFirmwareDownloadExceptionControl();
			updateFirmwareDownloadExceptionControl.OnRetryClick = new UpdateFirmwareDownloadExceptionControl.OnRetryClickDelegate(this.startDownloadFirmware);
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(updateFirmwareDownloadExceptionControl);
		}

		private void ShowFirmwareWaitingControl()
		{
			UpdateFirmwareWaitingControl value = new UpdateFirmwareWaitingControl();
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(value);
		}

		private void ShowFirmwareSuccessControl()
		{
			UpdateFirmwareSuccessControl updateFirmwareSuccessControl = new UpdateFirmwareSuccessControl();
			updateFirmwareSuccessControl.OnOkClick = new UpdateFirmwareSuccessControl.OnOkClickDelegate(this.OnFirmwareUpdateSuccess);
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(updateFirmwareSuccessControl);
		}

		private void ShowFirmwareFailControl()
		{
			UpdateFirmwareFailControl updateFirmwareFailControl = new UpdateFirmwareFailControl();
			updateFirmwareFailControl.OnRetryClick = new UpdateFirmwareFailControl.OnRetryClickDelegate(this.onRetry);
			updateFirmwareFailControl.OnCancelClick = new UpdateFirmwareFailControl.OnCancelClickDelegate(this.OnCancelClick);
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(updateFirmwareFailControl);
		}

		private void onRetry()
		{
			base.Close();
			base.Dispose();
			ForceUpdateDialog.OnRetryDelegate expr_12 = this.OnRetry;
			if (expr_12 == null)
			{
				return;
			}
			expr_12();
		}

		private void OnFirmwareUpdateSuccess()
		{
			if (!string.IsNullOrEmpty(this.softwareUrl))
			{
				this.startDownloadSoftware();
				return;
			}
			base.Close();
			base.Dispose();
		}

		private void OnCancelClick()
		{
			Environment.Exit(0);
		}

		private void ForceUpdateDialog_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (this.socketModel != null)
			{
				this.socketModel.removeSocketUpdateCallback(this);
				this.socketModel.removeSocketExceptionCallback(this);
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
			this.dialogGroup = new GroupBox();
			base.SuspendLayout();
			this.closeButton.FlatAppearance.BorderSize = 0;
			this.dialogGroup.Location = new Point(0, 20);
			this.dialogGroup.Name = "dialogGroup";
			this.dialogGroup.Size = new Size(500, 244);
			this.dialogGroup.TabIndex = 5;
			this.dialogGroup.TabStop = false;
			this.dialogGroup.Text = "groupBox1";
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.ClientSize = new Size(500, 264);
			base.Controls.Add(this.dialogGroup);
			base.Name = "ForceUpdateDialog";
			base.FormClosing += new FormClosingEventHandler(this.ForceUpdateDialog_FormClosing);
			base.Load += new EventHandler(this.ForceUpdateDialog_Load);
			base.Controls.SetChildIndex(this.closeButton, 0);
			base.Controls.SetChildIndex(this.dialogGroup, 0);
			base.ResumeLayout(false);
		}
	}
}
