using RestSharp;
using System;
using System.Windows.Forms;

namespace TPCASTWindows
{
	internal class Client
	{
		public delegate void UpdateResponseDelegate(IRestResponse<Update> response);

		private static readonly Client instance = new Client();

		private static Control sContext;

		private const string baseUrl = "http://192.168.60.106:7070";

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
			RestClient client = new RestClient("http://192.168.60.106:7070");
			RestRequest restRequest = new RestRequest("service/update", Method.GET);
			restRequest.AddParameter("sv", softwareVersion);
			restRequest.AddParameter("av", adapterVersion);
			restRequest.AddParameter("sn", sn);
			Console.WriteLine(string.Concat(new string[]
			{
				"http://192.168.60.106:7070/service/update?sv=",
				softwareVersion,
				"&av=",
				adapterVersion,
				"&sn=",
				sn
			}));
			client.ExecuteAsync(restRequest, delegate(IRestResponse<Update> callback)
			{
				if (Client.sContext != null && UpdateResponse != null)
				{
					if (Client.sContext.InvokeRequired)
					{
						Client.sContext.Invoke(UpdateResponse, new object[]
						{
							callback
						});
						return;
					}
					UpdateResponse(callback);
				}
			});
		}
	}
}
