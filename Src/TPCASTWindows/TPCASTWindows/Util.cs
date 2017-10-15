using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using Microsoft.Win32;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using TPCASTWindows.UI.Other;

namespace TPCASTWindows
{
	internal class Util
	{
		public delegate void OnInitCheckRouterSSIDFinishDelegate(bool modified);

		public delegate void OnInitCheckRouterFailDelegate();

		public delegate void OnCheckControlFinishDelegate(bool isLoaded);

		public delegate void BeginCheckControlDelegate();

		public delegate void OnCheckRouterSSIDFinishDelegate(bool modified);

		public delegate void OnCheckRouterConnectedDelegate();

		public delegate void OnCheckHostConnectDelegate();

		public delegate void OnCheckControlErrorDelegate(int status);

		public delegate void OnControlInterruptDelegate(int status);

		public delegate void OnRouterConnectedDelegate();

		public delegate void OnHostConnectedDelegate();

		public delegate void OnControlConnectedErrorDelegate(int error);

		public delegate void OnConnectAnimateionPauseDelegate();

		public delegate void OnConnectAnimateionResumeDelegate();

		public delegate void OnCheckRouterFinishDelegate(bool isOurRouter);

		public delegate void OnChannelSwitchedDelegate();

		public delegate void BeginCheckBluetoothDelegate();

		public delegate void NoBluetoothDelegate();

		public delegate void NoBluetoothDriverDelegate();

		public delegate void DeviceConnectedDelegate();

		public delegate void DeviceNotConnectedDelegate();

		public delegate void DeviceFoundNotConnectedDelegate();

		public delegate void OnOutputFileDelegate(string msg);

		public delegate void BeginCheckSteamDelegate();

		public delegate void OnCheckSteamFinishDelegate(bool isInstalled);

		private static Logger log = LogManager.GetCurrentClassLogger();

		public static Form sContext;

		public static Util.OnInitCheckRouterSSIDFinishDelegate OnInitCheckRouterSSIDFinish;

		public static Util.OnInitCheckRouterFailDelegate OnInitCheckRouterFail;

		private static Thread initCheckRouterSSIDThread;

		public static Util.OnCheckRouterFinishDelegate OnCheckRouterFinishListener;

		public static Util.OnCheckControlFinishDelegate OnCheckControlFinishListener;

		public static Util.BeginCheckControlDelegate BeginCheckControl;

		public static Util.OnCheckRouterSSIDFinishDelegate OnCheckRouterSSIDFinish;

		public static Util.OnCheckRouterConnectedDelegate OnCheckRouterConnected;

		public static Util.OnCheckHostConnectDelegate OnCheckHostConnect;

		public static Util.OnCheckControlErrorDelegate OnCheckControlError;

		private static bool abortCheckRouter = false;

		private static Thread checkRouterThread;

		private static System.Timers.Timer CheckRouterTimer;

		private const int ROUTER_TIMEOUT = 30;

		private const int RASPBERRY_TIMEOUT = 40;

		private static bool abortCheckHost = false;

		private static System.Timers.Timer CheckHostTimer;

		public static Util.OnControlInterruptDelegate OnControlInterrupt;

		private static Thread backgroundCheckControlThread;

		private static bool isLinked = true;

		public static Util.OnRouterConnectedDelegate OnRouterConnected;

		public static Util.OnHostConnectedDelegate OnHostConnected;

		public static Util.OnControlConnectedErrorDelegate OnControlConnectedError;

		private static Thread checkControlReloadThread;

		private static bool abortRouter = false;

		private static System.Timers.Timer RouterTimer;

		private static bool abortHost = false;

		private static System.Timers.Timer HostTimer;

		public static Util.OnConnectAnimateionPauseDelegate OnConnectAnimateionPause;

		public static Util.OnConnectAnimateionResumeDelegate OnConnectAnimateionResume;

		public static Util.OnCheckRouterFinishDelegate OnCheckRouterChannelFinishListener;

		public static Util.OnChannelSwitchedDelegate OnChannelSwitched;

		private static string currentChannel = "";

		private static string switchedChannel = "";

		private static bool abortSwitchChannel = false;

		private static System.Timers.Timer SwitchChannelTimer;

		public static Util.BeginCheckBluetoothDelegate BeginCheckBluetooth;

		public static Util.NoBluetoothDelegate NoBluetooth;

		public static Util.NoBluetoothDriverDelegate NoBluetoothDriver;

		private static Thread checkBluetoothThread;

		private static Thread scanBluetoothThread;

		public static Util.DeviceConnectedDelegate DeviceConnected;

		public static Util.DeviceNotConnectedDelegate DeviceNotConnected;

		public static Util.DeviceFoundNotConnectedDelegate DeviceFoundNotConnected;

		public static Util.OnOutputFileDelegate OnOutputFile;

		public static Util.BeginCheckSteamDelegate BeginCheckSteam;

		public static Util.OnCheckSteamFinishDelegate OnCheckSteamFinish;

		public static GrayForm grayForm = new GrayForm();

		private static int showCount = 0;

		private static GuideForm guideForm = new GuideForm();

		public static void Init(Form context)
		{
			Util.sContext = context;
		}

		public static void UnInit()
		{
			Util.sContext = null;
			Util.AbortInitCheckRouterSSIDThread();
			Util.AbortRouterThread();
			Util.AbortCheckBluetoothThread();
			Util.AbortScanBluetoothThread();
			Util.AbortBackgroundCheckControlThread();
			Util.AbortCheckControlReloadThread();
		}

		public List<IPAddress> GetGatewayAddresses()
		{
			List<IPAddress> addresses = new List<IPAddress>();
			NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
			Util.log.Trace("适配器个数：" + adapters.Length);
			NetworkInterface[] array = adapters;
			for (int i = 0; i < array.Length; i++)
			{
				NetworkInterface adapter = array[i];
				Util.log.Trace("描述：" + adapter.Description);
				Util.log.Trace("标识符：" + adapter.Id);
				Util.log.Trace("名称：" + adapter.Name);
				Util.log.Trace("类型：" + adapter.NetworkInterfaceType);
				Util.log.Trace("速度：" + (double)adapter.Speed * 0.001 * 0.001 + "M");
				Util.log.Trace("操作状态：" + adapter.OperationalStatus);
				Util.log.Trace("MAC 地址：" + adapter.GetPhysicalAddress());
				IPInterfaceProperties ipProperties = adapter.GetIPProperties();
				if (ipProperties.GatewayAddresses.Count > 0)
				{
					Util.log.Trace("默认网关：" + ipProperties.GatewayAddresses[0].Address);
					addresses.Add(ipProperties.GatewayAddresses[0].Address);
				}
			}
			return addresses;
		}

		public void CheckNetwork()
		{
			foreach (IPAddress address in this.GetGatewayAddresses())
			{
				Util.log.Trace("测试网关：" + address);
				new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			}
		}

		private static void initCheckRouterSSIDFinish(bool modified)
		{
			if (Util.sContext != null && Util.OnInitCheckRouterSSIDFinish != null)
			{
				if (Util.sContext.InvokeRequired)
				{
					Util.sContext.Invoke(Util.OnInitCheckRouterSSIDFinish, new object[]
					{
						modified
					});
					return;
				}
				Util.OnInitCheckRouterSSIDFinish(modified);
			}
		}

		private static void initCheckRouterFail()
		{
			if (Util.sContext != null && Util.OnInitCheckRouterFail != null)
			{
				if (Util.sContext.InvokeRequired)
				{
					Util.sContext.Invoke(Util.OnInitCheckRouterFail);
					return;
				}
				Util.OnInitCheckRouterFail();
			}
		}

		public static void InitCheckRouterSSID()
		{
			Util.initCheckRouterSSIDThread = new Thread(new ThreadStart(Util.initCheckRouterSSIDThreadStart));
			Util.initCheckRouterSSIDThread.Start();
		}

		private static void AbortInitCheckRouterSSIDThread()
		{
			if (Util.initCheckRouterSSIDThread != null)
			{
				Util.initCheckRouterSSIDThread.Abort();
			}
		}

		private static void initCheckRouterSSIDThreadStart()
		{
			string SSID = ChannelUtil.getWifiSSID();
			Util.log.Trace("ssid = " + SSID);
			if (string.IsNullOrEmpty(SSID))
			{
				Util.initCheckRouterFail();
				return;
			}
			if ("TPCast_AP".Equals(SSID))
			{
				Util.initCheckRouterSSIDFinish(false);
				return;
			}
			Util.initCheckRouterSSIDFinish(true);
		}

		private static void CheckRouterSSIDFinish(bool modified)
		{
			if (Util.sContext != null && Util.OnCheckRouterSSIDFinish != null)
			{
				if (Util.sContext.InvokeRequired)
				{
					Util.sContext.Invoke(Util.OnCheckRouterSSIDFinish, new object[]
					{
						modified
					});
					return;
				}
				Util.OnCheckRouterSSIDFinish(modified);
			}
		}

		public static void CheckControl()
		{
			if (Util.BeginCheckControl != null)
			{
				Util.BeginCheckControl();
			}
			Util.checkRouterThread = new Thread(new ThreadStart(Util.CheckRouterThreadStart));
			Util.checkRouterThread.Start();
		}

		private static void CheckRouterThreadStart()
		{
			Util.StartCheckRouterTimer();
			Util.CheckRouter();
		}

		private static void CheckRouter()
		{
			bool isCheckRouterConnected = false;
			Util.abortCheckRouter = false;
			while (!isCheckRouterConnected)
			{
				if (Util.abortCheckRouter)
				{
					Util.StopCheckRouterTimer();
					Util.CheckControlError(-1001);
					return;
				}
				if (ChannelUtil.pingRouterConnect())
				{
					Util.CheckRouterConnected();
					Util.checkRouterSSIDStart();
					return;
				}
				Thread.Sleep(5000);
			}
		}

		private static void checkRouterSSIDStart()
		{
			string SSID = ChannelUtil.getWifiSSID();
			if (string.IsNullOrEmpty(SSID))
			{
				Util.CheckHostThreadStart();
				return;
			}
			if ("TPCast_AP".Equals(SSID))
			{
				Util.CheckRouterSSIDFinish(false);
				return;
			}
			Util.CheckHostThreadStart();
		}

		private static void StartCheckRouterTimer()
		{
			Util.CheckRouterTimer = new System.Timers.Timer(30000.0);
			Util.CheckRouterTimer.Elapsed += new ElapsedEventHandler(Util.CheckRouterTimeout);
			Util.CheckRouterTimer.AutoReset = false;
			Util.CheckRouterTimer.Enabled = true;
		}

		private static void StopCheckRouterTimer()
		{
			if (Util.CheckRouterTimer != null)
			{
				Util.CheckRouterTimer.Enabled = false;
				Util.CheckRouterTimer.Dispose();
				Util.CheckRouterTimer = null;
			}
		}

		private static void CheckRouterTimeout(object sender, ElapsedEventArgs e)
		{
			Util.abortCheckRouter = true;
		}

		private static void CheckHostThreadStart()
		{
			Util.StartCheckHostTimer();
			Util.CheckHost();
		}

		private static void CheckHost()
		{
			bool isHostConnected = false;
			Util.abortCheckHost = false;
			while (!isHostConnected)
			{
				if (Util.abortCheckHost)
				{
					Util.StopCheckHostTimer();
					Util.CheckControlError(-1002);
					return;
				}
				int status = UsbIPUtil.isHostConnected();
				if (status == -1001)
				{
					Util.CheckControlError(status);
					return;
				}
				if (status != -1002 && status == 0)
				{
					Util.CheckHostConnected();
					Util.ConnectControl();
					return;
				}
				Thread.Sleep(5000);
			}
		}

		private static void StartCheckHostTimer()
		{
			Util.CheckHostTimer = new System.Timers.Timer(40000.0);
			Util.CheckHostTimer.Elapsed += new ElapsedEventHandler(Util.CheckHostTimeout);
			Util.CheckHostTimer.AutoReset = false;
			Util.CheckHostTimer.Enabled = true;
		}

		private static void StopCheckHostTimer()
		{
			if (Util.CheckHostTimer != null)
			{
				Util.CheckHostTimer.Enabled = false;
				Util.CheckHostTimer.Dispose();
				Util.CheckHostTimer = null;
			}
		}

		private static void CheckHostTimeout(object sender, ElapsedEventArgs e)
		{
			Util.abortCheckHost = true;
		}

		private static void ConnectControl()
		{
			if (!UsbIPUtil.isCableLinked())
			{
				Util.CheckControlError(-3000);
				return;
			}
			int status = UsbIPUtil.ForceConnectControl();
			if (status == 0)
			{
				Util.CheckControlError(0);
				return;
			}
			if (status != -2000)
			{
				if (status == -1000)
				{
					Util.CheckControlError(UsbIPUtil.isUSBConnected());
				}
				return;
			}
			if (UsbIPUtil.RebootService())
			{
				Util.ConnectControl();
				return;
			}
			Util.CheckControlError(-2000);
		}

		public static void CheckRouterConnected()
		{
			if (Util.sContext != null && Util.OnCheckRouterConnected != null)
			{
				if (Util.sContext.InvokeRequired)
				{
					Util.sContext.Invoke(Util.OnCheckRouterConnected);
					return;
				}
				Util.OnCheckRouterConnected();
			}
		}

		private static void CheckHostConnected()
		{
			if (Util.sContext != null && Util.OnCheckHostConnect != null)
			{
				if (Util.sContext.InvokeRequired)
				{
					Util.sContext.Invoke(Util.OnCheckHostConnect);
					return;
				}
				Util.OnCheckHostConnect();
			}
		}

		private static void CheckControlError(int status)
		{
			if (Util.sContext != null && Util.OnCheckControlError != null)
			{
				if (Util.sContext.InvokeRequired)
				{
					Util.sContext.Invoke(Util.OnCheckControlError, new object[]
					{
						status
					});
					return;
				}
				Util.OnCheckControlError(status);
			}
		}

		public static void AbortRouterThread()
		{
			Util.log.Trace("AbortRouterThread");
			if (Util.checkRouterThread != null)
			{
				Util.checkRouterThread.Abort();
			}
		}

		public static void StartBackgroundCheckControlThread()
		{
			Util.backgroundCheckControlThread = new Thread(new ThreadStart(Util.StartBackgroundCheckControlThreadStart));
			Util.isLinked = true;
			Util.backgroundCheckControlThread.Start();
		}

		private static void StartBackgroundCheckControlThreadStart()
		{
			int status = 0;
			while (Util.isLinked)
			{
				status = UsbIPUtil.isUsbLinked();
				if (status == 0)
				{
					Util.isLinked = true;
				}
				else if (status == -1000 || status == -2000)
				{
					Util.isLinked = false;
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
			Util.ConnectAnimateionPause();
			Util.ControlInterrupt(status);
		}

		private static void ControlInterrupt(int status)
		{
			if (Util.sContext != null && Util.OnControlInterrupt != null)
			{
				if (Util.sContext.InvokeRequired)
				{
					Util.sContext.Invoke(Util.OnControlInterrupt, new object[]
					{
						status
					});
					return;
				}
				Util.OnControlInterrupt(status);
			}
		}

		public static void AbortBackgroundCheckControlThread()
		{
			Util.log.Trace("AboutBackgroundCheckControlThread");
			if (Util.backgroundCheckControlThread != null)
			{
				Util.backgroundCheckControlThread.Abort();
			}
		}

		public static void StartCheckControlReloadThread()
		{
			Util.checkControlReloadThread = new Thread(new ThreadStart(Util.CheckControlReloadThreadStart));
			Util.checkControlReloadThread.Start();
		}

		private static void CheckControlReloadThreadStart()
		{
			Util.StartRouterTimer();
			Util.CheckControlReload();
		}

		private static void CheckControlReload()
		{
			bool isRouterConnected = false;
			Util.abortRouter = false;
			while (!isRouterConnected)
			{
				if (Util.abortRouter)
				{
					Util.StopRouterTimer();
					Util.ControlConnectedError(-1001);
					return;
				}
				if (ChannelUtil.pingRouterConnect())
				{
					Util.RouterConnected();
					Util.HostThreadStart();
					return;
				}
				Thread.Sleep(5000);
			}
		}

		private static void StartRouterTimer()
		{
			Util.RouterTimer = new System.Timers.Timer(30000.0);
			Util.RouterTimer.Elapsed += new ElapsedEventHandler(Util.RouterTimeout);
			Util.RouterTimer.AutoReset = false;
			Util.RouterTimer.Enabled = true;
		}

		private static void StopRouterTimer()
		{
			if (Util.RouterTimer != null)
			{
				Util.RouterTimer.Enabled = false;
				Util.RouterTimer.Dispose();
				Util.RouterTimer = null;
			}
		}

		private static void RouterTimeout(object sender, ElapsedEventArgs e)
		{
			Util.abortRouter = true;
		}

		private static void RouterConnected()
		{
			if (Util.sContext != null && Util.OnRouterConnected != null)
			{
				if (Util.sContext.InvokeRequired)
				{
					Util.sContext.Invoke(Util.OnRouterConnected);
					return;
				}
				Util.OnRouterConnected();
			}
		}

		public static void StartHostThread()
		{
			Util.checkControlReloadThread = new Thread(new ThreadStart(Util.HostThreadStart));
			Util.checkControlReloadThread.Start();
		}

		private static void HostThreadStart()
		{
			Util.StartHostTimer();
			Util.Host();
		}

		private static void Host()
		{
			bool isHostConnected = false;
			Util.abortHost = false;
			while (!isHostConnected)
			{
				if (Util.abortHost)
				{
					Util.StopHostTimer();
					Util.ControlConnectedError(-1002);
					return;
				}
				int status = UsbIPUtil.isHostConnected();
				if (status == -1001)
				{
					Util.ControlConnectedError(status);
					return;
				}
				if (status != -1002 && status == 0)
				{
					Util.HostConnected();
					Util.ReConnectControl();
					return;
				}
				Thread.Sleep(5000);
			}
		}

		private static void StartHostTimer()
		{
			Util.HostTimer = new System.Timers.Timer(40000.0);
			Util.HostTimer.Elapsed += new ElapsedEventHandler(Util.HostTimeout);
			Util.HostTimer.AutoReset = false;
			Util.HostTimer.Enabled = true;
		}

		private static void StopHostTimer()
		{
			if (Util.HostTimer != null)
			{
				Util.HostTimer.Enabled = false;
				Util.HostTimer.Dispose();
				Util.HostTimer = null;
			}
		}

		private static void HostTimeout(object sender, ElapsedEventArgs e)
		{
			Util.abortHost = true;
		}

		private static void HostConnected()
		{
			if (Util.sContext != null && Util.OnHostConnected != null)
			{
				if (Util.sContext.InvokeRequired)
				{
					Util.sContext.Invoke(Util.OnHostConnected);
					return;
				}
				Util.OnHostConnected();
			}
		}

		private static void ReConnectControl()
		{
			Thread.Sleep(5000);
			if (!UsbIPUtil.isCableLinked())
			{
				Util.ControlConnectedError(-3000);
				return;
			}
			int status = UsbIPUtil.ConnectControl();
			if (status == 0)
			{
				Util.ControlConnectedError(0);
				return;
			}
			if (status != -2000)
			{
				if (status == -1000)
				{
					Util.ControlConnectedError(UsbIPUtil.isUSBConnected());
				}
				return;
			}
			if (UsbIPUtil.RebootService())
			{
				Util.ReConnectControl();
				return;
			}
			Util.ControlConnectedError(-2000);
		}

		public static void ControlConnectedError(int error)
		{
			if (Util.sContext != null && Util.OnControlConnectedError != null)
			{
				if (Util.sContext.InvokeRequired)
				{
					Util.sContext.Invoke(Util.OnControlConnectedError, new object[]
					{
						error
					});
					return;
				}
				Util.OnControlConnectedError(error);
			}
		}

		public static void AbortCheckControlReloadThread()
		{
			Util.log.Trace("AboutCheckControlReloadThread");
			if (Util.checkControlReloadThread != null)
			{
				Util.checkControlReloadThread.Abort();
			}
		}

		public static void ConnectAnimateionPause()
		{
			if (Util.sContext != null && Util.OnConnectAnimateionPause != null)
			{
				if (Util.sContext.InvokeRequired)
				{
					Util.sContext.Invoke(Util.OnConnectAnimateionPause);
					return;
				}
				Util.OnConnectAnimateionPause();
			}
		}

		public static void ConnectAnimateionResume()
		{
			if (Util.sContext != null && Util.OnConnectAnimateionResume != null)
			{
				if (Util.sContext.InvokeRequired)
				{
					Util.sContext.Invoke(Util.OnConnectAnimateionResume);
					return;
				}
				Util.OnConnectAnimateionResume();
			}
		}

		public static void SwitchChannel()
		{
			Util.StartSwitchChannelTimer();
			Util.switchedChannel = "";
			Util.currentChannel = "";
			Util.ConnectAnimateionPause();
			new Thread(new ThreadStart(Util.getCurrentChannel)).Start();
		}

		private static void getCurrentChannel()
		{
			Util.log.Trace("getCurrentChannel");
			if (!ChannelUtil.pingRouterConnect())
			{
				Util.log.Trace("ping 144.1 fail ");
				Util.StopSwitchChannelTimer();
				Util.CheckRouterChannelFinish(false);
				return;
			}
			Util.currentChannel = ChannelUtil.getWirelessChannel();
			Util.log.Trace("current Channel = " + Util.currentChannel);
			if (Util.currentChannel != null)
			{
				ChannelUtil.switchWirelessChannel();
				Util.log.Trace("switchWirelessChannel");
				Util.checkIsChannelSwitched();
				return;
			}
			Util.log.Trace("other router or bad router");
			Util.StopSwitchChannelTimer();
			Util.CheckRouterChannelFinish(false);
		}

		private static void CheckRouterChannelFinish(bool isOurRouter)
		{
			if (Util.sContext != null && Util.OnCheckRouterChannelFinishListener != null)
			{
				if (Util.sContext.InvokeRequired)
				{
					Util.sContext.Invoke(Util.OnCheckRouterChannelFinishListener, new object[]
					{
						isOurRouter
					});
					return;
				}
				Util.OnCheckRouterChannelFinishListener(isOurRouter);
			}
		}

		private static void checkIsChannelSwitched()
		{
			bool isSwitched = false;
			Util.abortSwitchChannel = false;
			while (!isSwitched)
			{
				if (Util.abortSwitchChannel)
				{
					return;
				}
				Util.switchedChannel = ChannelUtil.getWirelessChannel();
				Util.log.Trace("switched channel = " + Util.switchedChannel);
				if (Util.switchedChannel != null && Util.switchedChannel != Util.currentChannel)
				{
					isSwitched = true;
					Util.StopSwitchChannelTimer();
					Util.ChannelSwitchedFinish();
				}
				Thread.Sleep(5000);
			}
		}

		private static void StartSwitchChannelTimer()
		{
			Util.SwitchChannelTimer = new System.Timers.Timer(60000.0);
			Util.SwitchChannelTimer.Elapsed += new ElapsedEventHandler(Util.SwitchChannelTimeout);
			Util.SwitchChannelTimer.AutoReset = false;
			Util.SwitchChannelTimer.Enabled = true;
		}

		private static void StopSwitchChannelTimer()
		{
			if (Util.SwitchChannelTimer != null)
			{
				Util.SwitchChannelTimer.Enabled = false;
				Util.SwitchChannelTimer.Dispose();
				Util.SwitchChannelTimer = null;
			}
		}

		private static void SwitchChannelTimeout(object sender, ElapsedEventArgs e)
		{
			Util.abortSwitchChannel = true;
			Util.StopSwitchChannelTimer();
			Util.ControlConnectedError(-1001);
		}

		private static void ChannelSwitchedFinish()
		{
			if (Util.sContext != null && Util.OnChannelSwitched != null)
			{
				if (Util.sContext.InvokeRequired)
				{
					Util.sContext.Invoke(Util.OnChannelSwitched);
					return;
				}
				Util.OnChannelSwitched();
			}
		}

		public static void CheckBluetooth()
		{
			if (Util.BeginCheckBluetooth != null)
			{
				Util.BeginCheckBluetooth();
			}
			Util.checkBluetoothThread = new Thread(new ThreadStart(Util.checkBluetoothThreadStart));
			Util.checkBluetoothThread.Start();
		}

		private static void checkBluetoothThreadStart()
		{
			Version currentVersion = Environment.OSVersion.Version;
			Version compareToVersion = new Version("6.2");
			Util.log.Trace(currentVersion.ToString());
			if (currentVersion.CompareTo(compareToVersion) >= 0)
			{
				Util.log.Trace("当前系统是WIN8及以上版本系统。");
				Util.CheckBluetoothWin8();
				return;
			}
			Util.log.Trace("当前系统不是WIN8及以上版本系统。");
			if (Util.isInstallCSR())
			{
				if (Util.sContext != null && Util.DeviceConnected != null)
				{
					if (Util.sContext.InvokeRequired)
					{
						Util.sContext.Invoke(Util.DeviceConnected);
						return;
					}
					Util.DeviceConnected();
					return;
				}
			}
			else if (Util.sContext != null && Util.NoBluetoothDriver != null)
			{
				if (Util.sContext.InvokeRequired)
				{
					Util.sContext.Invoke(Util.NoBluetoothDriver);
					return;
				}
				Util.NoBluetoothDriver();
			}
		}

		private static void CheckBluetoothWin8()
		{
			if (BluetoothRadio.PrimaryRadio == null)
			{
				if (Util.sContext != null && Util.NoBluetooth != null)
				{
					if (Util.sContext.InvokeRequired)
					{
						Util.sContext.Invoke(Util.NoBluetooth);
						return;
					}
					Util.NoBluetooth();
				}
				return;
			}
			Util.scanBluetoothThread = new Thread(new ThreadStart(Util.scan));
			Util.scanBluetoothThread.Start();
		}

		private static void scan()
		{
			if (Util.sContext != null)
			{
				BluetoothDeviceInfo[] bluetoothDevices = Util.getBluetoothDevices();
				for (int i = 0; i < bluetoothDevices.Length; i++)
				{
					BluetoothDeviceInfo device = bluetoothDevices[i];
					Util.log.Trace(string.Concat(new string[]
					{
						device.DeviceName,
						" 连接 ",
						device.Connected.ToString(),
						" 配对 ",
						device.Authenticated.ToString()
					}));
					if (device.DeviceName.StartsWith("TPCAST"))
					{
						if (device.Connected && device.Authenticated)
						{
							if (Util.DeviceConnected != null)
							{
								if (Util.sContext.InvokeRequired)
								{
									Util.sContext.Invoke(Util.DeviceConnected);
									return;
								}
								Util.DeviceConnected();
								return;
							}
						}
						else
						{
							if (!device.Connected && device.Authenticated)
							{
								new Thread(new ParameterizedThreadStart(Util.connectBluetoothStart)).Start(device);
								return;
							}
							new Thread(new ParameterizedThreadStart(Util.pairAndConnectBluetoothStart)).Start(device);
						}
						return;
					}
				}
				if (Util.DeviceNotConnected != null)
				{
					if (Util.sContext.InvokeRequired)
					{
						Util.sContext.Invoke(Util.DeviceNotConnected);
						return;
					}
					Util.DeviceNotConnected();
				}
			}
		}

		public static void connectBluetoothStart(object device)
		{
			try
			{
				if (device is BluetoothDeviceInfo)
				{
					Util.log.Trace("开始连接：" + ((BluetoothDeviceInfo)device).DeviceName);
					new BluetoothClient();
					if (BluetoothSecurity.RemoveDevice(((BluetoothDeviceInfo)device).DeviceAddress))
					{
						Util.pairAndConnectBluetoothStart(device);
					}
				}
			}
			catch
			{
				Util.log.Error("connect fail");
			}
		}

		public static void pairAndConnectBluetoothStart(object device)
		{
			try
			{
				if (device is BluetoothDeviceInfo)
				{
					Util.log.Trace("开始连接：" + ((BluetoothDeviceInfo)device).DeviceName);
					if (BluetoothSecurity.PairRequest(((BluetoothDeviceInfo)device).DeviceAddress, "0000"))
					{
						if (Util.DeviceConnected != null)
						{
							if (Util.sContext.InvokeRequired)
							{
								Util.sContext.Invoke(Util.DeviceConnected);
							}
							else
							{
								Util.DeviceConnected();
							}
						}
					}
					else if (Util.DeviceFoundNotConnected != null)
					{
						if (Util.sContext.InvokeRequired)
						{
							Util.sContext.Invoke(Util.DeviceFoundNotConnected);
						}
						else
						{
							Util.DeviceFoundNotConnected();
						}
					}
				}
			}
			catch
			{
				Util.log.Error("pair fail");
			}
		}

		public bool checkBluetoothRadio()
		{
			return BluetoothRadio.PrimaryRadio != null;
		}

		public static BluetoothDeviceInfo[] getBluetoothDevices()
		{
			BluetoothRadio arg_0C_0 = BluetoothRadio.PrimaryRadio;
			BluetoothClient Blueclient = new BluetoothClient();
			arg_0C_0.Mode = RadioMode.PowerOff;
			return Blueclient.DiscoverDevices();
		}

		public BluetoothDeviceInfo[] getAuthenticatedBluetoothDevices()
		{
			BluetoothDeviceInfo[] arg_0B_0 = Util.getBluetoothDevices();
			List<BluetoothDeviceInfo> authenticated = new List<BluetoothDeviceInfo>();
			BluetoothDeviceInfo[] array = arg_0B_0;
			for (int i = 0; i < array.Length; i++)
			{
				BluetoothDeviceInfo device = array[i];
				if (device.Authenticated)
				{
					authenticated.Add(device);
				}
			}
			if (authenticated.Count > 0)
			{
				return authenticated.ToArray();
			}
			return null;
		}

		public BluetoothDeviceInfo[] getConnectedBluetoothDevices()
		{
			BluetoothDeviceInfo[] arg_0B_0 = Util.getBluetoothDevices();
			List<BluetoothDeviceInfo> connected = new List<BluetoothDeviceInfo>();
			BluetoothDeviceInfo[] array = arg_0B_0;
			for (int i = 0; i < array.Length; i++)
			{
				BluetoothDeviceInfo device = array[i];
				if (device.Authenticated)
				{
					connected.Add(device);
				}
			}
			if (connected.Count > 0)
			{
				return connected.ToArray();
			}
			return null;
		}

		public static void AbortCheckBluetoothThread()
		{
			Util.log.Trace("AbortCheckBluetoothThread");
			if (Util.checkBluetoothThread != null)
			{
				Util.checkBluetoothThread.Abort();
			}
		}

		public static void AbortScanBluetoothThread()
		{
			Util.log.Trace("AbortScanBluetoothThread");
			if (Util.scanBluetoothThread != null)
			{
				Util.scanBluetoothThread.Abort();
			}
		}

		public static void CheckSteam()
		{
			if (Util.BeginCheckSteam != null)
			{
				Util.BeginCheckSteam();
			}
			if (Util.isViveInstalled() && Util.isViveSteamVRInstalled())
			{
				if (Util.OnCheckSteamFinish != null)
				{
					Util.OnCheckSteamFinish(true);
					return;
				}
			}
			else if (!string.IsNullOrEmpty(Util.searchSteamPath()))
			{
				if (Util.OnCheckSteamFinish != null)
				{
					Util.OnCheckSteamFinish(true);
					return;
				}
			}
			else if (Util.OnCheckSteamFinish != null)
			{
				Util.OnCheckSteamFinish(false);
			}
		}

		public static string searchSteamPath()
		{
			using (RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall", false))
			{
				if (key != null)
				{
					using (RegistryKey key2 = key.OpenSubKey("Steam", false))
					{
						if (key2 != null)
						{
							string uninstallString = key2.GetValue("UninstallString", "").ToString();
							string installLocation = uninstallString.Substring(0, uninstallString.LastIndexOf("\\")) + "\\Steam.exe";
							string result;
							if (File.Exists(installLocation))
							{
								result = installLocation;
								return result;
							}
							result = null;
							return result;
						}
					}
				}
			}
			return null;
		}

		public static bool isSteamVRRunning()
		{
			IntPtr handle = Util.CreateToolhelp32Snapshot(2u, 0u);
			List<ProcessEntry32> list = new List<ProcessEntry32>();
			if ((int)handle > 0)
			{
				ProcessEntry32 pe32 = default(ProcessEntry32);
				pe32.dwSize = (uint)Marshal.SizeOf(pe32);
				for (int bMore = Util.Process32First(handle, ref pe32); bMore == 1; bMore = Util.Process32Next(handle, ref pe32))
				{
					IntPtr temp = Marshal.AllocHGlobal((int)pe32.dwSize);
					Marshal.StructureToPtr(pe32, temp, true);
					ProcessEntry32 pe33 = (ProcessEntry32)Marshal.PtrToStructure(temp, typeof(ProcessEntry32));
					Marshal.FreeHGlobal(temp);
					list.Add(pe33);
				}
				Util.CloseHandle(handle);
				foreach (ProcessEntry32 p in list)
				{
					Util.log.Trace(p.szExeFile);
					if ("vrmonitor.exe".Equals(p.szExeFile))
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		[DllImport("KERNEL32.DLL ")]
		public static extern IntPtr CreateToolhelp32Snapshot(uint flags, uint processid);

		[DllImport("KERNEL32.DLL ")]
		public static extern int CloseHandle(IntPtr handle);

		[DllImport("KERNEL32.DLL ")]
		public static extern int Process32First(IntPtr handle, ref ProcessEntry32 pe);

		[DllImport("KERNEL32.DLL ")]
		public static extern int Process32Next(IntPtr handle, ref ProcessEntry32 pe);

		public static void LaunchSteam()
		{
			if (Util.isViveInstalled() && Util.isViveSteamVRInstalled())
			{
				new Thread(new ThreadStart(Util.lauchViveSteamVR)).Start();
				return;
			}
			if (!string.IsNullOrEmpty(Util.searchSteamPath()))
			{
				new Thread(new ThreadStart(Util.lauchSteamVR)).Start();
			}
		}

		public static void lauchSteamVR()
		{
			if (!Util.isSteamVRRunning())
			{
				Process.Start("steam://rungameid/250820");
			}
		}

		public static void lauchViveSteamVR()
		{
			Util.LaunchViveSteamVR();
		}

		public static bool isInstallCSR()
		{
			using (RegistryKey localMachine64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
			{
				using (RegistryKey key = localMachine64.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall"))
				{
					if (key != null)
					{
						string[] subKeyNames = key.GetSubKeyNames();
						for (int i = 0; i < subKeyNames.Length; i++)
						{
							string name = subKeyNames[i];
							using (RegistryKey key2 = key.OpenSubKey(name, false))
							{
								if (key2 != null)
								{
									string displayName = key2.GetValue("DisplayName", "").ToString();
									if ("CSR Harmony Wireless Software Stack".Equals(displayName))
									{
										return true;
									}
								}
							}
						}
					}
				}
			}
			return false;
		}

		public static void LaunchVive()
		{
			using (RegistryKey localMachine64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
			{
				using (RegistryKey key = localMachine64.OpenSubKey("SOFTWARE\\Wow6432Node\\HtcVive\\PCClient"))
				{
					if (key != null)
					{
						string launchPath = key.GetValue("LaunchPath", "").ToString();
						if (!string.IsNullOrEmpty(launchPath))
						{
							Process.Start(launchPath);
						}
					}
				}
			}
		}

		public static bool isViveInstalled()
		{
			using (RegistryKey localMachine64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
			{
				using (RegistryKey key = localMachine64.OpenSubKey("SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\VIVE Software"))
				{
					if (key != null)
					{
						string displayName = key.GetValue("DisplayName", "").ToString();
						if ("VIVE Software".Equals(displayName))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public static bool isViveSteamVRInstalled()
		{
			using (RegistryKey localMachine64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
			{
				using (RegistryKey key = localMachine64.OpenSubKey("SOFTWARE\\Wow6432Node\\HtcVive\\SteamVR"))
				{
					if (key != null && !string.IsNullOrEmpty(key.GetValue("LaunchPath", "").ToString()))
					{
						return true;
					}
				}
			}
			return false;
		}

		public static void LaunchViveSteamVR()
		{
			using (RegistryKey localMachine64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
			{
				using (RegistryKey key = localMachine64.OpenSubKey("SOFTWARE\\Wow6432Node\\HtcVive\\SteamVR"))
				{
					if (key != null)
					{
						string launchPath = key.GetValue("LaunchPath", "").ToString();
						if (!string.IsNullOrEmpty(launchPath))
						{
							Process.Start(launchPath);
						}
					}
				}
			}
		}

		public static void showGrayBackground()
		{
			if (Util.showCount == 0 && Util.sContext != null && !Util.sContext.IsDisposed)
			{
				Util.log.Trace("form state = " + Util.sContext.WindowState);
				Util.sContext.Show();
				Util.sContext.ShowInTaskbar = true;
				Util.sContext.WindowState = FormWindowState.Normal;
				Util.grayForm.Location = Util.sContext.Location;
				Util.grayForm.ShowInTaskbar = false;
				Util.grayForm.StartPosition = FormStartPosition.Manual;
				Util.grayForm.Show(Util.sContext);
			}
			Util.showCount++;
			Util.closeContextMenuStrip();
		}

		public static void HideGrayBackground()
		{
			Util.showCount--;
			if (Util.showCount == 0)
			{
				Util.grayForm.Hide();
			}
		}

		public static void showGuideForm()
		{
			if (Util.sContext != null && !Util.sContext.IsDisposed)
			{
				Util.log.Trace("form state = " + Util.sContext.WindowState);
				Util.sContext.Show();
				Util.sContext.ShowInTaskbar = true;
				Util.sContext.WindowState = FormWindowState.Normal;
				Util.guideForm.Location = Util.sContext.Location;
				Util.guideForm.ShowInTaskbar = false;
				Util.guideForm.StartPosition = FormStartPosition.Manual;
				Util.guideForm.Show(Util.sContext);
			}
		}

		public static void hideGuideForm()
		{
			Util.guideForm.Close();
		}

		public static void miniGrayBackground()
		{
			Util.grayForm.WindowState = FormWindowState.Minimized;
		}

		public static void normalGrayBackground()
		{
			Util.grayForm.WindowState = FormWindowState.Normal;
		}

		private static void closeContextMenuStrip()
		{
			if (Util.sContext != null && Util.sContext is BaseForm)
			{
				((BaseForm)Util.sContext).closeContextMenuStrip();
			}
		}

		public static void suicide()
		{
			IntPtr handle = Util.CreateToolhelp32Snapshot(2u, 0u);
			List<ProcessEntry32> list = new List<ProcessEntry32>();
			if ((int)handle > 0)
			{
				ProcessEntry32 pe32 = default(ProcessEntry32);
				pe32.dwSize = (uint)Marshal.SizeOf(pe32);
				for (int bMore = Util.Process32First(handle, ref pe32); bMore == 1; bMore = Util.Process32Next(handle, ref pe32))
				{
					IntPtr temp = Marshal.AllocHGlobal((int)pe32.dwSize);
					Marshal.StructureToPtr(pe32, temp, true);
					ProcessEntry32 pe33 = (ProcessEntry32)Marshal.PtrToStructure(temp, typeof(ProcessEntry32));
					Marshal.FreeHGlobal(temp);
					list.Add(pe33);
				}
				Util.CloseHandle(handle);
				foreach (ProcessEntry32 p in list)
				{
					Util.log.Trace(p.szExeFile);
					if ("TPCASTWindows.exe".Equals(p.szExeFile))
					{
						try
						{
							Process.GetProcessById((int)p.th32ProcessID).Kill();
						}
						catch (Exception e)
						{
							Util.log.Error("suicide" + e.Message + "\r\n" + e.StackTrace);
						}
					}
				}
			}
		}
	}
}
