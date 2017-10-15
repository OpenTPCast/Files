using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using TPCASTWindows.Properties;

namespace TPCASTWindows
{
	public class RouterCheckControl : UserControl
	{
		private delegate void OnWifiInfoAvailableDelegate(string SSID, string password);

		public delegate void OnOkButtonClickDelegate();

		public delegate void OnNoButtonClickDelegate();

		public delegate void OnNoSSIDPasswordClickDelegate();

		private RouterCheckControl.OnWifiInfoAvailableDelegate OnWifiInfoAvailable;

		public RouterCheckControl.OnOkButtonClickDelegate OnOkButtonClick;

		public RouterCheckControl.OnNoButtonClickDelegate OnNoButtonClick;

		public RouterCheckControl.OnNoSSIDPasswordClickDelegate OnNoSSIDPasswordClick;

		private IContainer components;

		private Label label3;

		private Label label2;

		private CustomLabel customLabel2;

		private Label label1;

		private Button okButton;

		private PictureBox pictureBox1;

		private Button noButton;

		private Label SSIDLabel;

		private Label passwordLabel;

		public RouterCheckControl()
		{
			this.InitializeComponent();
			this.SSIDLabel.Text = "----";
			this.passwordLabel.Text = "----";
			this.OnWifiInfoAvailable = new RouterCheckControl.OnWifiInfoAvailableDelegate(this.OnWifiInfoAvailableListener);
			this.getWifiSSIDPassword();
		}

		private void getWifiSSIDPassword()
		{
			new Thread(new ThreadStart(this.getWifiInfoThreadStart)).Start();
		}

		private void getWifiInfoThreadStart()
		{
			string SSID = ChannelUtil.getWifiSSID();
			string password = ChannelUtil.getWifiPassword();
			if (!string.IsNullOrEmpty(SSID) && !string.IsNullOrEmpty(password))
			{
				this.wifiInfoAvailable(SSID, password);
			}
		}

		private void wifiInfoAvailable(string SSID, string password)
		{
			if (this.OnWifiInfoAvailable != null)
			{
				if (base.InvokeRequired)
				{
					base.Invoke(this.OnWifiInfoAvailable, new object[]
					{
						SSID,
						password
					});
					return;
				}
				this.OnWifiInfoAvailable(SSID, password);
			}
		}

		private void OnWifiInfoAvailableListener(string SSID, string password)
		{
			this.SSIDLabel.Text = SSID;
			this.passwordLabel.Text = password;
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			RouterCheckControl.OnOkButtonClickDelegate expr_06 = this.OnOkButtonClick;
			if (expr_06 == null)
			{
				return;
			}
			expr_06();
		}

		private void noButton_Click(object sender, EventArgs e)
		{
			if ("----".Equals(this.SSIDLabel.Text) && "----".Equals(this.passwordLabel.Text))
			{
				RouterCheckControl.OnNoSSIDPasswordClickDelegate expr_34 = this.OnNoSSIDPasswordClick;
				if (expr_34 == null)
				{
					return;
				}
				expr_34();
				return;
			}
			else
			{
				RouterCheckControl.OnNoButtonClickDelegate expr_45 = this.OnNoButtonClick;
				if (expr_45 == null)
				{
					return;
				}
				expr_45();
				return;
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
			ComponentResourceManager arg_94_0 = new ComponentResourceManager(typeof(RouterCheckControl));
			this.label3 = new Label();
			this.label2 = new Label();
			this.customLabel2 = new CustomLabel(this.components);
			this.label1 = new Label();
			this.okButton = new Button();
			this.pictureBox1 = new PictureBox();
			this.noButton = new Button();
			this.SSIDLabel = new Label();
			this.passwordLabel = new Label();
			((ISupportInitialize)this.pictureBox1).BeginInit();
			base.SuspendLayout();
			arg_94_0.ApplyResources(this.label3, "label3");
			this.label3.Name = "label3";
			arg_94_0.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			arg_94_0.ApplyResources(this.customLabel2, "customLabel2");
			this.customLabel2.LineDistance = 3;
			this.customLabel2.Name = "customLabel2";
			arg_94_0.ApplyResources(this.label1, "label1");
			this.label1.BackColor = Color.FromArgb(216, 216, 216);
			this.label1.Name = "label1";
			arg_94_0.ApplyResources(this.okButton, "okButton");
			this.okButton.BackgroundImage = Resources.blue_background_1;
			this.okButton.FlatAppearance.BorderSize = 0;
			this.okButton.ForeColor = Color.White;
			this.okButton.Name = "okButton";
			this.okButton.UseVisualStyleBackColor = false;
			this.okButton.Click += new EventHandler(this.okButton_Click);
			arg_94_0.ApplyResources(this.pictureBox1, "pictureBox1");
			this.pictureBox1.Image = Resources.router_ssid;
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.TabStop = false;
			arg_94_0.ApplyResources(this.noButton, "noButton");
			this.noButton.BackgroundImage = Resources.blue_background_1;
			this.noButton.FlatAppearance.BorderSize = 0;
			this.noButton.ForeColor = Color.White;
			this.noButton.Name = "noButton";
			this.noButton.UseVisualStyleBackColor = false;
			this.noButton.Click += new EventHandler(this.noButton_Click);
			arg_94_0.ApplyResources(this.SSIDLabel, "SSIDLabel");
			this.SSIDLabel.Name = "SSIDLabel";
			arg_94_0.ApplyResources(this.passwordLabel, "passwordLabel");
			this.passwordLabel.Name = "passwordLabel";
			arg_94_0.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.White;
			base.Controls.Add(this.passwordLabel);
			base.Controls.Add(this.SSIDLabel);
			base.Controls.Add(this.noButton);
			base.Controls.Add(this.label3);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.customLabel2);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.okButton);
			base.Controls.Add(this.pictureBox1);
			base.Name = "RouterCheckControl";
			((ISupportInitialize)this.pictureBox1).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
