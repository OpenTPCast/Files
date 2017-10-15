using System;
using System.Runtime.CompilerServices;

namespace TPCASTWindows.Utils
{
	internal class WindowsMessage
	{
		public delegate void OnBaseFormTaskbarCloseClickDelegate();

		[method: CompilerGenerated]
		[CompilerGenerated]
		public static event WindowsMessage.OnBaseFormTaskbarCloseClickDelegate OnBaseFormTaskbarCloseClick;

		public static void BaseFormTaskbarCloseClick()
		{
			WindowsMessage.OnBaseFormTaskbarCloseClickDelegate expr_05 = WindowsMessage.OnBaseFormTaskbarCloseClick;
			if (expr_05 == null)
			{
				return;
			}
			expr_05();
		}
	}
}
