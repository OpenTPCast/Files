using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Windows.Forms;

namespace TPCASTUtil
{
	internal class Program
	{
		public struct ProcessEntry32
		{
			public uint dwSize;

			public uint cntUsage;

			public uint th32ProcessID;

			public IntPtr th32DefaultHeapID;

			public uint th32ModuleID;

			public uint cntThreads;

			public uint th32ParentProcessID;

			public int pcPriClassBase;

			public uint dwFlags;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string szExeFile;
		}

		private static void Main(string[] args)
		{
			bool flag = args != null && args.Count<string>() > 0;
			if (flag)
			{
				Console.WriteLine("args = " + args[0]);
				bool flag2 = args[0] == "copy";
				if (flag2)
				{
					string startupPath = Application.StartupPath;
					string text = startupPath + "\\TPCASTReboot.exe";
					string tempPath = Path.GetTempPath();
					string text2 = tempPath + "\\TPCAST";
					string destFileName = text2 + "\\TPCASTReboot.exe";
					Directory.CreateDirectory(text2);
					bool flag3 = File.Exists(text);
					if (flag3)
					{
						File.Copy(text, destFileName, true);
					}
				}
				else
				{
					bool flag4 = args[0] == "kill";
					if (flag4)
					{
						bool flag5 = args.Count<string>() > 1;
						if (flag5)
						{
							string text3 = args[1];
							bool flag6 = !string.IsNullOrEmpty(text3);
							if (flag6)
							{
								Program.killProcess(text3);
							}
						}
					}
					else
					{
						bool flag7 = args[0] == "properties";
						if (flag7)
						{
							string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TpcastForWPF");
							DirectoryInfo directoryInfo = new DirectoryInfo(path);
							DirectoryInfo[] directories = directoryInfo.GetDirectories("TpcastForWPF*");
							DirectoryInfo[] array = directories;
							for (int i = 0; i < array.Length; i++)
							{
								DirectoryInfo directoryInfo2 = array[i];
								FileInfo[] files = directoryInfo2.GetFiles();
								for (int j = 0; j < files.Length; j++)
								{
									FileInfo fileInfo = files[j];
									fileInfo.Delete();
								}
								DirectoryInfo[] directories2 = directoryInfo2.GetDirectories();
								for (int k = 0; k < directories2.Length; k++)
								{
									DirectoryInfo directoryInfo3 = directories2[k];
									directoryInfo3.Delete(true);
								}
								directoryInfo2.Delete(true);
							}
							string startupPath2 = Application.StartupPath;
							string path2 = startupPath2 + "\\configuration.ini";
							bool flag8 = File.Exists(path2);
							if (flag8)
							{
								File.Delete(path2);
							}
						}
						else
						{
							bool flag9 = args[0] == "generate";
							if (flag9)
							{
								string startupPath3 = Application.StartupPath;
								string path3 = startupPath3 + "\\configuration.ini";
								FileStream fileStream = new FileStream(path3, FileMode.Create);
								StreamWriter streamWriter = new StreamWriter(fileStream);
								string value = string.Empty;
								bool flag10 = args.Count<string>() > 1;
								if (flag10)
								{
									bool flag11 = args[1] == "VIVE";
									if (flag11)
									{
										streamWriter.WriteLine("192.168.144.88:5881");
										streamWriter.WriteLine("192.168.144.88:5882");
										streamWriter.WriteLine("192.168.144.88:5883");
										streamWriter.WriteLine("192.168.144.88:5884");
										value = "VIVE";
									}
									else
									{
										bool flag12 = args[1] == "Oculus";
										if (flag12)
										{
											streamWriter.WriteLine("192.168.144.88:5881");
											streamWriter.WriteLine("192.168.144.88:5882");
											value = "Oculus";
										}
										else
										{
											bool flag13 = args[1] == "VOAll";
											if (flag13)
											{
												bool flag14 = args[3] == "VO_VIVE";
												if (flag14)
												{
													streamWriter.WriteLine("192.168.144.88:5881");
													streamWriter.WriteLine("192.168.144.88:5882");
													streamWriter.WriteLine("192.168.144.88:5883");
													streamWriter.WriteLine("192.168.144.88:5884");
													value = "VO_VIVE";
												}
												else
												{
													bool flag15 = args[3] == "VO_Oculus";
													if (flag15)
													{
														streamWriter.WriteLine("192.168.144.88:5881");
														streamWriter.WriteLine("192.168.144.88:5882");
														value = "VO_Oculus";
													}
												}
											}
										}
									}
								}
								else
								{
									streamWriter.WriteLine("192.168.144.88:5881");
									streamWriter.WriteLine("192.168.144.88:5882");
									streamWriter.WriteLine("192.168.144.88:5883");
									streamWriter.WriteLine("192.168.144.88:5884");
									value = "VIVE";
								}
								streamWriter.Flush();
								streamWriter.Close();
								fileStream.Close();
								bool flag16 = !string.IsNullOrEmpty(value);
								if (flag16)
								{
									using (RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
									{
										using (RegistryKey registryKey2 = registryKey.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\" + args[2], RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl))
										{
											bool flag17 = registryKey2 != null;
											if (flag17)
											{
												registryKey2.SetValue("SubType", value, RegistryValueKind.String);
												registryKey2.SetValue("WindowsInstaller", 0, RegistryValueKind.DWord);
											}
										}
									}
								}
							}
							else
							{
								bool flag18 = args[0] == "configuration";
								if (flag18)
								{
									string startupPath4 = Application.StartupPath;
									string path4 = startupPath4 + "\\configuration.ini";
									bool flag19 = File.Exists(path4);
									if (flag19)
									{
										File.Delete(path4);
									}
								}
							}
						}
					}
				}
			}
		}

		[DllImport("KERNEL32.DLL ")]
		public static extern IntPtr CreateToolhelp32Snapshot(uint flags, uint processid);

		[DllImport("KERNEL32.DLL ")]
		public static extern int CloseHandle(IntPtr handle);

		[DllImport("KERNEL32.DLL ")]
		public static extern int Process32First(IntPtr handle, ref Program.ProcessEntry32 pe);

		[DllImport("KERNEL32.DLL ")]
		public static extern int Process32Next(IntPtr handle, ref Program.ProcessEntry32 pe);

		public static string UninstallService()
		{
			Process process = new Process();
			process.StartInfo.FileName = Application.StartupPath + "\\TpcastDaemon.exe";
			process.StartInfo.Arguments = "uninstall";
			process.StartInfo.WorkingDirectory = ".";
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardInput = true;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.CreateNoWindow = true;
			process.Start();
			string result = process.StandardOutput.ReadToEnd();
			process.Close();
			return result;
		}

		public static string stopService()
		{
			Process process = new Process();
			process.StartInfo.FileName = Application.StartupPath + "\\TpcastDaemon.exe";
			process.StartInfo.Arguments = "disable";
			process.StartInfo.WorkingDirectory = ".";
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardInput = true;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.CreateNoWindow = true;
			process.Start();
			string result = process.StandardOutput.ReadToEnd();
			process.Close();
			return result;
		}

		public static void killProcess(string name)
		{
			IntPtr intPtr = Program.CreateToolhelp32Snapshot(2u, 0u);
			List<Program.ProcessEntry32> list = new List<Program.ProcessEntry32>();
			bool flag = (int)intPtr > 0;
			if (flag)
			{
				Program.ProcessEntry32 processEntry = default(Program.ProcessEntry32);
				processEntry.dwSize = (uint)Marshal.SizeOf(processEntry);
				for (int num = Program.Process32First(intPtr, ref processEntry); num == 1; num = Program.Process32Next(intPtr, ref processEntry))
				{
					IntPtr intPtr2 = Marshal.AllocHGlobal((int)processEntry.dwSize);
					Marshal.StructureToPtr(processEntry, intPtr2, true);
					Program.ProcessEntry32 item = (Program.ProcessEntry32)Marshal.PtrToStructure(intPtr2, typeof(Program.ProcessEntry32));
					Marshal.FreeHGlobal(intPtr2);
					list.Add(item);
				}
				Program.CloseHandle(intPtr);
				foreach (Program.ProcessEntry32 current in list)
				{
					bool flag2 = name.Equals(current.szExeFile);
					if (flag2)
					{
						try
						{
							int th32ProcessID = (int)current.th32ProcessID;
							Process.GetProcessById(th32ProcessID).Kill();
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.Message);
						}
					}
				}
			}
		}
	}
}
