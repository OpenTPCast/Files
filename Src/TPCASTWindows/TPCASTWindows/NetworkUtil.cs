using NLog;
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

		private static Logger log = LogManager.GetCurrentClassLogger();

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
			int flag = 0;
			bool expr_05 = NetworkUtil.InternetGetConnectedState(out flag, 0);
			NetworkUtil.log.Trace("net state = " + flag);
			if ((flag & 64) != 0)
			{
				NetworkUtil.log.Trace("configured");
			}
			else
			{
				NetworkUtil.log.Trace("configured no");
			}
			if ((flag & 2) != 0)
			{
				NetworkUtil.log.Trace("lan");
			}
			if ((flag & 1) != 0)
			{
				NetworkUtil.log.Trace("modem");
			}
			if ((flag & 8) != 0)
			{
				NetworkUtil.log.Trace("modem busy");
			}
			if ((flag & 32) != 0)
			{
				NetworkUtil.log.Trace("offline");
			}
			if ((flag & 4) != 0)
			{
				NetworkUtil.log.Trace("proxy");
			}
			if ((flag & 16) != 0)
			{
				NetworkUtil.log.Trace("ras");
				return expr_05;
			}
			NetworkUtil.log.Trace("ras no");
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
				NetworkUtil.OnNetworkConnectedDelegate OnNetworkConnected = (NetworkUtil.OnNetworkConnectedDelegate)obj;
				try
				{
					if (new Ping().Send("www.baidu.com", 1000).Status == IPStatus.Success)
					{
						if (NetworkUtil.sContext != null && OnNetworkConnected != null)
						{
							if (NetworkUtil.sContext.InvokeRequired)
							{
								NetworkUtil.sContext.Invoke(OnNetworkConnected, new object[]
								{
									true
								});
							}
							else
							{
								OnNetworkConnected(true);
							}
						}
						return;
					}
				}
				catch (Exception e)
				{
					NetworkUtil.log.Error("pingThreadStart = " + e.Message + "\r\n" + e.StackTrace);
				}
				if (NetworkUtil.sContext != null && OnNetworkConnected != null)
				{
					if (NetworkUtil.sContext.InvokeRequired)
					{
						NetworkUtil.sContext.Invoke(OnNetworkConnected, new object[]
						{
							false
						});
						return;
					}
					OnNetworkConnected(false);
				}
			}
		}
	}
}
