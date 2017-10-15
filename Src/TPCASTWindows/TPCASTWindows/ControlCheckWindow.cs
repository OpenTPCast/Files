using NLog;
using RestSharp;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using TPCASTWindows.Properties;
using TPCASTWindows.UI.Main;
using TPCASTWindows.UI.Update;
using TPCASTWindows.Utils;

namespace TPCASTWindows
{
	public class ControlCheckWindow : UserControl, SocketConnectCallback, SocketExceptonCallback, ConnectStatusCallback
	{
		private delegate void OnWirelessFinishDelegate();

		public delegate void OnControlSuccessDelegate();

		private static Logger log = LogManager.GetCurrentClassLogger();

		private ConnectModel connectModel;

		private SocketModel socketModel;

		private string adapterVersion = "";

		private ForceUpdateDialog forceDialog;

		public static bool isChecking = false;

		private ControlCheckWindow.OnWirelessFinishDelegate OnWirelessFinishListener;

		private Thread WirelessThread;

		public static bool isAllPass = false;

		public ControlCheckWindow.OnControlSuccessDelegate OnControlSuccess;

		private IContainer components;

		private PictureBox progressBox;

		private PictureBox pictureBox1;

		private PictureBox pcBox;

		private PictureBox lineSystem;

		private PictureBox lineRaspberry;

		private PictureBox lineRouter;

		private Label label4;

		private Label label3;

		private Label label2;

		private Label label1;

		private PictureBox indicatorWireless;

		private PictureBox indicatorSystem;

		private PictureBox indicatorRaspberry;

		private PictureBox indicatorRouter;

		private Label label6;

		private Label routerLabel;

		private Label raspberryLabel;

		private Label systemLabel;

		private Label messageLabel;

		private Button startButton;

		public ControlCheckWindow()
		{
			ControlCheckWindow.log.Trace("ControlCheckWindow");
			this.InitializeComponent();
			ControlCheckWindow.log.Trace("after ControlCheckWindow");
			this.connectModel = new ConnectModel(this);
			this.connectModel.setConnectStatusCallback(this);
			this.OnWirelessFinishListener = new ControlCheckWindow.OnWirelessFinishDelegate(this.OnWirelessFinish);
			this.socketModel = new SocketModel(this);
			this.addSocketCallback();
			this.initView();
			ControlCheckWindow.log.Trace("ip == " + ConfigureUtil.AdapterIP());
			ControlCheckWindow.log.Trace("type = " + ConfigureUtil.getClientType());
		}

		public void MainFormShown()
		{
			ControlCheckWindow.log.Trace("MainFormShown");
		}

		public void OnFormClosing()
		{
			if (this.connectModel != null)
			{
				this.connectModel.unInit();
			}
		}

		public void OnSplashImageInvisible()
		{
			new Thread(delegate
			{
				Thread.Sleep(2500);
				base.Invoke(new MethodInvoker(delegate
				{
					this.showSplashForm();
				}));
			}).Start();
			this.checkRouterSSID();
		}

		private void showSplashForm()
		{
			if (Settings.Default.showGuideDialog)
			{
				new SplashForm
				{
					OnSplashFormClosing = new SplashForm.OnSplashFormClosingDelegate(this.OnSplashFormClosing)
				}.Show();
			}
		}

		private void OnSplashFormClosing()
		{
			Settings.Default.showGuideDialog = false;
			Settings.Default.Save();
		}

		private void checkRouterSSID()
		{
			if (this.connectModel != null)
			{
				this.connectModel.InitCheckRouterSSID();
			}
		}

		public void OnInitCheckRouterSSIDFinish(bool modified)
		{
			if (modified)
			{
				this.connectAdapter();
				return;
			}
			this.modifyWifiSSID();
		}

		public void OnInitCheckRouterFail()
		{
			this.connectAdapter();
		}

		private void modifyWifiSSID()
		{
			Util.showGrayBackground();
			RouterDialog expr_0A = new RouterDialog();
			expr_0A.setCloseButtonVisibility(false);
			expr_0A.OnRouterDialogClose = new RouterDialog.OnRouterDialogCloseDelegate(this.OnRouterDialogClose);
			expr_0A.Show(Util.sContext);
		}

		private void OnRouterDialogClose()
		{
			this.connectAdapter();
		}

		public void addSocketCallback()
		{
			if (this.socketModel != null)
			{
				this.socketModel.addSocketConnectCallback(this);
				this.socketModel.addSocketExceptionCallback(this);
			}
		}

		public void removeSocketCallback()
		{
			if (this.socketModel != null)
			{
				this.socketModel.removeSocketConnectCallback(this);
				this.socketModel.removeSocketExceptionCallback(this);
			}
		}

		private void connectAdapter()
		{
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
				ControlCheckWindow.log.Trace("connect fail");
				this.requestForceUpdate("", "");
			}
		}

		public void requestUpdateWhenAdapterConnected()
		{
			new Thread(new ThreadStart(this.adapterConnectedThreadStart)).Start();
		}

		private void adapterConnectedThreadStart()
		{
			while (!UsbIPUtil.isHostLinked())
			{
				Thread.Sleep(5000);
			}
			Thread.Sleep(5000);
			if (this.socketModel != null)
			{
				this.socketModel.GetVersion();
			}
		}

		public void OnVersionReceive(string version)
		{
			ControlCheckWindow.log.Trace("version = " + version);
			this.adapterVersion = version;
			if (this.socketModel != null)
			{
				this.socketModel.GetMac();
			}
		}

		public void OnMacReceive(string mac)
		{
			ControlCheckWindow.log.Trace("mac = " + mac);
			this.requestForceUpdate(this.adapterVersion, mac);
		}

		public void OnSendFail()
		{
			ControlCheckWindow.log.Trace("OnSendFail");
			this.requestForceUpdate("", "");
		}

		public void OnReceiveTimeout()
		{
			ControlCheckWindow.log.Trace("OnReceiveTimeout");
			this.requestForceUpdate("", "");
		}

		private void requestForceUpdate(string adapterVersion = "", string sn = "")
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
			ControlCheckWindow.log.Trace("CreateDirectory");
			Client.getInstance().requestUpdate(softwareVersion, adapterVersion, sn, delegate(IRestResponse<Update> response)
			{
				ControlCheckWindow.log.Trace("requestUpdate");
				if (response != null && response.StatusCode == HttpStatusCode.OK && response.Data != null && response.Data.roms != null)
				{
					string adapterUrl = "";
					string adapterMd5 = "";
					string updateAdapterVersionString = "";
					bool isAdapterDownloaded = false;
					string softwareUrl = "";
					string softwareMd5 = "";
					bool isSoftwareDownloaded = false;
					if (response.Data.roms.adapter != null && !string.IsNullOrEmpty(adapterVersion) && !string.IsNullOrEmpty(sn))
					{
						UpdateMessage updateMessage = response.Data.roms.adapter.forced;
						if (updateMessage != null)
						{
							updateAdapterVersionString = updateMessage.version;
							Version arg_D8_0 = new Version(updateMessage.version);
							Version currentAdapterVersion = new Version(adapterVersion);
							if (arg_D8_0.CompareTo(currentAdapterVersion) > 0)
							{
								adapterUrl = updateMessage.url;
								adapterMd5 = updateMessage.md5;
								if (File.Exists(Constants.updateAdapterFilePath))
								{
									string currentMd5 = CryptoUtil.GetMD5HashFromFile(Constants.updateAdapterFilePath);
									ControlCheckWindow.log.Trace("cur md5 = " + currentMd5);
									if (currentMd5.Equals(updateMessage.md5))
									{
										isAdapterDownloaded = true;
									}
								}
							}
						}
					}
					if (response.Data.roms.software != null)
					{
						UpdateMessage updateMessage2 = response.Data.roms.software.forced;
						if (updateMessage2 != null && new Version(updateMessage2.version).CompareTo(currentSoftwareVersion) > 0)
						{
							softwareUrl = updateMessage2.url;
							softwareMd5 = updateMessage2.md5;
							if (File.Exists(Constants.updateSoftwareFilePath) && CryptoUtil.GetMD5HashFromFile(Constants.updateSoftwareFilePath).Equals(updateMessage2.md5))
							{
								isSoftwareDownloaded = true;
							}
						}
					}
					ControlCheckWindow.log.Trace("adapterUrl = " + adapterUrl);
					ControlCheckWindow.log.Trace("adapterMd5 = " + adapterMd5);
					ControlCheckWindow.log.Trace("softwareUrl = " + softwareUrl);
					ControlCheckWindow.log.Trace("softwareMd5 = " + softwareMd5);
					ControlCheckWindow.log.Trace("updateAdapterVersionString = " + updateAdapterVersionString);
					if (!string.IsNullOrEmpty(adapterUrl) || !string.IsNullOrEmpty(softwareUrl))
					{
						Util.showGrayBackground();
						this.forceDialog = new ForceUpdateDialog();
						this.forceDialog.adapterUrl = adapterUrl;
						this.forceDialog.adapterMd5 = adapterMd5;
						this.forceDialog.updateAdapterVersionString = updateAdapterVersionString;
						this.forceDialog.isAdapterDownloaded = isAdapterDownloaded;
						this.forceDialog.softwareUrl = softwareUrl;
						this.forceDialog.softwareMd5 = softwareMd5;
						this.forceDialog.isSoftwareDownloaded = isSoftwareDownloaded;
						this.forceDialog.OnRetry = new ForceUpdateDialog.OnRetryDelegate(this.connectAdapter);
						this.forceDialog.Show(Util.sContext);
						return;
					}
				}
				ControlCheckWindow.log.Trace("driver");
				this.checkDriver();
			});
		}

		private void checkDriver()
		{
			if (UsbIPUtil.isDriverInstalled())
			{
				if (!UsbIPUtil.isServiceOk())
				{
					BroadcastModel.instance.send(92);
					Util.showGrayBackground();
					new ServerExceptionDialog().Show(Util.sContext);
					return;
				}
			}
			else
			{
				BroadcastModel.instance.send(91);
				Util.showGrayBackground();
				new DriverDialog().Show(Util.sContext);
			}
		}

		public void initView()
		{
			this.progressBox.Image = Resources.connect_normal_progress;
			this.indicatorRouter.Image = Resources.router_normal;
			this.lineRouter.Image = Resources.line_unload;
			this.indicatorRaspberry.Image = Resources.raspberry_normal;
			this.lineRaspberry.Image = Resources.line_unload;
			this.indicatorSystem.Image = Resources.system_normal;
			this.lineSystem.Image = Resources.line_unload;
			this.indicatorWireless.Image = Resources.vr_normal;
			this.messageLabel.Visible = false;
			this.routerLabel.Visible = false;
			this.raspberryLabel.Visible = false;
			this.systemLabel.Visible = false;
			this.startButton.Visible = true;
			ControlCheckWindow.isChecking = false;
			ControlCheckWindow.isAllPass = false;
		}

		private void ControlCheckWindow_Load(object sender, EventArgs e)
		{
			ControlCheckWindow.log.Trace("ControlCheckWindow_Load");
			this.startButton.Focus();
		}

		private void startButton_Click(object sender, EventArgs e)
		{
			ControlCheckWindow.log.Trace("startButton_Click");
			if (!ControlCheckWindow.isChecking)
			{
				this.CheckControl();
			}
		}

		private void CheckControl()
		{
			ControlCheckWindow.isChecking = true;
			if (this.connectModel != null)
			{
				this.connectModel.CheckControl();
			}
		}

		public void BeginCheckControl()
		{
			this.progressBox.Image = Resources.connnect_progress;
			this.startButton.Visible = false;
			this.routerLabel.Text = Resources.routerChecking;
			this.routerLabel.Visible = true;
			this.raspberryLabel.Visible = false;
			this.systemLabel.Visible = false;
			this.messageLabel.Visible = false;
		}

		public void OnCheckRouterSSIDFinish(bool modified)
		{
			if (!modified)
			{
				Util.showGrayBackground();
				RouterDialog expr_0D = new RouterDialog();
				expr_0D.setCloseButtonVisibility(true);
				expr_0D.OnRouterDialogClose = new RouterDialog.OnRouterDialogCloseDelegate(this.DialogClose);
				expr_0D.Show(Util.sContext);
			}
		}

		public void OnCheckRouterConnected()
		{
			this.indicatorRouter.Image = Resources.router_success;
			this.lineRouter.Image = Resources.line_loaded;
			this.routerLabel.Text = Resources.routerSuccess;
			this.raspberryLabel.Text = Resources.raspberryChecking;
			this.raspberryLabel.Visible = true;
		}

		public void OnCheckHostConnect()
		{
			this.indicatorRaspberry.Image = Resources.raspberry_success;
			this.lineRaspberry.Image = Resources.line_loaded;
			this.raspberryLabel.Text = Resources.raspberrySuccess;
			this.systemLabel.Text = Resources.systemChecking;
			this.systemLabel.Visible = true;
		}

		public void OnCheckControlError(int error)
		{
			if (error == -1001)
			{
				ControlCheckWindow.isChecking = false;
				this.indicatorRouter.Image = Resources.router_fail;
				this.lineRouter.Image = Resources.line_unload;
				this.routerLabel.Text = Resources.routerFail;
				Util.showGrayBackground();
				new ControlDialog
				{
					OnRetry = new ControlDialog.OnRetryDelegate(this.CheckControl),
					OnCloseClick = new BaseDialogForm.OnCloseClickDelegate(this.DialogClose),
					hasRouter = false
				}.Show(Util.sContext);
				return;
			}
			if (error == -1002)
			{
				ControlCheckWindow.isChecking = false;
				this.indicatorRaspberry.Image = Resources.raspberry_fail;
				this.lineRaspberry.Image = Resources.line_unload;
				this.raspberryLabel.Text = Resources.raspberryFail;
				Util.showGrayBackground();
				new ControlDialog
				{
					OnRetry = new ControlDialog.OnRetryDelegate(this.CheckControl),
					OnCloseClick = new BaseDialogForm.OnCloseClickDelegate(this.DialogClose)
				}.Show(Util.sContext);
				return;
			}
			if (error == -1000)
			{
				ControlCheckWindow.isChecking = false;
				this.indicatorSystem.Image = Resources.system_fail;
				this.lineSystem.Image = Resources.line_unload;
				this.systemLabel.Text = Resources.systemFail;
				Util.showGrayBackground();
				new ControlDialog
				{
					OnRetry = new ControlDialog.OnRetryDelegate(this.CheckControl),
					OnCloseClick = new BaseDialogForm.OnCloseClickDelegate(this.DialogClose)
				}.Show(Util.sContext);
				return;
			}
			if (error == -2000)
			{
				ControlCheckWindow.isChecking = false;
				this.indicatorSystem.Image = Resources.system_fail;
				this.lineSystem.Image = Resources.line_unload;
				this.systemLabel.Text = Resources.systemFail;
				Util.showGrayBackground();
				new ControlDialog
				{
					OnRetry = new ControlDialog.OnRetryDelegate(this.CheckControl),
					OnCloseClick = new BaseDialogForm.OnCloseClickDelegate(this.DialogClose)
				}.Show(Util.sContext);
				return;
			}
			if (error == -3000)
			{
				ControlCheckWindow.isChecking = false;
				this.indicatorSystem.Image = Resources.system_fail;
				this.lineSystem.Image = Resources.line_unload;
				this.systemLabel.Text = Resources.systemFail;
				Util.showGrayBackground();
				new ControlDialog
				{
					OnRetry = new ControlDialog.OnRetryDelegate(this.CheckControl),
					OnCloseClick = new BaseDialogForm.OnCloseClickDelegate(this.DialogClose),
					cableProblem = true
				}.Show(Util.sContext);
				return;
			}
			if (error == 0)
			{
				this.indicatorSystem.Image = Resources.system_success;
				this.lineSystem.Image = Resources.line_loaded;
				this.systemLabel.Text = Resources.systemSuccess;
				this.WirelessThread = new Thread(new ThreadStart(this.WirelessThreadStart));
				this.WirelessThread.Start();
			}
		}

		private void WirelessThreadStart()
		{
			Thread.Sleep(2000);
			this.WirelessFinish();
			if (Settings.Default.displayGuide)
			{
				Thread.Sleep(2000);
				this.ShowGuideImage();
			}
		}

		private void WirelessFinish()
		{
			if (this.OnWirelessFinishListener != null)
			{
				if (base.InvokeRequired)
				{
					base.Invoke(this.OnWirelessFinishListener);
					return;
				}
				this.OnWirelessFinishListener();
			}
		}

		private void OnWirelessFinish()
		{
			ControlCheckWindow.isChecking = false;
			ControlCheckWindow.isAllPass = true;
			this.progressBox.Image = Resources.connect_success_progress;
			this.indicatorWireless.Image = Resources.vr_success;
			this.routerLabel.Visible = false;
			this.raspberryLabel.Visible = false;
			this.systemLabel.Visible = false;
			this.messageLabel.Text = Resources.messageSuccess;
			this.messageLabel.Visible = true;
			LoopCheckModel.StartBackgroundCheckControlThread();
		}

		private void ShowGuideImage()
		{
			if (this.OnControlSuccess != null)
			{
				if (base.InvokeRequired)
				{
					base.Invoke(this.OnControlSuccess);
					return;
				}
				this.OnControlSuccess();
			}
		}

		private void tutorialLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("iexplore", "http://tpcast.cn/index.php?s=/Front/Public/forceDownload/fname/wirelessetup_CH.pdf");
		}

		private void DialogClose()
		{
			this.initView();
		}

		private void guideImagePreview()
		{
			this.startButton.Visible = false;
			this.indicatorRouter.Image = Resources.router_success;
			this.lineRouter.Image = Resources.line_loaded;
			this.indicatorRaspberry.Image = Resources.raspberry_success;
			this.lineRaspberry.Image = Resources.line_loaded;
			this.indicatorSystem.Image = Resources.system_success;
			this.lineSystem.Image = Resources.line_loaded;
			this.progressBox.Image = Resources.connect_success_progress;
			this.indicatorWireless.Image = Resources.vr_success;
			this.routerLabel.Visible = false;
			this.raspberryLabel.Visible = false;
			this.systemLabel.Visible = false;
			this.messageLabel.Text = Resources.messageSuccess;
			this.messageLabel.Visible = true;
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
			ComponentResourceManager arg_15F_0 = new ComponentResourceManager(typeof(ControlCheckWindow));
			this.label4 = new Label();
			this.label3 = new Label();
			this.label2 = new Label();
			this.label1 = new Label();
			this.label6 = new Label();
			this.routerLabel = new Label();
			this.raspberryLabel = new Label();
			this.systemLabel = new Label();
			this.messageLabel = new Label();
			this.startButton = new Button();
			this.progressBox = new PictureBox();
			this.pictureBox1 = new PictureBox();
			this.pcBox = new PictureBox();
			this.lineSystem = new PictureBox();
			this.lineRaspberry = new PictureBox();
			this.lineRouter = new PictureBox();
			this.indicatorWireless = new PictureBox();
			this.indicatorSystem = new PictureBox();
			this.indicatorRaspberry = new PictureBox();
			this.indicatorRouter = new PictureBox();
			((ISupportInitialize)this.progressBox).BeginInit();
			((ISupportInitialize)this.pictureBox1).BeginInit();
			((ISupportInitialize)this.pcBox).BeginInit();
			((ISupportInitialize)this.lineSystem).BeginInit();
			((ISupportInitialize)this.lineRaspberry).BeginInit();
			((ISupportInitialize)this.lineRouter).BeginInit();
			((ISupportInitialize)this.indicatorWireless).BeginInit();
			((ISupportInitialize)this.indicatorSystem).BeginInit();
			((ISupportInitialize)this.indicatorRaspberry).BeginInit();
			((ISupportInitialize)this.indicatorRouter).BeginInit();
			base.SuspendLayout();
			arg_15F_0.ApplyResources(this.label4, "label4");
			this.label4.Name = "label4";
			arg_15F_0.ApplyResources(this.label3, "label3");
			this.label3.Name = "label3";
			arg_15F_0.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			arg_15F_0.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			this.label6.BackColor = Color.FromArgb(230, 230, 230);
			this.label6.ForeColor = Color.Black;
			arg_15F_0.ApplyResources(this.label6, "label6");
			this.label6.Name = "label6";
			arg_15F_0.ApplyResources(this.routerLabel, "routerLabel");
			this.routerLabel.Name = "routerLabel";
			arg_15F_0.ApplyResources(this.raspberryLabel, "raspberryLabel");
			this.raspberryLabel.Name = "raspberryLabel";
			arg_15F_0.ApplyResources(this.systemLabel, "systemLabel");
			this.systemLabel.Name = "systemLabel";
			arg_15F_0.ApplyResources(this.messageLabel, "messageLabel");
			this.messageLabel.Name = "messageLabel";
			this.startButton.BackgroundImage = Resources.blue_background_0;
			arg_15F_0.ApplyResources(this.startButton, "startButton");
			this.startButton.FlatAppearance.BorderSize = 0;
			this.startButton.ForeColor = Color.White;
			this.startButton.Name = "startButton";
			this.startButton.UseVisualStyleBackColor = false;
			this.startButton.Click += new EventHandler(this.startButton_Click);
			this.progressBox.Image = Resources.connnect_progress;
			arg_15F_0.ApplyResources(this.progressBox, "progressBox");
			this.progressBox.Name = "progressBox";
			this.progressBox.TabStop = false;
			arg_15F_0.ApplyResources(this.pictureBox1, "pictureBox1");
			this.pictureBox1.Image = Resources.vive_icon;
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.TabStop = false;
			this.pcBox.Image = Resources.pc_icon;
			arg_15F_0.ApplyResources(this.pcBox, "pcBox");
			this.pcBox.Name = "pcBox";
			this.pcBox.TabStop = false;
			this.lineSystem.Image = Resources.line_unload;
			arg_15F_0.ApplyResources(this.lineSystem, "lineSystem");
			this.lineSystem.Name = "lineSystem";
			this.lineSystem.TabStop = false;
			this.lineRaspberry.Image = Resources.line_unload;
			arg_15F_0.ApplyResources(this.lineRaspberry, "lineRaspberry");
			this.lineRaspberry.Name = "lineRaspberry";
			this.lineRaspberry.TabStop = false;
			this.lineRouter.Image = Resources.line_unload;
			arg_15F_0.ApplyResources(this.lineRouter, "lineRouter");
			this.lineRouter.Name = "lineRouter";
			this.lineRouter.TabStop = false;
			this.indicatorWireless.Image = Resources.vr_normal;
			arg_15F_0.ApplyResources(this.indicatorWireless, "indicatorWireless");
			this.indicatorWireless.Name = "indicatorWireless";
			this.indicatorWireless.TabStop = false;
			this.indicatorSystem.Image = Resources.system_normal;
			arg_15F_0.ApplyResources(this.indicatorSystem, "indicatorSystem");
			this.indicatorSystem.Name = "indicatorSystem";
			this.indicatorSystem.TabStop = false;
			this.indicatorRaspberry.Image = Resources.raspberry_normal;
			arg_15F_0.ApplyResources(this.indicatorRaspberry, "indicatorRaspberry");
			this.indicatorRaspberry.Name = "indicatorRaspberry";
			this.indicatorRaspberry.TabStop = false;
			this.indicatorRouter.Image = Resources.router_normal;
			arg_15F_0.ApplyResources(this.indicatorRouter, "indicatorRouter");
			this.indicatorRouter.Name = "indicatorRouter";
			this.indicatorRouter.TabStop = false;
			arg_15F_0.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.WhiteSmoke;
			base.Controls.Add(this.startButton);
			base.Controls.Add(this.messageLabel);
			base.Controls.Add(this.systemLabel);
			base.Controls.Add(this.raspberryLabel);
			base.Controls.Add(this.label6);
			base.Controls.Add(this.routerLabel);
			base.Controls.Add(this.progressBox);
			base.Controls.Add(this.pictureBox1);
			base.Controls.Add(this.pcBox);
			base.Controls.Add(this.lineSystem);
			base.Controls.Add(this.lineRaspberry);
			base.Controls.Add(this.lineRouter);
			base.Controls.Add(this.label4);
			base.Controls.Add(this.label3);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.indicatorWireless);
			base.Controls.Add(this.indicatorSystem);
			base.Controls.Add(this.indicatorRaspberry);
			base.Controls.Add(this.indicatorRouter);
			base.Name = "ControlCheckWindow";
			base.Load += new EventHandler(this.ControlCheckWindow_Load);
			((ISupportInitialize)this.progressBox).EndInit();
			((ISupportInitialize)this.pictureBox1).EndInit();
			((ISupportInitialize)this.pcBox).EndInit();
			((ISupportInitialize)this.lineSystem).EndInit();
			((ISupportInitialize)this.lineRaspberry).EndInit();
			((ISupportInitialize)this.lineRouter).EndInit();
			((ISupportInitialize)this.indicatorWireless).EndInit();
			((ISupportInitialize)this.indicatorSystem).EndInit();
			((ISupportInitialize)this.indicatorRaspberry).EndInit();
			((ISupportInitialize)this.indicatorRouter).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
