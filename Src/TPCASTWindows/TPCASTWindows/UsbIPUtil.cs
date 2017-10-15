using System;
using System.Diagnostics;

namespace TPCASTWindows
{
	internal class UsbIPUtil
	{
		public const int CONNECT_ERROR_ROUTER = -1001;

		public const int CONNECT_ERROR_RASPBERRY = -1002;

		public const int CONNECT_ERROR_DISCONNECT = -1000;

		public const int CONNECT_ERROR_REBOOT = -2000;

		public const int CONNECT_ERROR_CABLE = -3000;

		public static int isUSBConnected()
		{
			int num = UsbIPUtil.isHostConnected();
			if (num != 0)
			{
				return num;
			}
			if (UsbIPUtil.isCableLinked())
			{
				return UsbIPUtil.ConnectControl();
			}
			return -3000;
		}

		public static int isHostConnected()
		{
			if (ChannelUtil.getWirelessChannel() == null)
			{
				return -1001;
			}
			if (UsbIPUtil.isHostLinked())
			{
				return 0;
			}
			return -1002;
		}

		public static int ConnectControl()
		{
			string text = UsbIPUtil.checkDevStateString();
			if (!string.IsNullOrEmpty(text))
			{
				if (text.Contains("no available"))
				{
					return -2000;
				}
				if (!text.Contains("disconnected"))
				{
					return 0;
				}
				string devStateString = UsbIPUtil.getDevStateString();
				if (!string.IsNullOrEmpty(devStateString))
				{
					if (devStateString.Contains("disconnected"))
					{
						return -1000;
					}
					return 0;
				}
			}
			return -1000;
		}

		public static int ForceConnectControl()
		{
			string devStateString = UsbIPUtil.getDevStateString();
			if (string.IsNullOrEmpty(devStateString))
			{
				return -1000;
			}
			if (devStateString.Contains("disconnected"))
			{
				return -1000;
			}
			return 0;
		}

		public static string getDevStateString()
		{
			Process process = new Process();
			process.StartInfo.FileName = "GetDevState.exe";
			process.StartInfo.WorkingDirectory = ".";
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardInput = true;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.CreateNoWindow = true;
			process.Start();
			string expr_74 = process.StandardOutput.ReadToEnd();
			Console.WriteLine(expr_74);
			process.Close();
			return expr_74;
		}

		public static int isUsbLinked()
		{
			string text = UsbIPUtil.checkDevStateString();
			if (string.IsNullOrEmpty(text))
			{
				return -1000;
			}
			if (text.Contains("disconnected"))
			{
				return -1000;
			}
			if (text.Contains("no available"))
			{
				return -2000;
			}
			return 0;
		}

		public static bool isHostLinked()
		{
			string text = UsbIPUtil.checkDevHostString();
			return !string.IsNullOrEmpty(text) && !text.Contains("can not connect");
		}

		public static bool isCableLinked()
		{
			Console.WriteLine("isCableLinked");
			string text = UsbIPUtil.checkHelmetCableString();
			return !string.IsNullOrEmpty(text) && !text.Contains("disconnected") && !text.Contains("failed");
		}

		public static bool isDriverInstalled()
		{
			string text = UsbIPUtil.checkDriverString();
			return !string.IsNullOrEmpty(text) && text.Contains("OK");
		}

		public static bool RebootService()
		{
			string text = UsbIPUtil.rebootTPCASTService();
			if (string.IsNullOrEmpty(text))
			{
				return false;
			}
			if (text.Contains("is enable"))
			{
				return true;
			}
			text.Contains("Error");
			return false;
		}

		public static string checkDevStateString()
		{
			Process process = new Process();
			process.StartInfo.FileName = "GetDevState.exe";
			process.StartInfo.Arguments = "status";
			process.StartInfo.WorkingDirectory = ".";
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardInput = true;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.CreateNoWindow = true;
			process.Start();
			string expr_84 = process.StandardOutput.ReadToEnd();
			Console.WriteLine(expr_84);
			process.Close();
			return expr_84;
		}

		public static string checkDevHostString()
		{
			Process process = new Process();
			process.StartInfo.FileName = "GetDevState.exe";
			process.StartInfo.Arguments = "host";
			process.StartInfo.WorkingDirectory = ".";
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardInput = true;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.CreateNoWindow = true;
			process.Start();
			string expr_84 = process.StandardOutput.ReadToEnd();
			Console.WriteLine(expr_84);
			process.Close();
			return expr_84;
		}

		public static string checkHelmetCableString()
		{
			Process process = new Process();
			process.StartInfo.FileName = "GetDevState.exe";
			process.StartInfo.Arguments = "cable";
			process.StartInfo.WorkingDirectory = ".";
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardInput = true;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.CreateNoWindow = true;
			process.Start();
			string expr_84 = process.StandardOutput.ReadToEnd();
			Console.WriteLine(expr_84);
			process.Close();
			return expr_84;
		}

		public static string checkDriverString()
		{
			Process process = new Process();
			process.StartInfo.FileName = "GetDevState.exe";
			process.StartInfo.Arguments = "driver";
			process.StartInfo.WorkingDirectory = ".";
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardInput = true;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.CreateNoWindow = true;
			process.Start();
			string expr_84 = process.StandardOutput.ReadToEnd();
			Console.WriteLine(expr_84);
			process.Close();
			return expr_84;
		}

		public static string rebootTPCASTService()
		{
			Process process = new Process();
			process.StartInfo.FileName = "TpcastDaemon.exe";
			process.StartInfo.Arguments = "enable";
			process.StartInfo.WorkingDirectory = ".";
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardInput = true;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.CreateNoWindow = true;
			process.Start();
			string expr_84 = process.StandardOutput.ReadToEnd();
			Console.WriteLine(expr_84);
			process.Close();
			return expr_84;
		}
	}
}
