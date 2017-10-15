using NLog;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace TPCASTWindows
{
	internal class ConfigureUtil
	{
		private static Logger log = LogManager.GetCurrentClassLogger();

		private static Control sContext;

		private static string adapter_ip = "";

		public static ClientType currentType = ClientType.NONE;

		private ConfigureUtil()
		{
		}

		public static void init(Control context)
		{
			ConfigureUtil.sContext = context;
			ConfigureUtil.readIPFromConfigurationFile();
		}

		public static void readIPFromConfigurationFile()
		{
			if (File.Exists(Constants.configFilePath))
			{
				StreamReader sr = new StreamReader(Constants.configFilePath);
				ConfigureUtil.adapter_ip = sr.ReadLine().Split(new string[]
				{
					":"
				}, StringSplitOptions.None)[0];
				sr.Close();
				sr.Dispose();
			}
		}

		public static string AdapterIP()
		{
			if (string.IsNullOrEmpty(ConfigureUtil.adapter_ip))
			{
				ConfigureUtil.readIPFromConfigurationFile();
			}
			return ConfigureUtil.adapter_ip;
		}

		public static ClientType getClientType()
		{
			if (!ConfigureUtil.currentType.Equals(ClientType.NONE))
			{
				return ConfigureUtil.currentType;
			}
			string ip = ConfigureUtil.AdapterIP();
			if (!string.IsNullOrEmpty(ip))
			{
				string[] ipSplit = ip.Split(new string[]
				{
					"."
				}, StringSplitOptions.None);
				if (ipSplit != null && ipSplit.Count<string>() > 3)
				{
					string ipType = ipSplit[3];
					if (ipType == "88")
					{
						ConfigureUtil.currentType = ClientType.TYPE_A;
						return ClientType.TYPE_A;
					}
					if (ipType == "90")
					{
						ConfigureUtil.currentType = ClientType.TYPE_B;
						return ClientType.TYPE_B;
					}
					if (ipType == "92")
					{
						ConfigureUtil.currentType = ClientType.TYPE_C;
						return ClientType.TYPE_C;
					}
					if (ipType == "94")
					{
						ConfigureUtil.currentType = ClientType.TYPE_D;
						return ClientType.TYPE_D;
					}
				}
			}
			ConfigureUtil.currentType = ClientType.TYPE_A;
			return ClientType.TYPE_A;
		}
	}
}
