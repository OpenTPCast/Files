using NLog;
using System;
using System.Threading;
using System.Timers;
using System.Windows.Forms;

namespace TPCASTWindows.Utils
{
	internal class ConnectModel
	{
		public delegate void OnInitCheckRouterSSIDFinishDelegate(bool modified);

		public delegate void OnInitCheckRouterFailDelegate();

		public delegate void BeginCheckControlDelegate();

		public delegate void OnCheckRouterSSIDFinishDelegate(bool modified);

		public delegate void OnCheckRouterConnectedDelegate();

		public delegate void OnCheckHostConnectDelegate();

		public delegate void OnCheckControlErrorDelegate(int status);

		public delegate void OnHostConnectedDelegate();

		public delegate void OnRouterConnectedDelegate();

		public delegate void OnReloadCheckErrorDelegate(int error);

		public delegate void OnCheckRouterFinishDelegate(bool isOurRouter);

		public delegate void OnChannelSwitchedDelegate();

		public delegate void OnSwitchCheckErrorDelegate(int error);

		private static Logger log = LogManager.GetCurrentClassLogger();

		private Control context;

		public ConnectModel.OnInitCheckRouterSSIDFinishDelegate OnInitCheckRouterSSIDFinish;

		public ConnectModel.OnInitCheckRouterFailDelegate OnInitCheckRouterFail;

		public ConnectModel.BeginCheckControlDelegate BeginCheckControl;

		public ConnectModel.OnCheckRouterSSIDFinishDelegate OnCheckRouterSSIDFinish;

		public ConnectModel.OnCheckRouterConnectedDelegate OnCheckRouterConnected;

		public ConnectModel.OnCheckHostConnectDelegate OnCheckHostConnect;

		public ConnectModel.OnCheckControlErrorDelegate OnCheckControlError;

		private Thread initCheckRouterSSIDThread;

		private bool abortCheckRouter;

		private Thread checkRouterThread;

		private System.Timers.Timer CheckRouterTimer;

		private const int ROUTER_TIMEOUT = 30;

		private const int RASPBERRY_TIMEOUT = 40;

		private bool abortCheckHost;

		private System.Timers.Timer CheckHostTimer;

		public ConnectModel.OnHostConnectedDelegate OnHostConnected;

		public ConnectModel.OnRouterConnectedDelegate OnRouterConnected;

		public ConnectModel.OnReloadCheckErrorDelegate OnReloadCheckError;

		private Thread checkControlReloadThread;

		private bool abortRouter;

		private System.Timers.Timer RouterTimer;

		private bool abortHost;

		private System.Timers.Timer HostTimer;

		public ConnectModel.OnCheckRouterFinishDelegate OnCheckRouterChannelFinishListener;

		public ConnectModel.OnChannelSwitchedDelegate OnChannelSwitched;

		public ConnectModel.OnSwitchCheckErrorDelegate OnSwitchCheckError;

		private string currentChannel = "";

		private string switchedChannel = "";

		private bool abortSwitchChannel;

		private System.Timers.Timer SwitchChannelTimer;

		public ConnectModel(Control context)
		{
			this.context = context;
		}

		public void unInit()
		{
			this.context = null;
			this.AbortInitCheckRouterSSIDThread();
			this.AbortRouterThread();
			this.AbortCheckControlReloadThread();
		}

		public void setConnectStatusCallback(ConnectStatusCallback statusCallback)
		{
			if (statusCallback != null)
			{
				this.OnInitCheckRouterSSIDFinish = new ConnectModel.OnInitCheckRouterSSIDFinishDelegate(statusCallback.OnInitCheckRouterSSIDFinish);
				this.OnInitCheckRouterFail = new ConnectModel.OnInitCheckRouterFailDelegate(statusCallback.OnInitCheckRouterFail);
				this.BeginCheckControl = new ConnectModel.BeginCheckControlDelegate(statusCallback.BeginCheckControl);
				this.OnCheckRouterSSIDFinish = new ConnectModel.OnCheckRouterSSIDFinishDelegate(statusCallback.OnCheckRouterSSIDFinish);
				this.OnCheckRouterConnected = new ConnectModel.OnCheckRouterConnectedDelegate(statusCallback.OnCheckRouterConnected);
				this.OnCheckHostConnect = new ConnectModel.OnCheckHostConnectDelegate(statusCallback.OnCheckHostConnect);
				this.OnCheckControlError = new ConnectModel.OnCheckControlErrorDelegate(statusCallback.OnCheckControlError);
			}
		}

		private void initCheckRouterSSIDFinish(bool modified)
		{
			if (this.context != null && this.OnInitCheckRouterSSIDFinish != null)
			{
				if (this.context.InvokeRequired)
				{
					this.context.Invoke(this.OnInitCheckRouterSSIDFinish, new object[]
					{
						modified
					});
					return;
				}
				this.OnInitCheckRouterSSIDFinish(modified);
			}
		}

		private void initCheckRouterFail()
		{
			if (this.context != null && this.OnInitCheckRouterFail != null)
			{
				if (this.context.InvokeRequired)
				{
					this.context.Invoke(this.OnInitCheckRouterFail);
					return;
				}
				this.OnInitCheckRouterFail();
			}
		}

		private void CheckRouterSSIDFinish(bool modified)
		{
			if (this.context != null && this.OnCheckRouterSSIDFinish != null)
			{
				if (this.context.InvokeRequired)
				{
					this.context.Invoke(this.OnCheckRouterSSIDFinish, new object[]
					{
						modified
					});
					return;
				}
				this.OnCheckRouterSSIDFinish(modified);
			}
		}

		public void CheckRouterConnected()
		{
			if (this.context != null && this.OnCheckRouterConnected != null)
			{
				if (this.context.InvokeRequired)
				{
					this.context.Invoke(this.OnCheckRouterConnected);
					return;
				}
				this.OnCheckRouterConnected();
			}
		}

		private void CheckHostConnected()
		{
			if (this.context != null && this.OnCheckHostConnect != null)
			{
				if (this.context.InvokeRequired)
				{
					this.context.Invoke(this.OnCheckHostConnect);
					return;
				}
				this.OnCheckHostConnect();
			}
		}

		private void CheckControlError(int status)
		{
			if (this.context != null && this.OnCheckControlError != null)
			{
				if (this.context.InvokeRequired)
				{
					this.context.Invoke(this.OnCheckControlError, new object[]
					{
						status
					});
					return;
				}
				this.OnCheckControlError(status);
			}
		}

		public void InitCheckRouterSSID()
		{
			this.initCheckRouterSSIDThread = new Thread(new ThreadStart(this.initCheckRouterSSIDThreadStart));
			this.initCheckRouterSSIDThread.Start();
		}

		private void AbortInitCheckRouterSSIDThread()
		{
			if (this.initCheckRouterSSIDThread != null)
			{
				this.initCheckRouterSSIDThread.Abort();
			}
		}

		private void initCheckRouterSSIDThreadStart()
		{
			string SSID = ChannelUtil.getWifiSSID();
			ConnectModel.log.Trace("ssid = " + SSID);
			if (string.IsNullOrEmpty(SSID))
			{
				this.initCheckRouterFail();
				return;
			}
			if ("TPCast_AP".Equals(SSID))
			{
				this.initCheckRouterSSIDFinish(false);
				return;
			}
			this.initCheckRouterSSIDFinish(true);
		}

		public void CheckControl()
		{
			if (this.BeginCheckControl != null)
			{
				this.BeginCheckControl();
			}
			this.checkRouterThread = new Thread(new ThreadStart(this.CheckRouterThreadStart));
			this.checkRouterThread.Start();
		}

		private void CheckRouterThreadStart()
		{
			this.StartCheckRouterTimer();
			this.CheckRouter();
		}

		private void CheckRouter()
		{
			bool isCheckRouterConnected = false;
			this.abortCheckRouter = false;
			while (!isCheckRouterConnected)
			{
				if (this.abortCheckRouter)
				{
					this.StopCheckRouterTimer();
					this.CheckControlError(-1001);
					return;
				}
				if (ChannelUtil.pingRouterConnect())
				{
					this.CheckRouterConnected();
					this.checkRouterSSIDStart();
					return;
				}
				Thread.Sleep(5000);
			}
		}

		private void checkRouterSSIDStart()
		{
			string SSID = ChannelUtil.getWifiSSID();
			if (string.IsNullOrEmpty(SSID))
			{
				this.CheckHostThreadStart();
				return;
			}
			if ("TPCast_AP".Equals(SSID))
			{
				this.CheckRouterSSIDFinish(false);
				return;
			}
			this.CheckHostThreadStart();
		}

		private void StartCheckRouterTimer()
		{
			this.CheckRouterTimer = new System.Timers.Timer(30000.0);
			this.CheckRouterTimer.Elapsed += new ElapsedEventHandler(this.CheckRouterTimeout);
			this.CheckRouterTimer.AutoReset = false;
			this.CheckRouterTimer.Enabled = true;
		}

		private void StopCheckRouterTimer()
		{
			if (this.CheckRouterTimer != null)
			{
				this.CheckRouterTimer.Enabled = false;
				this.CheckRouterTimer.Dispose();
				this.CheckRouterTimer = null;
			}
		}

		private void CheckRouterTimeout(object sender, ElapsedEventArgs e)
		{
			this.abortCheckRouter = true;
		}

		private void CheckHostThreadStart()
		{
			this.StartCheckHostTimer();
			this.CheckHost();
		}

		private void CheckHost()
		{
			bool isHostConnected = false;
			this.abortCheckHost = false;
			while (!isHostConnected)
			{
				if (this.abortCheckHost)
				{
					this.StopCheckHostTimer();
					this.CheckControlError(-1002);
					return;
				}
				int status = UsbIPUtil.isHostConnected();
				if (status == -1001)
				{
					this.CheckControlError(status);
					return;
				}
				if (status != -1002 && status == 0)
				{
					this.CheckHostConnected();
					this.ConnectControl();
					return;
				}
				Thread.Sleep(5000);
			}
		}

		private void StartCheckHostTimer()
		{
			this.CheckHostTimer = new System.Timers.Timer(40000.0);
			this.CheckHostTimer.Elapsed += new ElapsedEventHandler(this.CheckHostTimeout);
			this.CheckHostTimer.AutoReset = false;
			this.CheckHostTimer.Enabled = true;
		}

		private void StopCheckHostTimer()
		{
			if (this.CheckHostTimer != null)
			{
				this.CheckHostTimer.Enabled = false;
				this.CheckHostTimer.Dispose();
				this.CheckHostTimer = null;
			}
		}

		private void CheckHostTimeout(object sender, ElapsedEventArgs e)
		{
			this.abortCheckHost = true;
		}

		private void ConnectControl()
		{
			if (!UsbIPUtil.isCableLinked())
			{
				this.CheckControlError(-3000);
				return;
			}
			int status = UsbIPUtil.ForceConnectControl();
			if (status == 0)
			{
				this.CheckControlError(0);
				return;
			}
			if (status != -2000)
			{
				if (status == -1000)
				{
					this.CheckControlError(UsbIPUtil.isUSBConnected());
				}
				return;
			}
			if (UsbIPUtil.RebootService())
			{
				this.ConnectControl();
				return;
			}
			this.CheckControlError(-2000);
		}

		public void AbortRouterThread()
		{
			ConnectModel.log.Trace("AbortRouterThread");
			if (this.checkRouterThread != null)
			{
				this.checkRouterThread.Abort();
			}
		}

		public void setConnectReloadCallback(ConnectReloadCallback reloadCallback)
		{
			if (reloadCallback != null)
			{
				this.OnHostConnected = new ConnectModel.OnHostConnectedDelegate(reloadCallback.OnHostConnected);
				this.OnRouterConnected = new ConnectModel.OnRouterConnectedDelegate(reloadCallback.OnRouterConnected);
				this.OnReloadCheckError = new ConnectModel.OnReloadCheckErrorDelegate(reloadCallback.OnReloadCheckError);
			}
		}

		private void RouterConnected()
		{
			if (this.context != null && this.OnRouterConnected != null)
			{
				if (this.context.InvokeRequired)
				{
					this.context.Invoke(this.OnRouterConnected);
					return;
				}
				this.OnRouterConnected();
			}
		}

		private void HostConnected()
		{
			if (this.context != null && this.OnHostConnected != null)
			{
				if (this.context.InvokeRequired)
				{
					this.context.Invoke(this.OnHostConnected);
					return;
				}
				this.OnHostConnected();
			}
		}

		public void ReloadCheckError(int error)
		{
			if (this.context != null && this.OnReloadCheckError != null)
			{
				if (this.context.InvokeRequired)
				{
					this.context.Invoke(this.OnReloadCheckError, new object[]
					{
						error
					});
					return;
				}
				this.OnReloadCheckError(error);
			}
		}

		public void StartCheckControlReloadThread()
		{
			this.checkControlReloadThread = new Thread(new ThreadStart(this.CheckControlReloadThreadStart));
			this.checkControlReloadThread.Start();
		}

		private void CheckControlReloadThreadStart()
		{
			this.StartRouterTimer();
			this.CheckControlReload();
		}

		private void CheckControlReload()
		{
			bool isRouterConnected = false;
			this.abortRouter = false;
			while (!isRouterConnected)
			{
				if (this.abortRouter)
				{
					this.StopRouterTimer();
					this.ReloadCheckError(-1001);
					return;
				}
				if (ChannelUtil.pingRouterConnect())
				{
					this.RouterConnected();
					this.HostThreadStart();
					return;
				}
				Thread.Sleep(5000);
			}
		}

		private void StartRouterTimer()
		{
			this.RouterTimer = new System.Timers.Timer(30000.0);
			this.RouterTimer.Elapsed += new ElapsedEventHandler(this.RouterTimeout);
			this.RouterTimer.AutoReset = false;
			this.RouterTimer.Enabled = true;
		}

		private void StopRouterTimer()
		{
			if (this.RouterTimer != null)
			{
				this.RouterTimer.Enabled = false;
				this.RouterTimer.Dispose();
				this.RouterTimer = null;
			}
		}

		private void RouterTimeout(object sender, ElapsedEventArgs e)
		{
			this.abortRouter = true;
		}

		public void StartHostThread()
		{
			this.checkControlReloadThread = new Thread(new ThreadStart(this.HostThreadStart));
			this.checkControlReloadThread.Start();
		}

		private void HostThreadStart()
		{
			this.StartHostTimer();
			this.Host();
		}

		private void Host()
		{
			bool isHostConnected = false;
			this.abortHost = false;
			while (!isHostConnected)
			{
				if (this.abortHost)
				{
					this.StopHostTimer();
					this.ReloadCheckError(-1002);
					return;
				}
				int status = UsbIPUtil.isHostConnected();
				if (status == -1001)
				{
					this.ReloadCheckError(status);
					return;
				}
				if (status != -1002 && status == 0)
				{
					this.HostConnected();
					this.ReConnectControl();
					return;
				}
				Thread.Sleep(5000);
			}
		}

		private void StartHostTimer()
		{
			this.HostTimer = new System.Timers.Timer(40000.0);
			this.HostTimer.Elapsed += new ElapsedEventHandler(this.HostTimeout);
			this.HostTimer.AutoReset = false;
			this.HostTimer.Enabled = true;
		}

		private void StopHostTimer()
		{
			if (this.HostTimer != null)
			{
				this.HostTimer.Enabled = false;
				this.HostTimer.Dispose();
				this.HostTimer = null;
			}
		}

		private void HostTimeout(object sender, ElapsedEventArgs e)
		{
			this.abortHost = true;
		}

		private void ReConnectControl()
		{
			Thread.Sleep(5000);
			if (!UsbIPUtil.isCableLinked())
			{
				this.ReloadCheckError(-3000);
				return;
			}
			int status = UsbIPUtil.ConnectControl();
			if (status == 0)
			{
				this.ReloadCheckError(0);
				return;
			}
			if (status != -2000)
			{
				if (status == -1000)
				{
					this.ReloadCheckError(UsbIPUtil.isUSBConnected());
				}
				return;
			}
			if (UsbIPUtil.RebootService())
			{
				this.ReConnectControl();
				return;
			}
			this.ReloadCheckError(-2000);
		}

		public void AbortCheckControlReloadThread()
		{
			ConnectModel.log.Trace("AboutCheckControlReloadThread");
			if (this.checkControlReloadThread != null)
			{
				this.checkControlReloadThread.Abort();
			}
		}

		public void setSwitchChannelCallback(SwitchChannelCallback switchCallback)
		{
			if (switchCallback != null)
			{
				this.OnCheckRouterChannelFinishListener = new ConnectModel.OnCheckRouterFinishDelegate(switchCallback.OnCheckRouterChannelFinishListener);
				this.OnChannelSwitched = new ConnectModel.OnChannelSwitchedDelegate(switchCallback.OnChannelSwitched);
				this.OnSwitchCheckError = new ConnectModel.OnSwitchCheckErrorDelegate(switchCallback.OnSwitchCheckError);
			}
		}

		private void CheckRouterChannelFinish(bool isOurRouter)
		{
			if (this.context != null && this.OnCheckRouterChannelFinishListener != null)
			{
				if (this.context.InvokeRequired)
				{
					this.context.Invoke(this.OnCheckRouterChannelFinishListener, new object[]
					{
						isOurRouter
					});
					return;
				}
				this.OnCheckRouterChannelFinishListener(isOurRouter);
			}
		}

		private void ChannelSwitchedFinish()
		{
			if (this.context != null && this.OnChannelSwitched != null)
			{
				if (this.context.InvokeRequired)
				{
					this.context.Invoke(this.OnChannelSwitched);
					return;
				}
				this.OnChannelSwitched();
			}
		}

		public void SwitchCheckError(int error)
		{
			if (this.context != null && this.OnSwitchCheckError != null)
			{
				if (this.context.InvokeRequired)
				{
					this.context.Invoke(this.OnSwitchCheckError, new object[]
					{
						error
					});
					return;
				}
				this.OnSwitchCheckError(error);
			}
		}

		public void SwitchChannel()
		{
			this.StartSwitchChannelTimer();
			this.switchedChannel = "";
			this.currentChannel = "";
			AnimationModel.ConnectAnimateionPause();
			new Thread(new ThreadStart(this.getCurrentChannel)).Start();
		}

		private void getCurrentChannel()
		{
			ConnectModel.log.Trace("getCurrentChannel");
			if (!ChannelUtil.pingRouterConnect())
			{
				ConnectModel.log.Trace("ping 144.1 fail ");
				this.StopSwitchChannelTimer();
				this.CheckRouterChannelFinish(false);
				return;
			}
			this.currentChannel = ChannelUtil.getWirelessChannel();
			ConnectModel.log.Trace("current Channel = " + this.currentChannel);
			if (this.currentChannel != null)
			{
				ChannelUtil.switchWirelessChannel();
				ConnectModel.log.Trace("switchWirelessChannel");
				this.checkIsChannelSwitched();
				return;
			}
			ConnectModel.log.Trace("other router or bad router");
			this.StopSwitchChannelTimer();
			this.CheckRouterChannelFinish(false);
		}

		private void checkIsChannelSwitched()
		{
			bool isSwitched = false;
			this.abortSwitchChannel = false;
			while (!isSwitched)
			{
				if (this.abortSwitchChannel)
				{
					return;
				}
				this.switchedChannel = ChannelUtil.getWirelessChannel();
				ConnectModel.log.Trace("switched channel = " + this.switchedChannel);
				if (this.switchedChannel != null && this.switchedChannel != this.currentChannel)
				{
					isSwitched = true;
					this.StopSwitchChannelTimer();
					this.ChannelSwitchedFinish();
				}
				Thread.Sleep(5000);
			}
		}

		private void StartSwitchChannelTimer()
		{
			this.SwitchChannelTimer = new System.Timers.Timer(60000.0);
			this.SwitchChannelTimer.Elapsed += new ElapsedEventHandler(this.SwitchChannelTimeout);
			this.SwitchChannelTimer.AutoReset = false;
			this.SwitchChannelTimer.Enabled = true;
		}

		private void StopSwitchChannelTimer()
		{
			if (this.SwitchChannelTimer != null)
			{
				this.SwitchChannelTimer.Enabled = false;
				this.SwitchChannelTimer.Dispose();
				this.SwitchChannelTimer = null;
			}
		}

		private void SwitchChannelTimeout(object sender, ElapsedEventArgs e)
		{
			this.abortSwitchChannel = true;
			this.StopSwitchChannelTimer();
			this.SwitchCheckError(-1001);
		}
	}
}
