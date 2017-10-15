using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using TPCASTWindows.Properties;

namespace TPCASTWindows
{
	public class ConnectWindow : UserControl
	{
		public delegate void OnConnectSystemFinishDelegate();

		public delegate void OnConnectControlFinishDelegate();

		public delegate void OnConnectBluetootFinishDelegate();

		public delegate void OnLaunchSteamVRSuccessDelegate();

		public delegate void OnBackClickDelegate();

		private Util.OnControlInterruptDelegate tempInterrupDelegate;

		private Color colorBlue = Color.FromArgb(0, 159, 255);

		private Color colorGray = Color.FromArgb(153, 153, 153);

		private bool isInterrupt;

		private int step;

		public ConnectWindow.OnConnectSystemFinishDelegate OnConnectSystemFinishListener;

		private Thread ConnectControlThread;

		public ConnectWindow.OnConnectControlFinishDelegate OnConnectControlFinishListener;

		public ConnectWindow.OnConnectBluetootFinishDelegate OnConnectBluetootFinishListener;

		public ConnectWindow.OnLaunchSteamVRSuccessDelegate OnLaunchSteamVRSuccess;

		private Thread ShowGuideImageThread;

		public ConnectWindow.OnBackClickDelegate OnBackClickListener;

		private IContainer components;

		private PictureBox indicatorSystem;

		private PictureBox indicatorControl;

		private PictureBox indicatorBluetooth;

		private PictureBox indicatorSteam;

		private Label label1;

		private Label label2;

		private Label label3;

		private Label label4;

		private Label messageLabel;

		private Label label5;

		private Button backButton;

		private PictureBox lineSystem;

		private PictureBox lineControl;

		private PictureBox lineBluetooth;

		private PictureBox pcBox;

		private PictureBox pictureBox1;

		private PictureBox progressBox;

		public ConnectWindow()
		{
			this.InitializeComponent();
			this.messageLabel.Visible = false;
			this.lineSystem.BackgroundImage = Resources.line_unload;
			this.lineControl.BackgroundImage = Resources.line_unload;
			this.lineBluetooth.BackgroundImage = Resources.line_unload;
			this.indicatorSystem.Image = Resources.system_unload;
			this.indicatorControl.Image = Resources.control_disconnect;
			this.indicatorBluetooth.Image = Resources.voice_disconnect;
			this.indicatorSteam.Image = Resources.vr_unstart;
			this.backButton.Visible = false;
			this.messageLabel.Visible = true;
			Util.OnConnectAnimateionResume = new Util.OnConnectAnimateionResumeDelegate(this.OnConnectAnimateionResume);
			Util.OnConnectAnimateionPause = new Util.OnConnectAnimateionPauseDelegate(this.OnConnectAnimateionPause);
			this.progressBox.Image = Resources.connnect_progress;
			this.connectSystem();
		}

		private void connectSystem()
		{
			Console.WriteLine("connectSystem isInterrupt = " + this.isInterrupt.ToString());
			if (this.isInterrupt)
			{
				this.step = 1;
				return;
			}
			System.Timers.Timer expr_3E = new System.Timers.Timer(2000.0);
			expr_3E.Elapsed += new ElapsedEventHandler(this.connectSystemFinish);
			expr_3E.AutoReset = false;
			expr_3E.Enabled = true;
			this.OnConnectSystemFinishListener = new ConnectWindow.OnConnectSystemFinishDelegate(this.OnConnectSystemFinish);
		}

		public void connectSystemFinish(object sender, ElapsedEventArgs e)
		{
			this.OnConnectSystemFinish();
		}

		public void OnConnectSystemFinish()
		{
			if (base.InvokeRequired)
			{
				base.Invoke(this.OnConnectSystemFinishListener);
				return;
			}
			this.indicatorSystem.Image = Resources.system_loaded;
			this.lineSystem.BackgroundImage = Resources.line_loaded;
			this.connectControl();
		}

		public void connectControl()
		{
			Console.WriteLine("connectControl isInterrupt = " + this.isInterrupt.ToString());
			if (this.isInterrupt)
			{
				this.step = 2;
				return;
			}
			this.ConnectControlThread = new Thread(new ThreadStart(this.connectControlStart));
			this.ConnectControlThread.Start();
		}

		public void connectControlStart()
		{
			Thread.Sleep(2000);
			this.OnConnectControlFinishListener = new ConnectWindow.OnConnectControlFinishDelegate(this.OnConnectControlFinish);
			if (this.OnConnectControlFinishListener != null)
			{
				this.OnConnectControlFinishListener();
			}
		}

		public void OnConnectControlFinish()
		{
			if (base.InvokeRequired)
			{
				base.Invoke(this.OnConnectControlFinishListener);
				return;
			}
			this.indicatorControl.Image = Resources.control_connected;
			this.lineControl.BackgroundImage = Resources.line_loaded;
			this.connectBluetooth();
		}

		public void AbortConnectControlThread()
		{
			Console.WriteLine("AboutConnectControlThread");
			if (this.ConnectControlThread != null)
			{
				this.ConnectControlThread.Abort();
			}
		}

		public void connectBluetooth()
		{
			Console.WriteLine("connectBluetooth isInterrupt = " + this.isInterrupt.ToString());
			if (this.isInterrupt)
			{
				this.step = 3;
				return;
			}
			System.Timers.Timer expr_3E = new System.Timers.Timer(2000.0);
			expr_3E.Elapsed += new ElapsedEventHandler(this.connectBluetoothFinish);
			expr_3E.AutoReset = false;
			expr_3E.Enabled = true;
			this.OnConnectBluetootFinishListener = new ConnectWindow.OnConnectBluetootFinishDelegate(this.OnConnectBluetootFinish);
		}

		public void connectBluetoothFinish(object sender, ElapsedEventArgs e)
		{
			Console.WriteLine("connectBluetoothFinish");
			this.OnConnectBluetootFinish();
		}

		public void OnConnectBluetootFinish()
		{
			if (base.InvokeRequired)
			{
				base.Invoke(this.OnConnectBluetootFinishListener);
				return;
			}
			this.indicatorBluetooth.Image = Resources.voice_connected;
			this.lineBluetooth.BackgroundImage = Resources.line_loaded;
			this.connectSteam();
		}

		public void connectSteam()
		{
			Console.WriteLine("connectSteam isInterrupt = " + this.isInterrupt.ToString());
			if (this.isInterrupt)
			{
				this.step = 4;
				return;
			}
			if (Util.isViveInstalled() && Util.isViveSteamVRInstalled())
			{
				this.indicatorSteam.Image = Resources.vr_started;
				this.messageLabel.Text = "连接成功！";
				Util.LaunchSteam();
			}
			else if (!string.IsNullOrEmpty(Util.searchSteamPath()))
			{
				this.indicatorSteam.Image = Resources.vr_started;
				this.messageLabel.Text = "连接成功！";
				Util.LaunchSteam();
			}
			else
			{
				this.indicatorSteam.Image = Resources.vr_unstart;
				this.backButton.Visible = true;
				this.messageLabel.Text = "连接失败！";
			}
			this.progressBox.Image = Resources.connect_success_progres;
			if (Settings.Default.displayGuide)
			{
				this.ShowGuideImage();
			}
		}

		private void ShowGuideImage()
		{
			Console.WriteLine("ShowGuideImage isInterrupt = " + this.isInterrupt.ToString());
			if (this.isInterrupt)
			{
				this.step = 5;
				return;
			}
			this.ShowGuideImageThread = new Thread(new ThreadStart(this.ShowGuideImageThreadStart));
			this.ShowGuideImageThread.Start();
		}

		private void ShowGuideImageThreadStart()
		{
			Thread.Sleep(2000);
			if (base.InvokeRequired)
			{
				base.Invoke(this.OnLaunchSteamVRSuccess);
				return;
			}
			if (this.OnLaunchSteamVRSuccess != null)
			{
				this.OnLaunchSteamVRSuccess();
			}
		}

		public void AbortShowGuideImageThread()
		{
			Console.WriteLine("AbortShowGuideImageThread");
			if (this.ShowGuideImageThread != null)
			{
				this.ShowGuideImageThread.Abort();
			}
		}

		public void CloseControl()
		{
			Console.WriteLine("Connect Control Close");
			this.AbortConnectControlThread();
			this.AbortShowGuideImageThread();
		}

		public void OnConnectAnimateionPause()
		{
			this.isInterrupt = true;
		}

		private void OnConnectAnimateionResume()
		{
			this.isInterrupt = false;
			this.ReConnect();
		}

		public void ReConnect()
		{
			switch (this.step)
			{
			case 1:
				this.step = 0;
				this.connectSystem();
				return;
			case 2:
				this.step = 0;
				this.connectControl();
				return;
			case 3:
				this.step = 0;
				this.connectBluetooth();
				return;
			case 4:
				this.step = 0;
				this.connectSteam();
				return;
			case 5:
				this.step = 0;
				this.ShowGuideImage();
				return;
			default:
				return;
			}
		}

		private void backButton_Click(object sender, EventArgs e)
		{
			if (this.OnBackClickListener != null)
			{
				this.OnBackClickListener();
			}
		}

		public void CloseConnectControl()
		{
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
			this.label1 = new Label();
			this.label2 = new Label();
			this.label3 = new Label();
			this.label4 = new Label();
			this.messageLabel = new Label();
			this.label5 = new Label();
			this.progressBox = new PictureBox();
			this.pictureBox1 = new PictureBox();
			this.pcBox = new PictureBox();
			this.lineBluetooth = new PictureBox();
			this.lineControl = new PictureBox();
			this.lineSystem = new PictureBox();
			this.backButton = new Button();
			this.indicatorSteam = new PictureBox();
			this.indicatorBluetooth = new PictureBox();
			this.indicatorControl = new PictureBox();
			this.indicatorSystem = new PictureBox();
			((ISupportInitialize)this.progressBox).BeginInit();
			((ISupportInitialize)this.pictureBox1).BeginInit();
			((ISupportInitialize)this.pcBox).BeginInit();
			((ISupportInitialize)this.lineBluetooth).BeginInit();
			((ISupportInitialize)this.lineControl).BeginInit();
			((ISupportInitialize)this.lineSystem).BeginInit();
			((ISupportInitialize)this.indicatorSteam).BeginInit();
			((ISupportInitialize)this.indicatorBluetooth).BeginInit();
			((ISupportInitialize)this.indicatorControl).BeginInit();
			((ISupportInitialize)this.indicatorSystem).BeginInit();
			base.SuspendLayout();
			this.label1.AutoSize = true;
			this.label1.Font = new Font("微软雅黑", 9f);
			this.label1.Location = new Point(110, 205);
			this.label1.Name = "label1";
			this.label1.Size = new Size(56, 17);
			this.label1.TabIndex = 6;
			this.label1.Text = "系统加载";
			this.label2.AutoSize = true;
			this.label2.Font = new Font("微软雅黑", 9f);
			this.label2.Location = new Point(220, 205);
			this.label2.Name = "label2";
			this.label2.Size = new Size(56, 17);
			this.label2.TabIndex = 8;
			this.label2.Text = "操控连接";
			this.label3.AutoSize = true;
			this.label3.Font = new Font("微软雅黑", 9f);
			this.label3.Location = new Point(330, 205);
			this.label3.Name = "label3";
			this.label3.Size = new Size(56, 17);
			this.label3.TabIndex = 9;
			this.label3.Text = "声音连接";
			this.label4.AutoSize = true;
			this.label4.Font = new Font("微软雅黑", 9f);
			this.label4.Location = new Point(439, 205);
			this.label4.Name = "label4";
			this.label4.Size = new Size(48, 17);
			this.label4.TabIndex = 10;
			this.label4.Text = "启动VR";
			this.messageLabel.AutoSize = true;
			this.messageLabel.Font = new Font("微软雅黑", 14f, FontStyle.Regular, GraphicsUnit.Pixel);
			this.messageLabel.Location = new Point(60, 266);
			this.messageLabel.Name = "messageLabel";
			this.messageLabel.Size = new Size(116, 20);
			this.messageLabel.TabIndex = 11;
			this.messageLabel.Text = "无线套件连接中...";
			this.label5.BackColor = Color.FromArgb(230, 230, 230);
			this.label5.Location = new Point(50, 251);
			this.label5.Name = "label5";
			this.label5.Size = new Size(500, 1);
			this.label5.TabIndex = 17;
			this.label5.Text = "label5";
			this.progressBox.Image = Resources.connnect_progress;
			this.progressBox.Location = new Point(226, 80);
			this.progressBox.Name = "progressBox";
			this.progressBox.Size = new Size(118, 22);
			this.progressBox.TabIndex = 24;
			this.progressBox.TabStop = false;
			this.pictureBox1.BackgroundImageLayout = ImageLayout.None;
			this.pictureBox1.Image = Resources.vive_icon;
			this.pictureBox1.Location = new Point(380, 57);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new Size(79, 67);
			this.pictureBox1.TabIndex = 23;
			this.pictureBox1.TabStop = false;
			this.pcBox.Image = Resources.pc_icon;
			this.pcBox.Location = new Point(137, 60);
			this.pcBox.Name = "pcBox";
			this.pcBox.Size = new Size(43, 61);
			this.pcBox.TabIndex = 22;
			this.pcBox.TabStop = false;
			this.lineBluetooth.Image = Resources.line_unload;
			this.lineBluetooth.Location = new Point(380, 174);
			this.lineBluetooth.Name = "lineBluetooth";
			this.lineBluetooth.Size = new Size(59, 2);
			this.lineBluetooth.TabIndex = 21;
			this.lineBluetooth.TabStop = false;
			this.lineControl.Image = Resources.line_unload;
			this.lineControl.Location = new Point(271, 175);
			this.lineControl.Name = "lineControl";
			this.lineControl.Size = new Size(59, 2);
			this.lineControl.TabIndex = 20;
			this.lineControl.TabStop = false;
			this.lineSystem.Image = Resources.line_unload;
			this.lineSystem.Location = new Point(161, 175);
			this.lineSystem.Name = "lineSystem";
			this.lineSystem.Size = new Size(59, 2);
			this.lineSystem.TabIndex = 19;
			this.lineSystem.TabStop = false;
			this.backButton.AutoSize = true;
			this.backButton.BackgroundImage = Resources.blue_background_1;
			this.backButton.BackgroundImageLayout = ImageLayout.Stretch;
			this.backButton.FlatStyle = FlatStyle.Flat;
			this.backButton.Font = new Font("微软雅黑", 12f, FontStyle.Regular, GraphicsUnit.Pixel);
			this.backButton.ForeColor = Color.White;
			this.backButton.Location = new Point(475, 262);
			this.backButton.Name = "backButton";
			this.backButton.Size = new Size(80, 29);
			this.backButton.TabIndex = 18;
			this.backButton.Text = "重试";
			this.backButton.UseVisualStyleBackColor = true;
			this.backButton.Click += new EventHandler(this.backButton_Click);
			this.indicatorSteam.Image = Resources.vr_unstart;
			this.indicatorSteam.Location = new Point(440, 153);
			this.indicatorSteam.Name = "indicatorSteam";
			this.indicatorSteam.Size = new Size(46, 46);
			this.indicatorSteam.TabIndex = 4;
			this.indicatorSteam.TabStop = false;
			this.indicatorBluetooth.Image = Resources.voice_disconnect;
			this.indicatorBluetooth.Location = new Point(332, 153);
			this.indicatorBluetooth.Name = "indicatorBluetooth";
			this.indicatorBluetooth.Size = new Size(46, 46);
			this.indicatorBluetooth.TabIndex = 3;
			this.indicatorBluetooth.TabStop = false;
			this.indicatorControl.Image = Resources.control_disconnect;
			this.indicatorControl.Location = new Point(223, 153);
			this.indicatorControl.Name = "indicatorControl";
			this.indicatorControl.Size = new Size(46, 46);
			this.indicatorControl.TabIndex = 2;
			this.indicatorControl.TabStop = false;
			this.indicatorSystem.Image = Resources.system_unload;
			this.indicatorSystem.Location = new Point(112, 153);
			this.indicatorSystem.Name = "indicatorSystem";
			this.indicatorSystem.Size = new Size(46, 46);
			this.indicatorSystem.TabIndex = 1;
			this.indicatorSystem.TabStop = false;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.WhiteSmoke;
			base.Controls.Add(this.progressBox);
			base.Controls.Add(this.pictureBox1);
			base.Controls.Add(this.pcBox);
			base.Controls.Add(this.lineBluetooth);
			base.Controls.Add(this.lineControl);
			base.Controls.Add(this.lineSystem);
			base.Controls.Add(this.backButton);
			base.Controls.Add(this.label5);
			base.Controls.Add(this.messageLabel);
			base.Controls.Add(this.label4);
			base.Controls.Add(this.label3);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.indicatorSteam);
			base.Controls.Add(this.indicatorBluetooth);
			base.Controls.Add(this.indicatorControl);
			base.Controls.Add(this.indicatorSystem);
			base.Name = "ConnectWindow";
			base.Size = new Size(600, 319);
			((ISupportInitialize)this.progressBox).EndInit();
			((ISupportInitialize)this.pictureBox1).EndInit();
			((ISupportInitialize)this.pcBox).EndInit();
			((ISupportInitialize)this.lineBluetooth).EndInit();
			((ISupportInitialize)this.lineControl).EndInit();
			((ISupportInitialize)this.lineSystem).EndInit();
			((ISupportInitialize)this.indicatorSteam).EndInit();
			((ISupportInitialize)this.indicatorBluetooth).EndInit();
			((ISupportInitialize)this.indicatorControl).EndInit();
			((ISupportInitialize)this.indicatorSystem).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
