using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace TPCASTReboot
{
	internal static class Program
	{
		[STAThread]
		private static void Main(string[] args)
		{
			if (args != null && args.Count<string>() > 0)
			{
				Console.WriteLine("args = " + args[0]);
				if (args[0] == "auto")
				{
					Console.WriteLine("current culture =" + Thread.CurrentThread.CurrentUICulture);
					Console.WriteLine("current culture =" + Thread.CurrentThread.CurrentUICulture);
					Application.EnableVisualStyles();
					Application.SetCompatibleTextRenderingDefault(false);
					Application.Run(new Form1());
				}
			}
		}
	}
}
