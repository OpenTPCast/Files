using NLog;
using System;
using System.Threading;
using System.Windows.Forms;

namespace TPCASTWindows
{
	internal static class Program
	{
		private static Logger log = LogManager.GetCurrentClassLogger();

		public static EventWaitHandle ProgramStarted;

		[STAThread]
		private static void Main(string[] Args)
		{
			if (Args.Length == 0)
			{
				bool createNew;
				Program.ProgramStarted = new EventWaitHandle(false, EventResetMode.AutoReset, "MyStartEvent", out createNew);
				if (!createNew)
				{
					Program.ProgramStarted.Set();
					return;
				}
				Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
				Application.ThreadException += new ThreadExceptionEventHandler(Program.Application_ThreadException);
				AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(Program.CurrentDomain_UnhandledException);
				Program.log.Trace("current culture =" + Thread.CurrentThread.CurrentUICulture);
				Program.log.Trace("current culture =" + Thread.CurrentThread.CurrentUICulture);
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.Run(new MainForm());
			}
		}

		private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
		{
			Exception ex = e.Exception;
			if (ex != null)
			{
				Program.log.Error("Application_ThreadException " + ex.Message + "\r\n" + ex.StackTrace);
			}
		}

		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Exception ex = e.ExceptionObject as Exception;
			if (ex != null)
			{
				Program.log.Error("CurrentDomain_UnhandledException " + ex.Message + "\r\n" + ex.StackTrace);
			}
		}
	}
}
