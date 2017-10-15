using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TPCASTWindows
{
	public class ControlInterruptDialog : BaseDialogForm
	{
		public delegate void OnBackClickDelegate();

		public int status;

		private ControlInterruptWaitControl waitControl;

		public ControlInterruptDialog.OnBackClickDelegate OnBackClick;

		private IContainer components;

		private GroupBox dialogGroup;

		public ControlInterruptDialog()
		{
			this.InitializeComponent();
			this.closeButton.Visible = false;
			Util.OnHostConnected = new Util.OnHostConnectedDelegate(this.OnHostConnected);
			Util.OnControlConnectedError = new Util.OnControlConnectedErrorDelegate(this.OnControlConnectedError);
		}

		private void ControlInterruptDialog_Load(object sender, EventArgs e)
		{
			this.ShowControlByStatus(this.status);
		}

		private void ShowControlByStatus(int status)
		{
			if (status == -1000)
			{
				this.ShowCheckControl();
				return;
			}
			if (status == -2000)
			{
				this.ShowRebootControl();
				return;
			}
			if (status == -1001)
			{
				this.ShowRouterControl();
				return;
			}
			if (status == -1002)
			{
				this.ShowRaspberryControl();
				return;
			}
			if (status == -3000)
			{
				this.ShowCableControl();
				return;
			}
			if (status == 0)
			{
				base.Close();
				base.Dispose();
				Util.ConnectAnimateionResume();
				Util.StartBackgroundCheckControlThread();
			}
		}

		private void ShowCheckControl()
		{
			ControlInterruptCheckControl controlInterruptCheckControl = new ControlInterruptCheckControl();
			controlInterruptCheckControl.OnWaitClick = new ControlInterruptCheckControl.OnWaitClickDelegate(this.ShowWaitControl);
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(controlInterruptCheckControl);
		}

		private void ShowRebootControl()
		{
			ControlInterruptRebootControl controlInterruptRebootControl = new ControlInterruptRebootControl();
			controlInterruptRebootControl.OnOkClick = new ControlInterruptRebootControl.OnOkClickDelegate(this.BackClick);
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(controlInterruptRebootControl);
		}

		private void ShowRouterControl()
		{
			ControlInterruptRouterControl controlInterruptRouterControl = new ControlInterruptRouterControl();
			controlInterruptRouterControl.OnWaitClick = new ControlInterruptRouterControl.OnWaitClickDelegate(this.ShowWaitControl);
			controlInterruptRouterControl.OnBackClick = new ControlInterruptRouterControl.OnBackClickDelegate(this.BackClick);
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(controlInterruptRouterControl);
		}

		private void ShowRaspberryControl()
		{
			ControlInterruptRaspberryControl controlInterruptRaspberryControl = new ControlInterruptRaspberryControl();
			controlInterruptRaspberryControl.OnWaitClick = new ControlInterruptRaspberryControl.OnWaitClickDelegate(this.ShowWaitControl);
			controlInterruptRaspberryControl.OnBackClick = new ControlInterruptRaspberryControl.OnBackClickDelegate(this.BackClick);
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(controlInterruptRaspberryControl);
		}

		private void ShowCableControl()
		{
			ControlInterruptCableControl controlInterruptCableControl = new ControlInterruptCableControl();
			controlInterruptCableControl.OnRetry = new ControlInterruptCableControl.OnRetryDelegate(this.ShowWaitControl);
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(controlInterruptCableControl);
		}

		private void OnHostConnected()
		{
		}

		private void OnControlConnectedError(int error)
		{
			this.ShowControlByStatus(error);
		}

		private void ShowWaitControl()
		{
			this.waitControl = new ControlInterruptWaitControl();
			this.waitControl.OnBackClick = new ControlInterruptWaitControl.OnBackClickDelegate(this.BackClick);
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(this.waitControl);
			Util.StartCheckControlReloadThread();
		}

		private void BackClick()
		{
			Util.AbortBackgroundCheckControlThread();
			Util.AbortCheckControlReloadThread();
			base.Close();
			base.Dispose();
			if (this.OnBackClick != null)
			{
				this.OnBackClick();
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
			this.dialogGroup = new GroupBox();
			base.SuspendLayout();
			this.closeButton.FlatAppearance.BorderSize = 0;
			this.closeButton.Location = new Point(650, 6);
			this.dialogGroup.Location = new Point(0, 20);
			this.dialogGroup.Name = "dialogGroup";
			this.dialogGroup.Size = new Size(500, 244);
			this.dialogGroup.TabIndex = 3;
			this.dialogGroup.TabStop = false;
			this.dialogGroup.Text = "groupBox1";
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			this.BackColor = Color.White;
			base.ClientSize = new Size(500, 264);
			base.Controls.Add(this.dialogGroup);
			base.Name = "ControlInterruptDialog";
			base.Load += new EventHandler(this.ControlInterruptDialog_Load);
			base.Controls.SetChildIndex(this.closeButton, 0);
			base.Controls.SetChildIndex(this.dialogGroup, 0);
			base.ResumeLayout(false);
		}
	}
}
