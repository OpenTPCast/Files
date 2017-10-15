using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TPCASTWindows
{
	public class BluetoothDialog : BaseDialogForm
	{
		public delegate void OnRetryDelegate();

		public delegate void OnSkipBluetoothDelegate();

		public bool win7;

		public bool hasBluetooth = true;

		public bool foundTPCAST;

		public bool showSkip;

		public BluetoothDialog.OnRetryDelegate OnRetry;

		public BluetoothDialog.OnSkipBluetoothDelegate OnSkipBluetooth;

		private IContainer components;

		private GroupBox dialogGroup;

		public BluetoothDialog()
		{
			this.InitializeComponent();
		}

		private void BluetoothDialog_Load(object sender, EventArgs e)
		{
			if (this.win7)
			{
				BluetoothDialogDriverControl value = new BluetoothDialogDriverControl();
				this.dialogGroup.Controls.Clear();
				this.dialogGroup.Controls.Add(value);
				return;
			}
			if (this.hasBluetooth)
			{
				BluetoothDialogControl bluetoothDialogControl = new BluetoothDialogControl();
				BluetoothDialogControl bluetoothDialogControl2 = bluetoothDialogControl;
				bluetoothDialogControl2.OnOffButtonClick = (BluetoothDialogControl.OnOffButtonDelegate)Delegate.Combine(bluetoothDialogControl2.OnOffButtonClick, new BluetoothDialogControl.OnOffButtonDelegate(this.OffButtonClick));
				bluetoothDialogControl2 = bluetoothDialogControl;
				bluetoothDialogControl2.OnSlowFlashingClick = (BluetoothDialogControl.OnSlowFlashingButtonDelegate)Delegate.Combine(bluetoothDialogControl2.OnSlowFlashingClick, new BluetoothDialogControl.OnSlowFlashingButtonDelegate(this.SlowButtonClick));
				bluetoothDialogControl2 = bluetoothDialogControl;
				bluetoothDialogControl2.OnFastFlashingClick = (BluetoothDialogControl.OnFastFlashingButtonDelegate)Delegate.Combine(bluetoothDialogControl2.OnFastFlashingClick, new BluetoothDialogControl.OnFastFlashingButtonDelegate(this.FastButtonClick));
				this.dialogGroup.Controls.Clear();
				this.dialogGroup.Controls.Add(bluetoothDialogControl);
				if (this.foundTPCAST)
				{
					this.FastButtonClick();
					return;
				}
			}
			else
			{
				BluetoothDialogNoDongleControl bluetoothDialogNoDongleControl = new BluetoothDialogNoDongleControl();
				bluetoothDialogNoDongleControl.OnRetryClickListener = new BluetoothDialogNoDongleControl.OnRetryButtonClickDelegate(this.retry);
				this.dialogGroup.Controls.Clear();
				this.dialogGroup.Controls.Add(bluetoothDialogNoDongleControl);
			}
		}

		public void OffButtonClick()
		{
			BluetoothDialogOffControl bluetoothDialogOffControl = new BluetoothDialogOffControl();
			bluetoothDialogOffControl.showSkip = this.showSkip;
			bluetoothDialogOffControl.OnSkipBluetoothClick = new BluetoothDialogOffControl.OnSkipBluetoothClickDelegate(this.SkipBluetooth);
			bluetoothDialogOffControl.OnRetryClickListener = new BluetoothDialogOffControl.OnRetryButtonClickDelegate(this.retry);
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(bluetoothDialogOffControl);
		}

		public void SlowButtonClick()
		{
			BluetoothDialogFlashingSlowControl bluetoothDialogFlashingSlowControl = new BluetoothDialogFlashingSlowControl();
			bluetoothDialogFlashingSlowControl.OnRetryClickListener = new BluetoothDialogFlashingSlowControl.OnRetryButtonClickDelegate(this.retry);
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(bluetoothDialogFlashingSlowControl);
		}

		public void FastButtonClick()
		{
			BluetoothDialogFlashingFastControl bluetoothDialogFlashingFastControl = new BluetoothDialogFlashingFastControl();
			bluetoothDialogFlashingFastControl.showSkip = this.showSkip;
			bluetoothDialogFlashingFastControl.OnSkipBluetoothClick = new BluetoothDialogFlashingFastControl.OnSkipBluetoothClickDelegate(this.SkipBluetooth);
			bluetoothDialogFlashingFastControl.OnRetryClickListener = new BluetoothDialogFlashingFastControl.OnRetryButtonClickDelegate(this.retry);
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(bluetoothDialogFlashingFastControl);
		}

		public void retry()
		{
			base.Close();
			base.Dispose();
			if (this.OnRetry != null)
			{
				this.OnRetry();
			}
		}

		private void SkipBluetooth()
		{
			base.Close();
			base.Dispose();
			if (this.OnSkipBluetooth != null)
			{
				this.OnSkipBluetooth();
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
			this.dialogGroup.Location = new Point(0, 20);
			this.dialogGroup.Name = "dialogGroup";
			this.dialogGroup.Size = new Size(500, 244);
			this.dialogGroup.TabIndex = 1;
			this.dialogGroup.TabStop = false;
			this.dialogGroup.Text = "groupBox1";
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.White;
			base.ClientSize = new Size(500, 264);
			base.Controls.Add(this.dialogGroup);
			base.Name = "BluetoothDialog";
			this.Text = "BluetoothDialog";
			base.Load += new EventHandler(this.BluetoothDialog_Load);
			base.Controls.SetChildIndex(this.closeButton, 0);
			base.Controls.SetChildIndex(this.dialogGroup, 0);
			base.ResumeLayout(false);
		}
	}
}
