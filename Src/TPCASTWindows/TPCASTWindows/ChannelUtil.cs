using Renci.SshNet;
using System;
using Tamir.SharpSsh;

namespace TPCASTWindows
{
	internal class ChannelUtil
	{
		private const string ROUTER_IP = "192.168.1.1";

		private const string NAME = "root";

		private const string PASSWORD = "12345678";

		public static string getWirelessChannel()
		{
			try
			{
				SshExec expr_0F = new SshExec("192.168.1.1", "root");
				expr_0F.Password = "12345678";
				expr_0F.Connect();
				string text = expr_0F.RunCommand("uci show wireless.radio1.channel");
				expr_0F.Close();
				if (!string.IsNullOrEmpty(text))
				{
					int num = text.IndexOf("'") + 1;
					int length = text.LastIndexOf("'") - num;
					return text.Substring(num, length);
				}
			}
			catch (Exception arg_62_0)
			{
				Console.Write(arg_62_0.Message);
			}
			return null;
		}

		public static string getWirelessChannel2()
		{
			string arg_19_0 = "192.168.1.1";
			string username = "root";
			string password = "12345678";
			string commandText = "uci show wireless.radio1.channel";
			using (SshClient sshClient = new SshClient(arg_19_0, username, password))
			{
				try
				{
					sshClient.Connect();
					string arg_37_0 = sshClient.RunCommand(commandText).Execute();
					sshClient.Disconnect();
					return arg_37_0;
				}
				catch (Exception arg_3B_0)
				{
					Console.Write(arg_3B_0.Message);
				}
			}
			return null;
		}

		public static void setWirelessChannel(string channel)
		{
			try
			{
				SshExec expr_0F = new SshExec("192.168.1.1", "root");
				expr_0F.Password = "12345678";
				expr_0F.Connect();
				expr_0F.RunCommand("uci set wireless.radio1.channel=" + channel);
				expr_0F.RunCommand("uci commit");
				expr_0F.RunCommand("wifi");
				expr_0F.Close();
			}
			catch (Exception arg_51_0)
			{
				Console.Write(arg_51_0.Message);
			}
		}

		public static void switchWirelessChannel()
		{
			string[] array = new string[]
			{
				"36",
				"40",
				"44",
				"48"
			};
			string wirelessChannel = ChannelUtil.getWirelessChannel();
			if (!string.IsNullOrEmpty(wirelessChannel))
			{
				string text;
				do
				{
					int num = new Random().Next(0, array.Length - 1);
					text = array[num];
				}
				while (text.Equals(wirelessChannel));
				ChannelUtil.setWirelessChannel(text);
			}
		}
	}
}
