using Microsoft.Win32;
using System;
using System.Diagnostics;

namespace TPCASTUpdateHelper
{
	internal class RegistryUtil
	{
		public static bool findCode(string code)
		{
			using (RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
			{
				using (RegistryKey registryKey2 = registryKey.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall"))
				{
					if (registryKey2 != null)
					{
						string[] subKeyNames = registryKey2.GetSubKeyNames();
						for (int i = 0; i < subKeyNames.Length; i++)
						{
							string text = subKeyNames[i];
							Console.WriteLine("name = " + text);
							if (text.Equals(code))
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		public static void UninstallTPCASTCommand(string code)
		{
			Process expr_05 = new Process();
			expr_05.StartInfo.FileName = Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\msiexec.exe";
			expr_05.StartInfo.Arguments = "/x " + code + " /qb";
			expr_05.StartInfo.WorkingDirectory = ".";
			expr_05.StartInfo.UseShellExecute = false;
			expr_05.StartInfo.RedirectStandardInput = true;
			expr_05.StartInfo.RedirectStandardOutput = true;
			expr_05.StartInfo.RedirectStandardError = true;
			expr_05.StartInfo.CreateNoWindow = true;
			expr_05.Start();
			expr_05.WaitForExit();
			expr_05.Close();
		}
	}
}
