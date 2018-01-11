using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Threading;

namespace TPcastClassLibrary
{
	public class TPcastRemoteDevices
	{
		private volatile bool StopThread;

		private static bool ThreadIsRunning;

		private Thread workerThread;

		private const string ewhName = "Global\\TPcastDeviceEvent";

		private bool ewhwasCreated;

		private EventWaitHandle ewh_devstatus;

		private EventWaitHandle ewh_stopthread;

		private int devcount;

		private int devstatus;

		[method: CompilerGenerated]
		[CompilerGenerated]
		public event TPcastDevStsChgEventHandler DevStsChgEvent;

		public TPcastRemoteDevices()
		{
			this.devcount = 0;
			this.devstatus = 0;
		}

		[DllImport("TPcastTools.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool GetTPcastDevStatus(ref int DeviceCount, ref int DeviceStatus);

		[DllImport("TPcastTools.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool ConnectToRemote();

		public void StartRemoteDevice()
		{
			TPcastRemoteDevices.ConnectToRemote();
		}

		private void ProcessChecking(object comp)
		{
			if (TPcastRemoteDevices.GetTPcastDevStatus(ref this.devcount, ref this.devstatus) && this.DevStsChgEvent != null)
			{
				TPcastDeviceStatus tPcastDeviceStatus = new TPcastDeviceStatus();
				tPcastDeviceStatus.Count = this.devcount;
				tPcastDeviceStatus.Status = this.devstatus;
				this.DevStsChgEvent(comp, tPcastDeviceStatus);
				Console.WriteLine("run event handler!");
			}
			EventWaitHandle[] waitHandles = new EventWaitHandle[]
			{
				this.ewh_devstatus,
				this.ewh_stopthread
			};
			while (true)
			{
				Console.WriteLine("waiting for events!");
				TPcastEventType tPcastEventType = (TPcastEventType)WaitHandle.WaitAny(waitHandles);
				if (tPcastEventType != TPcastEventType.DeviceStatus)
				{
					if (tPcastEventType == TPcastEventType.StopThread)
					{
						break;
					}
				}
				else
				{
					this.ewh_devstatus.Reset();
					Console.WriteLine("TPcastEventType.DeviceStatus triggered!");
					if (TPcastRemoteDevices.GetTPcastDevStatus(ref this.devcount, ref this.devstatus) && this.DevStsChgEvent != null)
					{
						TPcastDeviceStatus tPcastDeviceStatus2 = new TPcastDeviceStatus();
						tPcastDeviceStatus2.Count = this.devcount;
						tPcastDeviceStatus2.Status = this.devstatus;
						this.DevStsChgEvent(comp, tPcastDeviceStatus2);
						Console.WriteLine("run event handler!");
					}
				}
				if (this.StopThread)
				{
					goto Block_7;
				}
			}
			Console.WriteLine("TPcastEventType.StopThread triggered!");
			TPcastRemoteDevices.ThreadIsRunning = false;
			return;
			Block_7:
			TPcastRemoteDevices.ThreadIsRunning = false;
			Console.WriteLine("checking exist");
		}

		public bool TPcastStartChecking()
		{
			this.StopThread = false;
			bool flag = false;
			try
			{
				this.ewh_stopthread = new EventWaitHandle(false, EventResetMode.AutoReset);
				this.ewh_devstatus = new EventWaitHandle(false, EventResetMode.ManualReset, "Global\\TPcastDeviceEvent", out this.ewhwasCreated);
			}
			catch (UnauthorizedAccessException ex)
			{
				Console.WriteLine("UnauthorizedAccessException: {0}", ex.Message);
				flag = true;
			}
			catch (Exception ex2)
			{
				Console.WriteLine("Exception: {0}", ex2.Message);
				bool result = false;
				return result;
			}
			if (flag)
			{
				try
				{
					this.ewh_devstatus = EventWaitHandle.OpenExisting("Global\\TPcastDeviceEvent", EventWaitHandleRights.ReadPermissions | EventWaitHandleRights.ChangePermissions);
					EventWaitHandleSecurity accessControl = this.ewh_devstatus.GetAccessControl();
					string expr_9B = Environment.UserDomainName + "\\" + Environment.UserName;
					EventWaitHandleAccessRule rule = new EventWaitHandleAccessRule(expr_9B, EventWaitHandleRights.Modify | EventWaitHandleRights.Synchronize, AccessControlType.Deny);
					accessControl.RemoveAccessRule(rule);
					rule = new EventWaitHandleAccessRule(expr_9B, EventWaitHandleRights.Modify | EventWaitHandleRights.Synchronize, AccessControlType.Allow);
					accessControl.AddAccessRule(rule);
					this.ewh_devstatus.SetAccessControl(accessControl);
					Console.WriteLine("Updated event security.");
					this.ewh_devstatus = EventWaitHandle.OpenExisting("Global\\TPcastDeviceEvent");
				}
				catch (UnauthorizedAccessException ex3)
				{
					Console.WriteLine("Unable to change permissions: {0}", ex3.Message);
					bool result = false;
					return result;
				}
			}
			if (!TPcastRemoteDevices.ThreadIsRunning)
			{
				TPcastRemoteDevices.ThreadIsRunning = true;
				this.workerThread = new Thread(new ParameterizedThreadStart(this.ProcessChecking));
				this.workerThread.Start(this);
			}
			return true;
		}

		public void TPcastStopChecking()
		{
			this.StopThread = true;
			if (this.ewh_stopthread != null)
			{
				Console.WriteLine("set Stopthread event!");
				this.ewh_stopthread.Set();
			}
		}
	}
}
