using System;

namespace TPCASTWindows
{
	internal interface SocketUpdateCallback
	{
		void OnUpdateVersionReceive(bool success);

		void OnUpdateMd5Receive(bool success);

		void OnUpdateReady(bool success);

		void OnMd5CheckResult(bool success);

		void OnUpdateFinish(bool success);
	}
}
