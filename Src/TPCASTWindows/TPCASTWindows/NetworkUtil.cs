using System;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace TPCASTWindows
{
	internal class NetworkUtil
	{
		public delegate void OnNetworkConnectedDelegate(bool connected);

		public static Control sContext;

		public const int INTERNET_CONNECTION_CONFIGURED = 64;

		public const int INTERNET_CONNECTION_LAN = 2;

		public const int INTERNET_CONNECTION_MODEM = 1;

		public const int INTERNET_CONNECTION_MODEM_BUSY = 8;

		public const int INTERNET_CONNECTION_OFFLINE = 32;

		public const int INTERNET_CONNECTION_PROXY = 4;

		public const int INTERNET_RAS_INSTALLED = 16;

		public static void Init(Control context)
		{
			NetworkUtil.sContext = context;
		}

		public static void isNetworkConnected(NetworkUtil.OnNetworkConnectedDelegate OnNetworkConnected)
		{
			if (NetworkUtil.isLanNetworkConnected())
			{
				NetworkUtil.ping(OnNetworkConnected);
				return;
			}
			OnNetworkConnected(false);
		}

		[DllImport("wininet.dll")]
		private static extern bool InternetGetConnectedState(out int connectionDescription, int reservedValue);

		public static bool isLanNetworkConnected()
		{
			int num = 0;
			bool expr_05 = NetworkUtil.InternetGetConnectedState(out num, 0);
			Console.WriteLine("net state = " + num);
			if ((num & 64) != 0)
			{
				Console.WriteLine("configured");
			}
			else
			{
				Console.WriteLine("configured no");
			}
			if ((num & 2) != 0)
			{
				Console.WriteLine("lan");
			}
			if ((num & 1) != 0)
			{
				Console.WriteLine("modem");
			}
			if ((num & 8) != 0)
			{
				Console.WriteLine("modem busy");
			}
			if ((num & 32) != 0)
			{
				Console.WriteLine("offline");
			}
			if ((num & 4) != 0)
			{
				Console.WriteLine("proxy");
			}
			if ((num & 16) != 0)
			{
				Console.WriteLine("ras");
				return expr_05;
			}
			Console.WriteLine("ras no");
			return expr_05;
		}

		private static void ping(NetworkUtil.OnNetworkConnectedDelegate OnNetworkConnected)
		{
			new Thread(new ParameterizedThreadStart(NetworkUtil.pingThreadStart)).Start(OnNetworkConnected);
		}

		private static void pingThreadStart(object obj)
		{
			if (obj is NetworkUtil.OnNetworkConnectedDelegate)
			{
				NetworkUtil.OnNetworkConnectedDelegate onNetworkConnectedDelegate = (NetworkUtil.OnNetworkConnectedDelegate)obj;
				try
				{
					if (new Ping().Send("www.baidu.com", 1000).Status == IPStatus.Success)
					{
						if (NetworkUtil.sContext != null && onNetworkConnectedDelegate != null)
						{
							if (NetworkUtil.sContext.InvokeRequired)
							{
								NetworkUtil.sContext.Invoke(onNetworkConnectedDelegate, new object[]
								{
									true
								});
							}
							else
							{
								onNetworkConnectedDelegate(true);
							}
						}
						return;
					}
				}
				catch (Exception arg)
				{
					Console.WriteLine("e = " + arg);
				}
				if (NetworkUtil.sContext != null && onNetworkConnectedDelegate != null)
				{
					if (NetworkUtil.sContext.InvokeRequired)
					{
						NetworkUtil.sContext.Invoke(onNetworkConnectedDelegate, new object[]
						{
							false
						});
						return;
					}
					onNetworkConnectedDelegate(false);
				}
			}
		}
	}
}
