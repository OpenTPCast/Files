using System;
using System.IO;
using System.Linq;

namespace TPCASTUpdateHelper
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			if (args != null && args.Count<string>() > 0)
			{
				Console.WriteLine("args = " + args[0]);
				if (args[0] == "uninstall" && args.Count<string>() > 1)
				{
					Console.WriteLine("args = " + args[1]);
					string expr_60 = Path.GetTempPath() + "\\TPCAST";
					string path = expr_60 + "\\UpdateFlag";
					Directory.CreateDirectory(expr_60);
					File.Create(path).Close();
					string expr_80 = args[1];
					RegistryUtil.UninstallTPCASTCommand(expr_80);
					if (RegistryUtil.findCode(expr_80))
					{
						Console.WriteLine("find");
					}
					else
					{
						Console.WriteLine("not find");
					}
					if (File.Exists(path))
					{
						File.Delete(path);
					}
					Console.WriteLine("");
				}
			}
		}
	}
}
