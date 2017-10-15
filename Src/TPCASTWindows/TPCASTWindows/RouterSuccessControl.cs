using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TPCASTWindows.Properties;

namespace TPCASTWindows
{
	public class RouterSuccessControl : UserControl
	{
		public delegate void OnOkClickDelegate();

		public RouterSuccessControl.OnOkClickDelegate OnOkClick;

		private IContainer components;

		private CustomLabel customLabel2;

		private Label label1;

		private Button okButton;

		private PictureBox pictureBox1;

		public RouterSuccessControl()
		{
			this.InitializeComponent();
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			RouterSuccessControl.OnOkClickDelegate expr_06 = this.OnOkClick;
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
			this.components = new Container();
			ComponentResourceManager arg_5D_0 = new ComponentResourceManager(typeof(RouterSuccessControl));
			this.customLabel2 = new CustomLabel(this.components);
			this.label1 = new Label();
			this.okButton = new Button();
			this.pictureBox1 = new PictureBox();
			((ISupportInitialize)this.pictureBox1).BeginInit();
			base.SuspendLayout();
			arg_5D_0.ApplyResources(this.customLabel2, "customLabel2");
			this.customLabel2.LineDistance = 3;
			this.customLabel2.Name = "customLabel2";
			arg_5D_0.ApplyResources(this.label1, "label1");
			this.label1.BackColor = Color.FromArgb(216, 216, 216);
			this.label1.Name = "label1";
			arg_5D_0.ApplyResources(this.okButton, "okButton");
			this.okButton.BackgroundImage = Resources.blue_background_1;
			this.okButton.FlatAppearance.BorderSize = 0;
			this.okButton.ForeColor = Color.White;
			this.okButton.Name = "okButton";
			this.okButton.UseVisualStyleBackColor = false;
			this.okButton.Click += new EventHandler(this.okButton_Click);
			arg_5D_0.ApplyResources(this.pictureBox1, "pictureBox1");
			this.pictureBox1.Image = Resources.success;
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.TabStop = false;
			arg_5D_0.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.White;
			base.Controls.Add(this.customLabel2);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.okButton);
			base.Controls.Add(this.pictureBox1);
			base.Name = "RouterSuccessControl";
			((ISupportInitialize)this.pictureBox1).EndInit();
			base.ResumeLayout(false);
		}
	}
}
