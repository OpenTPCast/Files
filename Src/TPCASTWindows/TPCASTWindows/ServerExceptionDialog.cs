using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TPCASTWindows.Properties;

namespace TPCASTWindows
{
	public class ServerExceptionDialog : BaseDialogForm
	{
		public delegate void OnOkButtonClickDelegate();

		public ServerExceptionDialog.OnOkButtonClickDelegate OnOkButtonClick;

		private IContainer components;

		private Label label1;

		private CustomLabel customLabel1;

		private PictureBox pictureBox1;

		private Button okButton;

		public ServerExceptionDialog()
		{
			this.InitializeComponent();
			this.closeButton.Visible = false;
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			ServerExceptionDialog.OnOkButtonClickDelegate expr_06 = this.OnOkButtonClick;
			if (expr_06 != null)
			{
				expr_06();
			}
			base.Close();
			base.Dispose();
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
			ComponentResourceManager arg_8D_0 = new ComponentResourceManager(typeof(ServerExceptionDialog));
			this.label1 = new Label();
			this.customLabel1 = new CustomLabel(this.components);
			this.pictureBox1 = new PictureBox();
			this.okButton = new Button();
			((ISupportInitialize)this.pictureBox1).BeginInit();
			base.SuspendLayout();
			this.closeButton.FlatAppearance.BorderSize = 0;
			this.label1.BackColor = Color.FromArgb(216, 216, 216);
			arg_8D_0.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			arg_8D_0.ApplyResources(this.customLabel1, "customLabel1");
			this.customLabel1.LineDistance = 1;
			this.customLabel1.Name = "customLabel1";
			this.pictureBox1.Image = Resources.exception;
			arg_8D_0.ApplyResources(this.pictureBox1, "pictureBox1");
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.TabStop = false;
			this.okButton.BackgroundImage = Resources.blue_background_1;
			arg_8D_0.ApplyResources(this.okButton, "okButton");
			this.okButton.FlatAppearance.BorderSize = 0;
			this.okButton.ForeColor = Color.White;
			this.okButton.Name = "okButton";
			this.okButton.UseVisualStyleBackColor = false;
			this.okButton.Click += new EventHandler(this.okButton_Click);
			arg_8D_0.ApplyResources(this, "$this");
			base.Controls.Add(this.okButton);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.customLabel1);
			base.Controls.Add(this.pictureBox1);
			base.Name = "ServerExceptionDialog";
			base.Controls.SetChildIndex(this.closeButton, 0);
			base.Controls.SetChildIndex(this.pictureBox1, 0);
			base.Controls.SetChildIndex(this.customLabel1, 0);
			base.Controls.SetChildIndex(this.label1, 0);
			base.Controls.SetChildIndex(this.okButton, 0);
			((ISupportInitialize)this.pictureBox1).EndInit();
			base.ResumeLayout(false);
		}
	}
}
