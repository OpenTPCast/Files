using System;
using System.ComponentModel;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using TPCASTWindows.Properties;
using TPCASTWindows.Resources;

namespace TPCASTWindows
{
	public class MainForm : BaseForm
	{
		public delegate void OnTimerFinishDelegate();

		private CheckWindow checkWindow;

		public MainForm.OnTimerFinishDelegate OnTimerFinishListener;

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
			Client.init(this);
			NetworkUtil.Init(this);
			ThreadPool.RegisterWaitForSingleObject(Program.ProgramStarted, new WaitOrTimerCallback(this.OnProgramStarted), null, -1, false);
			string launch_background = Localization.launch_background;
			this.backgroundImage.Image = LocalizeUtil.getImageFromResource(launch_background);
			this.backgroundImage.SizeMode = PictureBoxSizeMode.StretchImage;
			this.backgroundImage.BringToFront();
			System.Timers.Timer expr_74 = new System.Timers.Timer(2000.0);
			expr_74.Elapsed += new ElapsedEventHandler(this.dismissBackgroundImage);
			expr_74.AutoReset = false;
			expr_74.Enabled = true;
			this.OnTimerFinishListener = new MainForm.OnTimerFinishDelegate(this.OnTimerFinish);
			Util.OnControlInterrupt = new Util.OnControlInterruptDelegate(this.ControlInterrupt);
			this.OnRecommendedUpdateClick = new BaseForm.OnRecommendedUpdateClickDelegate(this.recommendedUpdateClick);
			this.OnUpdateDialogClose = new BaseForm.OnUpdateDialogCloseDelegate(this.updateDialogClose);
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
			this.backgroundImage.Visible = false;
			this.showGuideDialog();
		}

		private void showGuideDialog()
		{
			if (Settings.Default.showGuideDialog)
			{
				GuideDialog expr_11 = new GuideDialog();
				expr_11.OnCloseClick = new BaseDialogForm.OnCloseClickDelegate(this.OnCloseClick);
				Util.showGrayBackground();
				expr_11.ShowDialog(this);
			}
		}

		private void OnCloseClick()
		{
			Settings.Default.showGuideDialog = false;
			Settings.Default.Save();
		}

		public void displayGuideImage()
		{
			if (Settings.Default.displayGuide)
			{
				string guide_image = Localization.guide_image;
				this.guideImage.Image = LocalizeUtil.getImageFromResource(guide_image);
				this.guideImage.SizeMode = PictureBoxSizeMode.StretchImage;
				this.guideImage.BringToFront();
				this.guideImage.Visible = true;
			}
		}

		public void ShowControlCheckWindow()
		{
			this.controlCheckWindow = new ControlCheckWindow();
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

		private void ControlInterrupt(int status)
		{
			this.interruptDialog = new ControlInterruptDialog();
			this.interruptDialog.status = status;
			this.interruptDialog.OnBackClick = new ControlInterruptDialog.OnBackClickDelegate(this.ShowControlCheckWindow);
			Util.showGrayBackground();
			this.interruptDialog.ShowDialog(this);
		}

		private void recommendedUpdateClick()
		{
			if (this.controlCheckWindow != null)
			{
				this.controlCheckWindow.removeSocketCallback();
			}
		}

		private void updateDialogClose()
		{
			if (this.controlCheckWindow != null)
			{
				this.controlCheckWindow.addSocketCallback();
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
			Console.WriteLine("MainForm_FormClosing");
			if (this.connectWindow != null)
			{
				this.connectWindow.CloseControl();
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
			ComponentResourceManager arg_4C_0 = new ComponentResourceManager(typeof(MainForm));
			this.windowGroup = new GroupBox();
			this.pictureBox2 = new PictureBox();
			((ISupportInitialize)this.backgroundImage).BeginInit();
			((ISupportInitialize)this.guideImage).BeginInit();
			((ISupportInitialize)this.pictureBox2).BeginInit();
			base.SuspendLayout();
			arg_4C_0.ApplyResources(this.backgroundImage, "backgroundImage");
			arg_4C_0.ApplyResources(this.guideImage, "guideImage");
			arg_4C_0.ApplyResources(this.windowGroup, "windowGroup");
			this.windowGroup.Name = "windowGroup";
			this.windowGroup.TabStop = false;
			arg_4C_0.ApplyResources(this.pictureBox2, "pictureBox2");
			this.pictureBox2.Image = Resources.launch_background;
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.TabStop = false;
			arg_4C_0.ApplyResources(this, "$this");
			base.Controls.Add(this.windowGroup);
			base.Name = "MainForm";
			base.FormClosing += new FormClosingEventHandler(this.MainForm_FormClosing);
			base.Load += new EventHandler(this.MainForm_Load);
			base.Controls.SetChildIndex(this.guideImage, 0);
			base.Controls.SetChildIndex(this.backgroundImage, 0);
			base.Controls.SetChildIndex(this.windowGroup, 0);
			((ISupportInitialize)this.backgroundImage).EndInit();
			((ISupportInitialize)this.guideImage).EndInit();
			((ISupportInitialize)this.pictureBox2).EndInit();
			base.ResumeLayout(false);
		}
	}
}
