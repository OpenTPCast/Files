using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TPCASTWindows.Properties;

namespace TPCASTWindows
{
	public class DriverDialog : BaseDialogForm
	{
		private IContainer components;

		private Label label3;

		private Button uninstallButton;

		private CustomLabel customLabel2;

		private CustomLabel customLabel1;

		public DriverDialog()
		{
			this.InitializeComponent();
			this.closeButton.Visible = false;
		}

		private void uninstallButton_Click(object sender, EventArgs e)
		{
			RegistryUtil.UninstallTPCAST();
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
			ComponentResourceManager arg_58_0 = new ComponentResourceManager(typeof(DriverDialog));
			this.label3 = new Label();
			this.uninstallButton = new Button();
			this.customLabel2 = new CustomLabel(this.components);
			this.customLabel1 = new CustomLabel(this.components);
			base.SuspendLayout();
			arg_58_0.ApplyResources(this.closeButton, "closeButton");
			this.closeButton.FlatAppearance.BorderSize = 0;
			arg_58_0.ApplyResources(this.label3, "label3");
			this.label3.BackColor = Color.FromArgb(216, 216, 216);
			this.label3.Name = "label3";
			arg_58_0.ApplyResources(this.uninstallButton, "uninstallButton");
			this.uninstallButton.BackgroundImage = Resources.blue_background_1;
			this.uninstallButton.FlatAppearance.BorderSize = 0;
			this.uninstallButton.ForeColor = Color.White;
			this.uninstallButton.Name = "uninstallButton";
			this.uninstallButton.UseVisualStyleBackColor = false;
			this.uninstallButton.Click += new EventHandler(this.uninstallButton_Click);
			arg_58_0.ApplyResources(this.customLabel2, "customLabel2");
			this.customLabel2.LineDistance = 3;
			this.customLabel2.Name = "customLabel2";
			arg_58_0.ApplyResources(this.customLabel1, "customLabel1");
			this.customLabel1.LineDistance = 3;
			this.customLabel1.Name = "customLabel1";
			arg_58_0.ApplyResources(this, "$this");
			base.Controls.Add(this.customLabel1);
			base.Controls.Add(this.customLabel2);
			base.Controls.Add(this.uninstallButton);
			base.Controls.Add(this.label3);
			base.Name = "DriverDialog";
			base.Controls.SetChildIndex(this.closeButton, 0);
			base.Controls.SetChildIndex(this.label3, 0);
			base.Controls.SetChildIndex(this.uninstallButton, 0);
			base.Controls.SetChildIndex(this.customLabel2, 0);
			base.Controls.SetChildIndex(this.customLabel1, 0);
			base.ResumeLayout(false);
		}
	}
}
