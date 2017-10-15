using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using TPCASTWindows.Properties;
using TPCASTWindows.Resources;
using TPCASTWindows.UI;

namespace TPCASTWindows
{
	public class BaseForm : Form
	{
		public delegate void OnRecommendedUpdateClickDelegate();

		public delegate void OnUpdateDialogCloseDelegate();

		private bool moving;

		private Point oldMousePosition;

		public BaseForm.OnRecommendedUpdateClickDelegate OnRecommendedUpdateClick;

		public BaseForm.OnUpdateDialogCloseDelegate OnUpdateDialogClose;

		private IContainer components;

		private Panel titleBar;

		private Button closeButton;

		private Button minButton;

		private PictureBox pictureBox1;

		private Label label1;

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
				Point pt = new Point(e.Location.X - this.oldMousePosition.X, e.Location.Y - this.oldMousePosition.Y);
				if (base.Location.Y + pt.Y > SystemInformation.WorkingArea.Height - 20)
				{
					pt.Y = SystemInformation.WorkingArea.Height - 20 - base.Location.Y;
				}
				base.Location += new Size(pt);
			}
		}

		private void closeButton_Click(object sender, EventArgs e)
		{
			base.ShowInTaskbar = false;
			base.Hide();
		}

		private void minButton_Click(object sender, EventArgs e)
		{
			if (base.WindowState != FormWindowState.Minimized)
			{
				base.WindowState = FormWindowState.Minimized;
			}
		}

		private void quit_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
		{
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

		private void dropMenuButton_Click(object sender, EventArgs e)
		{
			Button button = sender as Button;
			this.dropMenu.Items[0].Enabled = !ControlCheckWindow.isChecking;
			this.dropMenu.Show(button, new Point(-10, button.Height + 8), ToolStripDropDownDirection.BelowRight);
		}

		private void switchChannel_Click(object sender, EventArgs e)
		{
			if (!ControlCheckWindow.isChecking)
			{
				Form arg_12_0 = new NetworkDialog();
				Util.showGrayBackground();
				arg_12_0.ShowDialog(this);
			}
		}

		private void commonProblems_Click(object sender, EventArgs e)
		{
			Process.Start("iexplore", Localization.faqLink);
		}

		private void about_Click(object sender, EventArgs e)
		{
			Form arg_0B_0 = new AboutDialog();
			Util.showGrayBackground();
			arg_0B_0.ShowDialog(this);
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
			}.ShowDialog(this);
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
			Console.WriteLine("size change = " + base.WindowState);
		}

		private void switchChannel_MouseEnter(object sender, EventArgs e)
		{
			Console.WriteLine("switchChannel_MouseEnter");
		}

		private void guideImage_Click(object sender, EventArgs e)
		{
			this.guideImage.Visible = false;
			Settings.Default.displayGuide = false;
			Settings.Default.Save();
		}

		private void BaseForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			Console.WriteLine("BaseForm_FormClosing");
			this.notifyIcon.Dispose();
			Util.UnInit();
		}

		private void BaseForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			Console.WriteLine("BaseForm_FormClosed");
			Process.GetCurrentProcess().Kill();
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(BaseForm));
			this.titleBar = new Panel();
			this.dropMenuButton = new Button();
			this.label1 = new Label();
			this.pictureBox1 = new PictureBox();
			this.minButton = new Button();
			this.closeButton = new Button();
			this.contextMenuStrip = new ContextMenuStrip(this.components);
			this.quit = new ToolStripMenuItem();
			this.notifyIcon = new NotifyIcon(this.components);
			this.backgroundImage = new PictureBox();
			this.dropMenu = new ContextMenuStrip(this.components);
			this.switchChannel = new ToolStripMenuItem();
			this.commonProblems = new ToolStripMenuItem();
			this.about = new ToolStripMenuItem();
			this.guideImage = new PictureBox();
			this.titleBar.SuspendLayout();
			((ISupportInitialize)this.pictureBox1).BeginInit();
			this.contextMenuStrip.SuspendLayout();
			((ISupportInitialize)this.backgroundImage).BeginInit();
			this.dropMenu.SuspendLayout();
			((ISupportInitialize)this.guideImage).BeginInit();
			base.SuspendLayout();
			this.titleBar.BackColor = Color.FromArgb(76, 76, 76);
			this.titleBar.Controls.Add(this.dropMenuButton);
			this.titleBar.Controls.Add(this.label1);
			this.titleBar.Controls.Add(this.pictureBox1);
			this.titleBar.Controls.Add(this.minButton);
			this.titleBar.Controls.Add(this.closeButton);
			componentResourceManager.ApplyResources(this.titleBar, "titleBar");
			this.titleBar.Name = "titleBar";
			this.titleBar.MouseDown += new MouseEventHandler(this.Titlepanel_MouseDown);
			this.titleBar.MouseMove += new MouseEventHandler(this.Titlepanel_MouseMove);
			this.titleBar.MouseUp += new MouseEventHandler(this.Titlepanel_MouseUp);
			this.dropMenuButton.FlatAppearance.BorderSize = 0;
			componentResourceManager.ApplyResources(this.dropMenuButton, "dropMenuButton");
			this.dropMenuButton.Name = "dropMenuButton";
			this.dropMenuButton.UseVisualStyleBackColor = true;
			this.dropMenuButton.Click += new EventHandler(this.dropMenuButton_Click);
			componentResourceManager.ApplyResources(this.label1, "label1");
			this.label1.BackColor = Color.Transparent;
			this.label1.ForeColor = Color.White;
			this.label1.Name = "label1";
			this.label1.MouseDown += new MouseEventHandler(this.Titlepanel_MouseDown);
			this.label1.MouseMove += new MouseEventHandler(this.Titlepanel_MouseMove);
			this.label1.MouseUp += new MouseEventHandler(this.Titlepanel_MouseUp);
			componentResourceManager.ApplyResources(this.pictureBox1, "pictureBox1");
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.TabStop = false;
			this.pictureBox1.MouseDown += new MouseEventHandler(this.Titlepanel_MouseDown);
			this.pictureBox1.MouseMove += new MouseEventHandler(this.Titlepanel_MouseMove);
			this.pictureBox1.MouseUp += new MouseEventHandler(this.Titlepanel_MouseUp);
			this.minButton.FlatAppearance.BorderSize = 0;
			componentResourceManager.ApplyResources(this.minButton, "minButton");
			this.minButton.Name = "minButton";
			this.minButton.UseVisualStyleBackColor = true;
			this.minButton.Click += new EventHandler(this.minButton_Click);
			componentResourceManager.ApplyResources(this.closeButton, "closeButton");
			this.closeButton.FlatAppearance.BorderSize = 0;
			this.closeButton.Name = "closeButton";
			this.closeButton.UseVisualStyleBackColor = true;
			this.closeButton.Click += new EventHandler(this.closeButton_Click);
			this.contextMenuStrip.Items.AddRange(new ToolStripItem[]
			{
				this.quit
			});
			this.contextMenuStrip.Name = "contextMenuStrip";
			componentResourceManager.ApplyResources(this.contextMenuStrip, "contextMenuStrip");
			this.quit.Name = "quit";
			componentResourceManager.ApplyResources(this.quit, "quit");
			this.quit.Click += new EventHandler(this.quit_Click);
			this.notifyIcon.ContextMenuStrip = this.contextMenuStrip;
			componentResourceManager.ApplyResources(this.notifyIcon, "notifyIcon");
			this.notifyIcon.MouseClick += new MouseEventHandler(this.notifyIcon_MouseClick);
			componentResourceManager.ApplyResources(this.backgroundImage, "backgroundImage");
			this.backgroundImage.Name = "backgroundImage";
			this.backgroundImage.TabStop = false;
			this.backgroundImage.MouseDown += new MouseEventHandler(this.Titlepanel_MouseDown);
			this.backgroundImage.MouseMove += new MouseEventHandler(this.Titlepanel_MouseMove);
			this.backgroundImage.MouseUp += new MouseEventHandler(this.Titlepanel_MouseUp);
			componentResourceManager.ApplyResources(this.dropMenu, "dropMenu");
			this.dropMenu.BackColor = Color.FromArgb(0, 0, 0, 0);
			this.dropMenu.Items.AddRange(new ToolStripItem[]
			{
				this.switchChannel,
				this.commonProblems,
				this.about
			});
			this.dropMenu.Name = "dropMenu";
			componentResourceManager.ApplyResources(this.switchChannel, "switchChannel");
			this.switchChannel.BackColor = Color.FromArgb(76, 76, 76);
			this.switchChannel.ForeColor = Color.White;
			this.switchChannel.Name = "switchChannel";
			this.switchChannel.Click += new EventHandler(this.switchChannel_Click);
			this.switchChannel.MouseEnter += new EventHandler(this.switchChannel_MouseEnter);
			componentResourceManager.ApplyResources(this.commonProblems, "commonProblems");
			this.commonProblems.BackColor = Color.FromArgb(76, 76, 76);
			this.commonProblems.ForeColor = Color.White;
			this.commonProblems.Name = "commonProblems";
			this.commonProblems.Click += new EventHandler(this.commonProblems_Click);
			componentResourceManager.ApplyResources(this.about, "about");
			this.about.BackColor = Color.FromArgb(76, 76, 76);
			this.about.ForeColor = Color.White;
			this.about.Name = "about";
			this.about.Click += new EventHandler(this.about_Click);
			componentResourceManager.ApplyResources(this.guideImage, "guideImage");
			this.guideImage.Name = "guideImage";
			this.guideImage.TabStop = false;
			this.guideImage.Click += new EventHandler(this.guideImage_Click);
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ControlBox = false;
			base.Controls.Add(this.titleBar);
			base.Controls.Add(this.backgroundImage);
			base.Controls.Add(this.guideImage);
			base.FormBorderStyle = FormBorderStyle.None;
			base.Name = "BaseForm";
			base.FormClosing += new FormClosingEventHandler(this.BaseForm_FormClosing);
			base.FormClosed += new FormClosedEventHandler(this.BaseForm_FormClosed);
			base.SizeChanged += new EventHandler(this.BaseForm_SizeChanged);
			this.titleBar.ResumeLayout(false);
			this.titleBar.PerformLayout();
			((ISupportInitialize)this.pictureBox1).EndInit();
			this.contextMenuStrip.ResumeLayout(false);
			((ISupportInitialize)this.backgroundImage).EndInit();
			this.dropMenu.ResumeLayout(false);
			((ISupportInitialize)this.guideImage).EndInit();
			base.ResumeLayout(false);
		}
	}
}
