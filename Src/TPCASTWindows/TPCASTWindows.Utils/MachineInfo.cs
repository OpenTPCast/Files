using Microsoft.Win32;
using NLog;
using System;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Windows.Forms;

namespace TPCASTWindows.Utils
{
	internal class MachineInfo
	{
		private static Logger log = LogManager.GetCurrentClassLogger();

		public static readonly MachineInfo Instance = new MachineInfo();

		private MachineInfo()
		{
		}

		public void buildMachineInfo()
		{
			string SystemInfo = this.GetSystemInfo();
			string BaseBoardInfo = this.GetBaseBoardInfo();
			string PhysicalMemoryInfo = this.GetPhysicalMemoryInfo();
			string DiskInfo = this.GetDiskInfo();
			string DisplayInfo = this.GetDisplayInfo();
			string Steam = this.GetSteamVersion();
			string SteamVR = this.GetSteamVRVersion();
			string Viveport = this.GetViveportVersion();
			string ViveportSteamVR = this.GetVivePortSteamVRVersion();
			string IPConfig = this.GetIPConfigString();
			Clipboard.SetText(string.Concat(new string[]
			{
				SystemInfo,
				Environment.NewLine,
				BaseBoardInfo,
				Environment.NewLine,
				PhysicalMemoryInfo,
				Environment.NewLine,
				DiskInfo,
				Environment.NewLine,
				DisplayInfo,
				Environment.NewLine,
				Steam,
				Environment.NewLine,
				SteamVR,
				Environment.NewLine,
				Viveport,
				Environment.NewLine,
				ViveportSteamVR,
				Environment.NewLine,
				IPConfig,
				Environment.NewLine
			}));
		}

		private string GetOSVersion()
		{
			Version OSVersion = Environment.OSVersion.Version;
			string text = OSVersion.Major + "." + OSVersion.Minor;
			uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
			string version;
			if (num <= 3236114161u)
			{
				if (num <= 3185781304u)
				{
					if (num != 2793341610u)
					{
						if (num == 3185781304u)
						{
							if (text == "5.2")
							{
								version = "Windows2003";
								goto IL_170;
							}
						}
					}
					else if (text == "10.0")
					{
						version = "Windows10";
						goto IL_170;
					}
				}
				else if (num != 3219336542u)
				{
					if (num == 3236114161u)
					{
						if (text == "5.1")
						{
							version = "WindowsXP";
							goto IL_170;
						}
					}
				}
				else if (text == "5.0")
				{
					version = "Windows2000";
					goto IL_170;
				}
			}
			else if (num <= 4235161167u)
			{
				if (num != 4218383548u)
				{
					if (num == 4235161167u)
					{
						if (text == "6.0")
						{
							version = "Windows2008";
							goto IL_170;
						}
					}
				}
				else if (text == "6.1")
				{
					version = "Windows7";
					goto IL_170;
				}
			}
			else if (num != 4251938786u)
			{
				if (num == 4268716405u)
				{
					if (text == "6.2")
					{
						version = "Windows8";
						goto IL_170;
					}
				}
			}
			else if (text == "6.3")
			{
				version = "Windows8.1";
				goto IL_170;
			}
			version = OSVersion.ToString();
			IL_170:
			MachineInfo.log.Trace("GetOSVersion " + version);
			return version;
		}

		private string GetSPVersion()
		{
			string spVersion = Environment.OSVersion.ServicePack;
			MachineInfo.log.Trace("GetSPVersion " + spVersion);
			return spVersion;
		}

		public string GetSystemInfo()
		{
			return string.Concat(new string[]
			{
				"OS Version: ",
				this.GetOSVersion(),
				" ",
				this.GetSPVersion(),
				Environment.NewLine
			});
		}

		public void dump(string key)
		{
			using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = new ManagementObjectSearcher("select * from " + key).Get().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ManagementObject mo = (ManagementObject)enumerator.Current;
					MachineInfo.log.Trace("mo = " + mo.ToString());
					foreach (PropertyData pd in mo.Properties)
					{
						MachineInfo.log.Trace(pd.Name + " = " + pd.Value);
					}
				}
			}
		}

		public string GetBaseBoardInfo()
		{
			string info = "Baseboard Info " + Environment.NewLine;
			using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = new ManagementObjectSearcher("select * from Win32_BaseBoard").Get().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ManagementObject mo = (ManagementObject)enumerator.Current;
					info = info + "Manufacturer: " + mo.GetPropertyValue("Manufacturer").ToString() + Environment.NewLine;
					info = info + "Product: " + mo["Product"].ToString() + Environment.NewLine;
				}
			}
			MachineInfo.log.Trace("GetBaseBoardInfo " + info);
			return info;
		}

		public string GetPhysicalMemoryInfo()
		{
			string info = "Physical Memory Info" + Environment.NewLine;
			using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = new ManagementObjectSearcher("select * from Win32_PhysicalMemory").Get().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ManagementObject mo = (ManagementObject)enumerator.Current;
					info = info + "Manufacturer: " + mo["Manufacturer"].ToString() + Environment.NewLine;
					info = info + "PartNumber: " + mo["PartNumber"].ToString() + Environment.NewLine;
					info = string.Concat(new object[]
					{
						info,
						"Capacity: ",
						long.Parse(mo["Capacity"].ToString()) / 1024L / 1024L / 1024L,
						"G",
						Environment.NewLine
					});
				}
			}
			MachineInfo.log.Trace("GetPhysicalMemoryInfo " + info);
			return info;
		}

		public string GetDiskInfo()
		{
			string info = "Disk Info" + Environment.NewLine;
			using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = new ManagementObjectSearcher("select * from Win32_DiskDrive").Get().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ManagementObject mo = (ManagementObject)enumerator.Current;
					info = info + "Manufacturer: " + mo["Manufacturer"].ToString() + Environment.NewLine;
					info = info + "Caption: " + mo["Caption"].ToString() + Environment.NewLine;
				}
			}
			MachineInfo.log.Trace("GetDiskInfo " + info);
			return info;
		}

		public string GetDisplayInfo()
		{
			string info = "Display Info" + Environment.NewLine;
			using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = new ManagementObjectSearcher("select * from Win32_DisplayConfiguration").Get().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ManagementObject mo = (ManagementObject)enumerator.Current;
					info = info + "DeviceName: " + mo["DeviceName"].ToString() + Environment.NewLine;
					info = info + "DriverVersion: " + mo["DriverVersion"].ToString() + Environment.NewLine;
				}
			}
			MachineInfo.log.Trace("GetDisplayInfo " + info);
			return info;
		}

		public string GetIPConfigString()
		{
			Process expr_05 = new Process();
			expr_05.StartInfo.FileName = "ipconfig.exe";
			expr_05.StartInfo.Arguments = "/all";
			expr_05.StartInfo.WorkingDirectory = ".";
			expr_05.StartInfo.UseShellExecute = false;
			expr_05.StartInfo.RedirectStandardInput = true;
			expr_05.StartInfo.RedirectStandardOutput = true;
			expr_05.StartInfo.RedirectStandardError = true;
			expr_05.StartInfo.CreateNoWindow = true;
			expr_05.Start();
			string output = "IP Config Info " + Environment.NewLine;
			output = expr_05.StandardOutput.ReadToEnd();
			expr_05.Close();
			return output;
		}

		private string GetVivePortSteamVRPath()
		{
			string path = "";
			using (RegistryKey localMachine64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
			{
				using (RegistryKey key = localMachine64.OpenSubKey("SOFTWARE\\Wow6432Node\\HtcVive\\SteamVR"))
				{
					if (key != null)
					{
						MachineInfo.log.Trace("key = " + key);
						path = key.GetValue("LaunchPath", "").ToString();
						MachineInfo.log.Trace("path = " + path);
					}
				}
			}
			return path;
		}

		public string GetVivePortSteamVRVersion()
		{
			string info = "Viveport SteamVR " + Environment.NewLine;
			string path = this.GetVivePortSteamVRPath();
			if (!string.IsNullOrEmpty(path) && File.Exists(path))
			{
				FileInfo fi = new FileInfo(path);
				MachineInfo.log.Trace("create time = " + fi.CreationTime);
				DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
				int time = (int)(fi.LastWriteTime - startTime).TotalSeconds;
				MachineInfo.log.Trace("time = " + time);
				info = string.Concat(new object[]
				{
					info,
					"Last Write Time: ",
					fi.CreationTime,
					Environment.NewLine
				});
				info = string.Concat(new object[]
				{
					info,
					"Last Write Timestamp: ",
					time,
					Environment.NewLine
				});
			}
			return info;
		}

		public string GetViveportVersion()
		{
			string info = "Viveport " + Environment.NewLine;
			using (RegistryKey localMachine64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
			{
				using (RegistryKey key = localMachine64.OpenSubKey("SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\VIVE Software"))
				{
					if (key != null)
					{
						string displayVersion = key.GetValue("DisplayVersion", "").ToString();
						info = info + "Version: " + displayVersion + Environment.NewLine;
					}
				}
			}
			return info;
		}

		public string GetSteamVersion()
		{
			string info = "Steam " + Environment.NewLine;
			using (RegistryKey localMachine64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
			{
				using (RegistryKey key = localMachine64.OpenSubKey("SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Steam"))
				{
					if (key != null)
					{
						string displayVersion = key.GetValue("DisplayVersion", "").ToString();
						info = info + "Version: " + displayVersion + Environment.NewLine;
					}
				}
			}
			return info;
		}

		private string GetSteamVRInstallPath()
		{
			string installPath = "";
			using (RegistryKey localMachine64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
			{
				using (RegistryKey key = localMachine64.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Steam App 250820"))
				{
					if (key != null)
					{
						installPath = key.GetValue("InstallLocation", "").ToString();
						MachineInfo.log.Trace("installPath = " + installPath);
					}
				}
			}
			return installPath;
		}

		public string GetSteamVRVersion()
		{
			string info = "SteamVR " + Environment.NewLine;
			string path = this.GetSteamVRInstallPath();
			if (!string.IsNullOrEmpty(path))
			{
				path += "\\tools\\bin\\win32\\vrmonitor.exe";
				if (File.Exists(path))
				{
					FileInfo fi = new FileInfo(path);
					MachineInfo.log.Trace("create time = " + fi.CreationTime);
					DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
					int time = (int)(fi.LastWriteTime - startTime).TotalSeconds;
					MachineInfo.log.Trace("time = " + time);
					info = string.Concat(new object[]
					{
						info,
						"Last Write Time: ",
						fi.CreationTime,
						Environment.NewLine
					});
					info = string.Concat(new object[]
					{
						info,
						"Last Write Timestamp: ",
						time,
						Environment.NewLine
					});
				}
			}
			return info;
		}
	}
}
