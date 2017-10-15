using NLog;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TPCASTWindows
{
	internal class BroadcastModel
	{
		private class PackageMessage
		{
			public byte type;

			public string message = "";
		}

		public const int ROUTER_FAIL = 11;

		public const int ROUTER_FAIL_LOOP = 12;

		public const int RASPBERRY_FAIL = 21;

		public const int RASPBERRY_FAIL_LOOP = 22;

		public const int CABLE_FAIL = 31;

		public const int CONTROL_DISCONNECT = 41;

		public const int CONTROL_DISCONNECT_LOOP = 42;

		public const int LOOP_SUCCESS = 100;

		public const int DRIVER_FAIL = 91;

		public const int SERVICE_FAIL = 92;

		public const int REBOOT = 93;

		public const int SUCCESS = 100;

		public const int NONE = 0;

		private static Logger log = LogManager.GetCurrentClassLogger();

		public static readonly BroadcastModel instance = new BroadcastModel();

		private Thread listenThread;

		private string endPoint = "";

		private Socket listenSocket;

		private bool listenAborted;

		private Socket broadcastSocket;

		private string host;

		private int port = 48764;

		private const int TIMEOUT_RECEIVE = 5000;

		private const int TIMEOUT_SEND = 5000;

		private const int TIMEOUT_CONNECT = 1000;

		private Thread heartbeatThread;

		private int message;

		private byte HEADER_1 = 126;

		private byte HEADER_2 = 254;

		private byte MESSAGE = 1;

		private byte HEARTBEAT = 3;

		private byte DEBUG = 144;

		private byte NOT_SUPPORT = 255;

		private BroadcastModel()
		{
		}

		public void startListen(int n)
		{
			BroadcastModel.log.Trace("startListen " + n);
			this.listenThread = new Thread(new ThreadStart(this.listenThreadStart));
			this.listenThread.Start();
		}

		private void AbortListenThread()
		{
			if (this.listenThread != null)
			{
				BroadcastModel.log.Trace("b la");
				this.listenThread.Abort();
				BroadcastModel.log.Trace("a la");
			}
		}

		private void listenThreadStart()
		{
			BroadcastModel.log.Trace("listenThreadStart");
			this.closeListenSocket();
			this.listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			IPEndPoint ipe = new IPEndPoint(IPAddress.Any, 48765);
			try
			{
				this.listenSocket.Bind(ipe);
				BroadcastModel.log.Trace("ipa = " + ipe.Address);
				EndPoint ep = ipe;
				byte[] data = new byte[1024];
				this.startListen(data, ep);
			}
			catch (Exception ex)
			{
				BroadcastModel.log.Error("bind socket e= " + ex.Message + ex.StackTrace);
				this.closeListenSocket();
				this.startListen(2);
			}
		}

		private void startListen(byte[] data, EndPoint ep)
		{
			while (true)
			{
				try
				{
					int recv = this.listenSocket.ReceiveFrom(data, ref ep);
					if (!this.listenAborted)
					{
						if (string.IsNullOrEmpty(this.endPoint) || !this.endPoint.Equals(ep.ToString()) || this.broadcastSocket == null)
						{
							this.endPoint = ep.ToString();
							string stringData = Encoding.Default.GetString(data, 0, recv);
							BroadcastModel.log.Trace<string, string>("received: {0} from: {1}", stringData, ep.ToString());
							string[] stringDatas = stringData.Split(new string[]
							{
								":"
							}, StringSplitOptions.None);
							if (stringDatas != null && stringDatas.Count<string>() > 1)
							{
								this.host = stringDatas[0];
								this.port = int.Parse(stringDatas[1]);
								this.listenAborted = true;
								this.AbortHeartbeatThread();
								this.closeBroadcastSocket();
								this.connectBroadcastSocket();
								BroadcastModel.log.Trace("listen abort");
							}
						}
						string receiveData = Encoding.Default.GetString(data, 0, recv);
						BroadcastModel.log.Trace<string, string>("receiveData: {0} from: {1}", receiveData, ep.ToString());
					}
					else
					{
						BroadcastModel.log.Trace("not listen");
					}
					continue;
				}
				catch (Exception e)
				{
					BroadcastModel.log.Error("listenThreadStart e= " + e.Message + e.StackTrace);
					this.closeListenSocket();
					this.startListen(1);
				}
				break;
			}
			BroadcastModel.log.Trace("while abort close");
		}

		private void closeListenSocket()
		{
			if (this.listenSocket != null)
			{
				this.listenSocket.Close();
				this.listenSocket = null;
			}
		}

		private void connectBroadcastSocket()
		{
			BroadcastModel.log.Trace("connectBroadcastSocket");
			BroadcastModel.log.Trace("socket == " + this.broadcastSocket);
			if (this.broadcastSocket == null || !this.broadcastSocket.Connected)
			{
				new Thread(new ThreadStart(this.connectBroadcastSocketThreadStart)).Start();
			}
		}

		private void closeBroadcastSocket()
		{
			if (this.broadcastSocket != null)
			{
				this.broadcastSocket.Close();
				this.broadcastSocket.Dispose();
				this.broadcastSocket = null;
			}
		}

		private void connectBroadcastSocketThreadStart()
		{
			try
			{
				BroadcastModel.log.Trace("connectBroadcastSocketThreadStart");
				IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(this.host), this.port);
				BroadcastModel.log.Trace("new socket");
				this.broadcastSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				this.broadcastSocket.ReceiveTimeout = 5000;
				BroadcastModel.log.Trace("socket begin connect");
				IAsyncResult ar = this.broadcastSocket.BeginConnect(ipe, new AsyncCallback(this.connectCallback), this.broadcastSocket);
				ar.AsyncWaitHandle.WaitOne(1000);
				if (ar.IsCompleted)
				{
					BroadcastModel.log.Trace("socket connect IsCompleted");
					BroadcastModel.log.Trace("socket connected = " + ((Socket)ar.AsyncState).Connected.ToString());
					if (((Socket)ar.AsyncState).Connected)
					{
						this.sendHeartBeat();
						this.sendLastMessage();
					}
				}
				else
				{
					BroadcastModel.log.Trace("socket connect notCompleted");
					this.closeBroadcastSocket();
				}
				this.listenAborted = false;
			}
			catch (Exception e)
			{
				BroadcastModel.log.Trace("connectBroadcastSocketThreadStart e = " + e);
				this.closeBroadcastSocket();
				this.listenAborted = false;
			}
		}

		private void connectCallback(IAsyncResult ar)
		{
			BroadcastModel.log.Trace("connectCallback" + ar);
		}

		private void sendHeartBeat()
		{
			BroadcastModel.log.Trace("sendHeartBeat ");
			this.heartbeatThread = new Thread(new ThreadStart(this.heartbeatThreadStart));
			this.heartbeatThread.Start();
		}

		private void AbortHeartbeatThread()
		{
			BroadcastModel.log.Trace("AbortHeartbeatThread");
			if (this.heartbeatThread != null)
			{
				this.heartbeatThread.Abort();
			}
		}

		private void heartbeatThreadStart()
		{
			BroadcastModel.log.Trace("heartbeatThreadStart ");
			while (this.broadcastSocket != null)
			{
				byte type = 0;
				switch (ConfigureUtil.getClientType())
				{
				case ClientType.TYPE_A:
					type = 1;
					break;
				case ClientType.TYPE_B:
					type = 2;
					break;
				case ClientType.TYPE_C:
					type = 3;
					break;
				case ClientType.TYPE_D:
					type = 4;
					break;
				}
				byte[] data = new byte[]
				{
					type
				};
				this.send(this.HEARTBEAT, data);
				Thread.Sleep(1000);
			}
		}

		public void send(int msg)
		{
			BroadcastModel.log.Trace("send " + msg);
			this.message = msg;
			byte type = 0;
			switch (ConfigureUtil.getClientType())
			{
			case ClientType.TYPE_A:
				type = 1;
				break;
			case ClientType.TYPE_B:
				type = 2;
				break;
			case ClientType.TYPE_C:
				type = 3;
				break;
			case ClientType.TYPE_D:
				type = 4;
				break;
			}
			byte[] data = new byte[]
			{
				type
			};
			this.send(this.MESSAGE, this.appendData(data, msg));
		}

		private void sendLastMessage()
		{
			BroadcastModel.log.Trace("sendLastMessage " + this.message);
			if (this.message != 0)
			{
				this.send(this.message);
			}
		}

		public void send(byte type, byte[] message)
		{
			byte[] buffer = this.buildBuffer(type, message);
			if (this.broadcastSocket != null)
			{
				try
				{
					this.broadcastSocket.SendTimeout = 5000;
					this.broadcastSocket.Send(buffer);
					return;
				}
				catch (Exception e)
				{
					BroadcastModel.log.Error("send e = " + e);
					this.sendFail();
					return;
				}
			}
			BroadcastModel.log.Error("socket == null");
		}

		private void sendFail()
		{
			this.AbortHeartbeatThread();
			this.closeBroadcastSocket();
			this.listenAborted = false;
		}

		private byte[] buildBuffer(byte type, byte[] message)
		{
			byte[] buffer = this.buildBufferWithHeader();
			buffer = this.appendData(buffer, new byte[1]);
			buffer = this.appendData(buffer, type);
			buffer = this.appendData(buffer, message.Length);
			buffer = this.appendData(buffer, message);
			byte[] arg_3D_1 = buffer;
			byte[] expr_35 = buffer;
			return this.appendData(arg_3D_1, BroadcastModel.computeCheckSum(expr_35, expr_35.Length));
		}

		private byte[] buildBufferWithHeader()
		{
			return new byte[]
			{
				this.HEADER_1
			}.Concat(new byte[]
			{
				this.HEADER_2
			}).ToArray<byte>();
		}

		private byte[] appendData(byte[] data, byte[] appendedData)
		{
			return data.Concat(appendedData).ToArray<byte>();
		}

		private byte[] appendData(byte[] data, int appendedData)
		{
			return this.appendData(data, (byte)appendedData);
		}

		private byte[] appendData(byte[] data, byte appendedData)
		{
			return data.Concat(new byte[]
			{
				appendedData
			}).ToArray<byte>();
		}

		private bool isCheckSum(byte[] recBytes)
		{
			int checkSum = (int)BroadcastModel.computeCheckSum(recBytes, recBytes.Length - 1);
			return Convert.ToInt32(recBytes.Skip(recBytes.Length - 1).Take(1).ToArray<byte>().GetValue(0)) == checkSum;
		}

		private static byte computeCheckSum(byte[] buff, int len)
		{
			int chksum = 0;
			for (int i = 0; i < len; i++)
			{
				chksum += (int)(buff[i] & Convert.ToByte(255));
			}
			while (chksum >> 8 != 0)
			{
				chksum = (chksum >> 8) + (chksum & (int)Convert.ToByte(255));
			}
			return (byte)(~chksum & (int)Convert.ToByte(255));
		}
	}
}
