using RestSharp;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using TPCASTWindows.Resources;
using TPCASTWindows.UI.Update;

namespace TPCASTWindows.UI
{
	public class UpdateDialog : BaseDialogForm, DownloadCallbackInterface, SocketConnectCallback, SocketUpdateCallback, SocketExceptonCallback
	{
		public delegate void OnDialogCloseDelegate();

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
			if (this.socketModel != null)
			{
				this.socketModel.addSocketConnectCallback(this);
				this.socketModel.addSocketUpdateCallback(this);
				this.socketModel.addSocketExceptionCallback(this);
			}
			this.ShowUpdateMessage();
		}

		private void ShowUpdateMessage()
		{
			this.ShowUpdateCheckControl();
			if (this.socketModel != null)
			{
				this.socketModel.connect();
			}
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
				this.checkControl.setFirmwareLabel(Localization.firmwareVersion + Localization.firmwareUnconnected);
				this.requestUpdate("", "");
			}
		}

		public void OnVersionReceive(string version)
		{
			Console.WriteLine("version = " + version);
			this.currentAdapterVersion = version;
			if (this.checkControl != null)
			{
				this.checkControl.setFirmwareLabel(Localization.firmwareVersion + "v" + version);
			}
			if (this.socketModel != null)
			{
				this.socketModel.getMac();
			}
		}

		public void OnMacReceive(string mac)
		{
			Console.WriteLine("mac = " + mac);
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
							UpdateMessage recommend = response.Data.roms.adapter.recommend;
							if (recommend != null)
							{
								this.updateAdapterVersionString = recommend.version;
								Version arg_AB_0 = new Version(recommend.version);
								Version value = new Version(adapterVersion);
								if (arg_AB_0.CompareTo(value) > 0)
								{
									this.adapterUrl = recommend.url;
									this.adapterMd5 = recommend.md5;
									if (File.Exists(Constants.updateAdapterFilePath) && CryptoUtil.GetMD5HashFromFile(Constants.updateAdapterFilePath).Equals(recommend.md5))
									{
										this.isSoftwareDownloaded = true;
									}
									if (this.checkControl != null)
									{
										this.checkControl.setFirmwareLabel(Localization.firmwareVersion + "v" + recommend.version);
										this.checkControl.setFirmwareButtonVisibility(true);
									}
								}
								else if (this.checkControl != null)
								{
									this.checkControl.setFirmwareLabel(string.Concat(new string[]
									{
										Localization.firmwareVersion,
										"v",
										adapterVersion,
										"(",
										Localization.alreadyNewest,
										")"
									}));
								}
							}
						}
						if (response.Data.roms.software != null)
						{
							UpdateMessage recommend2 = response.Data.roms.software.recommend;
							if (recommend2 != null)
							{
								this.updateSoftwareVersionString = recommend2.version;
								if (new Version(recommend2.version).CompareTo(currentSoftwareVersion) > 0)
								{
									this.softwareUrl = recommend2.url;
									this.softwareMd5 = recommend2.md5;
									if (File.Exists(Constants.updateSoftwareFilePath) && CryptoUtil.GetMD5HashFromFile(Constants.updateSoftwareFilePath).Equals(recommend2.md5))
									{
										this.isSoftwareDownloaded = true;
									}
									if (this.checkControl != null)
									{
										this.checkControl.setSoftwareLabel(Localization.softwareVersion + "v" + recommend2.version);
										this.checkControl.setSoftwareButtonVisibility(true);
										return;
									}
								}
								else if (this.checkControl != null)
								{
									this.checkControl.setSoftwareLabel(string.Concat(new string[]
									{
										Localization.softwareVersion,
										"v",
										softwareVersion,
										"(",
										Localization.alreadyNewest,
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
					Version version = Assembly.GetExecutingAssembly().GetName().Version;
					string text = string.Concat(new object[]
					{
						version.Major,
						".",
						version.Minor,
						".",
						version.Build
					});
					this.checkControl.setSoftwareLabel(string.Concat(new string[]
					{
						Localization.softwareVersion,
						"v",
						text,
						"(",
						Localization.noInternet,
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
			this.firmwareControl.setMessage(Localization.firmwareDonloadFinish);
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
			Console.WriteLine("sender = " + ((WebClient)sender).IsBusy.ToString());
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
			Console.WriteLine("UserState = " + e.UserState);
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
			Console.WriteLine("userToken = " + userToken);
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

		private void ShowUpdateCheckControl()
		{
			this.checkControl = new UpdateCheckControl();
			Version version = Assembly.GetExecutingAssembly().GetName().Version;
			string str = string.Concat(new object[]
			{
				version.Major,
				".",
				version.Minor,
				".",
				version.Build
			});
			this.checkControl.setFirmwareLabel(Localization.firmwareVersion + "v---");
			this.checkControl.setSoftwareLabel(Localization.softwareVersion + "v" + str);
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
			updateFirmwareFailControl.OnRetryClick = new UpdateFirmwareFailControl.OnRetryClickDelegate(this.ShowUpdateMessage);
			updateFirmwareFailControl.OnCancelClick = new UpdateFirmwareFailControl.OnCancelClickDelegate(this.OnCancelClick);
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(updateFirmwareFailControl);
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
