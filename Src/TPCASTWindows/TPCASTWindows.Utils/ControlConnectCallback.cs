using System;

namespace TPCASTWindows.Utils
{
	internal interface ControlConnectCallback
	{
		void CheckControlCode(ConnectCode code);

		void DevicesStatusChange(int count, int state);
	}
}
