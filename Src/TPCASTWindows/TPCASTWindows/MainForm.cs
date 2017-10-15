using NLog;
using System;
using System.ComponentModel;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using TPCASTWindows.Properties;
using TPCASTWindows.UI.Main;
using TPCASTWindows.Utils;

namespace TPCASTWindows
{
	public class MainForm : BaseForm, ConnectLoopInterruptCallback
	{
		public delegate void OnTimerFinishDelegate();

		private static Logger log = LogManager.GetCurrentClassLogger();

		private CheckWindow checkWindow;

		public MainForm.OnTimerFinishDelegate OnTimerFinishListener;

		private ConnectWindowGenTwo connectWindowGenTwo;

		private ControlCheckWindow controlCheckWindow;

		private ConnectWindow connectWindow;

		private ControlInterruptDialog interruptDialog;

		private IContainer components;

		private GroupBox windowGroup;

		private PictureBox pictureBox2;

		public MainForm()
		{
			this.InitializeComponent();
			Util.Init(this);
			ConfigureUtil.init(this);
			Client.init(this);
			NetworkUtil.Init(this);
			LoopCheckModel.Init(this);
			ThreadPool.RegisterWaitForSingleObject(Program.ProgramStarted, new WaitOrTimerCallback(this.OnProgramStarted), null, -1, false);
			this.appLabel.Text = Resources.titleText;
			this.backgroundImage.SizeMode = PictureBoxSizeMode.StretchImage;
			this.backgroundImagePanel.BringToFront();
			System.Timers.Timer expr_79 = new System.Timers.Timer(2000.0);
			expr_79.Elapsed += new ElapsedEventHandler(this.dismissBackgroundImage);
			expr_79.AutoReset = false;
			expr_79.Enabled = true;
			this.OnTimerFinishListener = new MainForm.OnTimerFinishDelegate(this.OnTimerFinish);
			LoopCheckModel.setConnectLoopInterruptCallback(this);
			this.OnRecommendedUpdateClick = new BaseForm.OnRecommendedUpdateClickDelegate(this.recommendedUpdateClick);
			this.OnUpdateDialogClose = new BaseForm.OnUpdateDialogCloseDelegate(this.updateDialogClose);
			this.OnNetworkDialogBackClick = new BaseForm.OnNetworkDialogBackClickDelegate(this.ShowControlCheckWindow);
			this.ShowControlCheckWindow();
		}

		private void OnOutputFile(string msg)
		{
			if (base.InvokeRequired)
			{
				base.Invoke(Util.OnOutputFile, new object[]
				{
					msg
				});
				return;
			}
			MessageBox.Show(msg);
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
		}

		public void dismissBackgroundImage(object sender, ElapsedEventArgs e)
		{
			if (this.backgroundImage.InvokeRequired)
			{
				base.Invoke(this.OnTimerFinishListener);
				return;
			}
			if (this.OnTimerFinishListener != null)
			{
				this.OnTimerFinishListener();
			}
		}

		public void OnTimerFinish()
		{
			MainForm.log.Trace("OnTimerFinish");
			this.backgroundImagePanel.Visible = false;
			if (this.controlCheckWindow != null)
			{
				this.controlCheckWindow.OnSplashImageInvisible();
			}
			if (this.connectWindowGenTwo != null)
			{
				this.connectWindowGenTwo.OnSplashImageInvisible();
			}
		}

		public void displayGuideImage()
		{
			if (Settings.Default.displayGuide)
			{
				Util.showGuideForm();
			}
		}

		public void ShowWindowGenTwo()
		{
			if (this.connectWindowGenTwo == null)
			{
				this.connectWindowGenTwo = new ConnectWindowGenTwo();
			}
			this.connectWindowGenTwo.initView();
			this.connectWindowGenTwo.OnControlSuccess = new ConnectWindowGenTwo.OnControlSuccessDelegate(this.displayGuideImage);
			ConnectWindowGenTwo.isAllPass = false;
			this.windowGroup.Controls.Clear();
			this.windowGroup.Controls.Add(this.connectWindowGenTwo);
		}

		public void ShowControlCheckWindow()
		{
			if (this.controlCheckWindow == null)
			{
				this.controlCheckWindow = new ControlCheckWindow();
			}
			this.controlCheckWindow.initView();
			this.controlCheckWindow.OnControlSuccess = new ControlCheckWindow.OnControlSuccessDelegate(this.displayGuideImage);
			ControlCheckWindow.isAllPass = false;
			this.windowGroup.Controls.Clear();
			this.windowGroup.Controls.Add(this.controlCheckWindow);
		}

		public void ShowCheckWindow()
		{
			this.checkWindow = new CheckWindow();
			CheckWindow.isChecking = false;
			CheckWindow.isAllPass = false;
			CheckWindow.failTimes = 0;
			this.checkWindow.OnConnect = new CheckWindow.OnConnectClickDelegate(this.Connect);
			if (this.connectWindow != null)
			{
				this.connectWindow.CloseConnectControl();
			}
			this.windowGroup.Controls.Clear();
			this.windowGroup.Controls.Add(this.checkWindow);
		}

		public void Connect()
		{
			this.connectWindow = new ConnectWindow();
			this.connectWindow.OnBackClickListener = new ConnectWindow.OnBackClickDelegate(this.ShowCheckWindow);
			this.connectWindow.OnLaunchSteamVRSuccess = new ConnectWindow.OnLaunchSteamVRSuccessDelegate(this.displayGuideImage);
			this.windowGroup.Controls.Clear();
			this.windowGroup.Controls.Add(this.connectWindow);
		}

		public void OnControlInterrupt(int status)
		{
			Util.showGrayBackground();
			this.interruptDialog = new ControlInterruptDialog();
			this.interruptDialog.status = status;
			this.interruptDialog.OnBackClick = new ControlInterruptDialog.OnBackClickDelegate(this.ShowControlCheckWindow);
			this.interruptDialog.Show(Util.sContext);
		}

		private void recommendedUpdateClick()
		{
			if (this.controlCheckWindow != null)
			{
				this.controlCheckWindow.removeSocketCallback();
			}
			if (this.connectWindowGenTwo != null)
			{
				this.connectWindowGenTwo.removeSocketCallback();
			}
		}

		private void updateDialogClose()
		{
			if (this.controlCheckWindow != null)
			{
				this.controlCheckWindow.addSocketCallback();
			}
			if (this.connectWindowGenTwo != null)
			{
				this.connectWindowGenTwo.addSocketCallback();
			}
		}

		private void OnProgramStarted(object state, bool timeout)
		{
			if (base.InvokeRequired)
			{
				base.Invoke(new WaitOrTimerCallback(this.OnProgramStarted), new object[]
				{
					state,
					timeout
				});
				return;
			}
			base.Show();
			base.ShowInTaskbar = true;
			base.WindowState = FormWindowState.Normal;
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			MainForm.log.Trace("MainForm_FormClosing");
			if (this.controlCheckWindow != null)
			{
				this.controlCheckWindow.OnFormClosing();
			}
			if (this.connectWindow != null)
			{
				this.connectWindow.CloseControl();
			}
			if (this.connectWindowGenTwo != null)
			{
				this.connectWindowGenTwo.OnFormClosing();
			}
		}

		private void MainForm_Shown(object sender, EventArgs e)
		{
			MainForm.log.Trace("MainForm_Shown");
			if (this.controlCheckWindow != null)
			{
				this.controlCheckWindow.MainFormShown();
			}
			if (this.connectWindowGenTwo != null)
			{
				this.connectWindowGenTwo.MainFormShown();
			}
		}

		private void MainForm_KeyDown(object sender, KeyEventArgs e)
		{
			MainForm.log.Trace("MainForm_KeyDown kc = " + e.KeyCode);
			MainForm.log.Trace("MainForm_KeyDown m = " + e.Modifiers);
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
			ComponentResourceManager arg_57_0 = new ComponentResourceManager(typeof(MainForm));
			this.windowGroup = new GroupBox();
			this.pictureBox2 = new PictureBox();
			((ISupportInitialize)this.backgroundImage).BeginInit();
			((ISupportInitialize)this.guideImage).BeginInit();
			this.backgroundImagePanel.SuspendLayout();
			((ISupportInitialize)this.pictureBox2).BeginInit();
			base.SuspendLayout();
			arg_57_0.ApplyResources(this.windowGroup, "windowGroup");
			this.windowGroup.Name = "windowGroup";
			this.windowGroup.TabStop = false;
			this.pictureBox2.Image = Resources.launch_background;
			arg_57_0.ApplyResources(this.pictureBox2, "pictureBox2");
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.TabStop = false;
			arg_57_0.ApplyResources(this, "$this");
			base.Controls.Add(this.windowGroup);
			base.Name = "MainForm";
			base.FormClosing += new FormClosingEventHandler(this.MainForm_FormClosing);
			base.Load += new EventHandler(this.MainForm_Load);
			base.Shown += new EventHandler(this.MainForm_Shown);
			base.KeyDown += new KeyEventHandler(this.MainForm_KeyDown);
			base.Controls.SetChildIndex(this.backgroundImagePanel, 0);
			base.Controls.SetChildIndex(this.guideImage, 0);
			base.Controls.SetChildIndex(this.windowGroup, 0);
			((ISupportInitialize)this.backgroundImage).EndInit();
			((ISupportInitialize)this.guideImage).EndInit();
			this.backgroundImagePanel.ResumeLayout(false);
			((ISupportInitialize)this.pictureBox2).EndInit();
			base.ResumeLayout(false);
		}
	}
}
