using Microsoft.Win32;
using NLog;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.AccessControl;

namespace TPCASTWindows
{
	internal class RegistryUtil
	{
		private static Logger log = LogManager.GetCurrentClassLogger();

		public static void addAutoRunOnce(string fullPath)
		{
			using (RegistryKey localMachine64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
			{
				using (RegistryKey key = localMachine64.OpenSubKey("SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\RunOnce", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl))
				{
					if (key != null)
					{
						RegistryUtil.log.Trace("key = " + key);
						key.SetValue("TPCASTUpdate", fullPath, RegistryValueKind.String);
					}
				}
			}
		}

		public static void removeAutoRunOnce()
		{
			using (RegistryKey localMachine64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
			{
				using (RegistryKey key = localMachine64.OpenSubKey("SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\RunOnce", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl))
				{
					if (key != null)
					{
						RegistryUtil.log.Trace("key = " + key);
						if (key.GetValue("TPCASTUpdate") != null)
						{
							key.DeleteValue("TPCASTUpdate");
						}
					}
				}
			}
		}

		public static void UninstallTPCAST()
		{
			string code = RegistryUtil.findTPCASTCode();
			RegistryUtil.log.Trace("code = " + code);
			if (code != null)
			{
				RegistryUtil.invokeUninstallTPCASTCommand(code);
			}
		}

		private static string findTPCASTCode()
		{
			using (RegistryKey localMachine64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
			{
				using (RegistryKey key = localMachine64.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall"))
				{
					if (key != null)
					{
						string[] subKeyNames = key.GetSubKeyNames();
						for (int i = 0; i < subKeyNames.Length; i++)
						{
							string name = subKeyNames[i];
							using (RegistryKey key2 = key.OpenSubKey(name, false))
							{
								if (key2 != null)
								{
									string URLInfoAbout = key2.GetValue("URLInfoAbout", "").ToString();
									string ProductType = key2.GetValue("ProductType", "").ToString();
									string type = "CE";
									if ("www.tpcast.cn".Equals(URLInfoAbout) && type.Equals(ProductType))
									{
										return key2.Name.Split(new string[]
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
			expr_05.StartInfo.FileName = Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\msiexec.exe";
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

		private static void invokeUninstallTPCASTCommand(string code)
		{
			Process expr_05 = new Process();
			expr_05.StartInfo.FileName = "TPCASTUpdateHelper.exe";
			expr_05.StartInfo.Arguments = "uninstall " + code;
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
