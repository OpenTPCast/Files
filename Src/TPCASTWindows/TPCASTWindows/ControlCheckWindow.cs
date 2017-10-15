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
using TPCASTWindows.Resources;
using TPCASTWindows.UI.Update;

namespace TPCASTWindows
{
	public class ControlCheckWindow : UserControl, SocketConnectCallback, SocketExceptonCallback
	{
		private delegate void OnWirelessFinishDelegate();

		public delegate void OnControlSuccessDelegate();

		private SocketModel socketModel;

		private string adapterVersion = "";

		private ForceUpdateDialog forceDialog;

		public static bool isChecking;

		private ControlCheckWindow.OnWirelessFinishDelegate OnWirelessFinishListener;

		private Thread WirelessThread;

		public static bool isAllPass;

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
			this.InitializeComponent();
			Console.WriteLine("ControlCheckWindow");
			Util.BeginCheckControl = new Util.BeginCheckControlDelegate(this.BeginCheckControl);
			Util.OnCheckRouterConnected = new Util.OnCheckRouterConnectedDelegate(this.OnRouterConnected);
			Util.OnCheckHostConnect = new Util.OnCheckHostConnectDelegate(this.OnHostConnected);
			Util.OnCheckControlError = new Util.OnCheckControlErrorDelegate(this.OnControlConnectedError);
			this.OnWirelessFinishListener = new ControlCheckWindow.OnWirelessFinishDelegate(this.OnWirelessFinish);
			this.socketModel = new SocketModel(this);
			this.addSocketCallback();
			this.initView();
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

		public void disconnectSocket()
		{
			if (this.socketModel != null)
			{
				this.socketModel.disconnect();
			}
		}

		private void connectAdapter()
		{
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
				this.socketModel.connect();
			}
		}

		public void OnVersionReceive(string version)
		{
			Console.WriteLine("version = " + version);
			this.adapterVersion = version;
			if (this.socketModel != null)
			{
				this.socketModel.getMac();
			}
		}

		public void OnMacReceive(string mac)
		{
			Console.WriteLine("mac = " + mac);
			this.requestForceUpdate(this.adapterVersion, mac);
		}

		public void OnSendFail()
		{
			Console.WriteLine("OnSendFail");
			if (this.forceDialog == null || !this.forceDialog.IsAccessible)
			{
				this.forceDialog = new ForceUpdateDialog();
				this.forceDialog.OnRetry = new ForceUpdateDialog.OnRetryDelegate(this.connectAdapter);
				Util.showGrayBackground();
				this.forceDialog.ShowDialog(this);
			}
		}

		public void OnReceiveTimeout()
		{
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
			Client.getInstance().requestUpdate(softwareVersion, adapterVersion, sn, delegate(IRestResponse<Update> response)
			{
				if (response != null && response.StatusCode == HttpStatusCode.OK && response.Data != null && response.Data.roms != null)
				{
					string text = "";
					string text2 = "";
					string text3 = "";
					bool isAdapterDownloaded = false;
					string text4 = "";
					string text5 = "";
					bool isSoftwareDownloaded = false;
					if (response.Data.roms.adapter != null && !string.IsNullOrEmpty(adapterVersion) && !string.IsNullOrEmpty(sn))
					{
						UpdateMessage forced = response.Data.roms.adapter.forced;
						if (forced != null)
						{
							text3 = forced.version;
							Version arg_C9_0 = new Version(forced.version);
							Version value = new Version(adapterVersion);
							if (arg_C9_0.CompareTo(value) > 0)
							{
								text = forced.url;
								text2 = forced.md5;
								if (File.Exists(Constants.updateAdapterFilePath))
								{
									string mD5HashFromFile = CryptoUtil.GetMD5HashFromFile(Constants.updateAdapterFilePath);
									Console.WriteLine("cur md5 = " + mD5HashFromFile);
									if (mD5HashFromFile.Equals(forced.md5))
									{
										isAdapterDownloaded = true;
									}
								}
							}
						}
					}
					if (response.Data.roms.software != null)
					{
						UpdateMessage forced2 = response.Data.roms.software.forced;
						if (forced2 != null && new Version(forced2.version).CompareTo(currentSoftwareVersion) > 0)
						{
							text4 = forced2.url;
							text5 = forced2.md5;
							if (File.Exists(Constants.updateSoftwareFilePath) && CryptoUtil.GetMD5HashFromFile(Constants.updateSoftwareFilePath).Equals(forced2.md5))
							{
								isSoftwareDownloaded = true;
							}
						}
					}
					Console.WriteLine("adapterUrl = " + text);
					Console.WriteLine("adapterMd5 = " + text2);
					Console.WriteLine("softwareUrl = " + text4);
					Console.WriteLine("softwareMd5 = " + text5);
					Console.WriteLine("updateAdapterVersionString = " + text3);
					if (!string.IsNullOrEmpty(text) || !string.IsNullOrEmpty(text4))
					{
						this.forceDialog = new ForceUpdateDialog();
						this.forceDialog.adapterUrl = text;
						this.forceDialog.adapterMd5 = text2;
						this.forceDialog.updateAdapterVersionString = text3;
						this.forceDialog.isAdapterDownloaded = isAdapterDownloaded;
						this.forceDialog.softwareUrl = text4;
						this.forceDialog.softwareMd5 = text5;
						this.forceDialog.isSoftwareDownloaded = isSoftwareDownloaded;
						this.forceDialog.OnRetry = new ForceUpdateDialog.OnRetryDelegate(this.connectAdapter);
						Util.showGrayBackground();
						this.forceDialog.ShowDialog(this);
					}
				}
			});
		}

		private void initView()
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
		}

		private void ControlCheckWindow_Load(object sender, EventArgs e)
		{
			this.startButton.Focus();
		}

		private void startButton_Click(object sender, EventArgs e)
		{
			if (!ControlCheckWindow.isChecking)
			{
				Console.WriteLine("startButton_Click");
				this.CheckControl();
			}
		}

		private void CheckControl()
		{
			ControlCheckWindow.isChecking = true;
			Util.CheckControl();
		}

		private void BeginCheckControl()
		{
			this.progressBox.Image = Resources.connnect_progress;
			this.startButton.Visible = false;
			this.routerLabel.Text = Localization.routerChecking;
			this.routerLabel.Visible = true;
			this.raspberryLabel.Visible = false;
			this.systemLabel.Visible = false;
			this.messageLabel.Visible = false;
		}

		private void OnRouterConnected()
		{
			this.indicatorRouter.Image = Resources.router_success;
			this.lineRouter.Image = Resources.line_loaded;
			this.routerLabel.Text = Localization.routerSuccess;
			this.raspberryLabel.Text = Localization.raspberryChecking;
			this.raspberryLabel.Visible = true;
		}

		private void OnHostConnected()
		{
			this.indicatorRaspberry.Image = Resources.raspberry_success;
			this.lineRaspberry.Image = Resources.line_loaded;
			this.raspberryLabel.Text = Localization.raspberrySuccess;
			this.systemLabel.Text = Localization.systemChecking;
			this.systemLabel.Visible = true;
		}

		private void OnControlConnectedError(int error)
		{
			if (error == -1001)
			{
				ControlCheckWindow.isChecking = false;
				this.indicatorRouter.Image = Resources.router_fail;
				this.lineRouter.Image = Resources.line_unload;
				this.routerLabel.Text = Localization.routerFail;
				ControlDialog expr_43 = new ControlDialog();
				expr_43.OnRetry = new ControlDialog.OnRetryDelegate(this.CheckControl);
				expr_43.OnCloseClick = new BaseDialogForm.OnCloseClickDelegate(this.DialogClose);
				expr_43.hasRouter = false;
				Util.showGrayBackground();
				expr_43.ShowDialog(this);
				return;
			}
			if (error == -1002)
			{
				ControlCheckWindow.isChecking = false;
				this.indicatorRaspberry.Image = Resources.raspberry_fail;
				this.lineRaspberry.Image = Resources.line_unload;
				this.raspberryLabel.Text = Localization.raspberryFail;
				ControlDialog expr_BE = new ControlDialog();
				expr_BE.OnRetry = new ControlDialog.OnRetryDelegate(this.CheckControl);
				expr_BE.OnCloseClick = new BaseDialogForm.OnCloseClickDelegate(this.DialogClose);
				Util.showGrayBackground();
				expr_BE.ShowDialog(this);
				return;
			}
			if (error == -1000)
			{
				ControlCheckWindow.isChecking = false;
				this.indicatorSystem.Image = Resources.system_fail;
				this.lineSystem.Image = Resources.line_unload;
				this.systemLabel.Text = Localization.systemFail;
				ControlDialog expr_132 = new ControlDialog();
				expr_132.OnRetry = new ControlDialog.OnRetryDelegate(this.CheckControl);
				expr_132.OnCloseClick = new BaseDialogForm.OnCloseClickDelegate(this.DialogClose);
				Util.showGrayBackground();
				expr_132.ShowDialog(this);
				return;
			}
			if (error == -2000)
			{
				ControlCheckWindow.isChecking = false;
				this.indicatorSystem.Image = Resources.system_fail;
				this.lineSystem.Image = Resources.line_unload;
				this.systemLabel.Text = Localization.systemFail;
				ControlDialog expr_1A6 = new ControlDialog();
				expr_1A6.OnRetry = new ControlDialog.OnRetryDelegate(this.CheckControl);
				expr_1A6.OnCloseClick = new BaseDialogForm.OnCloseClickDelegate(this.DialogClose);
				Util.showGrayBackground();
				expr_1A6.ShowDialog(this);
				return;
			}
			if (error == -3000)
			{
				ControlCheckWindow.isChecking = false;
				this.indicatorSystem.Image = Resources.system_fail;
				this.lineSystem.Image = Resources.line_unload;
				this.systemLabel.Text = Localization.systemFail;
				ControlDialog expr_21A = new ControlDialog();
				expr_21A.OnRetry = new ControlDialog.OnRetryDelegate(this.CheckControl);
				expr_21A.OnCloseClick = new BaseDialogForm.OnCloseClickDelegate(this.DialogClose);
				expr_21A.cableProblem = true;
				Util.showGrayBackground();
				expr_21A.ShowDialog(this);
				return;
			}
			if (error == 0)
			{
				this.indicatorSystem.Image = Resources.system_success;
				this.lineSystem.Image = Resources.line_loaded;
				this.systemLabel.Text = Localization.systemSuccess;
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
			this.progressBox.Image = Resources.connect_success_progres;
			this.indicatorWireless.Image = Resources.vr_success;
			this.routerLabel.Visible = false;
			this.raspberryLabel.Visible = false;
			this.systemLabel.Visible = false;
			this.messageLabel.Text = Localization.messageSuccess;
			this.messageLabel.Visible = true;
			Util.StartBackgroundCheckControlThread();
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
			this.progressBox.Image = Resources.connect_success_progres;
			this.indicatorWireless.Image = Resources.vr_success;
			this.routerLabel.Visible = false;
			this.raspberryLabel.Visible = false;
			this.systemLabel.Visible = false;
			this.messageLabel.Text = Localization.messageSuccess;
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
