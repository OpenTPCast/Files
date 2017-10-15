using NLog;
using System;
using System.Threading;
using System.Windows.Forms;

namespace TPCASTWindows.Utils
{
	internal class LoopCheckModel
	{
		public delegate void OnControlInterruptDelegate(int status);

		private static Logger log = LogManager.GetCurrentClassLogger();

		private static Control sContext;

		public static LoopCheckModel.OnControlInterruptDelegate OnControlInterrupt;

		private static Thread backgroundCheckControlThread;

		private static bool isLinked = true;

		public static void Init(Control context)
		{
			LoopCheckModel.sContext = context;
		}

		public static void UnInit()
		{
			LoopCheckModel.sContext = null;
			LoopCheckModel.AbortBackgroundCheckControlThread();
		}

		public static void setConnectLoopInterruptCallback(ConnectLoopInterruptCallback interruptCallback)
		{
			if (interruptCallback != null)
			{
				LoopCheckModel.OnControlInterrupt = new LoopCheckModel.OnControlInterruptDelegate(interruptCallback.OnControlInterrupt);
			}
		}

		private static void ControlInterrupt(int status)
		{
			if (LoopCheckModel.sContext != null && LoopCheckModel.OnControlInterrupt != null)
			{
				if (LoopCheckModel.sContext.InvokeRequired)
				{
					LoopCheckModel.sContext.Invoke(LoopCheckModel.OnControlInterrupt, new object[]
					{
						status
					});
					return;
				}
				LoopCheckModel.OnControlInterrupt(status);
			}
		}

		public static void StartBackgroundCheckControlThread()
		{
			LoopCheckModel.backgroundCheckControlThread = new Thread(new ThreadStart(LoopCheckModel.StartBackgroundCheckControlThreadStart));
			LoopCheckModel.isLinked = true;
			LoopCheckModel.backgroundCheckControlThread.Start();
		}

		private static void StartBackgroundCheckControlThreadStart()
		{
			int status = 0;
			while (LoopCheckModel.isLinked)
			{
				status = UsbIPUtil.isUsbLinked();
				if (status == 0)
				{
					LoopCheckModel.isLinked = true;
				}
				else if (status == -1000 || status == -2000)
				{
					LoopCheckModel.isLinked = false;
					break;
				}
				Thread.Sleep(5000);
			}
			if (status == -1000)
			{
				if (ChannelUtil.pingRouterConnect())
				{
					if (UsbIPUtil.isHostLinked())
					{
						if (UsbIPUtil.isCableLinked())
						{
							status = -1000;
						}
						else
						{
							status = -3000;
						}
					}
					else
					{
						status = -1002;
					}
				}
				else
				{
					status = -1001;
				}
			}
			else if (status == -2000)
			{
				UsbIPUtil.RebootService();
				status = -1000;
			}
			AnimationModel.ConnectAnimateionPause();
			LoopCheckModel.ControlInterrupt(status);
		}

		public static void AbortBackgroundCheckControlThread()
		{
			LoopCheckModel.log.Trace("AboutBackgroundCheckControlThread");
			if (LoopCheckModel.backgroundCheckControlThread != null)
			{
				LoopCheckModel.backgroundCheckControlThread.Abort();
			}
		}
	}
}
