using NLog;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using TPCASTWindows.Properties;
using TPCASTWindows.UI;
using TPCASTWindows.Utils;

namespace TPCASTWindows
{
	public class BaseForm : Form
	{
		public delegate void OnNetworkDialogBackClickDelegate();

		public delegate void OnRecommendedUpdateClickDelegate();

		public delegate void OnUpdateDialogCloseDelegate();

		private static Logger log = LogManager.GetCurrentClassLogger();

		private bool moving;

		private Point oldMousePosition;

		public BaseForm.OnNetworkDialogBackClickDelegate OnNetworkDialogBackClick;

		public BaseForm.OnRecommendedUpdateClickDelegate OnRecommendedUpdateClick;

		public BaseForm.OnUpdateDialogCloseDelegate OnUpdateDialogClose;

		private IContainer components;

		private Panel titleBar;

		private Button closeButton;

		private Button minButton;

		private PictureBox pictureBox1;

		private Label titleLabel;

		public PictureBox backgroundImage;

		private NotifyIcon notifyIcon;

		private ContextMenuStrip contextMenuStrip;

		private ToolStripMenuItem quit;

		private Button dropMenuButton;

		private ContextMenuStrip dropMenu;

		private ToolStripMenuItem switchChannel;

		private ToolStripMenuItem about;

		public PictureBox guideImage;

		private ToolStripMenuItem commonProblems;

		private ToolStripMenuItem wifiSetting;

		private Label typeLabel;

		private Panel panel2;

		public Panel backgroundImagePanel;

		public Label appLabel;

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams expr_06 = base.CreateParams;
				expr_06.Style |= 131072;
				return expr_06;
			}
		}

		public BaseForm()
		{
			base.StartPosition = FormStartPosition.CenterScreen;
			this.InitializeComponent();
			this.typeLabel.Visible = false;
		}

		private void Titlepanel_MouseDown(object sender, MouseEventArgs e)
		{
			if (base.WindowState == FormWindowState.Maximized)
			{
				return;
			}
			this.oldMousePosition = e.Location;
			this.moving = true;
		}

		private void Titlepanel_MouseUp(object sender, MouseEventArgs e)
		{
			this.moving = false;
		}

		private void Titlepanel_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left && this.moving)
			{
				Point newPosition = new Point(e.Location.X - this.oldMousePosition.X, e.Location.Y - this.oldMousePosition.Y);
				if (base.Location.Y + newPosition.Y > SystemInformation.WorkingArea.Height - 20)
				{
					newPosition.Y = SystemInformation.WorkingArea.Height - 20 - base.Location.Y;
				}
				base.Location += new Size(newPosition);
			}
		}

		private void closeButton_Click(object sender, EventArgs e)
		{
			this.pictureBox1.Focus();
			base.ShowInTaskbar = false;
			base.Hide();
		}

		private void minButton_Click(object sender, EventArgs e)
		{
			this.pictureBox1.Focus();
			if (base.WindowState != FormWindowState.Minimized)
			{
				base.WindowState = FormWindowState.Minimized;
			}
		}

		private void quit_Click(object sender, EventArgs e)
		{
			BaseForm.log.Trace("quit");
			base.Close();
		}

		private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
		{
			BaseForm.log.Trace("mouse Click");
			if (e.Button == MouseButtons.Right)
			{
				this.contextMenuStrip.Show();
				return;
			}
			if (e.Button == MouseButtons.Left)
			{
				base.Show();
				base.ShowInTaskbar = true;
				base.WindowState = FormWindowState.Normal;
			}
		}

		public void closeContextMenuStrip()
		{
			if (this.contextMenuStrip != null)
			{
				this.contextMenuStrip.Close();
			}
		}

		private void dropMenuButton_Click(object sender, EventArgs e)
		{
			this.pictureBox1.Focus();
			Button button = sender as Button;
			this.dropMenu.Show(button, new Point(int.Parse(Resources.leftOffset), button.Height + 8), ToolStripDropDownDirection.BelowRight);
			this.dropMenu.Items[0].Enabled = !ControlCheckWindow.isChecking;
		}

		private void networkDialogBackClick()
		{
			BaseForm.OnNetworkDialogBackClickDelegate expr_06 = this.OnNetworkDialogBackClick;
			if (expr_06 == null)
			{
				return;
			}
			expr_06();
		}

		private void switchChannel_Click(object sender, EventArgs e)
		{
			if (!ControlCheckWindow.isChecking)
			{
				LoopCheckModel.AbortBackgroundCheckControlThread();
				Util.showGrayBackground();
				new NetworkDialog
				{
					OnBackClick = new NetworkDialog.OnBackClickDelegate(this.networkDialogBackClick)
				}.Show(Util.sContext);
			}
		}

		private void wifiSetting_Click(object sender, EventArgs e)
		{
			Util.showGrayBackground();
			RouterDialog expr_0A = new RouterDialog();
			expr_0A.setCloseButtonVisibility(true);
			expr_0A.Show(Util.sContext);
		}

		private void commonProblems_Click(object sender, EventArgs e)
		{
			Process.Start("iexplore", Resources.faqLink);
		}

		private void about_Click(object sender, EventArgs e)
		{
			Util.showGrayBackground();
			new AboutDialog().Show(Util.sContext);
		}

		private void recommendedUpdate()
		{
			BaseForm.OnRecommendedUpdateClickDelegate expr_06 = this.OnRecommendedUpdateClick;
			if (expr_06 != null)
			{
				expr_06();
			}
			new UpdateDialog
			{
				OnDialogClose = new UpdateDialog.OnDialogCloseDelegate(this.OnDialogClose)
			}.Show(Util.sContext);
		}

		private void OnDialogClose()
		{
			BaseForm.OnUpdateDialogCloseDelegate expr_06 = this.OnUpdateDialogClose;
			if (expr_06 == null)
			{
				return;
			}
			expr_06();
		}

		private void BaseForm_SizeChanged(object sender, EventArgs e)
		{
			BaseForm.log.Trace("size change = " + base.WindowState);
		}

		private void switchChannel_MouseEnter(object sender, EventArgs e)
		{
			BaseForm.log.Trace("switchChannel_MouseEnter");
		}

		private void guideImage_Click(object sender, EventArgs e)
		{
		}

		private void BaseForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			BaseForm.log.Trace("BaseForm_FormClosing");
			this.notifyIcon.Dispose();
			Util.UnInit();
			LoopCheckModel.UnInit();
		}

		private void BaseForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			BaseForm.log.Trace("BaseForm_FormClosed");
			Process.GetCurrentProcess().Kill();
		}

		protected override void WndProc(ref Message m)
		{
			int msg = m.Msg;
			if (msg == 274 && m.WParam.ToInt32() == 61536)
			{
				Console.WriteLine("close");
				this.OnTaskbarCloseClick();
			}
			base.WndProc(ref m);
		}

		public void OnTaskbarCloseClick()
		{
			WindowsMessage.BaseFormTaskbarCloseClick();
			base.Close();
		}

		private void BaseForm_KeyDown(object sender, KeyEventArgs e)
		{
			BaseForm.log.Trace("BaseForm_KeyDown kc = " + e.KeyCode);
			BaseForm.log.Trace("BaseForm_KeyDown m = " + e.Modifiers);
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
			ComponentResourceManager resources = new ComponentResourceManager(typeof(BaseForm));
			this.titleBar = new Panel();
			this.typeLabel = new Label();
			this.dropMenuButton = new Button();
			this.titleLabel = new Label();
			this.pictureBox1 = new PictureBox();
			this.minButton = new Button();
			this.closeButton = new Button();
			this.contextMenuStrip = new ContextMenuStrip(this.components);
			this.quit = new ToolStripMenuItem();
			this.notifyIcon = new NotifyIcon(this.components);
			this.backgroundImage = new PictureBox();
			this.dropMenu = new ContextMenuStrip(this.components);
			this.switchChannel = new ToolStripMenuItem();
			this.wifiSetting = new ToolStripMenuItem();
			this.commonProblems = new ToolStripMenuItem();
			this.about = new ToolStripMenuItem();
			this.guideImage = new PictureBox();
			this.backgroundImagePanel = new Panel();
			this.panel2 = new Panel();
			this.appLabel = new Label();
			this.titleBar.SuspendLayout();
			((ISupportInitialize)this.pictureBox1).BeginInit();
			this.contextMenuStrip.SuspendLayout();
			((ISupportInitialize)this.backgroundImage).BeginInit();
			this.dropMenu.SuspendLayout();
			((ISupportInitialize)this.guideImage).BeginInit();
			this.backgroundImagePanel.SuspendLayout();
			this.panel2.SuspendLayout();
			base.SuspendLayout();
			this.titleBar.BackColor = Color.FromArgb(76, 76, 76);
			this.titleBar.Controls.Add(this.typeLabel);
			this.titleBar.Controls.Add(this.dropMenuButton);
			this.titleBar.Controls.Add(this.titleLabel);
			this.titleBar.Controls.Add(this.pictureBox1);
			this.titleBar.Controls.Add(this.minButton);
			this.titleBar.Controls.Add(this.closeButton);
			resources.ApplyResources(this.titleBar, "titleBar");
			this.titleBar.Name = "titleBar";
			this.titleBar.MouseDown += new MouseEventHandler(this.Titlepanel_MouseDown);
			this.titleBar.MouseMove += new MouseEventHandler(this.Titlepanel_MouseMove);
			this.titleBar.MouseUp += new MouseEventHandler(this.Titlepanel_MouseUp);
			resources.ApplyResources(this.typeLabel, "typeLabel");
			this.typeLabel.BackColor = Color.Transparent;
			this.typeLabel.ForeColor = Color.White;
			this.typeLabel.Name = "typeLabel";
			this.dropMenuButton.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.dropMenuButton, "dropMenuButton");
			this.dropMenuButton.Name = "dropMenuButton";
			this.dropMenuButton.UseVisualStyleBackColor = true;
			this.dropMenuButton.Click += new EventHandler(this.dropMenuButton_Click);
			resources.ApplyResources(this.titleLabel, "titleLabel");
			this.titleLabel.BackColor = Color.Transparent;
			this.titleLabel.ForeColor = Color.White;
			this.titleLabel.Name = "titleLabel";
			this.titleLabel.MouseDown += new MouseEventHandler(this.Titlepanel_MouseDown);
			this.titleLabel.MouseMove += new MouseEventHandler(this.Titlepanel_MouseMove);
			this.titleLabel.MouseUp += new MouseEventHandler(this.Titlepanel_MouseUp);
			resources.ApplyResources(this.pictureBox1, "pictureBox1");
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.TabStop = false;
			this.pictureBox1.MouseDown += new MouseEventHandler(this.Titlepanel_MouseDown);
			this.pictureBox1.MouseMove += new MouseEventHandler(this.Titlepanel_MouseMove);
			this.pictureBox1.MouseUp += new MouseEventHandler(this.Titlepanel_MouseUp);
			this.minButton.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.minButton, "minButton");
			this.minButton.Name = "minButton";
			this.minButton.UseVisualStyleBackColor = true;
			this.minButton.Click += new EventHandler(this.minButton_Click);
			resources.ApplyResources(this.closeButton, "closeButton");
			this.closeButton.FlatAppearance.BorderSize = 0;
			this.closeButton.Name = "closeButton";
			this.closeButton.UseVisualStyleBackColor = true;
			this.closeButton.Click += new EventHandler(this.closeButton_Click);
			this.contextMenuStrip.Items.AddRange(new ToolStripItem[]
			{
				this.quit
			});
			this.contextMenuStrip.Name = "contextMenuStrip";
			resources.ApplyResources(this.contextMenuStrip, "contextMenuStrip");
			this.quit.Name = "quit";
			resources.ApplyResources(this.quit, "quit");
			this.quit.Click += new EventHandler(this.quit_Click);
			this.notifyIcon.ContextMenuStrip = this.contextMenuStrip;
			resources.ApplyResources(this.notifyIcon, "notifyIcon");
			this.notifyIcon.MouseClick += new MouseEventHandler(this.notifyIcon_MouseClick);
			resources.ApplyResources(this.backgroundImage, "backgroundImage");
			this.backgroundImage.Image = Resources.launch_background_pannel;
			this.backgroundImage.Name = "backgroundImage";
			this.backgroundImage.TabStop = false;
			this.backgroundImage.MouseDown += new MouseEventHandler(this.Titlepanel_MouseDown);
			this.backgroundImage.MouseMove += new MouseEventHandler(this.Titlepanel_MouseMove);
			this.backgroundImage.MouseUp += new MouseEventHandler(this.Titlepanel_MouseUp);
			resources.ApplyResources(this.dropMenu, "dropMenu");
			this.dropMenu.BackColor = Color.FromArgb(0, 0, 0, 0);
			this.dropMenu.Items.AddRange(new ToolStripItem[]
			{
				this.switchChannel,
				this.wifiSetting,
				this.commonProblems,
				this.about
			});
			this.dropMenu.Name = "dropMenu";
			resources.ApplyResources(this.switchChannel, "switchChannel");
			this.switchChannel.BackColor = Color.FromArgb(76, 76, 76);
			this.switchChannel.ForeColor = Color.White;
			this.switchChannel.Name = "switchChannel";
			this.switchChannel.Click += new EventHandler(this.switchChannel_Click);
			this.switchChannel.MouseEnter += new EventHandler(this.switchChannel_MouseEnter);
			this.wifiSetting.BackColor = Color.FromArgb(76, 76, 76);
			this.wifiSetting.ForeColor = Color.White;
			this.wifiSetting.Image = Resources.drop_wifi;
			this.wifiSetting.Name = "wifiSetting";
			resources.ApplyResources(this.wifiSetting, "wifiSetting");
			this.wifiSetting.Click += new EventHandler(this.wifiSetting_Click);
			resources.ApplyResources(this.commonProblems, "commonProblems");
			this.commonProblems.BackColor = Color.FromArgb(76, 76, 76);
			this.commonProblems.ForeColor = Color.White;
			this.commonProblems.Name = "commonProblems";
			this.commonProblems.Click += new EventHandler(this.commonProblems_Click);
			resources.ApplyResources(this.about, "about");
			this.about.BackColor = Color.FromArgb(76, 76, 76);
			this.about.ForeColor = Color.White;
			this.about.Name = "about";
			this.about.Click += new EventHandler(this.about_Click);
			resources.ApplyResources(this.guideImage, "guideImage");
			this.guideImage.Name = "guideImage";
			this.guideImage.TabStop = false;
			this.guideImage.Click += new EventHandler(this.guideImage_Click);
			this.backgroundImagePanel.BackColor = Color.Transparent;
			this.backgroundImagePanel.Controls.Add(this.panel2);
			this.backgroundImagePanel.Controls.Add(this.backgroundImage);
			resources.ApplyResources(this.backgroundImagePanel, "backgroundImagePanel");
			this.backgroundImagePanel.Name = "backgroundImagePanel";
			this.panel2.BackColor = Color.WhiteSmoke;
			this.panel2.Controls.Add(this.appLabel);
			resources.ApplyResources(this.panel2, "panel2");
			this.panel2.Name = "panel2";
			resources.ApplyResources(this.appLabel, "appLabel");
			this.appLabel.ForeColor = Color.FromArgb(25, 25, 25);
			this.appLabel.Name = "appLabel";
			resources.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ControlBox = false;
			base.Controls.Add(this.titleBar);
			base.Controls.Add(this.guideImage);
			base.Controls.Add(this.backgroundImagePanel);
			base.FormBorderStyle = FormBorderStyle.None;
			base.Name = "BaseForm";
			base.FormClosing += new FormClosingEventHandler(this.BaseForm_FormClosing);
			base.FormClosed += new FormClosedEventHandler(this.BaseForm_FormClosed);
			base.SizeChanged += new EventHandler(this.BaseForm_SizeChanged);
			base.KeyDown += new KeyEventHandler(this.BaseForm_KeyDown);
			this.titleBar.ResumeLayout(false);
			this.titleBar.PerformLayout();
			((ISupportInitialize)this.pictureBox1).EndInit();
			this.contextMenuStrip.ResumeLayout(false);
			((ISupportInitialize)this.backgroundImage).EndInit();
			this.dropMenu.ResumeLayout(false);
			((ISupportInitialize)this.guideImage).EndInit();
			this.backgroundImagePanel.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			base.ResumeLayout(false);
		}
	}
}
