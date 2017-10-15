using NLog;
using RestSharp;
using System;
using System.Windows.Forms;

namespace TPCASTWindows
{
	internal class Client
	{
		public delegate void UpdateResponseDelegate(IRestResponse<Update> response);

		private static Logger log = LogManager.GetCurrentClassLogger();

		private static readonly Client instance = new Client();

		private static Control sContext;

		private const string baseUrl = "http://114.215.220.124:7070";

		private Client()
		{
		}

		public static Client getInstance()
		{
			return Client.instance;
		}

		public static void init(Control context)
		{
			Client.sContext = context;
		}

		public void requestUpdate(string softwareVersion, string adapterVersion, string sn, Client.UpdateResponseDelegate UpdateResponse)
		{
			RestClient client = new RestClient("http://114.215.220.124:7070");
			client.Timeout = 1000;
			RestRequest request = new RestRequest("service/update", Method.GET);
			request.AddParameter("sv", softwareVersion);
			request.AddParameter("av", adapterVersion);
			request.AddParameter("sn", sn);
			string vt = "CE";
			request.AddParameter("vt", vt);
			Client.log.Trace(string.Concat(new string[]
			{
				"http://114.215.220.124:7070/service/update?sv=",
				softwareVersion,
				"&av=",
				adapterVersion,
				"&sn=",
				sn,
				"&vt=",
				vt
			}));
			client.ExecuteAsync(request, delegate(IRestResponse<Update> callback)
			{
				if (Client.sContext != null && UpdateResponse != null)
				{
					if (Client.sContext.InvokeRequired)
					{
						try
						{
							Client.sContext.Invoke(UpdateResponse, new object[]
							{
								callback
							});
							return;
						}
						catch (Exception e)
						{
							Client.log.Trace("error = = " + e.Message + e.StackTrace);
							return;
						}
					}
					UpdateResponse(callback);
				}
			});
		}
	}
}
