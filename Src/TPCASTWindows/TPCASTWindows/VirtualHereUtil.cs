using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace TPCASTWindows
{
	internal class VirtualHereUtil
	{
		public delegate void OnOutputFileDelegate(string msg);

		public static VirtualHereUtil.OnOutputFileDelegate OnOutputFile;

		public static void openAutoFind()
		{
			VirtualHereUtil.command("tpui64.exe -t \"AUTOFIND\"");
		}

		public static bool isControlLoaded()
		{
			VirtualHereUtil.openAutoFind();
			string output = VirtualHereUtil.command("tpui64.exe -t list");
			if (!string.IsNullOrEmpty(output))
			{
				if (output.Contains("ERROR"))
				{
					VirtualHereUtil.reStartTpui();
					Thread.Sleep(500);
					return VirtualHereUtil.isControlLoaded();
				}
				if (output.Contains("tpcast.1121"))
				{
					return true;
				}
			}
			return false;
		}

		private static void reStartTpui()
		{
			Process expr_05 = new Process();
			expr_05.StartInfo.FileName = "cmd.exe";
			expr_05.StartInfo.WorkingDirectory = ".";
			expr_05.StartInfo.UseShellExecute = false;
			expr_05.StartInfo.RedirectStandardInput = true;
			expr_05.StartInfo.RedirectStandardOutput = true;
			expr_05.StartInfo.RedirectStandardError = true;
			expr_05.StartInfo.CreateNoWindow = true;
			expr_05.Start();
			expr_05.StandardInput.WriteLine("tpui64.exe -n");
			expr_05.StandardInput.AutoFlush = true;
			expr_05.Close();
		}

		public static void connectControl()
		{
			VirtualHereUtil.openAutoFind();
			Process expr_0A = new Process();
			expr_0A.StartInfo.FileName = "cmd.exe";
			expr_0A.StartInfo.WorkingDirectory = ".";
			expr_0A.StartInfo.UseShellExecute = false;
			expr_0A.StartInfo.RedirectStandardInput = true;
			expr_0A.StartInfo.RedirectStandardOutput = true;
			expr_0A.StartInfo.RedirectStandardError = true;
			expr_0A.StartInfo.CreateNoWindow = true;
			expr_0A.Start();
			string outputFile = VirtualHereUtil.getOutputFile();
			expr_0A.StandardInput.WriteLine("tpui64.exe -t \"AUTO USE DEVICE PORT,tpcast.1121\" -r " + outputFile);
			expr_0A.StandardInput.WriteLine("tpui64.exe -t \"AUTO USE DEVICE PORT,tpcast.1127\" -r " + outputFile);
			expr_0A.StandardInput.WriteLine("tpui64.exe -t \"AUTO USE DEVICE PORT,tpcast.1126\" -r " + outputFile);
			expr_0A.StandardInput.WriteLine("tpui64.exe -t \"AUTO USE DEVICE PORT,tpcast.1125\" -r " + outputFile);
			expr_0A.StandardInput.AutoFlush = true;
			expr_0A.Close();
		}

		public static string command(string cmd)
		{
			Process expr_05 = new Process();
			expr_05.StartInfo.FileName = "cmd.exe";
			expr_05.StartInfo.WorkingDirectory = ".";
			expr_05.StartInfo.UseShellExecute = false;
			expr_05.StartInfo.RedirectStandardInput = true;
			expr_05.StartInfo.RedirectStandardOutput = true;
			expr_05.StartInfo.RedirectStandardError = true;
			expr_05.StartInfo.CreateNoWindow = true;
			expr_05.Start();
			string outputFile = VirtualHereUtil.getOutputFile();
			expr_05.StandardInput.WriteLine(cmd + " -r " + outputFile);
			expr_05.StandardInput.AutoFlush = true;
			expr_05.Close();
			expr_05.Dispose();
			Thread.Sleep(1000);
			return VirtualHereUtil.readTextFile(outputFile);
		}

		public static string getOutputFile()
		{
			string outputDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\TPCAST";
			if (!Directory.Exists(outputDir))
			{
				Directory.CreateDirectory(outputDir);
			}
			string outputFile = outputDir + "\\out.txt";
			if (!File.Exists(outputFile))
			{
				File.Create(outputFile).Close();
			}
			return outputFile;
		}

		public static string readTextFile(string path)
		{
			string result;
			using (StreamReader sr = new StreamReader(path, Encoding.Default))
			{
				result = sr.ReadToEnd();
			}
			return result;
		}
	}
}
