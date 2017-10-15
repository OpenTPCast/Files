using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using TPCASTWindows.Entity;
using TPCASTWindows.Properties;

namespace TPCASTWindows.UI.Main
{
	public class SplashForm : Form
	{
		public delegate void OnSplashFormClosingDelegate();

		private static Logger log = LogManager.GetCurrentClassLogger();

		private List<SplashItem> splashList;

		private Color selectBackColor = ColorTranslator.FromHtml("#099df1");

		private Color selectTextColor = ColorTranslator.FromHtml("#ffffff");

		private Color unSelectBackColor = ColorTranslator.FromHtml("#ffffff");

		private Color unSelectTextColor = ColorTranslator.FromHtml("#040446");

		private Button lastButton;

		private bool moving;

		private Point oldMousePosition;

		public SplashForm.OnSplashFormClosingDelegate OnSplashFormClosing;

		private IContainer components;

		private Panel panel1;

		private Button button5;

		private Button button4;

		private Button button3;

		private Button button2;

		private Button button1;

		private PictureBox pictureBox1;

		private Panel titlePanel;

		private Button splashClose;

		private Button splashMin;

		private Label label1;

		private LinkLabel linkLabel1;

		private Panel panel2;

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams expr_06 = base.CreateParams;
				expr_06.Style |= 131072;
				return expr_06;
			}
		}

		public SplashForm()
		{
			this.InitializeComponent();
			this.linkLabel1.Parent = this.pictureBox1;
			if ("en".Equals(Resources.language))
			{
				this.linkLabel1.Location = new Point(this.linkLabel1.Parent.Width - this.linkLabel1.Width - 20, this.linkLabel1.Parent.Height - this.linkLabel1.Height - 20);
			}
			else
			{
				this.linkLabel1.Location = new Point(this.linkLabel1.Parent.Width - this.linkLabel1.Width - 40, this.linkLabel1.Parent.Height - this.linkLabel1.Height - 20);
			}
			this.button1.PerformClick();
			this.splashList = new List<SplashItem>
			{
				new SplashItem("splash01"),
				new SplashItem("splash02"),
				new SplashItem("splash03"),
				new SplashItem("splash04"),
				new SplashItem("splash05")
			};
			SplashForm.log.Trace("splash locat = " + base.Location);
			SplashForm.log.Trace("splash size = " + base.Size);
			SplashForm.log.Trace("title height = " + this.titlePanel.Height);
			SplashForm.log.Trace("panel1 locat = " + this.panel1.Location);
			SplashForm.log.Trace("panel1 size = " + this.panel1.Size);
			SplashForm.log.Trace("panel2 locat = " + this.panel2.Location);
			SplashForm.log.Trace("panel2 size = " + this.panel2.Size);
			SplashForm.log.Trace("pic1 locat = " + this.pictureBox1.Location);
			SplashForm.log.Trace("pic1 size = " + this.pictureBox1.Size);
		}

		private void SplashForm_Load(object sender, EventArgs e)
		{
			this.button1.PerformClick();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			this.OnButtonClick(sender, 0);
		}

		private void button2_Click(object sender, EventArgs e)
		{
			this.OnButtonClick(sender, 1);
		}

		private void button3_Click(object sender, EventArgs e)
		{
			this.OnButtonClick(sender, 2);
		}

		private void button4_Click(object sender, EventArgs e)
		{
			this.OnButtonClick(sender, 3);
		}

		private void button5_Click(object sender, EventArgs e)
		{
			this.OnButtonClick(sender, 4);
		}

		private void OnButtonClick(object sender, int index)
		{
			Button button = sender as Button;
			if (button != null)
			{
				if (this.lastButton == button)
				{
					return;
				}
				if (this.lastButton != null)
				{
					this.lastButton.BackColor = this.unSelectBackColor;
					this.lastButton.ForeColor = this.unSelectTextColor;
				}
				button.BackColor = this.selectBackColor;
				button.ForeColor = this.selectTextColor;
				this.lastButton = button;
			}
			SplashItem splashItem = this.splashList[index];
			Bitmap bitmap = (Bitmap)Resources.ResourceManager.GetObject(splashItem.imageName);
			this.pictureBox1.Image = bitmap;
			this.linkLabel1.Visible = (index == 0);
		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("iexplore", Resources.tutorialLink);
		}

		private void titlePanel_MouseDown(object sender, MouseEventArgs e)
		{
			if (base.WindowState == FormWindowState.Maximized)
			{
				return;
			}
			this.oldMousePosition = e.Location;
			this.moving = true;
		}

		private void titlePanel_MouseMove(object sender, MouseEventArgs e)
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

		private void titlePanel_MouseUp(object sender, MouseEventArgs e)
		{
			this.moving = false;
		}

		private void splashClose_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void splashMin_Click(object sender, EventArgs e)
		{
			if (base.WindowState != FormWindowState.Minimized)
			{
				base.WindowState = FormWindowState.Minimized;
			}
		}

		private void SplashForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			SplashForm.OnSplashFormClosingDelegate expr_06 = this.OnSplashFormClosing;
			if (expr_06 == null)
			{
				return;
			}
			expr_06();
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
			ComponentResourceManager arg_15A_0 = new ComponentResourceManager(typeof(SplashForm));
			this.panel1 = new Panel();
			this.button5 = new Button();
			this.button4 = new Button();
			this.button3 = new Button();
			this.button2 = new Button();
			this.button1 = new Button();
			this.pictureBox1 = new PictureBox();
			this.titlePanel = new Panel();
			this.label1 = new Label();
			this.splashMin = new Button();
			this.splashClose = new Button();
			this.linkLabel1 = new LinkLabel();
			this.panel2 = new Panel();
			this.panel1.SuspendLayout();
			((ISupportInitialize)this.pictureBox1).BeginInit();
			this.titlePanel.SuspendLayout();
			this.panel2.SuspendLayout();
			base.SuspendLayout();
			this.panel1.BackColor = Color.FromArgb(9, 157, 241);
			this.panel1.Controls.Add(this.button5);
			this.panel1.Controls.Add(this.button4);
			this.panel1.Controls.Add(this.button3);
			this.panel1.Controls.Add(this.button2);
			this.panel1.Controls.Add(this.button1);
			arg_15A_0.ApplyResources(this.panel1, "panel1");
			this.panel1.Name = "panel1";
			this.button5.BackColor = Color.White;
			this.button5.FlatAppearance.BorderSize = 0;
			arg_15A_0.ApplyResources(this.button5, "button5");
			this.button5.Name = "button5";
			this.button5.UseVisualStyleBackColor = false;
			this.button5.Click += new EventHandler(this.button5_Click);
			this.button4.BackColor = Color.White;
			this.button4.FlatAppearance.BorderSize = 0;
			arg_15A_0.ApplyResources(this.button4, "button4");
			this.button4.Name = "button4";
			this.button4.UseVisualStyleBackColor = false;
			this.button4.Click += new EventHandler(this.button4_Click);
			this.button3.BackColor = Color.White;
			this.button3.FlatAppearance.BorderSize = 0;
			arg_15A_0.ApplyResources(this.button3, "button3");
			this.button3.Name = "button3";
			this.button3.UseVisualStyleBackColor = false;
			this.button3.Click += new EventHandler(this.button3_Click);
			this.button2.BackColor = Color.White;
			this.button2.FlatAppearance.BorderSize = 0;
			arg_15A_0.ApplyResources(this.button2, "button2");
			this.button2.Name = "button2";
			this.button2.UseVisualStyleBackColor = false;
			this.button2.Click += new EventHandler(this.button2_Click);
			this.button1.BackColor = Color.White;
			this.button1.FlatAppearance.BorderSize = 0;
			arg_15A_0.ApplyResources(this.button1, "button1");
			this.button1.Name = "button1";
			this.button1.UseVisualStyleBackColor = false;
			this.button1.Click += new EventHandler(this.button1_Click);
			this.pictureBox1.BackColor = Color.Transparent;
			arg_15A_0.ApplyResources(this.pictureBox1, "pictureBox1");
			this.pictureBox1.Image = Resources.back;
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.TabStop = false;
			this.titlePanel.BackColor = Color.FromArgb(10, 158, 241);
			this.titlePanel.Controls.Add(this.label1);
			this.titlePanel.Controls.Add(this.splashMin);
			this.titlePanel.Controls.Add(this.splashClose);
			arg_15A_0.ApplyResources(this.titlePanel, "titlePanel");
			this.titlePanel.Name = "titlePanel";
			this.titlePanel.MouseDown += new MouseEventHandler(this.titlePanel_MouseDown);
			this.titlePanel.MouseMove += new MouseEventHandler(this.titlePanel_MouseMove);
			this.titlePanel.MouseUp += new MouseEventHandler(this.titlePanel_MouseUp);
			arg_15A_0.ApplyResources(this.label1, "label1");
			this.label1.ForeColor = Color.White;
			this.label1.Name = "label1";
			this.label1.MouseDown += new MouseEventHandler(this.titlePanel_MouseDown);
			this.label1.MouseMove += new MouseEventHandler(this.titlePanel_MouseMove);
			this.label1.MouseUp += new MouseEventHandler(this.titlePanel_MouseUp);
			this.splashMin.FlatAppearance.BorderSize = 0;
			arg_15A_0.ApplyResources(this.splashMin, "splashMin");
			this.splashMin.Image = Resources.splash_min;
			this.splashMin.Name = "splashMin";
			this.splashMin.UseVisualStyleBackColor = true;
			this.splashMin.Click += new EventHandler(this.splashMin_Click);
			this.splashClose.FlatAppearance.BorderSize = 0;
			arg_15A_0.ApplyResources(this.splashClose, "splashClose");
			this.splashClose.Image = Resources.splash_close;
			this.splashClose.Name = "splashClose";
			this.splashClose.UseVisualStyleBackColor = true;
			this.splashClose.Click += new EventHandler(this.splashClose_Click);
			arg_15A_0.ApplyResources(this.linkLabel1, "linkLabel1");
			this.linkLabel1.BackColor = Color.Transparent;
			this.linkLabel1.ForeColor = Color.White;
			this.linkLabel1.LinkColor = Color.White;
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.TabStop = true;
			this.linkLabel1.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
			this.panel2.BackColor = Color.Transparent;
			this.panel2.Controls.Add(this.linkLabel1);
			this.panel2.Controls.Add(this.pictureBox1);
			arg_15A_0.ApplyResources(this.panel2, "panel2");
			this.panel2.Name = "panel2";
			arg_15A_0.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = SystemColors.Control;
			base.Controls.Add(this.panel2);
			base.Controls.Add(this.titlePanel);
			base.Controls.Add(this.panel1);
			base.FormBorderStyle = FormBorderStyle.None;
			base.Name = "SplashForm";
			base.FormClosing += new FormClosingEventHandler(this.SplashForm_FormClosing);
			base.Load += new EventHandler(this.SplashForm_Load);
			this.panel1.ResumeLayout(false);
			((ISupportInitialize)this.pictureBox1).EndInit();
			this.titlePanel.ResumeLayout(false);
			this.titlePanel.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			base.ResumeLayout(false);
		}
	}
}
