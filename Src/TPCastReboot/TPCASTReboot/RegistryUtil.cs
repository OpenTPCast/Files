using Microsoft.Win32;
using System;
using System.Security.AccessControl;

namespace TPCASTReboot
{
	internal class RegistryUtil
	{
		public static void addAutoRunOnce(string fullPath)
		{
			using (RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64))
			{
				using (RegistryKey registryKey2 = registryKey.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl))
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
				using (RegistryKey registryKey2 = registryKey.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl))
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
	}
}
