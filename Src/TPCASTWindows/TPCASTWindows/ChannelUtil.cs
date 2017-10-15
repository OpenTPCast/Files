using NLog;
using Renci.SshNet;
using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

namespace TPCASTWindows
{
	internal class ChannelUtil
	{
		private static Logger log = LogManager.GetCurrentClassLogger();

		private const string ROUTER_IP = "192.168.144.1";

		private const string NAME = "tproot";

		private const string PASSWORD = "8427531";

		public static bool pingRouterConnect()
		{
			ChannelUtil.log.Trace("pingRouterConnect");
			try
			{
				if (new Ping().Send("192.168.144.1", 1000).Status == IPStatus.Success)
				{
					return true;
				}
			}
			catch (Exception e)
			{
				ChannelUtil.log.Error("error = " + e.Message + "\r\n" + e.StackTrace);
			}
			return false;
		}

		public static bool isRouterConnect()
		{
			IPAddress[] ips = Dns.GetHostAddresses(Dns.GetHostName());
			if (ips != null)
			{
				IPAddress[] array = ips;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].ToString().StartsWith("192.168.144"))
					{
						return true;
					}
				}
			}
			return false;
		}

		private static ConnectionInfo getDefaultConnectInfo()
		{
			return new PasswordConnectionInfo("192.168.144.1", "tproot", "8427531")
			{
				Timeout = TimeSpan.FromSeconds(1.0)
			};
		}

		public static string getWirelessChannel()
		{
			using (SshClient client = new SshClient(ChannelUtil.getDefaultConnectInfo()))
			{
				try
				{
					client.Connect();
					SshCommand expr_1C = client.CreateCommand("user_tool show wl5g_info");
					expr_1C.CommandTimeout = TimeSpan.FromSeconds(1.0);
					string arg_4A_0 = expr_1C.Execute();
					client.Disconnect();
					string[] outputs = arg_4A_0.Split(new string[]
					{
						"\n"
					}, StringSplitOptions.None);
					if (outputs != null)
					{
						string[] array = outputs;
						for (int i = 0; i < array.Length; i++)
						{
							string entry = array[i];
							ChannelUtil.log.Trace("entry = " + entry);
							if (entry.StartsWith("Channel"))
							{
								string[] channnelInfo = entry.Split(new string[]
								{
									":"
								}, StringSplitOptions.None);
								if (channnelInfo != null && channnelInfo.Count<string>() > 1)
								{
									return channnelInfo[1];
								}
							}
						}
					}
				}
				catch (Exception e)
				{
					ChannelUtil.log.Error("getWirelessChannel " + e.Message + "\r\n" + e.StackTrace);
				}
			}
			return null;
		}

		public static void setWirelessChannel(string channel)
		{
			ChannelUtil.log.Trace("setWirelessChannel");
			using (SshClient client = new SshClient(ChannelUtil.getDefaultConnectInfo()))
			{
				try
				{
					client.Connect();
					SshCommand expr_31 = client.CreateCommand("user_tool wl5g_set Channel " + channel);
					expr_31.CommandTimeout = TimeSpan.FromSeconds(1.0);
					expr_31.Execute();
					client.Disconnect();
				}
				catch (Exception e)
				{
					ChannelUtil.log.Error("setWirelessChannel " + e.Message + "\r\n" + e.StackTrace);
				}
			}
		}

		public static void switchWirelessChannel()
		{
			string[] channels = new string[]
			{
				"36",
				"40",
				"44",
				"48",
				"149",
				"153",
				"157",
				"161",
				"165"
			};
			string currentChannel = ChannelUtil.getWirelessChannel();
			if (!string.IsNullOrEmpty(currentChannel))
			{
				string ranChannel;
				do
				{
					int ranIndex = new Random().Next(0, channels.Length - 1);
					ranChannel = channels[ranIndex];
				}
				while (ranChannel.Equals(currentChannel));
				ChannelUtil.setWirelessChannel(ranChannel);
			}
		}

		public static string getWifiSSID()
		{
			ChannelUtil.log.Trace("getWifiSSID");
			using (SshClient client = new SshClient(ChannelUtil.getDefaultConnectInfo()))
			{
				try
				{
					client.Connect();
					SshCommand expr_2B = client.CreateCommand("user_tool show wl5g_info");
					expr_2B.CommandTimeout = TimeSpan.FromSeconds(1.0);
					string arg_59_0 = expr_2B.Execute();
					client.Disconnect();
					string[] outputs = arg_59_0.Split(new string[]
					{
						"\n"
					}, StringSplitOptions.None);
					if (outputs != null)
					{
						string[] array = outputs;
						for (int i = 0; i < array.Length; i++)
						{
							string entry = array[i];
							ChannelUtil.log.Trace("entry = " + entry);
							if (entry.StartsWith("SSID"))
							{
								string[] SSIDInfo = entry.Split(new string[]
								{
									":"
								}, StringSplitOptions.None);
								if (SSIDInfo != null && SSIDInfo.Count<string>() > 1)
								{
									return SSIDInfo[1];
								}
							}
						}
					}
				}
				catch (Exception e)
				{
					ChannelUtil.log.Error("getWifiSSID " + e.Message + "\r\n" + e.StackTrace);
				}
			}
			return "";
		}

		public static string getWifiPassword()
		{
			ChannelUtil.log.Trace("getWifiPassword");
			using (SshClient client = new SshClient(ChannelUtil.getDefaultConnectInfo()))
			{
				try
				{
					client.Connect();
					SshCommand expr_2B = client.CreateCommand("user_tool show wl5g_info");
					expr_2B.CommandTimeout = TimeSpan.FromSeconds(1.0);
					string result = expr_2B.Execute();
					client.Disconnect();
					ChannelUtil.log.Trace("result = " + result);
					string[] outputs = result.Split(new string[]
					{
						"\n"
					}, StringSplitOptions.None);
					if (outputs != null)
					{
						string[] array = outputs;
						for (int i = 0; i < array.Length; i++)
						{
							string entry = array[i];
							ChannelUtil.log.Trace("entry = " + entry);
							if (entry.StartsWith("Security_key"))
							{
								string[] passwordInfo = entry.Split(new string[]
								{
									":"
								}, StringSplitOptions.None);
								if (passwordInfo != null && passwordInfo.Count<string>() > 1)
								{
									return passwordInfo[1];
								}
							}
						}
					}
				}
				catch (Exception e)
				{
					ChannelUtil.log.Error("getWifiPassword " + e.Message + "\r\n" + e.StackTrace);
				}
			}
			return "";
		}

		public static void setWifi(string SSID, string password)
		{
			ChannelUtil.log.Trace("setWifi");
			using (SshClient client = new SshClient(ChannelUtil.getDefaultConnectInfo()))
			{
				try
				{
					client.Connect();
					ChannelUtil.log.Trace("setWifi ssid = " + SSID);
					SshCommand expr_46 = client.CreateCommand("user_tool wl5g_set SSID " + SSID);
					expr_46.CommandTimeout = TimeSpan.FromSeconds(1.0);
					expr_46.Execute();
					client.Disconnect();
				}
				catch (Exception e)
				{
					ChannelUtil.log.Trace(e.Message);
				}
			}
			using (SshClient client2 = new SshClient(ChannelUtil.getDefaultConnectInfo()))
			{
				try
				{
					client2.Connect();
					ChannelUtil.log.Trace("setWifi pass = " + password);
					SshCommand expr_BC = client2.CreateCommand("user_tool wl5g_set Security " + password);
					expr_BC.CommandTimeout = TimeSpan.FromSeconds(1.0);
					expr_BC.Execute();
					client2.Disconnect();
				}
				catch (Exception e2)
				{
					ChannelUtil.log.Trace(e2.Message);
				}
			}
		}
	}
}
