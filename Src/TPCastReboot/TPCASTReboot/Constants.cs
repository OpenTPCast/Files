using System;

namespace TPCASTReboot
{
	internal class Constants
	{
		public static string downloadPath = "C:\\Users\\" + Environment.UserName + "\\Downloads\\TPCAST";

		public static string updateSoftwareFilePath = Constants.downloadPath + "\\TPCASTUpdate.exe";

		public static string updateSoftwareMd5Path = Constants.downloadPath + "\\TPCASTUpdateMd5";

		public static string updateAdapterFilePath = Constants.downloadPath + "\\update.tar.gz";
	}
}
