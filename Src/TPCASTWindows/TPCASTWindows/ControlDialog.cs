using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TPCASTWindows
{
	public class ControlDialog : BaseDialogForm
	{
		public delegate void OnRetryDelegate();

		public bool hasRouter = true;

		public bool cableProblem;

		public ControlDialog.OnRetryDelegate OnRetry;

		private IContainer components;

		private GroupBox dialogGroup;

		public ControlDialog()
		{
			this.InitializeComponent();
		}

		private void ControlDialog_Load(object sender, EventArgs e)
		{
			if (!this.hasRouter)
			{
				ControlDialogRouterControl controlDialogRouterControl = new ControlDialogRouterControl();
				controlDialogRouterControl.OnRetryClick = new ControlDialogRouterControl.RetryButtonClickDelegate(this.retry);
				this.dialogGroup.Controls.Clear();
				this.dialogGroup.Controls.Add(controlDialogRouterControl);
				return;
			}
			if (this.cableProblem)
			{
				this.ShowCableControl();
				return;
			}
			ControlDialogControl controlDialogControl = new ControlDialogControl();
			ControlDialogControl controlDialogControl2 = controlDialogControl;
			controlDialogControl2.OnOffButtonClick = (ControlDialogControl.OffButtonDelegate)Delegate.Combine(controlDialogControl2.OnOffButtonClick, new ControlDialogControl.OffButtonDelegate(this.OffButtonClick));
			controlDialogControl2 = controlDialogControl;
			controlDialogControl2.OnFlashingButtonClick = (ControlDialogControl.FlashingButtonDelegate)Delegate.Combine(controlDialogControl2.OnFlashingButtonClick, new ControlDialogControl.FlashingButtonDelegate(this.FlashingButtonClick));
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(controlDialogControl);
		}

		public void OffButtonClick()
		{
			ControlDialogOffControl controlDialogOffControl = new ControlDialogOffControl();
			controlDialogOffControl.OnRetryClick = new ControlDialogOffControl.RetryButtonClickDelegate(this.retry);
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(controlDialogOffControl);
		}

		public void FlashingButtonClick()
		{
			ControlDialogFlashingControl controlDialogFlashingControl = new ControlDialogFlashingControl();
			controlDialogFlashingControl.OnRetryClick = new ControlDialogFlashingControl.RetryButtonClickDelegate(this.retry);
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(controlDialogFlashingControl);
		}

		private void ShowCableControl()
		{
			ControlInterruptCableControl controlInterruptCableControl = new ControlInterruptCableControl();
			controlInterruptCableControl.OnRetry = new ControlInterruptCableControl.OnRetryDelegate(this.retry);
			this.dialogGroup.Controls.Clear();
			this.dialogGroup.Controls.Add(controlInterruptCableControl);
		}

		public void retry()
		{
			base.Close();
			if (this.OnRetry != null)
			{
				this.OnRetry();
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
			this.dialogGroup.TabIndex = 2;
			this.dialogGroup.TabStop = false;
			this.dialogGroup.Text = "groupBox1";
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			this.BackColor = Color.White;
			base.ClientSize = new Size(500, 264);
			base.Controls.Add(this.dialogGroup);
			base.Name = "ControlDialog";
			base.Load += new EventHandler(this.ControlDialog_Load);
			base.Controls.SetChildIndex(this.closeButton, 0);
			base.Controls.SetChildIndex(this.dialogGroup, 0);
			base.ResumeLayout(false);
		}
	}
}
