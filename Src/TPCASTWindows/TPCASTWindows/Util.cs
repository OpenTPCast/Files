using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using Microsoft.Win32;
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

namespace TPCASTWindows
{
	internal class Util
	{
		public delegate void OnCheckControlFinishDelegate(bool isLoaded);

		public delegate void BeginCheckControlDelegate();

		public delegate void OnCheckRouterConnectedDelegate();

		public delegate void OnCheckHostConnectDelegate();

		public delegate void OnCheckControlErrorDelegate(int status);

		public delegate void OnControlInterruptDelegate(int status);

		public delegate void OnHostConnectedDelegate();

		public delegate void OnRouterConnectedDelegate();

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

		public static Form sContext;

		public static Util.OnCheckRouterFinishDelegate OnCheckRouterFinishListener;

		public static Util.OnCheckControlFinishDelegate OnCheckControlFinishListener;

		public static Util.BeginCheckControlDelegate BeginCheckControl;

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

		public static Util.OnHostConnectedDelegate OnHostConnected;

		public static Util.OnRouterConnectedDelegate OnRouterConnected;

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

		private static GrayForm grayForm = new GrayForm();

		private static int showCount = 0;

		public static void Init(Form context)
		{
			Util.sContext = context;
		}

		public static void UnInit()
		{
			Util.sContext = null;
			Util.AbortRouterThread();
			Util.AbortCheckBluetoothThread();
			Util.AbortScanBluetoothThread();
			Util.AbortBackgroundCheckControlThread();
			Util.AbortCheckControlReloadThread();
		}

		public List<IPAddress> GetGatewayAddresses()
		{
			List<IPAddress> list = new List<IPAddress>();
			NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
			Console.WriteLine("适配器个数：" + allNetworkInterfaces.Length);
			Console.WriteLine();
			NetworkInterface[] array = allNetworkInterfaces;
			for (int i = 0; i < array.Length; i++)
			{
				NetworkInterface networkInterface = array[i];
				Console.WriteLine("描述：" + networkInterface.Description);
				Console.WriteLine("标识符：" + networkInterface.Id);
				Console.WriteLine("名称：" + networkInterface.Name);
				Console.WriteLine("类型：" + networkInterface.NetworkInterfaceType);
				Console.WriteLine("速度：" + (double)networkInterface.Speed * 0.001 * 0.001 + "M");
				Console.WriteLine("操作状态：" + networkInterface.OperationalStatus);
				Console.WriteLine("MAC 地址：" + networkInterface.GetPhysicalAddress());
				IPInterfaceProperties iPProperties = networkInterface.GetIPProperties();
				if (iPProperties.GatewayAddresses.Count > 0)
				{
					Console.WriteLine("默认网关：" + iPProperties.GatewayAddresses[0].Address);
					list.Add(iPProperties.GatewayAddresses[0].Address);
				}
			}
			return list;
		}

		public void CheckNetwork()
		{
			foreach (IPAddress current in this.GetGatewayAddresses())
			{
				Console.WriteLine("测试网关：" + current);
				new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
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
			bool flag = false;
			Util.abortCheckRouter = false;
			while (!flag)
			{
				if (Util.abortCheckRouter)
				{
					Util.StopCheckRouterTimer();
					Util.CheckControlError(-1001);
					return;
				}
				if (ChannelUtil.getWirelessChannel() != null)
				{
					Util.CheckRouterConnected();
					Util.CheckHostThreadStart();
					return;
				}
				Thread.Sleep(5000);
			}
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
			bool flag = false;
			Util.abortCheckHost = false;
			while (!flag)
			{
				if (Util.abortCheckHost)
				{
					Util.StopCheckHostTimer();
					Util.CheckControlError(-1002);
					return;
				}
				int num = UsbIPUtil.isHostConnected();
				if (num == -1001)
				{
					Util.CheckControlError(num);
					return;
				}
				if (num != -1002 && num == 0)
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
			int num = UsbIPUtil.ForceConnectControl();
			if (num == 0)
			{
				Util.CheckControlError(0);
				return;
			}
			if (num != -2000)
			{
				if (num == -1000)
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
			Console.WriteLine("AbortRouterThread");
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
			int num = 0;
			while (Util.isLinked)
			{
				num = UsbIPUtil.isUsbLinked();
				if (num == 0)
				{
					Util.isLinked = true;
				}
				else if (num == -1000 || num == -2000)
				{
					Util.isLinked = false;
					break;
				}
				Thread.Sleep(5000);
			}
			if (num == -1000)
			{
				if (ChannelUtil.getWirelessChannel() != null)
				{
					if (UsbIPUtil.isHostLinked())
					{
						if (UsbIPUtil.isCableLinked())
						{
							num = -1000;
						}
						else
						{
							num = -3000;
						}
					}
					else
					{
						num = -1002;
					}
				}
				else
				{
					num = -1001;
				}
			}
			else if (num == -2000)
			{
				UsbIPUtil.RebootService();
				num = -1000;
			}
			Util.ConnectAnimateionPause();
			Util.ControlInterrupt(num);
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
			Console.WriteLine("AboutBackgroundCheckControlThread");
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
			bool flag = false;
			Util.abortRouter = false;
			while (!flag)
			{
				if (Util.abortRouter)
				{
					Util.StopRouterTimer();
					Util.ControlConnectedError(-1001);
					return;
				}
				if (ChannelUtil.getWirelessChannel() != null)
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
			bool flag = false;
			Util.abortHost = false;
			while (!flag)
			{
				if (Util.abortHost)
				{
					Util.StopHostTimer();
					Util.ControlConnectedError(-1002);
					return;
				}
				int num = UsbIPUtil.isHostConnected();
				if (num == -1001)
				{
					Util.ControlConnectedError(num);
					return;
				}
				if (num != -1002 && num == 0)
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
			int num = UsbIPUtil.ConnectControl();
			if (num == 0)
			{
				Util.ControlConnectedError(0);
				return;
			}
			if (num != -2000)
			{
				if (num == -1000)
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
			Console.WriteLine("AboutCheckControlReloadThread");
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
			Util.currentChannel = ChannelUtil.getWirelessChannel();
			Console.WriteLine("current Channel = " + Util.currentChannel);
			if (Util.currentChannel != null)
			{
				ChannelUtil.switchWirelessChannel();
				Console.WriteLine("switchWirelessChannel");
				Util.checkIsChannelSwitched();
				return;
			}
			Console.WriteLine("else");
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
			bool flag = false;
			Util.abortSwitchChannel = false;
			while (!flag)
			{
				if (Util.abortSwitchChannel)
				{
					return;
				}
				Util.switchedChannel = ChannelUtil.getWirelessChannel();
				if (Util.switchedChannel != null && Util.switchedChannel != Util.currentChannel)
				{
					flag = true;
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
			Version arg_15_0 = Environment.OSVersion.Version;
			Version value = new Version("6.2");
			Console.WriteLine(arg_15_0);
			if (arg_15_0.CompareTo(value) >= 0)
			{
				Console.WriteLine("当前系统是WIN8及以上版本系统。");
				Util.CheckBluetoothWin8();
				return;
			}
			Console.WriteLine("当前系统不是WIN8及以上版本系统。");
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
					BluetoothDeviceInfo bluetoothDeviceInfo = bluetoothDevices[i];
					Console.WriteLine(string.Concat(new string[]
					{
						bluetoothDeviceInfo.DeviceName,
						" 连接 ",
						bluetoothDeviceInfo.Connected.ToString(),
						" 配对 ",
						bluetoothDeviceInfo.Authenticated.ToString()
					}));
					if (bluetoothDeviceInfo.DeviceName.StartsWith("TPCAST"))
					{
						if (bluetoothDeviceInfo.Connected && bluetoothDeviceInfo.Authenticated)
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
							if (!bluetoothDeviceInfo.Connected && bluetoothDeviceInfo.Authenticated)
							{
								new Thread(new ParameterizedThreadStart(Util.connectBluetoothStart)).Start(bluetoothDeviceInfo);
								return;
							}
							new Thread(new ParameterizedThreadStart(Util.pairAndConnectBluetoothStart)).Start(bluetoothDeviceInfo);
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
					Console.WriteLine("开始连接：" + ((BluetoothDeviceInfo)device).DeviceName);
					new BluetoothClient();
					if (BluetoothSecurity.RemoveDevice(((BluetoothDeviceInfo)device).DeviceAddress))
					{
						Util.pairAndConnectBluetoothStart(device);
					}
				}
			}
			catch
			{
				Console.WriteLine("connect fail");
			}
		}

		public static void pairAndConnectBluetoothStart(object device)
		{
			try
			{
				if (device is BluetoothDeviceInfo)
				{
					Console.WriteLine("开始连接：" + ((BluetoothDeviceInfo)device).DeviceName);
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
				Console.WriteLine("pair fail");
			}
		}

		public bool checkBluetoothRadio()
		{
			return BluetoothRadio.PrimaryRadio != null;
		}

		public static BluetoothDeviceInfo[] getBluetoothDevices()
		{
			BluetoothRadio arg_0C_0 = BluetoothRadio.PrimaryRadio;
			BluetoothClient bluetoothClient = new BluetoothClient();
			arg_0C_0.Mode = RadioMode.PowerOff;
			return bluetoothClient.DiscoverDevices();
		}

		public BluetoothDeviceInfo[] getAuthenticatedBluetoothDevices()
		{
			BluetoothDeviceInfo[] arg_0B_0 = Util.getBluetoothDevices();
			List<BluetoothDeviceInfo> list = new List<BluetoothDeviceInfo>();
			BluetoothDeviceInfo[] array = arg_0B_0;
			for (int i = 0; i < array.Length; i++)
			{
				BluetoothDeviceInfo bluetoothDeviceInfo = array[i];
				if (bluetoothDeviceInfo.Authenticated)
				{
					list.Add(bluetoothDeviceInfo);
				}
			}
			if (list.Count > 0)
			{
				return list.ToArray();
			}
			return null;
		}

		public BluetoothDeviceInfo[] getConnectedBluetoothDevices()
		{
			BluetoothDeviceInfo[] arg_0B_0 = Util.getBluetoothDevices();
			List<BluetoothDeviceInfo> list = new List<BluetoothDeviceInfo>();
			BluetoothDeviceInfo[] array = arg_0B_0;
			for (int i = 0; i < array.Length; i++)
			{
				BluetoothDeviceInfo bluetoothDeviceInfo = array[i];
				if (bluetoothDeviceInfo.Authenticated)
				{
					list.Add(bluetoothDeviceInfo);
				}
			}
			if (list.Count > 0)
			{
				return list.ToArray();
			}
			return null;
		}

		public static void AbortCheckBluetoothThread()
		{
			Console.WriteLine("AbortCheckBluetoothThread");
			if (Util.checkBluetoothThread != null)
			{
				Util.checkBluetoothThread.Abort();
			}
		}

		public static void AbortScanBluetoothThread()
		{
			Console.WriteLine("AbortScanBluetoothThread");
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
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall", false))
			{
				if (registryKey != null)
				{
					using (RegistryKey registryKey2 = registryKey.OpenSubKey("Steam", false))
					{
						if (registryKey2 != null)
						{
							string text = registryKey2.GetValue("UninstallString", "").ToString();
							string text2 = text.Substring(0, text.LastIndexOf("\\")) + "\\Steam.exe";
							string result;
							if (File.Exists(text2))
							{
								result = text2;
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
			IntPtr intPtr = Util.CreateToolhelp32Snapshot(2u, 0u);
			List<ProcessEntry32> list = new List<ProcessEntry32>();
			if ((int)intPtr > 0)
			{
				ProcessEntry32 processEntry = default(ProcessEntry32);
				processEntry.dwSize = (uint)Marshal.SizeOf(processEntry);
				for (int num = Util.Process32First(intPtr, ref processEntry); num == 1; num = Util.Process32Next(intPtr, ref processEntry))
				{
					IntPtr intPtr2 = Marshal.AllocHGlobal((int)processEntry.dwSize);
					Marshal.StructureToPtr(processEntry, intPtr2, true);
					ProcessEntry32 item = (ProcessEntry32)Marshal.PtrToStructure(intPtr2, typeof(ProcessEntry32));
					Marshal.FreeHGlobal(intPtr2);
					list.Add(item);
				}
				Util.CloseHandle(intPtr);
				foreach (ProcessEntry32 current in list)
				{
					Console.WriteLine(current.szExeFile);
					if ("vrmonitor.exe".Equals(current.szExeFile))
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
			using (RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
			{
				using (RegistryKey registryKey2 = registryKey.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall"))
				{
					if (registryKey2 != null)
					{
						string[] subKeyNames = registryKey2.GetSubKeyNames();
						for (int i = 0; i < subKeyNames.Length; i++)
						{
							string name = subKeyNames[i];
							using (RegistryKey registryKey3 = registryKey2.OpenSubKey(name, false))
							{
								if (registryKey3 != null)
								{
									string value = registryKey3.GetValue("DisplayName", "").ToString();
									if ("CSR Harmony Wireless Software Stack".Equals(value))
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
			using (RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
			{
				using (RegistryKey registryKey2 = registryKey.OpenSubKey("SOFTWARE\\Wow6432Node\\HtcVive\\PCClient"))
				{
					if (registryKey2 != null)
					{
						string text = registryKey2.GetValue("LaunchPath", "").ToString();
						if (!string.IsNullOrEmpty(text))
						{
							Process.Start(text);
						}
					}
				}
			}
		}

		public static bool isViveInstalled()
		{
			using (RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
			{
				using (RegistryKey registryKey2 = registryKey.OpenSubKey("SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\VIVE Software"))
				{
					if (registryKey2 != null)
					{
						string value = registryKey2.GetValue("DisplayName", "").ToString();
						if ("VIVE Software".Equals(value))
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
			using (RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
			{
				using (RegistryKey registryKey2 = registryKey.OpenSubKey("SOFTWARE\\Wow6432Node\\HtcVive\\SteamVR"))
				{
					if (registryKey2 != null && !string.IsNullOrEmpty(registryKey2.GetValue("LaunchPath", "").ToString()))
					{
						return true;
					}
				}
			}
			return false;
		}

		public static void LaunchViveSteamVR()
		{
			using (RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
			{
				using (RegistryKey registryKey2 = registryKey.OpenSubKey("SOFTWARE\\Wow6432Node\\HtcVive\\SteamVR"))
				{
					if (registryKey2 != null)
					{
						string text = registryKey2.GetValue("LaunchPath", "").ToString();
						if (!string.IsNullOrEmpty(text))
						{
							Process.Start(text);
						}
					}
				}
			}
		}

		public static void showGrayBackground()
		{
			if (Util.showCount == 0 && Util.sContext != null && !Util.sContext.IsDisposed)
			{
				Util.sContext.Show();
				Util.sContext.ShowInTaskbar = true;
				Util.sContext.WindowState = FormWindowState.Normal;
				Util.grayForm.Location = Util.sContext.Location;
				Util.grayForm.ShowInTaskbar = false;
				Util.grayForm.StartPosition = FormStartPosition.Manual;
				Util.grayForm.Show(Util.sContext);
			}
			Util.showCount++;
		}

		public static void HideGrayBackground()
		{
			Util.showCount--;
			if (Util.showCount == 0)
			{
				Util.grayForm.Hide();
			}
		}

		public static void miniGrayBackground()
		{
			Util.grayForm.WindowState = FormWindowState.Minimized;
		}

		public static void normalGrayBackground()
		{
			Util.grayForm.WindowState = FormWindowState.Normal;
		}

		public static void suicide()
		{
			IntPtr intPtr = Util.CreateToolhelp32Snapshot(2u, 0u);
			List<ProcessEntry32> list = new List<ProcessEntry32>();
			if ((int)intPtr > 0)
			{
				ProcessEntry32 processEntry = default(ProcessEntry32);
				processEntry.dwSize = (uint)Marshal.SizeOf(processEntry);
				for (int num = Util.Process32First(intPtr, ref processEntry); num == 1; num = Util.Process32Next(intPtr, ref processEntry))
				{
					IntPtr intPtr2 = Marshal.AllocHGlobal((int)processEntry.dwSize);
					Marshal.StructureToPtr(processEntry, intPtr2, true);
					ProcessEntry32 item = (ProcessEntry32)Marshal.PtrToStructure(intPtr2, typeof(ProcessEntry32));
					Marshal.FreeHGlobal(intPtr2);
					list.Add(item);
				}
				Util.CloseHandle(intPtr);
				foreach (ProcessEntry32 current in list)
				{
					Console.WriteLine(current.szExeFile);
					if ("TPCASTWindows.exe".Equals(current.szExeFile))
					{
						try
						{
							Process.GetProcessById((int)current.th32ProcessID).Kill();
						}
						catch (Exception arg_DA_0)
						{
							Console.WriteLine(arg_DA_0.Message);
						}
					}
				}
			}
		}
	}
}
