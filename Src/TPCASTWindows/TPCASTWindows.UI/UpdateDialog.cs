using NLog;
using RestSharp;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using TPCASTWindows.Properties;
using TPCASTWindows.UI.Update;

namespace TPCASTWindows.UI
{
	public class UpdateDialog : BaseDialogForm, DownloadCallbackInterface, SocketConnectCallback, SocketUpdateCallback, SocketExceptonCallback
	{
		public delegate void OnDialogCloseDelegate();

		private static Logger log = LogManager.GetCurrentClassLogger();

		private SocketModel socketModel;

		private string currentAdapterVersion = "";

		private string adapterUrl = "";

		private string adapterMd5 = "";

		private string updateAdapterVersionString = "";

		private bool isAdapterDownloaded;

		private string softwareUrl = "";

		private string softwareMd5 = "";

		private string updateSoftwareVersionString = "";

		private bool isSoftwareDownloaded;

		private bool isAdapterDownloading;

		private bool isSoftwareDownloading;

		private UpdateCheckControl checkControl;

		private UpdateSoftwareDownloadControl softwareControl;

		private UpdateFirmwareDownloadControl firmwareControl;

		public UpdateDialog.OnDialogCloseDelegate OnDialogClose;

		private IContainer components;

		private GroupBox dialogGroup;

		public UpdateDialog()
		{
			this.InitializeComponent();
			this.socketModel = new SocketModel(this);
			this.socketModel.addSocketConnectCallback(this);
			this.socketModel.addSocketUpdateCallback(this);
			this.socketModel.addSocketExceptionCallback(this);
			this.ShowUpdateMessage();
		}

		private void ShowUpdateMessage()
		{
			this.ShowUpdateCheckControl();
			if (this.socketModel != null)
			{
				this.socketModel.GetVersion();
			}
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
				this.checkControl.setFirmwareLabel(Resources.firmwareVersion + Resources.firmwareUnconnected);
				this.requestUpdate("", "");
			}
		}

		public void OnVersionReceive(string version)
		{
			UpdateDialog.log.Trace("version = " + version);
			this.currentAdapterVersion = version;
			if (this.checkControl != null)
			{
				this.checkControl.setFirmwareLabel(Resources.firmwareVersion + "v" + version);
			}
			if (this.socketModel != null)
			{
				this.socketModel.GetMac();
			}
		}

		public void OnMacReceive(string mac)
		{
			UpdateDialog.log.Trace("mac = " + mac);
			this.requestUpdate(this.currentAdapterVersion, mac);
		}

		private void requestUpdate(string adapterVersion = "", string sn = "")
		{
			Version currentSoftwareVersion = Assembly.GetExecutingAssembly().GetName().Version;
			string softwareVersion = string.Concat(new object[]
			{
				currentSoftwareVersion.Major,
				".",
				currentSoftwareVersion.Minor,
				".",
				currentSoftwareVersion.Build
			});
			Directory.CreateDirectory(Constants.downloadPath);
			Client.getInstance().requestUpdate(softwareVersion, adapterVersion, sn, delegate(IRestResponse<Update> response)
			{
				if (response != null && response.StatusCode == HttpStatusCode.OK)
				{
					if (response.Data != null && response.Data.roms != null)
					{
						if (response.Data.roms.adapter != null && !string.IsNullOrEmpty(adapterVersion) && !string.IsNullOrEmpty(sn))
						{
							UpdateMessage updateMessage = response.Data.roms.adapter.recommend;
							if (updateMessage != null)
							{
								this.updateAdapterVersionString = updateMessage.version;
								Version arg_AB_0 = new Version(updateMessage.version);
								Version currentAdapterVersion = new Version(adapterVersion);
								if (arg_AB_0.CompareTo(currentAdapterVersion) > 0)
								{
									this.adapterUrl = updateMessage.url;
									this.adapterMd5 = updateMessage.md5;
									if (File.Exists(Constants.updateAdapterFilePath) && CryptoUtil.GetMD5HashFromFile(Constants.updateAdapterFilePath).Equals(updateMessage.md5))
									{
										this.isSoftwareDownloaded = true;
									}
									if (this.checkControl != null)
									{
										this.checkControl.setFirmwareLabel(Resources.firmwareVersion + "v" + updateMessage.version);
										this.checkControl.setFirmwareButtonVisibility(true);
									}
								}
								else if (this.checkControl != null)
								{
									this.checkControl.setFirmwareLabel(string.Concat(new string[]
									{
										Resources.firmwareVersion,
										"v",
										adapterVersion,
										"(",
										Resources.alreadyNewest,
										")"
									}));
								}
							}
						}
						if (response.Data.roms.software != null)
						{
							UpdateMessage updateMessage2 = response.Data.roms.software.recommend;
							if (updateMessage2 != null)
							{
								this.updateSoftwareVersionString = updateMessage2.version;
								if (new Version(updateMessage2.version).CompareTo(currentSoftwareVersion) > 0)
								{
									this.softwareUrl = updateMessage2.url;
									this.softwareMd5 = updateMessage2.md5;
									if (File.Exists(Constants.updateSoftwareFilePath) && CryptoUtil.GetMD5HashFromFile(Constants.updateSoftwareFilePath).Equals(updateMessage2.md5))
									{
										this.isSoftwareDownloaded = true;
									}
									if (this.checkControl != null)
									{
										this.checkControl.setSoftwareLabel(Resources.softwareVersion + "v" + updateMessage2.version);
										this.checkControl.setSoftwareButtonVisibility(true);
										return;
									}
								}
								else if (this.checkControl != null)
								{
									this.checkControl.setSoftwareLabel(string.Concat(new string[]
									{
										Resources.softwareVersion,
										"v",
										softwareVersion,
										"(",
										Resources.alreadyNewest,
										")"
									}));
									return;
								}
							}
						}
					}
				}
				else
				{
					UpdateDialog.log.Trace("fail to connect internet");
					Version v = Assembly.GetExecutingAssembly().GetName().Version;
					string softVersion = string.Concat(new object[]
					{
						v.Major,
						".",
						v.Minor,
						".",
						v.Build
					});
					this.checkControl.setSoftwareLabel(string.Concat(new string[]
					{
						Resources.softwareVersion,
						"v",
						softVersion,
						"(",
						Resources.noInternet,
						")"
					}));
				}
			});
		}

		private void updateFirmware()
		{
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
			this.firmwareControl.setMessage(Resources.firmwareDonloadFinish);
			this.firmwareControl.setProgress(100);
			this.firmwareControl.setContinueButtonVisble(true);
		}

		private void downloadAdapterThreadStart()
		{
			DownloadModel expr_06 = new DownloadModel(this);
			expr_06.setDownloadCallback(this);
			expr_06.DownloadFile(this.adapterUrl, Constants.updateAdapterFilePath, "adapter");
		}

		private void updateSoftware()
		{
			if (this.isSoftwareDownloaded)
			{
				this.showDoanloadSoftwareFinish();
				return;
			}
			this.startDownloadSoftware();
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
			this.softwareControl.setMessage(Resources.softwareDownloadFinish);
			this.softwareControl.setProgress(100);
			this.softwareControl.setContinueButtonVisble(true);
		}

		public void OnUpdateVersionReceive(bool success)
		{
			if (success)
			{
				if (this.socketModel != null)
				{
					this.socketModel.TransUpdateMd5(this.adapterMd5);
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
					this.socketModel.PrepareUpdate();
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
			UpdateDialog.log.Trace("sender = " + ((WebClient)sender).IsBusy.ToString());
			UpdateDialog.log.Trace("percent = " + e.ProgressPercentage);
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
			UpdateDialog.log.Trace("sender = " + sender);
			UpdateDialog.log.Trace("UserState = " + e.UserState);
			if ("adapter".Equals(e.UserState))
			{
				this.isAdapterDownloading = false;
				if (File.Exists(Constants.updateAdapterFilePath))
				{
					string md5 = CryptoUtil.GetMD5HashFromFile(Constants.updateAdapterFilePath);
					if (!this.adapterMd5.Equals(md5))
					{
						this.ShowFirmwareDownloadExceptionControl();
						return;
					}
					if (this.firmwareControl != null)
					{
						this.firmwareControl.setMessage(Resources.firmwareDonloadFinish);
						this.firmwareControl.setContinueButtonVisble(true);
						return;
					}
				}
			}
			else if ("software".Equals(e.UserState))
			{
				this.isSoftwareDownloading = false;
				if (File.Exists(Constants.updateSoftwareFilePath))
				{
					string md6 = CryptoUtil.GetMD5HashFromFile(Constants.updateSoftwareFilePath);
					if (this.softwareMd5.Equals(md6))
					{
						if (this.softwareControl != null)
						{
							this.softwareControl.setMessage(Resources.softwareDownloadFinish);
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
			UpdateDialog.log.Trace("userToken = " + userToken);
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
				this.socketModel.TransUpdateVersion(this.updateAdapterVersionString);
			}
		}

		private void uninstallTPCAST()
		{
			RegistryUtil.UninstallTPCAST();
		}

		private void ShowUpdateCheckControl()
		{
			this.checkControl = new UpdateCheckControl();
			Version v = Assembly.GetExecutingAssembly().GetName().Version;
			string softVersion = string.Concat(new object[]
			{
				v.Major,
				".",
				v.Minor,
				".",
				v.Build
			});
			this.checkControl.setFirmwareLabel(Resources.firmwareVersion + "v---");
			this.checkControl.setSoftwareLabel(Resources.softwareVersion + "v" + softVersion);
			this.checkControl.OnOkClick = new UpdateCheckControl.OnOkClickDelegate(this.OnCancelClick);
			this.checkControl.OnSoftwareClick = new UpdateCheckControl.OnSoftwareClickDelegate(this.updateSoftware);
			this.checkControl.OnFirmwareClick = new UpdateCheckControl.OnFirmwareClickDelegate(this.updateFirmware);
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(this.checkControl);
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
			UpdateSoftwareDownloadExceptionControl downloadExceptionControl = new UpdateSoftwareDownloadExceptionControl();
			downloadExceptionControl.OnRetryClick = new UpdateSoftwareDownloadExceptionControl.OnRetryClickDelegate(this.startDownloadSoftware);
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(downloadExceptionControl);
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
			UpdateFirmwareDownloadExceptionControl downloadExceptionControl = new UpdateFirmwareDownloadExceptionControl();
			downloadExceptionControl.OnRetryClick = new UpdateFirmwareDownloadExceptionControl.OnRetryClickDelegate(this.startDownloadFirmware);
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(downloadExceptionControl);
		}

		private void ShowFirmwareWaitingControl()
		{
			UpdateFirmwareWaitingControl waitingControl = new UpdateFirmwareWaitingControl();
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(waitingControl);
		}

		private void ShowFirmwareSuccessControl()
		{
			UpdateFirmwareSuccessControl successControl = new UpdateFirmwareSuccessControl();
			successControl.OnOkClick = new UpdateFirmwareSuccessControl.OnOkClickDelegate(this.OnFirmwareUpdateSuccess);
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(successControl);
		}

		private void ShowFirmwareFailControl()
		{
			UpdateFirmwareFailControl failControl = new UpdateFirmwareFailControl();
			failControl.OnRetryClick = new UpdateFirmwareFailControl.OnRetryClickDelegate(this.ShowUpdateMessage);
			failControl.OnCancelClick = new UpdateFirmwareFailControl.OnCancelClickDelegate(this.OnCancelClick);
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(failControl);
		}

		private void OnFirmwareUpdateSuccess()
		{
			this.ShowUpdateMessage();
		}

		private void OnCancelClick()
		{
			base.Close();
			base.Dispose();
		}

		private void UpdateDialog_FormClosing(object sender, FormClosingEventArgs e)
		{
			UpdateDialog.OnDialogCloseDelegate expr_06 = this.OnDialogClose;
			if (expr_06 != null)
			{
				expr_06();
			}
			if (this.socketModel != null)
			{
				this.socketModel.removeSocketConnectCallback(this);
				this.socketModel.removeSocketUpdateCallback(this);
				this.socketModel.removeSocketExceptionCallback(this);
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
			this.dialogGroup.TabIndex = 4;
			this.dialogGroup.TabStop = false;
			this.dialogGroup.Text = "groupBox1";
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.ClientSize = new Size(500, 264);
			base.Controls.Add(this.dialogGroup);
			base.Name = "UpdateDialog";
			base.FormClosing += new FormClosingEventHandler(this.UpdateDialog_FormClosing);
			base.Controls.SetChildIndex(this.closeButton, 0);
			base.Controls.SetChildIndex(this.dialogGroup, 0);
			base.ResumeLayout(false);
		}
	}
}
