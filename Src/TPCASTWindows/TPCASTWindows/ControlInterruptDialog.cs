using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TPCASTWindows.Utils;

namespace TPCASTWindows
{
	public class ControlInterruptDialog : BaseDialogForm, ConnectReloadCallback
	{
		public delegate void OnBackClickDelegate();

		private ConnectModel connectModel;

		public int status;

		private ControlInterruptWaitControl waitControl;

		public ControlInterruptDialog.OnBackClickDelegate OnBackClick;

		private IContainer components;

		private GroupBox dialogGroup;

		public ControlInterruptDialog()
		{
			this.InitializeComponent();
			this.closeButton.Visible = false;
			this.connectModel = new ConnectModel(this);
			this.connectModel.setConnectReloadCallback(this);
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
				AnimationModel.ConnectAnimateionResume();
				LoopCheckModel.StartBackgroundCheckControlThread();
			}
		}

		private void ShowCheckControl()
		{
			ControlInterruptCheckControl checkControl = new ControlInterruptCheckControl();
			checkControl.OnWaitClick = new ControlInterruptCheckControl.OnWaitClickDelegate(this.ShowWaitControl);
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(checkControl);
		}

		private void ShowRebootControl()
		{
			ControlInterruptRebootControl rebootControl = new ControlInterruptRebootControl();
			rebootControl.OnOkClick = new ControlInterruptRebootControl.OnOkClickDelegate(this.BackClick);
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(rebootControl);
		}

		private void ShowRouterControl()
		{
			ControlInterruptRouterControl routerControl = new ControlInterruptRouterControl();
			routerControl.OnWaitClick = new ControlInterruptRouterControl.OnWaitClickDelegate(this.ShowWaitControl);
			routerControl.OnBackClick = new ControlInterruptRouterControl.OnBackClickDelegate(this.BackClick);
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(routerControl);
		}

		private void ShowRaspberryControl()
		{
			ControlInterruptRaspberryControl raspberryControl = new ControlInterruptRaspberryControl();
			raspberryControl.OnWaitClick = new ControlInterruptRaspberryControl.OnWaitClickDelegate(this.ShowWaitControl);
			raspberryControl.OnBackClick = new ControlInterruptRaspberryControl.OnBackClickDelegate(this.BackClick);
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(raspberryControl);
		}

		private void ShowCableControl()
		{
			ControlInterruptCableControl cableControl = new ControlInterruptCableControl();
			cableControl.OnRetry = new ControlInterruptCableControl.OnRetryDelegate(this.ShowWaitControl);
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(cableControl);
		}

		public void OnRouterConnected()
		{
		}

		public void OnHostConnected()
		{
		}

		public void OnReloadCheckError(int error)
		{
			this.ShowControlByStatus(error);
		}

		private void ShowWaitControl()
		{
			this.waitControl = new ControlInterruptWaitControl();
			this.waitControl.OnBackClick = new ControlInterruptWaitControl.OnBackClickDelegate(this.BackClick);
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(this.waitControl);
			if (this.connectModel != null)
			{
				this.connectModel.StartCheckControlReloadThread();
			}
		}

		private void BackClick()
		{
			LoopCheckModel.AbortBackgroundCheckControlThread();
			if (this.connectModel != null)
			{
				this.connectModel.AbortCheckControlReloadThread();
			}
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
