using System;
using System.Threading;
using System.Windows.Forms;

namespace TPCASTWindows
{
	internal static class Program
	{
		public static EventWaitHandle ProgramStarted;

		[STAThread]
		private static void Main(string[] Args)
		{
			if (Args.Length == 0)
			{
				Console.WriteLine("windowssss");
				bool flag;
				Program.ProgramStarted = new EventWaitHandle(false, EventResetMode.AutoReset, "MyStartEvent", out flag);
				if (!flag)
				{
					Program.ProgramStarted.Set();
					return;
				}
				Console.WriteLine("current culture =" + Thread.CurrentThread.CurrentUICulture);
				Console.WriteLine("current culture =" + Thread.CurrentThread.CurrentUICulture);
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.Run(new MainForm());
			}
		}
	}
}
