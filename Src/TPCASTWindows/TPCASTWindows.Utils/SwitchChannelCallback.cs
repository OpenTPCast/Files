using System;

namespace TPCASTWindows.Utils
{
	internal interface SwitchChannelCallback
	{
		void OnCheckRouterChannelFinishListener(bool isOurRouter);

		void OnChannelSwitched();

		void OnSwitchCheckError(int error);
	}
}
