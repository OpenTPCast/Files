using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TPCASTWindows.Properties;
using TPCASTWindows.Utils;

namespace TPCASTWindows
{
	public class NetworkDialog : BaseDialogForm, ConnectReloadCallback, SwitchChannelCallback
	{
		public delegate void OnBackClickDelegate();

		private ConnectModel connectModel;

		private NetworkDialogWaitControl waitControl;

		public NetworkDialog.OnBackClickDelegate OnBackClick;

		private IContainer components;

		private GroupBox dialogGroup;

		public NetworkDialog()
		{
			this.InitializeComponent();
			this.connectModel = new ConnectModel(this);
			this.connectModel.setConnectReloadCallback(this);
			this.connectModel.setSwitchChannelCallback(this);
			this.OnCloseClick = new BaseDialogForm.OnCloseClickDelegate(this.OnCloseButtonClick);
		}

		private void NetworkDialog_Load(object sender, EventArgs e)
		{
			NetworkDialogSwitchControl switchControl = new NetworkDialogSwitchControl();
			switchControl.OnSwitchClick = new NetworkDialogSwitchControl.OnSwitchClickDelegate(this.SwitchClick);
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(switchControl);
		}

		public void SwitchClick()
		{
			this.ShowWaitControl();
			if (this.connectModel != null)
			{
				this.connectModel.SwitchChannel();
			}
		}

		public void OnCheckRouterChannelFinishListener(bool isOurRouter)
		{
			this.closeButton.Visible = true;
			ControlDialogRouterControl routerControl = new ControlDialogRouterControl();
			routerControl.OnRetryClick = new ControlDialogRouterControl.RetryButtonClickDelegate(this.SwitchClick);
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(routerControl);
		}

		public void OnChannelSwitched()
		{
			if (ControlCheckWindow.isAllPass)
			{
				if (this.waitControl != null)
				{
					this.waitControl.setRouterLabelText(Resources.routerRebootFinish);
					this.waitControl.setRaspberryLabelText(Resources.raspberryConnecting);
				}
				if (this.connectModel != null)
				{
					this.connectModel.StartHostThread();
					return;
				}
			}
			else
			{
				this.ShowSwitchChannelFinishControl();
			}
		}

		public void OnRouterConnected()
		{
			if (this.waitControl != null)
			{
				this.waitControl.setRouterLabelText(Resources.routerRebootFinish);
				this.waitControl.setRaspberryLabelText(Resources.raspberryConnecting);
			}
		}

		public void OnHostConnected()
		{
			if (this.waitControl != null)
			{
				this.waitControl.setRaspberryLabelText(Resources.RaspberryRebootFinish);
				this.waitControl.setControlLabelText(Resources.ControlReconnecting);
			}
		}

		public void OnReloadCheckError(int error)
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

		public void OnSwitchCheckError(int error)
		{
			this.OnReloadCheckError(error);
		}

		private void ShowWaitControl()
		{
			this.closeButton.Visible = false;
			this.waitControl = new NetworkDialogWaitControl();
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(this.waitControl);
		}

		private void OnWaitClick()
		{
			if (ControlCheckWindow.isAllPass)
			{
				this.ShowWaitControl();
				if (this.connectModel != null)
				{
					this.connectModel.StartCheckControlReloadThread();
					return;
				}
			}
			else
			{
				this.SwitchClick();
			}
		}

		private void ShowCheckControl()
		{
			ControlInterruptCheckControl checkControl = new ControlInterruptCheckControl();
			checkControl.OnWaitClick = new ControlInterruptCheckControl.OnWaitClickDelegate(this.OnWaitClick);
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
			routerControl.OnWaitClick = new ControlInterruptRouterControl.OnWaitClickDelegate(this.OnWaitClick);
			routerControl.OnBackClick = new ControlInterruptRouterControl.OnBackClickDelegate(this.BackClick);
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(routerControl);
		}

		private void ShowRaspberryControl()
		{
			ControlInterruptRaspberryControl raspberryControl = new ControlInterruptRaspberryControl();
			raspberryControl.OnWaitClick = new ControlInterruptRaspberryControl.OnWaitClickDelegate(this.OnWaitClick);
			raspberryControl.OnBackClick = new ControlInterruptRaspberryControl.OnBackClickDelegate(this.BackClick);
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(raspberryControl);
		}

		private void ShowCableControl()
		{
			ControlInterruptCableControl cableControl = new ControlInterruptCableControl();
			cableControl.OnRetry = new ControlInterruptCableControl.OnRetryDelegate(this.OnWaitClick);
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(cableControl);
		}

		private void ShowSwitchChannelFinishControl()
		{
			NetworkDialogFinishControl finishControl = new NetworkDialogFinishControl();
			finishControl.OnOkClick = new NetworkDialogFinishControl.OnOkClickDelegate(this.OnOkClick);
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(finishControl);
		}

		private void OnOkClick()
		{
			base.Close();
			base.Dispose();
			if (ControlCheckWindow.isAllPass)
			{
				AnimationModel.ConnectAnimateionResume();
				LoopCheckModel.StartBackgroundCheckControlThread();
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

		private void OnCloseButtonClick()
		{
			if (ControlCheckWindow.isAllPass)
			{
				AnimationModel.ConnectAnimateionResume();
				LoopCheckModel.StartBackgroundCheckControlThread();
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
