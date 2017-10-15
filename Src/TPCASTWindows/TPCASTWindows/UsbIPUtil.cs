using NLog;
using System;
using System.Diagnostics;

namespace TPCASTWindows
{
	internal class UsbIPUtil
	{
		private static Logger log = LogManager.GetCurrentClassLogger();

		public const int CONNECT_ERROR_ROUTER = -1001;

		public const int CONNECT_ERROR_RASPBERRY = -1002;

		public const int CONNECT_ERROR_DISCONNECT = -1000;

		public const int CONNECT_ERROR_REBOOT = -2000;

		public const int CONNECT_ERROR_CABLE = -3000;

		public static int isUSBConnected()
		{
			int status = UsbIPUtil.isHostConnected();
			if (status != 0)
			{
				return status;
			}
			if (UsbIPUtil.isCableLinked())
			{
				return UsbIPUtil.ConnectControl();
			}
			return -3000;
		}

		public static int isHostConnected()
		{
			if (!ChannelUtil.pingRouterConnect())
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
			string linkStatus = UsbIPUtil.checkDevStateString();
			if (!string.IsNullOrEmpty(linkStatus))
			{
				if (linkStatus.Contains("no available"))
				{
					return -2000;
				}
				if (!linkStatus.Contains("disconnected"))
				{
					return 0;
				}
				string output = UsbIPUtil.getDevStateString();
				if (!string.IsNullOrEmpty(output))
				{
					if (output.Contains("disconnected"))
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
			string output = UsbIPUtil.getDevStateString();
			if (string.IsNullOrEmpty(output))
			{
				return -1000;
			}
			if (output.Contains("disconnected"))
			{
				return -1000;
			}
			return 0;
		}

		public static string getDevStateString()
		{
			Process expr_05 = new Process();
			expr_05.StartInfo.FileName = "GetDevState.exe";
			expr_05.StartInfo.WorkingDirectory = ".";
			expr_05.StartInfo.UseShellExecute = false;
			expr_05.StartInfo.RedirectStandardInput = true;
			expr_05.StartInfo.RedirectStandardOutput = true;
			expr_05.StartInfo.RedirectStandardError = true;
			expr_05.StartInfo.CreateNoWindow = true;
			expr_05.Start();
			string output = expr_05.StandardOutput.ReadToEnd();
			UsbIPUtil.log.Trace(output);
			expr_05.Close();
			return output;
		}

		public static int isUsbLinked()
		{
			string output = UsbIPUtil.checkDevStateString();
			if (string.IsNullOrEmpty(output))
			{
				return -1000;
			}
			if (output.Contains("disconnected"))
			{
				return -1000;
			}
			if (output.Contains("no available"))
			{
				return -2000;
			}
			return 0;
		}

		public static bool isHostLinked()
		{
			string hostState = UsbIPUtil.checkDevHostString();
			return !string.IsNullOrEmpty(hostState) && !hostState.Contains("can not connect") && !hostState.Contains("fail");
		}

		public static bool isCableLinked()
		{
			UsbIPUtil.log.Trace("isCableLinked");
			string cableState = UsbIPUtil.checkHelmetCableString();
			return !string.IsNullOrEmpty(cableState) && !cableState.Contains("disconnected") && !cableState.Contains("failed");
		}

		public static bool isDriverInstalled()
		{
			string driverState = UsbIPUtil.checkDriverString();
			return !string.IsNullOrEmpty(driverState) && driverState.Contains("OK");
		}

		public static bool isServiceOk()
		{
			string serviceState = UsbIPUtil.checkServerString();
			return !string.IsNullOrEmpty(serviceState) && serviceState.Contains("OK");
		}

		public static bool RebootService()
		{
			string result = UsbIPUtil.rebootTPCASTService();
			if (string.IsNullOrEmpty(result))
			{
				return false;
			}
			if (result.Contains("is enable"))
			{
				return true;
			}
			result.Contains("Error");
			return false;
		}

		public static string checkDevStateString()
		{
			Process expr_05 = new Process();
			expr_05.StartInfo.FileName = "GetDevState.exe";
			expr_05.StartInfo.Arguments = "status";
			expr_05.StartInfo.WorkingDirectory = ".";
			expr_05.StartInfo.UseShellExecute = false;
			expr_05.StartInfo.RedirectStandardInput = true;
			expr_05.StartInfo.RedirectStandardOutput = true;
			expr_05.StartInfo.RedirectStandardError = true;
			expr_05.StartInfo.CreateNoWindow = true;
			expr_05.Start();
			string output = expr_05.StandardOutput.ReadToEnd();
			UsbIPUtil.log.Trace(output);
			expr_05.Close();
			return output;
		}

		public static bool isDevStateConnect()
		{
			string output = UsbIPUtil.checkDevStateString();
			return !string.IsNullOrEmpty(output) && !output.Contains("disconnected") && !output.Contains("no available");
		}

		public static string checkDevHostString()
		{
			Process expr_05 = new Process();
			expr_05.StartInfo.FileName = "GetDevState.exe";
			expr_05.StartInfo.Arguments = "host";
			expr_05.StartInfo.WorkingDirectory = ".";
			expr_05.StartInfo.UseShellExecute = false;
			expr_05.StartInfo.RedirectStandardInput = true;
			expr_05.StartInfo.RedirectStandardOutput = true;
			expr_05.StartInfo.RedirectStandardError = true;
			expr_05.StartInfo.CreateNoWindow = true;
			expr_05.Start();
			string output = expr_05.StandardOutput.ReadToEnd();
			UsbIPUtil.log.Trace(output);
			expr_05.Close();
			return output;
		}

		public static string checkHelmetCableString()
		{
			Process expr_05 = new Process();
			expr_05.StartInfo.FileName = "GetDevState.exe";
			expr_05.StartInfo.Arguments = "cable";
			expr_05.StartInfo.WorkingDirectory = ".";
			expr_05.StartInfo.UseShellExecute = false;
			expr_05.StartInfo.RedirectStandardInput = true;
			expr_05.StartInfo.RedirectStandardOutput = true;
			expr_05.StartInfo.RedirectStandardError = true;
			expr_05.StartInfo.CreateNoWindow = true;
			expr_05.Start();
			string output = expr_05.StandardOutput.ReadToEnd();
			UsbIPUtil.log.Trace(output);
			expr_05.Close();
			return output;
		}

		public static string checkDriverString()
		{
			Process expr_05 = new Process();
			expr_05.StartInfo.FileName = "GetDevState.exe";
			expr_05.StartInfo.Arguments = "driver";
			expr_05.StartInfo.WorkingDirectory = ".";
			expr_05.StartInfo.UseShellExecute = false;
			expr_05.StartInfo.RedirectStandardInput = true;
			expr_05.StartInfo.RedirectStandardOutput = true;
			expr_05.StartInfo.RedirectStandardError = true;
			expr_05.StartInfo.CreateNoWindow = true;
			expr_05.Start();
			string output = expr_05.StandardOutput.ReadToEnd();
			UsbIPUtil.log.Trace(output);
			expr_05.Close();
			return output;
		}

		public static string checkServerString()
		{
			Process expr_05 = new Process();
			expr_05.StartInfo.FileName = "GetDevState.exe";
			expr_05.StartInfo.Arguments = "service";
			expr_05.StartInfo.WorkingDirectory = ".";
			expr_05.StartInfo.UseShellExecute = false;
			expr_05.StartInfo.RedirectStandardInput = true;
			expr_05.StartInfo.RedirectStandardOutput = true;
			expr_05.StartInfo.RedirectStandardError = true;
			expr_05.StartInfo.CreateNoWindow = true;
			expr_05.Start();
			string output = expr_05.StandardOutput.ReadToEnd();
			UsbIPUtil.log.Trace(output);
			expr_05.Close();
			return output;
		}

		public static string rebootTPCASTService()
		{
			Process expr_05 = new Process();
			expr_05.StartInfo.FileName = "TpcastDaemon.exe";
			expr_05.StartInfo.Arguments = "enable";
			expr_05.StartInfo.WorkingDirectory = ".";
			expr_05.StartInfo.UseShellExecute = false;
			expr_05.StartInfo.RedirectStandardInput = true;
			expr_05.StartInfo.RedirectStandardOutput = true;
			expr_05.StartInfo.RedirectStandardError = true;
			expr_05.StartInfo.CreateNoWindow = true;
			expr_05.Start();
			string output = expr_05.StandardOutput.ReadToEnd();
			UsbIPUtil.log.Trace(output);
			expr_05.Close();
			return output;
		}
	}
}
