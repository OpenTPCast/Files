using System;

namespace TPCASTWindows.Utils
{
	internal interface ConnectLoopInterruptCallback
	{
		void OnControlInterrupt(int status);
	}
}
