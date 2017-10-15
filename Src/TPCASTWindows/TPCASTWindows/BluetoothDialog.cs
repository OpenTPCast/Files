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
				BluetoothDialogDriverControl driveControl = new BluetoothDialogDriverControl();
				this.dialogGroup.Controls.Clear();
				this.dialogGroup.Controls.Add(driveControl);
				return;
			}
			if (this.hasBluetooth)
			{
				BluetoothDialogControl bluetoothControl = new BluetoothDialogControl();
				BluetoothDialogControl bluetoothDialogControl = bluetoothControl;
				bluetoothDialogControl.OnOffButtonClick = (BluetoothDialogControl.OnOffButtonDelegate)Delegate.Combine(bluetoothDialogControl.OnOffButtonClick, new BluetoothDialogControl.OnOffButtonDelegate(this.OffButtonClick));
				bluetoothDialogControl = bluetoothControl;
				bluetoothDialogControl.OnSlowFlashingClick = (BluetoothDialogControl.OnSlowFlashingButtonDelegate)Delegate.Combine(bluetoothDialogControl.OnSlowFlashingClick, new BluetoothDialogControl.OnSlowFlashingButtonDelegate(this.SlowButtonClick));
				bluetoothDialogControl = bluetoothControl;
				bluetoothDialogControl.OnFastFlashingClick = (BluetoothDialogControl.OnFastFlashingButtonDelegate)Delegate.Combine(bluetoothDialogControl.OnFastFlashingClick, new BluetoothDialogControl.OnFastFlashingButtonDelegate(this.FastButtonClick));
				this.dialogGroup.Controls.Clear();
				this.dialogGroup.Controls.Add(bluetoothControl);
				if (this.foundTPCAST)
				{
					this.FastButtonClick();
					return;
				}
			}
			else
			{
				BluetoothDialogNoDongleControl noDongleControl = new BluetoothDialogNoDongleControl();
				noDongleControl.OnRetryClickListener = new BluetoothDialogNoDongleControl.OnRetryButtonClickDelegate(this.retry);
				this.dialogGroup.Controls.Clear();
				this.dialogGroup.Controls.Add(noDongleControl);
			}
		}

		public void OffButtonClick()
		{
			BluetoothDialogOffControl offControl = new BluetoothDialogOffControl();
			offControl.showSkip = this.showSkip;
			offControl.OnSkipBluetoothClick = new BluetoothDialogOffControl.OnSkipBluetoothClickDelegate(this.SkipBluetooth);
			offControl.OnRetryClickListener = new BluetoothDialogOffControl.OnRetryButtonClickDelegate(this.retry);
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(offControl);
		}

		public void SlowButtonClick()
		{
			BluetoothDialogFlashingSlowControl slowControl = new BluetoothDialogFlashingSlowControl();
			slowControl.OnRetryClickListener = new BluetoothDialogFlashingSlowControl.OnRetryButtonClickDelegate(this.retry);
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(slowControl);
		}

		public void FastButtonClick()
		{
			BluetoothDialogFlashingFastControl fastControl = new BluetoothDialogFlashingFastControl();
			fastControl.showSkip = this.showSkip;
			fastControl.OnSkipBluetoothClick = new BluetoothDialogFlashingFastControl.OnSkipBluetoothClickDelegate(this.SkipBluetooth);
			fastControl.OnRetryClickListener = new BluetoothDialogFlashingFastControl.OnRetryButtonClickDelegate(this.retry);
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(fastControl);
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
