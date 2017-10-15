using System;

namespace TPCASTWindows.Utils
{
	internal interface ConnectReloadCallback
	{
		void OnHostConnected();

		void OnRouterConnected();

		void OnReloadCheckError(int error);
	}
}
