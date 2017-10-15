using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TPCASTWindows
{
	public class GuideDialogControl : UserControl
	{
		private IContainer components;

		private PictureBox pictureBox1;

		private Label label1;

		private PictureBox pictureBox2;

		private Button button1;

		private TableLayoutPanel tableLayoutPanel1;

		public GuideDialogControl()
		{
			this.InitializeComponent();
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
			this.button1 = new Button();
			this.tableLayoutPanel1 = new TableLayoutPanel();
			this.pictureBox1 = new PictureBox();
			this.pictureBox2 = new PictureBox();
			this.tableLayoutPanel1.SuspendLayout();
			((ISupportInitialize)this.pictureBox1).BeginInit();
			((ISupportInitialize)this.pictureBox2).BeginInit();
			base.SuspendLayout();
			this.label1.Anchor = AnchorStyles.Left;
			this.label1.Font = new Font("微软雅黑", 9f);
			this.label1.Location = new Point(76, 9);
			this.label1.Name = "label1";
			this.label1.Size = new Size(212, 49);
			this.label1.TabIndex = 1;
			this.label1.Text = "系统连接超时，请将移动电源断开再重\r\n新接入，信号指示灯再次变为闪烁后点\r\n击“重试”。";
			this.label1.TextAlign = ContentAlignment.MiddleLeft;
			this.button1.Font = new Font("微软雅黑", 9f);
			this.button1.Location = new Point(243, 128);
			this.button1.Name = "button1";
			this.button1.Size = new Size(75, 23);
			this.button1.TabIndex = 3;
			this.button1.Text = "button1";
			this.button1.UseVisualStyleBackColor = true;
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25.1497f));
			this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 74.8503f));
			this.tableLayoutPanel1.Controls.Add(this.pictureBox1, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.label1, 1, 0);
			this.tableLayoutPanel1.Location = new Point(16, 4);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 67f));
			this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 33f));
			this.tableLayoutPanel1.Size = new Size(291, 68);
			this.tableLayoutPanel1.TabIndex = 5;
			this.pictureBox1.Anchor = AnchorStyles.Right;
			this.pictureBox1.Location = new Point(14, 10);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new Size(56, 48);
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			this.pictureBox2.Location = new Point(126, 78);
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new Size(59, 48);
			this.pictureBox2.TabIndex = 2;
			this.pictureBox2.TabStop = false;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.Controls.Add(this.tableLayoutPanel1);
			base.Controls.Add(this.button1);
			base.Controls.Add(this.pictureBox2);
			base.Name = "GuideDialogControl";
			base.Size = new Size(334, 154);
			this.tableLayoutPanel1.ResumeLayout(false);
			((ISupportInitialize)this.pictureBox1).EndInit();
			((ISupportInitialize)this.pictureBox2).EndInit();
			base.ResumeLayout(false);
		}
	}
}
