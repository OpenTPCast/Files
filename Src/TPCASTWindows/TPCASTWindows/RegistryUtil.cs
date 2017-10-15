using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.AccessControl;

namespace TPCASTWindows
{
	internal class RegistryUtil
	{
		public static void addAutoRunOnce(string fullPath)
		{
			using (RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
			{
				using (RegistryKey registryKey2 = registryKey.OpenSubKey("SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\RunOnce", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl))
				{
					if (registryKey2 != null)
					{
						Console.WriteLine("key = " + registryKey2);
						registryKey2.SetValue("TPCASTUpdate", fullPath, RegistryValueKind.String);
					}
				}
			}
		}

		public static void removeAutoRunOnce()
		{
			using (RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
			{
				using (RegistryKey registryKey2 = registryKey.OpenSubKey("SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\RunOnce", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl))
				{
					if (registryKey2 != null)
					{
						Console.WriteLine("key = " + registryKey2);
						if (registryKey2.GetValue("TPCASTUpdate") != null)
						{
							registryKey2.DeleteValue("TPCASTUpdate");
						}
					}
				}
			}
		}

		public static void UninstallTPCAST()
		{
			string text = RegistryUtil.findTPCASTCode();
			Console.WriteLine("code = " + text);
			if (text != null)
			{
				RegistryUtil.UninstallTPCASTCommand(text);
			}
		}

		private static string findTPCASTCode()
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
							string name = subKeyNames[i];
							using (RegistryKey registryKey3 = registryKey2.OpenSubKey(name, false))
							{
								if (registryKey3 != null)
								{
									string value = registryKey3.GetValue("URLInfoAbout", "").ToString();
									if ("www.tpcast.cn".Equals(value))
									{
										return registryKey3.Name.Split(new string[]
										{
											"\\"
										}, StringSplitOptions.None).Last<string>();
									}
								}
							}
						}
					}
				}
			}
			return null;
		}

		private static void UninstallTPCASTCommand(string code)
		{
			Process expr_05 = new Process();
			expr_05.StartInfo.FileName = "C:\\Windows\\System32\\msiexec.exe";
			expr_05.StartInfo.Arguments = "/x " + code + " /qb";
			expr_05.StartInfo.WorkingDirectory = ".";
			expr_05.StartInfo.UseShellExecute = false;
			expr_05.StartInfo.RedirectStandardInput = true;
			expr_05.StartInfo.RedirectStandardOutput = true;
			expr_05.StartInfo.RedirectStandardError = true;
			expr_05.StartInfo.CreateNoWindow = true;
			expr_05.Start();
			expr_05.Close();
		}
	}
}
