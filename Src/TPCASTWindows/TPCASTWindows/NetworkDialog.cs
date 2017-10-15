using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TPCASTWindows.Resources;

namespace TPCASTWindows
{
	public class NetworkDialog : BaseDialogForm
	{
		public delegate void OnBackClickDelegate();

		private NetworkDialogWaitControl waitControl;

		public NetworkDialog.OnBackClickDelegate OnBackClick;

		private IContainer components;

		private GroupBox dialogGroup;

		public NetworkDialog()
		{
			this.InitializeComponent();
			Util.OnCheckRouterChannelFinishListener = new Util.OnCheckRouterFinishDelegate(this.CheckRouterFinish);
			Util.OnChannelSwitched = new Util.OnChannelSwitchedDelegate(this.OnChannelSwitched);
			Util.OnRouterConnected = new Util.OnRouterConnectedDelegate(this.OnRouterConnected);
			Util.OnHostConnected = new Util.OnHostConnectedDelegate(this.OnHostConnected);
			Util.OnControlConnectedError = new Util.OnControlConnectedErrorDelegate(this.OnControlConnectedError);
		}

		private void NetworkDialog_Load(object sender, EventArgs e)
		{
			NetworkDialogSwitchControl networkDialogSwitchControl = new NetworkDialogSwitchControl();
			networkDialogSwitchControl.OnSwitchClick = new NetworkDialogSwitchControl.OnSwitchClickDelegate(this.SwitchClick);
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(networkDialogSwitchControl);
		}

		public void SwitchClick()
		{
			this.closeButton.Visible = false;
			Util.AbortBackgroundCheckControlThread();
			this.ShowWaitControl();
			Util.SwitchChannel();
		}

		private void CheckRouterFinish(bool isOurRouter)
		{
			ControlDialogRouterControl controlDialogRouterControl = new ControlDialogRouterControl();
			controlDialogRouterControl.OnRetryClick = new ControlDialogRouterControl.RetryButtonClickDelegate(this.SwitchClick);
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(controlDialogRouterControl);
		}

		private void OnChannelSwitched()
		{
			if (ControlCheckWindow.isAllPass)
			{
				if (this.waitControl != null)
				{
					this.waitControl.setRouterLabelText(Localization.routerRebootFinish);
					this.waitControl.setRaspberryLabelText(Localization.raspberryConnecting);
				}
				Util.StartHostThread();
				return;
			}
			this.ShowSwitchChannelFinishControl();
		}

		private void OnHostConnected()
		{
			if (this.waitControl != null)
			{
				this.waitControl.setRaspberryLabelText(Localization.RaspberryRebootFinish);
				this.waitControl.setControlLabelText(Localization.ControlReconnecting);
			}
		}

		private void OnRouterConnected()
		{
			if (this.waitControl != null)
			{
				this.waitControl.setRouterLabelText(Localization.routerRebootFinish);
				this.waitControl.setRaspberryLabelText(Localization.raspberryConnecting);
			}
		}

		private void OnControlConnectedError(int error)
		{
			this.ShowControlByStatus(error);
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
				this.ShowSwitchChannelFinishControl();
			}
		}

		private void ShowWaitControl()
		{
			this.waitControl = new NetworkDialogWaitControl();
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(this.waitControl);
		}

		private void OnWaitClick()
		{
			this.ShowWaitControl();
			Util.StartCheckControlReloadThread();
		}

		private void ShowCheckControl()
		{
			ControlInterruptCheckControl controlInterruptCheckControl = new ControlInterruptCheckControl();
			controlInterruptCheckControl.OnWaitClick = new ControlInterruptCheckControl.OnWaitClickDelegate(this.OnWaitClick);
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
			controlInterruptRouterControl.OnWaitClick = new ControlInterruptRouterControl.OnWaitClickDelegate(this.OnWaitClick);
			controlInterruptRouterControl.OnBackClick = new ControlInterruptRouterControl.OnBackClickDelegate(this.BackClick);
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(controlInterruptRouterControl);
		}

		private void ShowRaspberryControl()
		{
			ControlInterruptRaspberryControl controlInterruptRaspberryControl = new ControlInterruptRaspberryControl();
			controlInterruptRaspberryControl.OnWaitClick = new ControlInterruptRaspberryControl.OnWaitClickDelegate(this.OnWaitClick);
			controlInterruptRaspberryControl.OnBackClick = new ControlInterruptRaspberryControl.OnBackClickDelegate(this.BackClick);
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(controlInterruptRaspberryControl);
		}

		private void ShowCableControl()
		{
			ControlInterruptCableControl controlInterruptCableControl = new ControlInterruptCableControl();
			controlInterruptCableControl.OnRetry = new ControlInterruptCableControl.OnRetryDelegate(this.OnWaitClick);
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(controlInterruptCableControl);
		}

		private void ShowSwitchChannelFinishControl()
		{
			NetworkDialogFinishControl networkDialogFinishControl = new NetworkDialogFinishControl();
			networkDialogFinishControl.OnOkClick = new NetworkDialogFinishControl.OnOkClickDelegate(this.OnOkClick);
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(networkDialogFinishControl);
		}

		private void OnOkClick()
		{
			base.Close();
			base.Dispose();
			if (ControlCheckWindow.isAllPass)
			{
				Util.ConnectAnimateionResume();
				Util.StartBackgroundCheckControlThread();
			}
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

		private void NetworkDialog_FormClosed(object sender, FormClosedEventArgs e)
		{
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
			ComponentResourceManager arg_20_0 = new ComponentResourceManager(typeof(NetworkDialog));
			this.dialogGroup = new GroupBox();
			base.SuspendLayout();
			arg_20_0.ApplyResources(this.closeButton, "closeButton");
			this.closeButton.FlatAppearance.BorderSize = 0;
			arg_20_0.ApplyResources(this.dialogGroup, "dialogGroup");
			this.dialogGroup.Name = "dialogGroup";
			this.dialogGroup.TabStop = false;
			arg_20_0.ApplyResources(this, "$this");
			this.BackColor = Color.White;
			base.Controls.Add(this.dialogGroup);
			base.Name = "NetworkDialog";
			base.FormClosed += new FormClosedEventHandler(this.NetworkDialog_FormClosed);
			base.Load += new EventHandler(this.NetworkDialog_Load);
			base.Controls.SetChildIndex(this.closeButton, 0);
			base.Controls.SetChildIndex(this.dialogGroup, 0);
			base.ResumeLayout(false);
		}
	}
}
