using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

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
			if (args != null && args.Count<string>() > 0)
			{
				Console.WriteLine("args = " + args[0]);
				if (args[0] == "copy")
				{
					string expr_3B = Environment.CurrentDirectory;
					string text = expr_3B + "\\TPCASTReboot.exe";
					string text2 = expr_3B + "\\en\\TPCASTReboot.resources.dll";
					string text3 = expr_3B + "\\zh\\TPCASTReboot.resources.dll";
					string expr_6D = Path.GetTempPath() + "\\TPCAST";
					string destFileName = expr_6D + "\\TPCASTReboot.exe";
					string destFileName2 = expr_6D + "\\en\\TPCASTReboot.resources.dll";
					string destFileName3 = expr_6D + "\\zh\\TPCASTReboot.resources.dll";
					Directory.CreateDirectory(expr_6D);
					Directory.CreateDirectory(expr_6D + "\\en");
					Directory.CreateDirectory(expr_6D + "\\zh");
					if (File.Exists(text))
					{
						File.Copy(text, destFileName, true);
					}
					if (File.Exists(text2))
					{
						File.Copy(text2, destFileName2, true);
					}
					if (File.Exists(text3))
					{
						File.Copy(text3, destFileName3, true);
						return;
					}
				}
				else if (args[0] == "kill")
				{
					if (args.Count<string>() > 1)
					{
						string text4 = args[1];
						if (!string.IsNullOrEmpty(text4))
						{
							Program.killProcess(text4);
							return;
						}
					}
				}
				else
				{
					if (args[0] == "properties")
					{
						DirectoryInfo[] directories = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Microsoft").GetDirectories("TPCASTWindows*");
						for (int i = 0; i < directories.Length; i++)
						{
							DirectoryInfo directoryInfo = directories[i];
							Console.WriteLine("dir " + directoryInfo);
							directoryInfo.Delete(true);
						}
						return;
					}
					if (args[0] == "generate")
					{
						FileStream expr_1B1 = new FileStream(Environment.CurrentDirectory + "\\configuration.ini", FileMode.Create);
						StreamWriter streamWriter = new StreamWriter(expr_1B1);
						if (args.Count<string>() > 1)
						{
							if (args[1] == "Group_1")
							{
								streamWriter.WriteLine("192.168.144.88:5881");
								streamWriter.WriteLine("192.168.144.88:5882");
								streamWriter.WriteLine("192.168.144.88:5883");
								streamWriter.WriteLine("192.168.144.88:5884");
							}
							else if (args[1] == "Group_2")
							{
								streamWriter.WriteLine("192.168.144.90:5881");
								streamWriter.WriteLine("192.168.144.90:5882");
								streamWriter.WriteLine("192.168.144.90:5883");
								streamWriter.WriteLine("192.168.144.90:5884");
							}
							else if (args[1] == "Group_3")
							{
								streamWriter.WriteLine("192.168.144.92:5881");
								streamWriter.WriteLine("192.168.144.92:5882");
								streamWriter.WriteLine("192.168.144.92:5883");
								streamWriter.WriteLine("192.168.144.92:5884");
							}
							else if (args[1] == "Group_4")
							{
								streamWriter.WriteLine("192.168.144.94:5881");
								streamWriter.WriteLine("192.168.144.94:5882");
								streamWriter.WriteLine("192.168.144.94:5883");
								streamWriter.WriteLine("192.168.144.94:5884");
							}
						}
						streamWriter.Flush();
						streamWriter.Close();
						expr_1B1.Close();
						return;
					}
					if (args[0] == "configuration")
					{
						string path = Environment.CurrentDirectory + "\\configuration.ini";
						if (File.Exists(path))
						{
							File.Delete(path);
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

		public static void killProcess(string name)
		{
			IntPtr intPtr = Program.CreateToolhelp32Snapshot(2u, 0u);
			List<Program.ProcessEntry32> list = new List<Program.ProcessEntry32>();
			if ((int)intPtr > 0)
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
					if (name.Equals(current.szExeFile))
					{
						try
						{
							Process.GetProcessById((int)current.th32ProcessID).Kill();
						}
						catch (Exception arg_CA_0)
						{
							Console.WriteLine(arg_CA_0.Message);
						}
					}
				}
			}
		}
	}
}
