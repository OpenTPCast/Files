using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TPCASTWindows.Properties;

namespace TPCASTWindows
{
	public class NetworkDialogChannelControl : UserControl
	{
		public delegate void OnApplyClickDelegate();

		private List<Channel> channelList = new List<Channel>();

		private PictureBox checkedImage;

		private bool isFirstTime = true;

		public NetworkDialogChannelControl.OnApplyClickDelegate OnApplyClick;

		private IContainer components;

		private Button applyButton;

		private PictureBox pictureBox1;

		private Label label2;

		private CustomTableLayoutPanel channelTableLayout;

		private CustomLabel customLabel1;

		public NetworkDialogChannelControl()
		{
			this.InitializeComponent();
		}

		private void NetworkDialogChannelControl_Load(object sender, EventArgs e)
		{
			for (int i = 0; i < 10; i++)
			{
				Channel channel = new Channel();
				channel.name = "信道" + i;
				channel.isChecked = false;
				if (i == 3)
				{
					channel.isChecked = true;
				}
				this.channelList.Add(channel);
			}
			string expr_52 = JsonConvert.SerializeObject(this.channelList);
			Console.WriteLine(expr_52);
			this.channelTableLayout.HorizontalScroll.Visible = false;
			this.channelTableLayout.VerticalScroll.Visible = false;
			foreach (Channel current in JsonConvert.DeserializeObject<List<Channel>>(expr_52))
			{
				Button button = new Button();
				button.FlatStyle = FlatStyle.Flat;
				button.Font = new Font("宋体", 12f, FontStyle.Regular, GraphicsUnit.Pixel);
				button.ForeColor = Color.FromArgb(25, 25, 25);
				button.Size = new Size(50, 20);
				button.Text = current.name;
				PictureBox pictureBox = new PictureBox();
				pictureBox.Size = new Size(10, 10);
				pictureBox.Image = Resources.network_channel_check;
				pictureBox.Parent = button;
				PictureBox expr_116 = pictureBox;
				expr_116.Location = new Point(expr_116.Parent.Width - pictureBox.Width, pictureBox.Parent.Height - pictureBox.Height);
				pictureBox.Visible = current.isChecked;
				if (current.isChecked)
				{
					this.checkedImage = pictureBox;
				}
				button.Click += new EventHandler(this.button_Click);
				this.channelTableLayout.Controls.Add(button);
			}
			this.channelTableLayout.Focus();
			Console.WriteLine(this.channelTableLayout.Controls[0]);
			this.applyButton.Focus();
		}

		private void button_Click(object sender, EventArgs e)
		{
			Console.WriteLine(sender);
			Button expr_0C = sender as Button;
			Console.WriteLine(expr_0C.HasChildren);
			foreach (Control expr_2F in expr_0C.Controls)
			{
				Console.WriteLine(expr_2F);
				PictureBox pictureBox = expr_2F as PictureBox;
				pictureBox.Visible = true;
				if (this.checkedImage != null && this.checkedImage != pictureBox)
				{
					this.checkedImage.Visible = false;
				}
				this.checkedImage = pictureBox;
			}
		}

		private void NetworkDialogChannelControl_MouseHover(object sender, EventArgs e)
		{
			if (this.isFirstTime)
			{
				this.isFirstTime = false;
				Console.WriteLine("hover");
				this.channelTableLayout.Controls[0].Focus();
			}
		}

		private void applyButton_Click(object sender, EventArgs e)
		{
			if (this.OnApplyClick != null)
			{
				this.OnApplyClick();
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
			this.applyButton = new Button();
			this.pictureBox1 = new PictureBox();
			this.label2 = new Label();
			this.channelTableLayout = new CustomTableLayoutPanel();
			this.customLabel1 = new CustomLabel(this.components);
			((ISupportInitialize)this.pictureBox1).BeginInit();
			base.SuspendLayout();
			this.applyButton.BackgroundImage = Resources.blue_background_0;
			this.applyButton.BackgroundImageLayout = ImageLayout.Stretch;
			this.applyButton.FlatAppearance.BorderSize = 0;
			this.applyButton.FlatStyle = FlatStyle.Flat;
			this.applyButton.Font = new Font("微软雅黑", 12f, FontStyle.Regular, GraphicsUnit.Pixel);
			this.applyButton.ForeColor = Color.White;
			this.applyButton.Location = new Point(258, 119);
			this.applyButton.Name = "applyButton";
			this.applyButton.Size = new Size(50, 26);
			this.applyButton.TabIndex = 16;
			this.applyButton.Text = "应用";
			this.applyButton.UseVisualStyleBackColor = false;
			this.applyButton.Click += new EventHandler(this.applyButton_Click);
			this.pictureBox1.BackgroundImage = Resources.exception;
			this.pictureBox1.Location = new Point(39, 2);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new Size(30, 30);
			this.pictureBox1.TabIndex = 14;
			this.pictureBox1.TabStop = false;
			this.label2.BackColor = Color.FromArgb(216, 216, 216);
			this.label2.Location = new Point(15, 44);
			this.label2.Name = "label2";
			this.label2.Size = new Size(304, 1);
			this.label2.TabIndex = 17;
			this.label2.Text = "label2";
			this.channelTableLayout.AutoScroll = true;
			this.channelTableLayout.ColumnCount = 4;
			this.channelTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
			this.channelTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
			this.channelTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
			this.channelTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
			this.channelTableLayout.Location = new Point(44, 61);
			this.channelTableLayout.Name = "channelTableLayout";
			this.channelTableLayout.RowCount = 2;
			this.channelTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));
			this.channelTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));
			this.channelTableLayout.Size = new Size(245, 52);
			this.channelTableLayout.TabIndex = 18;
			this.customLabel1.Font = new Font("微软雅黑", 9f);
			this.customLabel1.LineDistance = 3;
			this.customLabel1.Location = new Point(91, 0);
			this.customLabel1.Name = "customLabel1";
			this.customLabel1.Size = new Size(215, 38);
			this.customLabel1.TabIndex = 19;
			this.customLabel1.Text = "当前无线连接网络环境信道拥堵，为了保证最无忧体验，建议更换其他信道。";
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.Controls.Add(this.customLabel1);
			base.Controls.Add(this.channelTableLayout);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.applyButton);
			base.Controls.Add(this.pictureBox1);
			base.Name = "NetworkDialogChannelControl";
			base.Size = new Size(334, 154);
			base.Load += new EventHandler(this.NetworkDialogChannelControl_Load);
			base.MouseHover += new EventHandler(this.NetworkDialogChannelControl_MouseHover);
			((ISupportInitialize)this.pictureBox1).EndInit();
			base.ResumeLayout(false);
		}
	}
}
