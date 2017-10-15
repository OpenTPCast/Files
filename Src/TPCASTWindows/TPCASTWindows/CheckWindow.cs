using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using TPCASTWindows.Properties;

namespace TPCASTWindows
{
	public class CheckWindow : UserControl
	{
		public delegate void OnConnectClickDelegate();

		private Image retry = Resources.indicator_retry;

		private Image success = Resources.indicator_success;

		private Point controlOriginLocation;

		private Point bluetoothOriginLocation;

		private Point steamOriginLocation;

		public static bool isChecking;

		public static int failTimes;

		public static bool isAllPass;

		public CheckWindow.OnConnectClickDelegate OnConnect;

		private IContainer components;

		private Button checkButton;

		private PictureBox picControl;

		private LinkLabel tutorialLinkLabel;

		private Label label1;

		private Label label3;

		private PictureBox picBluetooth;

		private Label label4;

		private PictureBox picSteam;

		private Label messageLabel;

		private Label label6;

		private Button connectButton;

		private PictureBox waiting_control;

		private PictureBox waiting_bluetooth;

		private PictureBox waiting_steam;

		public CheckWindow()
		{
			this.InitializeComponent();
			Console.WriteLine("CheckWindow");
			this.controlOriginLocation = this.picControl.Location;
			this.bluetoothOriginLocation = this.picBluetooth.Location;
			this.steamOriginLocation = this.picSteam.Location;
			this.waiting_control.Visible = false;
			this.waiting_bluetooth.Visible = false;
			this.waiting_steam.Visible = false;
			Util.BeginCheckControl = new Util.BeginCheckControlDelegate(this.BeginCheckControl);
			Util.BeginCheckBluetooth = new Util.BeginCheckBluetoothDelegate(this.BeginCheckBluetooth);
			Util.NoBluetooth = new Util.NoBluetoothDelegate(this.NoBluetooth);
			Util.DeviceConnected = new Util.DeviceConnectedDelegate(this.DeviceConnected);
			Util.DeviceNotConnected = new Util.DeviceNotConnectedDelegate(this.DeviceNotConnected);
			Util.DeviceFoundNotConnected = new Util.DeviceFoundNotConnectedDelegate(this.DeviceFoundNotConnected);
			Util.NoBluetoothDriver = new Util.NoBluetoothDriverDelegate(this.NoBluetoothDriver);
			Util.OnCheckRouterFinishListener = new Util.OnCheckRouterFinishDelegate(this.OnCheckRouterFinish);
			Util.OnCheckControlFinishListener = new Util.OnCheckControlFinishDelegate(this.OnCheckControlFinish);
			Util.BeginCheckSteam = new Util.BeginCheckSteamDelegate(this.BeginCheckSteam);
			Util.OnCheckSteamFinish = new Util.OnCheckSteamFinishDelegate(this.OnCheckSteamFinish);
		}

		private void CheckWindow_Load(object sender, EventArgs e)
		{
			Console.WriteLine("CheckWindow_Load");
			this.checkButton.Focus();
		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("iexplore", "http://tpcast.cn/index.php?s=/Front/Public/forceDownload/fname/wirelessetup_CH.pdf");
		}

		private void checkButton_Click(object sender, EventArgs e)
		{
			if (!CheckWindow.isChecking)
			{
				Console.WriteLine("checkButton_Click");
				this.tutorialLinkLabel.Visible = false;
				this.checkButton.Visible = false;
				this.messageLabel.Visible = true;
				this.messageLabel.Text = "无线连接系统检测中...";
				this.picControl.Image = Resources.image_control;
				this.picBluetooth.Image = Resources.image_bluetooth;
				this.picSteam.Image = Resources.image_steam;
				this.CheckControl();
			}
		}

		public void CheckControl()
		{
			CheckWindow.isChecking = true;
			Util.CheckControl();
		}

		public void BeginCheckControl()
		{
			this.showWaitingControl();
		}

		public void OnCheckRouterFinish(bool isOurRouter)
		{
			if (!isOurRouter)
			{
				CheckWindow.isChecking = false;
				this.hideWatingControl();
				this.picControl.Image = Resources.image_control_waiting;
				Util.showGrayBackground();
				new ControlDialog
				{
					OnRetry = new ControlDialog.OnRetryDelegate(this.CheckControl),
					OnCloseClick = new BaseDialogForm.OnCloseClickDelegate(this.DialogClose),
					hasRouter = false
				}.Show(Util.sContext);
			}
		}

		public void OnCheckControlFinish(bool isLoaded)
		{
			if (isLoaded)
			{
				this.hideWatingControl();
				this.picControl.Image = Resources.image_control_ok;
				this.CheckBluetooth();
				return;
			}
			CheckWindow.isChecking = false;
			this.hideWatingControl();
			this.picControl.Image = Resources.image_control_waiting;
			Util.showGrayBackground();
			new ControlDialog
			{
				OnRetry = new ControlDialog.OnRetryDelegate(this.CheckControl),
				OnCloseClick = new BaseDialogForm.OnCloseClickDelegate(this.DialogClose)
			}.Show(Util.sContext);
		}

		private void CheckBluetooth()
		{
			CheckWindow.isChecking = true;
			Util.CheckBluetooth();
		}

		public void BeginCheckBluetooth()
		{
			this.showWaitingBluetooth();
		}

		public void NoBluetoothDriver()
		{
			CheckWindow.isChecking = false;
			this.hideWatingBluetooth();
			this.picBluetooth.Image = Resources.image_bluetooth_waiting;
			BluetoothDialog bluetoothDialog = new BluetoothDialog();
			bluetoothDialog.OnSkipBluetooth = new BluetoothDialog.OnSkipBluetoothDelegate(this.SkipBluetooth);
			bluetoothDialog.win7 = true;
			bluetoothDialog.OnRetry = new BluetoothDialog.OnRetryDelegate(this.CheckBluetooth);
			bluetoothDialog.hasBluetooth = false;
			bluetoothDialog.OnCloseClick = new BaseDialogForm.OnCloseClickDelegate(this.DialogClose);
			CheckWindow.failTimes++;
			if (CheckWindow.failTimes > 1)
			{
				bluetoothDialog.showSkip = true;
			}
			Util.showGrayBackground();
			bluetoothDialog.ShowDialog(this);
		}

		public void NoBluetooth()
		{
			CheckWindow.isChecking = false;
			this.hideWatingBluetooth();
			this.picBluetooth.Image = Resources.image_bluetooth_waiting;
			BluetoothDialog bluetoothDialog = new BluetoothDialog();
			bluetoothDialog.OnSkipBluetooth = new BluetoothDialog.OnSkipBluetoothDelegate(this.SkipBluetooth);
			bluetoothDialog.OnRetry = new BluetoothDialog.OnRetryDelegate(this.CheckBluetooth);
			bluetoothDialog.hasBluetooth = false;
			bluetoothDialog.OnCloseClick = new BaseDialogForm.OnCloseClickDelegate(this.DialogClose);
			CheckWindow.failTimes++;
			if (CheckWindow.failTimes > 1)
			{
				bluetoothDialog.showSkip = true;
			}
			Util.showGrayBackground();
			bluetoothDialog.ShowDialog(this);
		}

		public void DeviceFoundNotConnected()
		{
			CheckWindow.isChecking = false;
			this.hideWatingBluetooth();
			this.picBluetooth.Image = Resources.image_bluetooth_waiting;
			BluetoothDialog bluetoothDialog = new BluetoothDialog();
			bluetoothDialog.OnSkipBluetooth = new BluetoothDialog.OnSkipBluetoothDelegate(this.SkipBluetooth);
			bluetoothDialog.foundTPCAST = true;
			bluetoothDialog.OnRetry = new BluetoothDialog.OnRetryDelegate(this.CheckBluetooth);
			bluetoothDialog.OnCloseClick = new BaseDialogForm.OnCloseClickDelegate(this.DialogClose);
			CheckWindow.failTimes++;
			if (CheckWindow.failTimes > 1)
			{
				bluetoothDialog.showSkip = true;
			}
			Util.showGrayBackground();
			bluetoothDialog.ShowDialog(this);
		}

		public void DeviceNotConnected()
		{
			CheckWindow.isChecking = false;
			this.hideWatingBluetooth();
			this.picBluetooth.Image = Resources.image_bluetooth_waiting;
			BluetoothDialog bluetoothDialog = new BluetoothDialog();
			bluetoothDialog.OnSkipBluetooth = new BluetoothDialog.OnSkipBluetoothDelegate(this.SkipBluetooth);
			bluetoothDialog.OnRetry = new BluetoothDialog.OnRetryDelegate(this.CheckBluetooth);
			bluetoothDialog.OnCloseClick = new BaseDialogForm.OnCloseClickDelegate(this.DialogClose);
			CheckWindow.failTimes++;
			if (CheckWindow.failTimes > 1)
			{
				bluetoothDialog.showSkip = true;
			}
			Util.showGrayBackground();
			bluetoothDialog.ShowDialog(this);
		}

		public void DeviceConnected()
		{
			this.hideWatingBluetooth();
			this.picBluetooth.Image = Resources.image_bluetooth_ok;
			this.CheckSteam();
		}

		private void SkipBluetooth()
		{
			this.hideWatingBluetooth();
			this.picBluetooth.Image = Resources.image_bluetooth_waiting;
			this.CheckSteam();
		}

		private void CheckSteam()
		{
			CheckWindow.isChecking = true;
			Util.CheckSteam();
		}

		public void BeginCheckSteam()
		{
			this.showWatingSteam();
		}

		public void OnCheckSteamFinish(bool isInstalled)
		{
			if (isInstalled)
			{
				this.hideWatingSteam();
				this.picSteam.Image = Resources.image_steam_ok;
				this.AllPass();
				return;
			}
			CheckWindow.isChecking = false;
			this.hideWatingSteam();
			this.picSteam.Image = Resources.image_steam_waiting;
			SteamDialog expr_41 = new SteamDialog();
			expr_41.OnRetry = new SteamDialog.OnRetryDelegate(this.CheckSteam);
			expr_41.OnCloseClick = new BaseDialogForm.OnCloseClickDelegate(this.DialogClose);
			Util.showGrayBackground();
			expr_41.ShowDialog(this);
		}

		public void AllPass()
		{
			this.messageLabel.Text = "检测完成";
			this.connectButton.Visible = true;
			CheckWindow.isChecking = false;
			this.picControl.Click -= new EventHandler(this.picControl_Click);
			this.picBluetooth.Click -= new EventHandler(this.picBluetooth_Click);
			this.picSteam.Click -= new EventHandler(this.picSteam_Click);
			CheckWindow.isAllPass = true;
			Util.StartBackgroundCheckControlThread();
		}

		public void DialogClose()
		{
			this.messageLabel.Visible = false;
			this.tutorialLinkLabel.Visible = true;
			this.checkButton.Visible = true;
			this.connectButton.Visible = false;
			CheckWindow.isChecking = false;
			this.waiting_control.Visible = false;
			this.waiting_bluetooth.Visible = false;
			this.waiting_steam.Visible = false;
		}

		private void showWaitingControl()
		{
			this.picControl.Image = Resources.image_control;
			this.picControl.Parent = this.waiting_control;
			this.picControl.Location = new Point((this.picControl.Parent.Width - this.picControl.Width) / 2, (this.picControl.Parent.Height - this.picControl.Height) / 2);
			this.waiting_control.Visible = true;
		}

		private void hideWatingControl()
		{
			this.picControl.Parent = this;
			this.picControl.Location = this.controlOriginLocation;
			this.waiting_control.Visible = false;
		}

		private void showWaitingBluetooth()
		{
			this.picBluetooth.Image = Resources.image_bluetooth;
			this.picBluetooth.Parent = this.waiting_bluetooth;
			this.picBluetooth.Location = new Point((this.picBluetooth.Parent.Width - this.picBluetooth.Width) / 2, (this.picBluetooth.Parent.Height - this.picBluetooth.Height) / 2);
			this.waiting_bluetooth.Visible = true;
		}

		private void hideWatingBluetooth()
		{
			this.picBluetooth.Parent = this;
			this.picBluetooth.Location = this.bluetoothOriginLocation;
			this.waiting_bluetooth.Visible = false;
		}

		private void showWatingSteam()
		{
			this.picSteam.Image = Resources.image_steam;
			this.picSteam.Parent = this.waiting_steam;
			this.picSteam.Location = new Point((this.picSteam.Parent.Width - this.picSteam.Width) / 2, (this.picSteam.Parent.Height - this.picSteam.Height) / 2);
			this.waiting_steam.Visible = true;
		}

		private void hideWatingSteam()
		{
			this.picSteam.Parent = this;
			this.picSteam.Location = this.steamOriginLocation;
			this.waiting_steam.Visible = false;
		}

		private void connectButton_Click(object sender, EventArgs e)
		{
			if (this.OnConnect != null)
			{
				this.OnConnect();
			}
		}

		private void picControl_Click(object sender, EventArgs e)
		{
			this.checkButton_Click(sender, e);
		}

		private void picBluetooth_Click(object sender, EventArgs e)
		{
			this.checkButton_Click(sender, e);
		}

		private void picSteam_Click(object sender, EventArgs e)
		{
			this.checkButton_Click(sender, e);
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
			this.tutorialLinkLabel = new LinkLabel();
			this.label1 = new Label();
			this.label3 = new Label();
			this.label4 = new Label();
			this.messageLabel = new Label();
			this.label6 = new Label();
			this.connectButton = new Button();
			this.picSteam = new PictureBox();
			this.picBluetooth = new PictureBox();
			this.picControl = new PictureBox();
			this.checkButton = new Button();
			this.waiting_control = new PictureBox();
			this.waiting_steam = new PictureBox();
			this.waiting_bluetooth = new PictureBox();
			((ISupportInitialize)this.picSteam).BeginInit();
			((ISupportInitialize)this.picBluetooth).BeginInit();
			((ISupportInitialize)this.picControl).BeginInit();
			((ISupportInitialize)this.waiting_control).BeginInit();
			((ISupportInitialize)this.waiting_steam).BeginInit();
			((ISupportInitialize)this.waiting_bluetooth).BeginInit();
			base.SuspendLayout();
			this.tutorialLinkLabel.AutoSize = true;
			this.tutorialLinkLabel.Font = new Font("微软雅黑", 14f, FontStyle.Regular, GraphicsUnit.Pixel);
			this.tutorialLinkLabel.LinkArea = new LinkArea(31, 6);
			this.tutorialLinkLabel.Location = new Point(35, 224);
			this.tutorialLinkLabel.Name = "tutorialLinkLabel";
			this.tutorialLinkLabel.Size = new Size(528, 24);
			this.tutorialLinkLabel.TabIndex = 2;
			this.tutorialLinkLabel.TabStop = true;
			this.tutorialLinkLabel.Text = "启动无线连接前，请确认所有套件均已正确连接，移动电源电量充足 查看安装指南";
			this.tutorialLinkLabel.UseCompatibleTextRendering = true;
			this.tutorialLinkLabel.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
			this.label1.AutoSize = true;
			this.label1.Font = new Font("微软雅黑", 12f, FontStyle.Regular, GraphicsUnit.Pixel);
			this.label1.Location = new Point(80, 158);
			this.label1.Name = "label1";
			this.label1.Size = new Size(72, 17);
			this.label1.TabIndex = 3;
			this.label1.Text = "VR操控连接";
			this.label3.AutoSize = true;
			this.label3.Font = new Font("微软雅黑", 12f, FontStyle.Regular, GraphicsUnit.Pixel);
			this.label3.Location = new Point(274, 158);
			this.label3.Name = "label3";
			this.label3.Size = new Size(56, 17);
			this.label3.TabIndex = 7;
			this.label3.Text = "声音连接";
			this.label4.AutoSize = true;
			this.label4.Font = new Font("微软雅黑", 12f, FontStyle.Regular, GraphicsUnit.Pixel);
			this.label4.Location = new Point(453, 158);
			this.label4.Name = "label4";
			this.label4.Size = new Size(64, 17);
			this.label4.TabIndex = 9;
			this.label4.Text = "Steam VR";
			this.messageLabel.AutoSize = true;
			this.messageLabel.Font = new Font("宋体", 14f, FontStyle.Regular, GraphicsUnit.Pixel, 134);
			this.messageLabel.Location = new Point(60, 224);
			this.messageLabel.Name = "messageLabel";
			this.messageLabel.Size = new Size(0, 14);
			this.messageLabel.TabIndex = 10;
			this.label6.BackColor = Color.FromArgb(230, 230, 230);
			this.label6.ForeColor = Color.Black;
			this.label6.Location = new Point(50, 211);
			this.label6.Name = "label6";
			this.label6.Size = new Size(500, 1);
			this.label6.TabIndex = 11;
			this.label6.Text = "label6";
			this.connectButton.BackgroundImage = Resources.blue_background_0;
			this.connectButton.BackgroundImageLayout = ImageLayout.Stretch;
			this.connectButton.FlatAppearance.BorderSize = 0;
			this.connectButton.FlatStyle = FlatStyle.Flat;
			this.connectButton.Font = new Font("微软雅黑", 20f, FontStyle.Regular, GraphicsUnit.Pixel);
			this.connectButton.ForeColor = Color.White;
			this.connectButton.Location = new Point(200, 258);
			this.connectButton.Name = "connectButton";
			this.connectButton.Size = new Size(200, 40);
			this.connectButton.TabIndex = 16;
			this.connectButton.Text = "开始连接";
			this.connectButton.UseVisualStyleBackColor = false;
			this.connectButton.Visible = false;
			this.connectButton.Click += new EventHandler(this.connectButton_Click);
			this.picSteam.BackColor = Color.Transparent;
			this.picSteam.Image = Resources.image_steam;
			this.picSteam.Location = new Point(444, 68);
			this.picSteam.Name = "picSteam";
			this.picSteam.Size = new Size(80, 80);
			this.picSteam.TabIndex = 8;
			this.picSteam.TabStop = false;
			this.picSteam.Click += new EventHandler(this.picSteam_Click);
			this.picBluetooth.BackColor = Color.Transparent;
			this.picBluetooth.Image = Resources.image_bluetooth;
			this.picBluetooth.Location = new Point(260, 68);
			this.picBluetooth.Name = "picBluetooth";
			this.picBluetooth.Size = new Size(80, 80);
			this.picBluetooth.TabIndex = 6;
			this.picBluetooth.TabStop = false;
			this.picBluetooth.Click += new EventHandler(this.picBluetooth_Click);
			this.picControl.BackColor = Color.Transparent;
			this.picControl.BackgroundImageLayout = ImageLayout.Stretch;
			this.picControl.Image = Resources.image_control;
			this.picControl.Location = new Point(76, 68);
			this.picControl.Name = "picControl";
			this.picControl.Size = new Size(80, 80);
			this.picControl.TabIndex = 1;
			this.picControl.TabStop = false;
			this.picControl.Click += new EventHandler(this.picControl_Click);
			this.checkButton.BackgroundImage = Resources.blue_background_0;
			this.checkButton.BackgroundImageLayout = ImageLayout.Stretch;
			this.checkButton.FlatAppearance.BorderSize = 0;
			this.checkButton.FlatStyle = FlatStyle.Flat;
			this.checkButton.Font = new Font("微软雅黑", 20f, FontStyle.Regular, GraphicsUnit.Pixel);
			this.checkButton.ForeColor = Color.White;
			this.checkButton.Location = new Point(200, 258);
			this.checkButton.Name = "checkButton";
			this.checkButton.Size = new Size(200, 40);
			this.checkButton.TabIndex = 0;
			this.checkButton.Text = "开 始";
			this.checkButton.UseVisualStyleBackColor = false;
			this.checkButton.Click += new EventHandler(this.checkButton_Click);
			this.waiting_control.Image = Resources.waiting_circle;
			this.waiting_control.Location = new Point(72, 64);
			this.waiting_control.Name = "waiting_control";
			this.waiting_control.Size = new Size(88, 88);
			this.waiting_control.TabIndex = 17;
			this.waiting_control.TabStop = false;
			this.waiting_steam.Image = Resources.waiting_circle;
			this.waiting_steam.Location = new Point(440, 64);
			this.waiting_steam.Name = "waiting_steam";
			this.waiting_steam.Size = new Size(88, 88);
			this.waiting_steam.TabIndex = 19;
			this.waiting_steam.TabStop = false;
			this.waiting_bluetooth.Image = Resources.waiting_circle;
			this.waiting_bluetooth.Location = new Point(256, 64);
			this.waiting_bluetooth.Name = "waiting_bluetooth";
			this.waiting_bluetooth.Size = new Size(88, 88);
			this.waiting_bluetooth.TabIndex = 18;
			this.waiting_bluetooth.TabStop = false;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.WhiteSmoke;
			base.Controls.Add(this.connectButton);
			base.Controls.Add(this.label6);
			base.Controls.Add(this.messageLabel);
			base.Controls.Add(this.label4);
			base.Controls.Add(this.picSteam);
			base.Controls.Add(this.label3);
			base.Controls.Add(this.picBluetooth);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.tutorialLinkLabel);
			base.Controls.Add(this.picControl);
			base.Controls.Add(this.checkButton);
			base.Controls.Add(this.waiting_control);
			base.Controls.Add(this.waiting_steam);
			base.Controls.Add(this.waiting_bluetooth);
			base.Name = "CheckWindow";
			base.Size = new Size(600, 319);
			base.Load += new EventHandler(this.CheckWindow_Load);
			((ISupportInitialize)this.picSteam).EndInit();
			((ISupportInitialize)this.picBluetooth).EndInit();
			((ISupportInitialize)this.picControl).EndInit();
			((ISupportInitialize)this.waiting_control).EndInit();
			((ISupportInitialize)this.waiting_steam).EndInit();
			((ISupportInitialize)this.waiting_bluetooth).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
