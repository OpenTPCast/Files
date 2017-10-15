using NLog;
using System;
using System.ComponentModel;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace TPCASTWindows
{
	internal class DownloadModel
	{
		private delegate void OnDownloadProgressChangedDelegate(object sender, DownloadProgressChangedEventArgs e);

		private delegate void OnDownloadFileCompletedDelegate(object sender, AsyncCompletedEventArgs e);

		private delegate void OnDownloadTimeoutDelegate(object userToken);

		private static Logger log = LogManager.GetCurrentClassLogger();

		private Control context;

		private DownloadCallbackInterface DownloadCallback;

		private WebClient webClient;

		private bool isDownloading;

		private DateTime lastTime;

		private DownloadModel.OnDownloadProgressChangedDelegate OnDownloadProgressChanged;

		private DownloadModel.OnDownloadFileCompletedDelegate OnDownloadFileCompleted;

		private DownloadModel.OnDownloadTimeoutDelegate DownloadTimeout;

		public DownloadModel(Control context)
		{
			this.context = context;
		}

		public void setDownloadCallback(DownloadCallbackInterface c)
		{
			this.DownloadCallback = c;
			this.OnDownloadProgressChanged = new DownloadModel.OnDownloadProgressChangedDelegate(c.DownloadProgressChanged);
			this.OnDownloadFileCompleted = new DownloadModel.OnDownloadFileCompletedDelegate(c.DownloadFileCompleted);
			this.DownloadTimeout = new DownloadModel.OnDownloadTimeoutDelegate(c.DownloadTimeout);
		}

		public void DownloadFile(string address, string fileName, object userToken)
		{
			this.webClient = new WebClient();
			this.webClient.DownloadFileAsync(new Uri(address), fileName, userToken);
			this.webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(this.downloadFileCompleted);
			this.webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(this.downloadProgressChanged);
			this.lastTime = DateTime.Now;
			this.isDownloading = true;
			new Thread(new ParameterizedThreadStart(this.timeOutThreadStart)).Start(userToken);
		}

		private void timeOutThreadStart(object userToken)
		{
			while (this.isDownloading && !this.checkTimeout())
			{
				Thread.Sleep(1000);
			}
			if (this.isDownloading)
			{
				this.timeout(userToken);
			}
		}

		private bool checkTimeout()
		{
			return (DateTime.Now - this.lastTime).Seconds > 15;
		}

		private void downloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
		{
			this.lastTime = DateTime.Now;
			if (e.ProgressPercentage % 10 == 0 && this.context != null && this.OnDownloadProgressChanged != null)
			{
				if (this.context.InvokeRequired)
				{
					this.context.Invoke(this.OnDownloadProgressChanged, new object[]
					{
						sender,
						e
					});
					return;
				}
				this.OnDownloadProgressChanged(sender, e);
			}
		}

		private void downloadFileCompleted(object sender, AsyncCompletedEventArgs e)
		{
			DownloadModel.log.Trace("complete " + e.UserState);
			this.isDownloading = false;
			if (this.webClient != null)
			{
				this.webClient.Dispose();
				this.webClient = null;
			}
			Thread.Sleep(1000);
			if (this.context != null && this.OnDownloadFileCompleted != null)
			{
				if (this.context.InvokeRequired)
				{
					this.context.Invoke(this.OnDownloadFileCompleted, new object[]
					{
						sender,
						e
					});
					return;
				}
				this.OnDownloadFileCompleted(sender, e);
			}
		}

		private void timeout(object userToken)
		{
			if (this.webClient != null)
			{
				this.webClient.CancelAsync();
			}
			if (this.context != null && this.DownloadTimeout != null)
			{
				if (this.context.InvokeRequired)
				{
					this.context.Invoke(this.DownloadTimeout, new object[]
					{
						userToken
					});
					return;
				}
				this.DownloadTimeout(userToken);
			}
		}
	}
}
