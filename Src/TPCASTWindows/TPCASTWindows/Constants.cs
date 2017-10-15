using System;

namespace TPCASTWindows
{
	internal class Constants
	{
		public static string downloadPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads\\TPCAST";

		public static string updateSoftwareFilePath = Constants.downloadPath + "\\TPCASTUpdate.exe";

		public static string updateSoftwareMd5Path = Constants.downloadPath + "\\TPCASTUpdateMd5";

		public static string updateAdapterFilePath = Constants.downloadPath + "\\update.tar.gz";

		public const string DEFAULT_WIFI_SSID = "TPCast_AP";

		public static string configFilePath = Environment.CurrentDirectory + "\\configuration.ini";
	}
}
