using System;

namespace TPCASTWindows
{
	internal interface SocketExceptonCallback
	{
		void OnSendFail();

		void OnReceiveTimeout();
	}
}
