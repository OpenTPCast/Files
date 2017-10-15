using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TPCASTReboot
{
	internal class CryptoUtil
	{
		public static string GetMD5HashFromFile(string fileName)
		{
			string result;
			try
			{
				FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
				byte[] array = new MD5CryptoServiceProvider().ComputeHash(fileStream);
				fileStream.Close();
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < array.Length; i++)
				{
					stringBuilder.Append(array[i].ToString("x2"));
				}
				result = stringBuilder.ToString();
			}
			catch (Exception ex)
			{
				throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
			}
			return result;
		}
	}
}
