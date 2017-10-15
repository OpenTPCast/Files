using System;

namespace TPCASTWindows
{
	internal interface SocketConnectCallback
	{
		void OnConnected(bool success);

		void OnVersionReceive(string version);

		void OnMacReceive(string mac);
	}
}
