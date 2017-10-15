using System;
using System.ComponentModel;
using System.Net;

namespace TPCASTWindows
{
	internal interface DownloadCallbackInterface
	{
		void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e);

		void DownloadFileCompleted(object sender, AsyncCompletedEventArgs e);

		void DownloadTimeout(object userToken);
	}
}
