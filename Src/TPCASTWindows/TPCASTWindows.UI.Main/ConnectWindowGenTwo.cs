using NLog;
using RestSharp;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using TPCASTWindows.Properties;
using TPCASTWindows.UI.Update;
using TPCASTWindows.Utils;

namespace TPCASTWindows.UI.Main
{
	public class ConnectWindowGenTwo : UserControl, ControlConnectCallback, SocketConnectCallback, SocketExceptonCallback
	{
		public delegate void OnControlSuccessDelegate();

		private static Logger log = LogManager.GetCurrentClassLogger();

		private ControlConnectModel connectModel;

		private SocketModel socketModel;

		private string adapterVersion = "";

		private ForceUpdateDialog forceDialog;

		public static bool isChecking = false;

		public static bool isAllPass = false;

		public ConnectWindowGenTwo.OnControlSuccessDelegate OnControlSuccess;

		private IContainer components;

		private Label label6;

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

		private CustomLabel messageLabel;

		private Button startButton;

		public ConnectWindowGenTwo()
		{
			ConnectWindowGenTwo.log.Trace("ConnectWindowGenTwo");
			this.InitializeComponent();
			this.connectModel = new ControlConnectModel(this);
			this.connectModel.addControlConnectCallback(this);
			this.socketModel = new SocketModel(this);
			this.addSocketCallback();
			this.initView();
			ConnectWindowGenTwo.log.Trace("ip == " + ConfigureUtil.AdapterIP());
			ConnectWindowGenTwo.log.Trace("type = " + ConfigureUtil.getClientType());
			NetworkChange.NetworkAddressChanged += new NetworkAddressChangedEventHandler(ConnectWindowGenTwo.AddressChangedCallback);
			Console.WriteLine("Listening for address changes. Press any key to exit.");
		}

		private static void AddressChangedCallback(object sender, EventArgs e)
		{
			NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
			for (int j = 0; j < allNetworkInterfaces.Length; j++)
			{
				NetworkInterface i = allNetworkInterfaces[j];
				Console.WriteLine("   {0} is {1}", i.Name, i.OperationalStatus);
			}
		}

		public void MainFormShown()
		{
			ConnectWindowGenTwo.log.Trace("MainFormShown");
		}

		public void OnFormClosing()
		{
			ConnectWindowGenTwo.log.Trace("OnFormClosing");
			if (this.connectModel != null)
			{
				this.connectModel.unInit();
			}
		}

		public void OnSplashImageInvisible()
		{
			ConnectWindowGenTwo.log.Trace("OnSplashImageInvisible");
			this.showGuideDialog();
		}

		private void showGuideDialog()
		{
			ConnectWindowGenTwo.log.Trace("showGuideDialog");
			if (Settings.Default.showGuideDialog)
			{
				Util.showGrayBackground();
				new GuideDialog
				{
					OnCloseClick = new BaseDialogForm.OnCloseClickDelegate(this.OnCloseClick)
				}.ShowDialog(this);
				return;
			}
			this.checkRouterSSID();
		}

		private void OnCloseClick()
		{
			ConnectWindowGenTwo.log.Trace("OnCloseClick");
			Settings.Default.showGuideDialog = false;
			Settings.Default.Save();
			this.checkRouterSSID();
		}

		private void checkRouterSSID()
		{
			ConnectWindowGenTwo.log.Trace("checkRouterSSID");
			if (this.connectModel != null)
			{
				this.connectModel.InitCheckRouterSSID();
			}
		}

		private void modifyWifiSSID()
		{
			Util.showGrayBackground();
			RouterDialog expr_0A = new RouterDialog();
			expr_0A.setCloseButtonVisibility(false);
			expr_0A.OnRouterDialogClose = new RouterDialog.OnRouterDialogCloseDelegate(this.OnRouterDialogClose);
			expr_0A.ShowDialog(this);
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
				ConnectWindowGenTwo.log.Trace("connect fail");
				this.requestForceUpdate("", "");
			}
		}

		public void requestUpdateWhenAdapterConnected()
		{
			new Thread(new ThreadStart(this.adapterConnectedThreadStart)).Start();
		}

		private void adapterConnectedThreadStart()
		{
			while (!TPCASTUtil.isHostConnected())
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
			ConnectWindowGenTwo.log.Trace("version = " + version);
			this.adapterVersion = version;
			if (this.socketModel != null)
			{
				this.socketModel.GetMac();
			}
		}

		public void OnMacReceive(string mac)
		{
			ConnectWindowGenTwo.log.Trace("mac = " + mac);
			this.requestForceUpdate(this.adapterVersion, mac);
		}

		public void OnSendFail()
		{
			ConnectWindowGenTwo.log.Trace("OnSendFail");
			this.requestForceUpdate("", "");
		}

		public void OnReceiveTimeout()
		{
			ConnectWindowGenTwo.log.Trace("OnReceiveTimeout");
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
			ConnectWindowGenTwo.log.Trace("CreateDirectory");
			Client.getInstance().requestUpdate(softwareVersion, adapterVersion, sn, delegate(IRestResponse<Update> response)
			{
				ConnectWindowGenTwo.log.Trace("requestUpdate");
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
									ConnectWindowGenTwo.log.Trace("cur md5 = " + currentMd5);
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
					ConnectWindowGenTwo.log.Trace("adapterUrl = " + adapterUrl);
					ConnectWindowGenTwo.log.Trace("adapterMd5 = " + adapterMd5);
					ConnectWindowGenTwo.log.Trace("softwareUrl = " + softwareUrl);
					ConnectWindowGenTwo.log.Trace("softwareMd5 = " + softwareMd5);
					ConnectWindowGenTwo.log.Trace("updateAdapterVersionString = " + updateAdapterVersionString);
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
						this.forceDialog.ShowDialog(this);
						return;
					}
				}
				ConnectWindowGenTwo.log.Trace("driver");
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
					new ServerExceptionDialog().ShowDialog(this);
					return;
				}
			}
			else
			{
				BroadcastModel.instance.send(91);
				Util.showGrayBackground();
				new DriverDialog().ShowDialog(this);
			}
		}

		public void initView()
		{
			ConnectWindowGenTwo.log.Trace("initView");
			this.indicatorRouter.Image = Resources.router_normal_gen2;
			this.lineRouter.Image = Resources.line_unload_gen2;
			this.indicatorRaspberry.Image = Resources.raspberry_normal_gen2;
			this.lineRaspberry.Image = Resources.line_unload_gen2;
			this.indicatorSystem.Image = Resources.system_normal_gen2;
			this.lineSystem.Image = Resources.line_unload_gen2;
			this.indicatorWireless.Image = Resources.vr_normal_gen2;
			this.messageLabel.Visible = false;
			this.startButton.Visible = true;
			ConnectWindowGenTwo.isChecking = false;
			ConnectWindowGenTwo.isAllPass = false;
		}

		private void ConnectWindowGenTwo_Load(object sender, EventArgs e)
		{
			ConnectWindowGenTwo.log.Trace("ConnectWindowGenTwo_Load");
			this.startButton.Focus();
		}

		private void startButton_Click(object sender, EventArgs e)
		{
			ConnectWindowGenTwo.log.Trace("startButton_Click");
			if (!ConnectWindowGenTwo.isChecking)
			{
				this.CheckControl();
			}
		}

		private void CheckControl()
		{
			ConnectWindowGenTwo.isChecking = true;
			if (this.connectModel != null)
			{
				this.connectModel.CheckControl();
			}
		}

		public void BeginCheckControl()
		{
			ConnectWindowGenTwo.log.Trace("BeginCheckControl");
			this.startButton.Visible = false;
			this.messageLabel.Text = Resources.routerChecking;
			this.messageLabel.Visible = true;
		}

		public void CheckControlCode(ConnectCode code)
		{
			ConnectWindowGenTwo.log.Trace("CheckControlCode code = " + code);
			switch (code)
			{
			case ConnectCode.SSID_NOT_MODIFIED:
				this.modifyWifiSSID();
				return;
			case ConnectCode.SSID_MODIFIED:
				this.connectAdapter();
				return;
			case ConnectCode.INIT_CHECK_ROURTER_FAIL:
				this.connectAdapter();
				return;
			case ConnectCode.BEGIN_CHECK_ROUTER:
				this.BeginCheckControl();
				return;
			case ConnectCode.CHECK_ROUTER_FAIL:
				this.OnCheckRouterConnected(false);
				return;
			case ConnectCode.CHECK_ROUTER_SUCCESS:
				this.OnCheckRouterConnected(true);
				return;
			case ConnectCode.CHECK_ROUTER_SSID_NOT_MODIFIED:
				this.OnCheckRouterSSIDFinish(false);
				return;
			case ConnectCode.CHECK_ROUTER_SSID_MODIFIED:
			case ConnectCode.BEGIN_CHECK_HOST:
			case ConnectCode.CHECK_CABLE_SUCCESS:
				break;
			case ConnectCode.CHECK_HOST_FAIL:
				this.OnCheckHostConnect(false);
				return;
			case ConnectCode.CHECK_HOST_SUCCESS:
				this.OnCheckHostConnect(true);
				return;
			case ConnectCode.CHECK_CABLE_FAIL:
				this.OnCheckCabelConnect(false);
				return;
			case ConnectCode.CHECK_VR_SUCCESS:
				this.OnCheckVR();
				return;
			case ConnectCode.SHOW_GUIDE_IMAGE:
				this.OnShowGuideImage();
				break;
			default:
				return;
			}
		}

		public void OnCheckRouterSSIDFinish(bool modified)
		{
			if (!modified)
			{
				Util.showGrayBackground();
				RouterDialog expr_0D = new RouterDialog();
				expr_0D.setCloseButtonVisibility(true);
				expr_0D.OnRouterDialogClose = new RouterDialog.OnRouterDialogCloseDelegate(this.DialogClose);
				expr_0D.ShowDialog(this);
			}
		}

		public void OnCheckRouterConnected(bool connected)
		{
			if (connected)
			{
				this.indicatorRouter.Image = Resources.router_success_gen2;
				this.lineRouter.Image = Resources.line_loaded_gen2;
				string message = Resources.routerSuccess + Environment.NewLine + Resources.raspberryChecking;
				this.messageLabel.Text = message;
				ConnectWindowGenTwo.log.Trace("OnCheckRouterConnected " + message);
				return;
			}
			this.indicatorRouter.Image = Resources.router_fail_gen2;
			this.lineRouter.Image = Resources.line_unload_gen2;
			this.indicatorRaspberry.Image = Resources.raspberry_normal_gen2;
			this.lineRaspberry.Image = Resources.line_unload_gen2;
			this.indicatorSystem.Image = Resources.system_normal_gen2;
			this.lineSystem.Image = Resources.line_unload_gen2;
			this.indicatorWireless.Image = Resources.vr_normal_gen2;
			string message2 = Resources.routerFail;
			this.messageLabel.Text = message2;
		}

		public void OnCheckHostConnect(bool connected)
		{
			if (connected)
			{
				this.indicatorRaspberry.Image = Resources.raspberry_success_gen2;
				this.lineRaspberry.Image = Resources.line_loaded_gen2;
				string message = string.Concat(new string[]
				{
					Resources.routerSuccess,
					Environment.NewLine,
					Resources.raspberrySuccess,
					Environment.NewLine,
					Resources.systemChecking
				});
				this.messageLabel.Text = message;
				return;
			}
			ConnectWindowGenTwo.isChecking = false;
			this.indicatorRaspberry.Image = Resources.raspberry_fail_gen2;
			this.lineRaspberry.Image = Resources.line_unload_gen2;
			this.indicatorSystem.Image = Resources.system_normal_gen2;
			this.lineSystem.Image = Resources.line_unload_gen2;
			this.indicatorWireless.Image = Resources.vr_normal_gen2;
			string message2 = Resources.routerSuccess + Environment.NewLine + Resources.raspberryFail;
			this.messageLabel.Text = message2;
		}

		private void OnCheckCabelConnect(bool connected)
		{
			if (!connected)
			{
				ConnectWindowGenTwo.isChecking = false;
				this.indicatorSystem.Image = Resources.system_fail_gen2;
				this.lineSystem.Image = Resources.line_unload_gen2;
				this.indicatorWireless.Image = Resources.vr_normal_gen2;
				string message = string.Concat(new string[]
				{
					Resources.routerSuccess,
					Environment.NewLine,
					Resources.raspberrySuccess,
					Environment.NewLine,
					Resources.systemFail
				});
				this.messageLabel.Text = message;
			}
		}

		private void DialogClose()
		{
			if (this.connectModel != null)
			{
				this.connectModel.ssidModified = true;
			}
			this.CheckControl();
		}

		public void DevicesStatusChange(int count, int state)
		{
			if (count == 4)
			{
				uint DeviceHMDMask = 1u;
				uint DeviceHand1Mask = 2u;
				uint DeviceHand2Mask = 4u;
				uint DeviceLightHouseMask = 8u;
				bool DeviceHMDConnected = ((ulong)DeviceHMDMask & (ulong)((long)state)) == (ulong)DeviceHMDMask;
				bool DeviceHand1Connected = ((ulong)DeviceHand1Mask & (ulong)((long)state)) == (ulong)DeviceHand1Mask;
				bool DeviceHand2Connected = ((ulong)DeviceHand2Mask & (ulong)((long)state)) == (ulong)DeviceHand2Mask;
				bool DeviceLightHouseConnected = ((ulong)DeviceLightHouseMask & (ulong)((long)state)) == (ulong)DeviceLightHouseMask;
				string message = string.Concat(new string[]
				{
					"灯塔定位器 ",
					DeviceHMDConnected ? "已连接" : "未连接",
					Environment.NewLine,
					"头盔 ",
					DeviceHand1Connected ? "已连接" : "未连接",
					Environment.NewLine,
					"手柄1 ",
					DeviceHand2Connected ? "已连接" : "未连接",
					Environment.NewLine,
					"手柄2 ",
					DeviceLightHouseConnected ? "已连接" : "未连接",
					Environment.NewLine
				});
				this.messageLabel.Text = message;
				if (state == 15)
				{
					this.indicatorSystem.Image = Resources.system_success_gen2;
					this.lineSystem.Image = Resources.line_loaded_gen2;
					string successMessage = Resources.systemSuccess;
					this.messageLabel.Text = successMessage;
					return;
				}
				ConnectWindowGenTwo.log.Trace("isAllPass " + ConnectWindowGenTwo.isAllPass.ToString());
				this.indicatorWireless.Image = Resources.vr_normal_gen2;
				if (ConnectWindowGenTwo.isAllPass)
				{
					ConnectWindowGenTwo.isAllPass = false;
					this.indicatorSystem.Image = Resources.system_fail_gen2;
					this.lineSystem.Image = Resources.line_unload_gen2;
					return;
				}
			}
			else if (count == 2)
			{
				uint DeviceHMDMask2 = 1u;
				uint DeviceUSBMask = 2u;
				bool DeviceHMDConnected2 = ((ulong)DeviceHMDMask2 & (ulong)((long)state)) == (ulong)DeviceHMDMask2;
				bool DeviceUSBConnected = ((ulong)DeviceUSBMask & (ulong)((long)state)) == (ulong)DeviceUSBMask;
				string message2 = string.Concat(new string[]
				{
					"Device1 ",
					DeviceHMDConnected2 ? "已连接" : "未连接",
					Environment.NewLine,
					"Device2 ",
					DeviceUSBConnected ? "已连接" : "未连接",
					Environment.NewLine
				});
				this.messageLabel.Text = message2;
				if (state == 3)
				{
					this.indicatorSystem.Image = Resources.system_success_gen2;
					this.lineSystem.Image = Resources.line_loaded_gen2;
					string successMessage2 = Resources.systemSuccess;
					this.messageLabel.Text = successMessage2;
					return;
				}
				ConnectWindowGenTwo.log.Trace("isAllPass " + ConnectWindowGenTwo.isAllPass.ToString());
				this.indicatorWireless.Image = Resources.vr_normal_gen2;
				if (ConnectWindowGenTwo.isAllPass)
				{
					ConnectWindowGenTwo.isAllPass = false;
					this.indicatorSystem.Image = Resources.system_fail_gen2;
					this.lineSystem.Image = Resources.line_unload_gen2;
				}
			}
		}

		private void OnCheckVR()
		{
			ConnectWindowGenTwo.isChecking = false;
			ConnectWindowGenTwo.isAllPass = true;
			this.indicatorWireless.Image = Resources.vr_success_gen2;
			this.messageLabel.Text = Resources.messageSuccess;
			this.messageLabel.Visible = true;
		}

		private void OnShowGuideImage()
		{
			this.ShowGuideImage();
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
			ComponentResourceManager arg_10D_0 = new ComponentResourceManager(typeof(ConnectWindowGenTwo));
			this.label6 = new Label();
			this.label4 = new Label();
			this.label3 = new Label();
			this.label2 = new Label();
			this.label1 = new Label();
			this.lineSystem = new PictureBox();
			this.lineRaspberry = new PictureBox();
			this.lineRouter = new PictureBox();
			this.indicatorWireless = new PictureBox();
			this.indicatorSystem = new PictureBox();
			this.indicatorRaspberry = new PictureBox();
			this.indicatorRouter = new PictureBox();
			this.startButton = new Button();
			this.messageLabel = new CustomLabel(this.components);
			((ISupportInitialize)this.lineSystem).BeginInit();
			((ISupportInitialize)this.lineRaspberry).BeginInit();
			((ISupportInitialize)this.lineRouter).BeginInit();
			((ISupportInitialize)this.indicatorWireless).BeginInit();
			((ISupportInitialize)this.indicatorSystem).BeginInit();
			((ISupportInitialize)this.indicatorRaspberry).BeginInit();
			((ISupportInitialize)this.indicatorRouter).BeginInit();
			base.SuspendLayout();
			arg_10D_0.ApplyResources(this.label6, "label6");
			this.label6.BackColor = Color.FromArgb(230, 230, 230);
			this.label6.ForeColor = Color.Black;
			this.label6.Name = "label6";
			arg_10D_0.ApplyResources(this.label4, "label4");
			this.label4.Name = "label4";
			arg_10D_0.ApplyResources(this.label3, "label3");
			this.label3.Name = "label3";
			arg_10D_0.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			arg_10D_0.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			arg_10D_0.ApplyResources(this.lineSystem, "lineSystem");
			this.lineSystem.Image = Resources.line_unload_gen2;
			this.lineSystem.Name = "lineSystem";
			this.lineSystem.TabStop = false;
			arg_10D_0.ApplyResources(this.lineRaspberry, "lineRaspberry");
			this.lineRaspberry.Image = Resources.line_unload_gen2;
			this.lineRaspberry.Name = "lineRaspberry";
			this.lineRaspberry.TabStop = false;
			arg_10D_0.ApplyResources(this.lineRouter, "lineRouter");
			this.lineRouter.Image = Resources.line_unload_gen2;
			this.lineRouter.Name = "lineRouter";
			this.lineRouter.TabStop = false;
			arg_10D_0.ApplyResources(this.indicatorWireless, "indicatorWireless");
			this.indicatorWireless.Image = Resources.vr_normal_gen2;
			this.indicatorWireless.Name = "indicatorWireless";
			this.indicatorWireless.TabStop = false;
			arg_10D_0.ApplyResources(this.indicatorSystem, "indicatorSystem");
			this.indicatorSystem.Image = Resources.system_normal_gen2;
			this.indicatorSystem.Name = "indicatorSystem";
			this.indicatorSystem.TabStop = false;
			arg_10D_0.ApplyResources(this.indicatorRaspberry, "indicatorRaspberry");
			this.indicatorRaspberry.Image = Resources.raspberry_normal_gen2;
			this.indicatorRaspberry.Name = "indicatorRaspberry";
			this.indicatorRaspberry.TabStop = false;
			arg_10D_0.ApplyResources(this.indicatorRouter, "indicatorRouter");
			this.indicatorRouter.Image = Resources.router_normal_gen2;
			this.indicatorRouter.Name = "indicatorRouter";
			this.indicatorRouter.TabStop = false;
			arg_10D_0.ApplyResources(this.startButton, "startButton");
			this.startButton.BackgroundImage = Resources.blue_background_0;
			this.startButton.FlatAppearance.BorderSize = 0;
			this.startButton.ForeColor = Color.White;
			this.startButton.Name = "startButton";
			this.startButton.UseVisualStyleBackColor = false;
			this.startButton.Click += new EventHandler(this.startButton_Click);
			arg_10D_0.ApplyResources(this.messageLabel, "messageLabel");
			this.messageLabel.LineDistance = 3;
			this.messageLabel.Name = "messageLabel";
			arg_10D_0.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.WhiteSmoke;
			base.Controls.Add(this.startButton);
			base.Controls.Add(this.messageLabel);
			base.Controls.Add(this.label6);
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
			base.Name = "ConnectWindowGenTwo";
			base.Load += new EventHandler(this.ConnectWindowGenTwo_Load);
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
