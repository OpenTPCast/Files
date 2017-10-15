using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace TPCASTReboot
{
	public class Form1 : Form
	{
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		internal struct TokPriv1Luid
		{
			public int Count;

			public long Luid;

			public int Attr;
		}

		internal const int SE_PRIVILEGE_ENABLED = 2;

		internal const int TOKEN_QUERY = 8;

		internal const int TOKEN_ADJUST_PRIVILEGES = 32;

		internal const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";

		internal const int EWX_LOGOFF = 0;

		internal const int EWX_SHUTDOWN = 1;

		internal const int EWX_REBOOT = 2;

		internal const int EWX_FORCE = 4;

		internal const int EWX_POWEROFF = 8;

		internal const int EWX_FORCEIFHUNG = 16;

		private IContainer components;

		private Button button1;

		private Button button2;

		private Label label1;

		public Form1()
		{
			this.InitializeComponent();
			base.TopMost = true;
			this.checkSoftwareUpdateFile();
		}

		private void checkSoftwareUpdateFile()
		{
			if (File.Exists(Constants.updateSoftwareFilePath) && File.Exists(Constants.updateSoftwareMd5Path))
			{
				string mD5HashFromFile = CryptoUtil.GetMD5HashFromFile(Constants.updateSoftwareFilePath);
				StreamReader expr_2D = new StreamReader(Constants.updateSoftwareMd5Path);
				string text = expr_2D.ReadLine();
				expr_2D.Close();
				expr_2D.Dispose();
				if (text != null && text.Equals(mD5HashFromFile))
				{
					RegistryUtil.addAutoRunOnce(Constants.updateSoftwareFilePath);
				}
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			new Thread(new ThreadStart(this.rebootStart)).Start();
			base.Close();
		}

		private void rebootStart()
		{
			if (File.Exists(Constants.updateSoftwareMd5Path))
			{
				File.Delete(Constants.updateSoftwareMd5Path);
			}
			Form1.Reboot();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		[DllImport("kernel32.dll", ExactSpelling = true)]
		internal static extern IntPtr GetCurrentProcess();

		[DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
		internal static extern bool OpenProcessToken(IntPtr h, int acc, ref IntPtr phtok);

		[DllImport("advapi32.dll", SetLastError = true)]
		internal static extern bool LookupPrivilegeValue(string host, string name, ref long pluid);

		[DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
		internal static extern bool AdjustTokenPrivileges(IntPtr htok, bool disall, ref Form1.TokPriv1Luid newst, int len, IntPtr prev, IntPtr relen);

		[DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
		internal static extern bool ExitWindowsEx(int DoFlag, int rea);

		private static void DoExitWin(int DoFlag)
		{
			IntPtr arg_0F_0 = Form1.GetCurrentProcess();
			IntPtr zero = IntPtr.Zero;
			Form1.OpenProcessToken(arg_0F_0, 40, ref zero);
			Form1.TokPriv1Luid tokPriv1Luid;
			tokPriv1Luid.Count = 1;
			tokPriv1Luid.Luid = 0L;
			tokPriv1Luid.Attr = 2;
			Form1.LookupPrivilegeValue(null, "SeShutdownPrivilege", ref tokPriv1Luid.Luid);
			Form1.AdjustTokenPrivileges(zero, false, ref tokPriv1Luid, 0, IntPtr.Zero, IntPtr.Zero);
			Form1.ExitWindowsEx(DoFlag, 0);
		}

		public static void Reboot()
		{
			Form1.DoExitWin(6);
		}

		public static void PowerOff()
		{
			Form1.DoExitWin(12);
		}

		public static void LogOff()
		{
			Form1.DoExitWin(4);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			ComponentResourceManager arg_36_0 = new ComponentResourceManager(typeof(Form1));
			this.button1 = new Button();
			this.button2 = new Button();
			this.label1 = new Label();
			base.SuspendLayout();
			arg_36_0.ApplyResources(this.button1, "button1");
			this.button1.Name = "button1";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new EventHandler(this.button1_Click);
			arg_36_0.ApplyResources(this.button2, "button2");
			this.button2.Name = "button2";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new EventHandler(this.button2_Click);
			arg_36_0.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			arg_36_0.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			base.Controls.Add(this.label1);
			base.Controls.Add(this.button2);
			base.Controls.Add(this.button1);
			base.FormBorderStyle = FormBorderStyle.FixedToolWindow;
			base.Name = "Form1";
			base.ShowIcon = false;
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
