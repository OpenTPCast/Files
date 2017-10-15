using System;

namespace TPCASTWindows.Utils
{
	internal interface ConnectStatusCallback
	{
		void OnInitCheckRouterSSIDFinish(bool modified);

		void OnInitCheckRouterFail();

		void BeginCheckControl();

		void OnCheckRouterSSIDFinish(bool modified);

		void OnCheckRouterConnected();

		void OnCheckHostConnect();

		void OnCheckControlError(int status);
	}
}
