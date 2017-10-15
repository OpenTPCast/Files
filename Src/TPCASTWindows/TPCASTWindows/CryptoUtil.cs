using NLog;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TPCASTWindows
{
	internal class CryptoUtil
	{
		private static Logger log = LogManager.GetCurrentClassLogger();

		public static string GetMD5HashFromFile(string fileName)
		{
			string result;
			try
			{
				FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
				byte[] retVal = new MD5CryptoServiceProvider().ComputeHash(file);
				file.Close();
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < retVal.Length; i++)
				{
					sb.Append(retVal[i].ToString("x2"));
				}
				result = sb.ToString();
			}
			catch (Exception ex)
			{
				CryptoUtil.log.Error("GetMD5HashFromFile" + ex.Message + "\r\n" + ex.StackTrace);
				throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
			}
			return result;
		}
	}
}
