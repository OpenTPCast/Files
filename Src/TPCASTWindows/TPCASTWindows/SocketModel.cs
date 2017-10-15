using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace TPCASTWindows
{
	internal class SocketModel
	{
		private delegate void OnConnectedDelegate(bool success);

		private delegate void OnVersionReceiveDelegate(string version);

		private delegate void OnMacReceiveDelegate(string mac);

		private delegate void OnUpdateVersionReceiveDelegate(bool success);

		private delegate void OnUpdateMd5ReceiveDelegate(bool success);

		private delegate void OnUpdateReadyDelegate(bool success);

		private delegate void OnMd5CheckResultDelegate(bool success);

		private delegate void OnUpdateFinishDelegate(bool success);

		private delegate void OnRecoverResultDelegate(bool success);

		private delegate void OnSendFailDelegate();

		private delegate void OnReceiveTimeoutDelegate();

		private class PackageMessage
		{
			public byte type;

			public string message = "";
		}

		private Control context;

		private List<SocketConnectCallback> socketConnectCallbacks = new List<SocketConnectCallback>();

		private List<SocketUpdateCallback> socketUpdateCallbacks = new List<SocketUpdateCallback>();

		private List<SocketExceptonCallback> socketExceptionCallbacks = new List<SocketExceptonCallback>();

		private SocketModel.OnConnectedDelegate OnConnected;

		private SocketModel.OnVersionReceiveDelegate OnVersionReceive;

		private SocketModel.OnMacReceiveDelegate OnMacReceive;

		private SocketModel.OnUpdateVersionReceiveDelegate OnUpdateVersionReceive;

		private SocketModel.OnUpdateMd5ReceiveDelegate OnUpdateMd5Receive;

		private SocketModel.OnUpdateReadyDelegate OnUpdateReady;

		private SocketModel.OnMd5CheckResultDelegate OnMd5CheckResult;

		private SocketModel.OnUpdateFinishDelegate OnUpdateFinish;

		private SocketModel.OnRecoverResultDelegate OnRecoverResult;

		private SocketModel.OnSendFailDelegate OnSendFail;

		private SocketModel.OnReceiveTimeoutDelegate OnReceiveTimeout;

		private int commandPort = 8000;

		private int fileTransPort = 8001;

		private string host = "192.168.1.88";

		private Socket socket;

		private Thread connectThread;

		private Thread recieveThread;

		private bool isConnected;

		private byte HEADER_1 = 126;

		private byte HEADER_2 = 254;

		private byte VERSION_REQUEST = 1;

		private byte VERSION_RECEIVE = 2;

		private byte UPDATE_VERSION = 3;

		private byte UPDATE_VERSION_RECEIVE = 4;

		private byte UPDATE_MD5 = 5;

		private byte UPDATE_MD5_RECEIVE = 6;

		private byte MAC_REQUEST = 7;

		private byte MAC_RECEIVE = 8;

		private byte PREPARE_UPDATE = 16;

		private byte STADNBY = 17;

		private byte TRANS_FINISH = 18;

		private byte MD5_CHECK_RESULT = 19;

		private byte UPDATE_RESULT = 32;

		private byte ERROR_CODE = 64;

		private byte RECOVER_ORIGIN_VERSION = 128;

		private byte RECOVER_RESULT = 129;

		private byte DEBUG = 144;

		private byte NOT_SUPPORT = 255;

		private SocketModel()
		{
		}

		public SocketModel(Control context)
		{
			this.context = context;
		}

		public void addSocketConnectCallback(SocketConnectCallback c)
		{
			this.socketConnectCallbacks.Add(c);
			this.OnConnected = (SocketModel.OnConnectedDelegate)Delegate.Combine(this.OnConnected, new SocketModel.OnConnectedDelegate(c.OnConnected));
			this.OnVersionReceive = (SocketModel.OnVersionReceiveDelegate)Delegate.Combine(this.OnVersionReceive, new SocketModel.OnVersionReceiveDelegate(c.OnVersionReceive));
			this.OnMacReceive = (SocketModel.OnMacReceiveDelegate)Delegate.Combine(this.OnMacReceive, new SocketModel.OnMacReceiveDelegate(c.OnMacReceive));
		}

		public void removeSocketConnectCallback(SocketConnectCallback c)
		{
			this.socketConnectCallbacks.Remove(c);
			this.OnConnected = (SocketModel.OnConnectedDelegate)Delegate.Remove(this.OnConnected, new SocketModel.OnConnectedDelegate(c.OnConnected));
			this.OnVersionReceive = (SocketModel.OnVersionReceiveDelegate)Delegate.Remove(this.OnVersionReceive, new SocketModel.OnVersionReceiveDelegate(c.OnVersionReceive));
			this.OnMacReceive = (SocketModel.OnMacReceiveDelegate)Delegate.Remove(this.OnMacReceive, new SocketModel.OnMacReceiveDelegate(c.OnMacReceive));
		}

		public void addSocketUpdateCallback(SocketUpdateCallback c)
		{
			this.socketUpdateCallbacks.Add(c);
			this.OnUpdateVersionReceive = (SocketModel.OnUpdateVersionReceiveDelegate)Delegate.Combine(this.OnUpdateVersionReceive, new SocketModel.OnUpdateVersionReceiveDelegate(c.OnUpdateVersionReceive));
			this.OnUpdateMd5Receive = (SocketModel.OnUpdateMd5ReceiveDelegate)Delegate.Combine(this.OnUpdateMd5Receive, new SocketModel.OnUpdateMd5ReceiveDelegate(c.OnUpdateMd5Receive));
			this.OnUpdateReady = (SocketModel.OnUpdateReadyDelegate)Delegate.Combine(this.OnUpdateReady, new SocketModel.OnUpdateReadyDelegate(c.OnUpdateReady));
			this.OnMd5CheckResult = (SocketModel.OnMd5CheckResultDelegate)Delegate.Combine(this.OnMd5CheckResult, new SocketModel.OnMd5CheckResultDelegate(c.OnMd5CheckResult));
			this.OnUpdateFinish = (SocketModel.OnUpdateFinishDelegate)Delegate.Combine(this.OnUpdateFinish, new SocketModel.OnUpdateFinishDelegate(c.OnUpdateFinish));
		}

		public void removeSocketUpdateCallback(SocketUpdateCallback c)
		{
			this.socketUpdateCallbacks.Remove(c);
			this.OnUpdateVersionReceive = (SocketModel.OnUpdateVersionReceiveDelegate)Delegate.Remove(this.OnUpdateVersionReceive, new SocketModel.OnUpdateVersionReceiveDelegate(c.OnUpdateVersionReceive));
			this.OnUpdateMd5Receive = (SocketModel.OnUpdateMd5ReceiveDelegate)Delegate.Remove(this.OnUpdateMd5Receive, new SocketModel.OnUpdateMd5ReceiveDelegate(c.OnUpdateMd5Receive));
			this.OnUpdateReady = (SocketModel.OnUpdateReadyDelegate)Delegate.Remove(this.OnUpdateReady, new SocketModel.OnUpdateReadyDelegate(c.OnUpdateReady));
			this.OnMd5CheckResult = (SocketModel.OnMd5CheckResultDelegate)Delegate.Remove(this.OnMd5CheckResult, new SocketModel.OnMd5CheckResultDelegate(c.OnMd5CheckResult));
			this.OnUpdateFinish = (SocketModel.OnUpdateFinishDelegate)Delegate.Remove(this.OnUpdateFinish, new SocketModel.OnUpdateFinishDelegate(c.OnUpdateFinish));
		}

		public void addSocketExceptionCallback(SocketExceptonCallback c)
		{
			this.socketExceptionCallbacks.Add(c);
			this.OnSendFail = (SocketModel.OnSendFailDelegate)Delegate.Combine(this.OnSendFail, new SocketModel.OnSendFailDelegate(c.OnSendFail));
			this.OnReceiveTimeout = (SocketModel.OnReceiveTimeoutDelegate)Delegate.Combine(this.OnReceiveTimeout, new SocketModel.OnReceiveTimeoutDelegate(c.OnReceiveTimeout));
			Console.WriteLine("count = " + this.socketExceptionCallbacks.Count);
		}

		public void removeSocketExceptionCallback(SocketExceptonCallback c)
		{
			this.socketExceptionCallbacks.Remove(c);
			this.OnSendFail = (SocketModel.OnSendFailDelegate)Delegate.Remove(this.OnSendFail, new SocketModel.OnSendFailDelegate(c.OnSendFail));
			this.OnReceiveTimeout = (SocketModel.OnReceiveTimeoutDelegate)Delegate.Remove(this.OnReceiveTimeout, new SocketModel.OnReceiveTimeoutDelegate(c.OnReceiveTimeout));
		}

		private void connected(bool success)
		{
			if (this.context != null && this.OnConnected != null)
			{
				if (this.context.InvokeRequired)
				{
					this.context.Invoke(this.OnConnected, new object[]
					{
						success
					});
					return;
				}
				this.OnConnected(success);
			}
		}

		private void versionReceive(string version)
		{
			if (this.context != null && this.OnVersionReceive != null)
			{
				if (this.context.InvokeRequired)
				{
					this.context.Invoke(this.OnVersionReceive, new object[]
					{
						version
					});
					return;
				}
				this.OnVersionReceive(version);
			}
		}

		private void macReceive(string mac)
		{
			if (this.context != null && this.OnMacReceive != null)
			{
				if (this.context.InvokeRequired)
				{
					this.context.Invoke(this.OnMacReceive, new object[]
					{
						mac
					});
					return;
				}
				this.OnMacReceive(mac);
			}
		}

		private void updateVersionReceive(bool success)
		{
			if (this.context != null && this.OnUpdateVersionReceive != null)
			{
				if (this.context.InvokeRequired)
				{
					this.context.Invoke(this.OnUpdateVersionReceive, new object[]
					{
						success
					});
					return;
				}
				this.OnUpdateVersionReceive(success);
			}
		}

		private void updateMd5Receive(bool success)
		{
			if (this.context != null && this.OnUpdateMd5Receive != null)
			{
				if (this.context.InvokeRequired)
				{
					this.context.Invoke(this.OnUpdateMd5Receive, new object[]
					{
						success
					});
					return;
				}
				this.OnUpdateMd5Receive(success);
			}
		}

		private void updateReady(bool success)
		{
			if (this.context != null && this.OnUpdateReady != null)
			{
				if (this.context.InvokeRequired)
				{
					this.context.Invoke(this.OnUpdateReady, new object[]
					{
						success
					});
					return;
				}
				this.OnUpdateReady(success);
			}
		}

		private void md5CheckResult(bool success)
		{
			if (this.context != null && this.OnMd5CheckResult != null)
			{
				if (this.context.InvokeRequired)
				{
					this.context.Invoke(this.OnMd5CheckResult, new object[]
					{
						success
					});
					return;
				}
				this.OnMd5CheckResult(success);
			}
		}

		private void updateFinish(bool success)
		{
			if (this.context != null && this.OnUpdateFinish != null)
			{
				if (this.context.InvokeRequired)
				{
					this.context.Invoke(this.OnUpdateFinish, new object[]
					{
						success
					});
					return;
				}
				this.OnUpdateFinish(success);
			}
		}

		private void recoverResult(bool success)
		{
			if (this.context != null && this.OnUpdateFinish != null)
			{
				if (this.context.InvokeRequired)
				{
					this.context.Invoke(this.OnUpdateFinish, new object[]
					{
						success
					});
					return;
				}
				this.OnUpdateFinish(success);
			}
		}

		private void sendFail()
		{
			if (this.context != null && this.OnSendFail != null)
			{
				if (this.context.InvokeRequired)
				{
					this.context.Invoke(this.OnSendFail);
					return;
				}
				this.OnSendFail();
			}
		}

		private void receiveTimeout()
		{
			if (this.context != null && this.OnSendFail != null)
			{
				if (this.context.InvokeRequired)
				{
					this.context.Invoke(this.OnReceiveTimeout);
					return;
				}
				this.OnReceiveTimeout();
			}
		}

		public void connect()
		{
			Console.WriteLine("connect");
			if (!this.isConnected)
			{
				this.connectThread = new Thread(new ThreadStart(this.connectThreadStart));
				this.connectThread.Start();
				return;
			}
			this.connected(true);
		}

		private void connectThreadStart()
		{
			try
			{
				Console.WriteLine("connectThreadStart");
				IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(this.host), this.commandPort);
				this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				this.socket.Connect(remoteEP);
				this.socket.ReceiveTimeout = 5000;
				this.isConnected = true;
				this.connected(true);
			}
			catch (Exception arg)
			{
				Console.WriteLine("e = " + arg);
				this.connected(false);
			}
		}

		private void abortConnectThread()
		{
			if (this.connectThread != null)
			{
				this.connectThread.Abort();
			}
		}

		private void abortRecieveThread()
		{
			if (this.recieveThread != null)
			{
				this.recieveThread.Abort();
			}
		}

		public void disconnect()
		{
			this.abortConnectThread();
			this.abortRecieveThread();
			this.isConnected = false;
			if (this.socket != null)
			{
				this.socket.Close();
				this.socket.Dispose();
				this.socket = null;
			}
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
			int num = (int)SocketModel.computeCheckSum(recBytes, recBytes.Length - 1);
			return Convert.ToInt32(recBytes.Skip(recBytes.Length - 1).Take(1).ToArray<byte>().GetValue(0)) == num;
		}

		public void getVerion()
		{
			Console.WriteLine("getVerion");
			this.send(this.VERSION_REQUEST, "");
		}

		public void getMac()
		{
			Console.WriteLine("getMac");
			this.send(this.MAC_REQUEST, "");
		}

		public void transUpdateVersion(string updateVersion)
		{
			Console.WriteLine("transUpdateVersion " + updateVersion);
			this.send(this.UPDATE_VERSION, updateVersion);
		}

		public void transUpdateMd5(string updateMd5)
		{
			Console.WriteLine("transUpdateMd5 " + updateMd5);
			this.send(this.UPDATE_MD5, updateMd5);
		}

		public void prepareUpdate()
		{
			Console.WriteLine("prepareUpdate");
			this.send(this.PREPARE_UPDATE, "");
		}

		public void transFinish()
		{
			Console.WriteLine("transFinish");
			this.sendUpdateFinish(this.TRANS_FINISH, "");
		}

		public void recoverOriginVersion()
		{
			Console.WriteLine("recoverOriginVersion");
			this.send(this.RECOVER_ORIGIN_VERSION, "");
		}

		public void debugMessage()
		{
			Console.WriteLine("debugMessage");
			this.send(this.DEBUG, "");
		}

		private void send(byte type, string message = "")
		{
			SocketModel.PackageMessage packageMessage = new SocketModel.PackageMessage();
			packageMessage.type = type;
			packageMessage.message = message;
			new Thread(new ParameterizedThreadStart(this.sendThreadStart)).Start(packageMessage);
		}

		private void sendThreadStart(object obj)
		{
			if (obj is SocketModel.PackageMessage)
			{
				SocketModel.PackageMessage expr_0E = (SocketModel.PackageMessage)obj;
				byte type = expr_0E.type;
				string message = expr_0E.message;
				byte[] buffer = this.buildBuffer(type, message);
				if (this.socket != null)
				{
					try
					{
						this.socket.Send(buffer);
					}
					catch (SocketException arg)
					{
						Console.WriteLine("send e = " + arg);
						this.sendFail();
					}
					this.socket.ReceiveTimeout = 5000;
					this.receiveData();
				}
			}
		}

		private void sendUpdateFinish(byte type, string message = "")
		{
			SocketModel.PackageMessage packageMessage = new SocketModel.PackageMessage();
			packageMessage.type = type;
			packageMessage.message = message;
			new Thread(new ParameterizedThreadStart(this.sendUpdateThreadStart)).Start(packageMessage);
		}

		private void sendUpdateThreadStart(object obj)
		{
			if (obj is SocketModel.PackageMessage)
			{
				SocketModel.PackageMessage expr_0E = (SocketModel.PackageMessage)obj;
				byte type = expr_0E.type;
				string message = expr_0E.message;
				byte[] buffer = this.buildBuffer(type, message);
				if (this.socket != null)
				{
					try
					{
						this.socket.Send(buffer);
					}
					catch (SocketException arg)
					{
						Console.WriteLine("send e = " + arg);
						this.sendFail();
					}
					this.socket.ReceiveTimeout = 5000;
					this.receiveData();
					this.socket.ReceiveTimeout = 60000;
					this.receiveData();
				}
			}
		}

		private byte[] buildBuffer(byte type, string message = "")
		{
			byte[] array = this.buildBufferWithHeader();
			array = this.appendData(array, new byte[1]);
			array = this.appendData(array, type);
			byte[] array2;
			if (string.IsNullOrEmpty(message))
			{
				array2 = new byte[1];
			}
			else
			{
				array2 = Encoding.Default.GetBytes(message);
			}
			array = this.appendData(array, array2.Length);
			array = this.appendData(array, array2);
			byte[] arg_5A_1 = array;
			byte[] expr_52 = array;
			return this.appendData(arg_5A_1, SocketModel.computeCheckSum(expr_52, expr_52.Length));
		}

		private void receiveData()
		{
			try
			{
				byte[] array = new byte[64];
				Socket arg_13_0 = this.socket;
				byte[] expr_0F = array;
				int num = arg_13_0.Receive(expr_0F, expr_0F.Length, SocketFlags.None);
				if (num > 0)
				{
					Console.WriteLine("read bytes = " + num);
					array = array.Take(num).ToArray<byte>();
					Console.WriteLine("cut bytes = " + array.Count<byte>());
					if (this.isCheckSum(array))
					{
						Console.WriteLine("check sum ok");
						this.parseData(array);
					}
					else
					{
						Console.WriteLine("check sum no");
					}
				}
			}
			catch (Exception arg)
			{
				Console.WriteLine("error = " + arg);
				this.receiveTimeout();
				if (this.socket != null)
				{
					this.socket.Close();
					this.socket = null;
				}
			}
		}

		private void receiveThreadStart(object socketObject)
		{
			if (socketObject is Socket)
			{
				Socket socket = (Socket)socketObject;
				while (true)
				{
					try
					{
						byte[] array = new byte[64];
						Socket arg_1E_0 = socket;
						byte[] expr_1A = array;
						int count = arg_1E_0.Receive(expr_1A, expr_1A.Length, SocketFlags.None);
						array = array.Take(count).ToArray<byte>();
						if (this.isCheckSum(array))
						{
							Console.WriteLine("check sum ok");
							this.parseData(array);
						}
						else
						{
							Console.WriteLine("check sum no");
						}
						continue;
					}
					catch (Exception arg)
					{
						Console.WriteLine("error = " + arg);
						this.receiveTimeout();
						if (socket != null)
						{
							socket.Close();
							socket = null;
						}
					}
					break;
				}
			}
		}

		private void parseData(byte[] recBytes)
		{
			byte b = Convert.ToByte(recBytes.Skip(3).Take(1).ToArray<byte>().GetValue(0));
			Console.WriteLine("type = " + b);
			if (b == this.VERSION_RECEIVE)
			{
				Console.WriteLine("version recieve");
				int count = Convert.ToInt32(recBytes.Skip(4).Take(1).ToArray<byte>().GetValue(0));
				byte[] bytes = recBytes.Skip(5).Take(count).ToArray<byte>();
				string @string = Encoding.UTF8.GetString(bytes, 0, count);
				this.versionReceive(@string.Trim());
				Console.WriteLine("version = " + @string);
			}
			else if (b == this.MAC_RECEIVE)
			{
				Console.WriteLine("mac recieve");
				int count2 = Convert.ToInt32(recBytes.Skip(4).Take(1).ToArray<byte>().GetValue(0));
				byte[] bytes2 = recBytes.Skip(5).Take(count2).ToArray<byte>();
				string string2 = Encoding.UTF8.GetString(bytes2, 0, count2);
				this.macReceive(string2);
				Console.WriteLine("mac = " + string2);
			}
			else if (b == this.UPDATE_VERSION_RECEIVE)
			{
				Console.WriteLine("update version receive");
				int count3 = Convert.ToInt32(recBytes.Skip(4).Take(1).ToArray<byte>().GetValue(0));
				byte b2 = recBytes.Skip(5).Take(count3).ToArray<byte>()[0];
				if (b2 == 1)
				{
					Console.WriteLine("update version success");
					this.updateVersionReceive(true);
				}
				else if (b2 == 255)
				{
					Console.WriteLine("update version fail");
					this.updateVersionReceive(false);
				}
				else
				{
					Console.WriteLine("update version result = " + b2);
					this.updateVersionReceive(false);
				}
			}
			else if (b == this.UPDATE_MD5_RECEIVE)
			{
				Console.WriteLine("update md5 receive");
				int count4 = Convert.ToInt32(recBytes.Skip(4).Take(1).ToArray<byte>().GetValue(0));
				byte b3 = recBytes.Skip(5).Take(count4).ToArray<byte>()[0];
				if (b3 == 1)
				{
					Console.WriteLine("update md5 success");
					this.updateMd5Receive(true);
				}
				else if (b3 == 255)
				{
					Console.WriteLine("update md5 fail");
					this.updateMd5Receive(false);
				}
				else
				{
					Console.WriteLine("update md5 result = " + b3);
					this.updateMd5Receive(false);
				}
			}
			else if (b == this.STADNBY)
			{
				Console.WriteLine("update standby");
				int num = Convert.ToInt32(recBytes.Skip(4).Take(1).ToArray<byte>().GetValue(0));
				Console.WriteLine("dataCount = " + num);
				byte b4 = recBytes.Skip(5).Take(num).ToArray<byte>()[0];
				Console.WriteLine("result = " + b4);
				if (b4 == 1)
				{
					Console.WriteLine("update standby success");
					this.updateReady(true);
				}
				else if (b4 == 255)
				{
					Console.WriteLine("update standby fail");
					this.updateReady(false);
				}
				else
				{
					Console.WriteLine("update standby result = " + b4);
					this.updateReady(false);
				}
			}
			else if (b == this.MD5_CHECK_RESULT)
			{
				Console.WriteLine("md5 result");
				int count5 = Convert.ToInt32(recBytes.Skip(4).Take(1).ToArray<byte>().GetValue(0));
				byte b5 = recBytes.Skip(5).Take(count5).ToArray<byte>()[0];
				if (b5 == 1)
				{
					Console.WriteLine("md5 success");
					this.md5CheckResult(true);
				}
				else if (b5 == 255)
				{
					Console.WriteLine("md5 fail");
					this.md5CheckResult(false);
				}
				else
				{
					Console.WriteLine("md5 result = " + b5);
					this.md5CheckResult(false);
				}
			}
			else if (b == this.UPDATE_RESULT)
			{
				Console.WriteLine("update result");
				int count6 = Convert.ToInt32(recBytes.Skip(4).Take(1).ToArray<byte>().GetValue(0));
				byte b6 = recBytes.Skip(5).Take(count6).ToArray<byte>()[0];
				if (b6 == 1)
				{
					Console.WriteLine("update success");
					this.updateFinish(true);
				}
				else if (b6 == 255)
				{
					Console.WriteLine("update fail");
					this.updateFinish(false);
				}
				else
				{
					Console.WriteLine("update result = " + b6);
					this.updateFinish(false);
				}
			}
			else if (b == this.RECOVER_RESULT)
			{
				Console.WriteLine("recover result");
				int count7 = Convert.ToInt32(recBytes.Skip(4).Take(1).ToArray<byte>().GetValue(0));
				byte b7 = recBytes.Skip(5).Take(count7).ToArray<byte>()[0];
				if (b7 == 1)
				{
					Console.WriteLine("recover success");
					this.recoverResult(true);
				}
				else if (b7 == 255)
				{
					Console.WriteLine("recover fail");
					this.recoverResult(false);
				}
				else
				{
					Console.WriteLine("recover result = " + b7);
					this.recoverResult(false);
				}
			}
			else if (b == this.ERROR_CODE)
			{
				Console.WriteLine("error code");
				int count8 = Convert.ToInt32(recBytes.Skip(4).Take(1).ToArray<byte>().GetValue(0));
				byte b8 = recBytes.Skip(5).Take(count8).ToArray<byte>()[0];
				Console.WriteLine("error codet = " + b8);
			}
			else if (b == this.NOT_SUPPORT)
			{
				Console.WriteLine("not support");
			}
			Thread.Sleep(38);
		}

		public void transFile(string filePath)
		{
			Console.WriteLine("transFile");
			try
			{
				IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(this.host), this.fileTransPort);
				Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				socket.Connect(remoteEP);
				FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
				string s = filePath.Split(new string[]
				{
					"\\"
				}, StringSplitOptions.None).Last<string>();
				byte[] buffer = new byte[fileStream.Length];
				fileStream.Read(buffer, 0, (int)fileStream.Length);
				byte[] bytes = Encoding.Default.GetBytes(s);
				Console.WriteLine("send data");
				socket.Send(bytes);
				Thread.Sleep(1000);
				socket.Send(buffer);
				Console.WriteLine("send data finish");
				fileStream.Close();
				socket.Close();
			}
			catch (Exception arg)
			{
				Console.WriteLine("e = " + arg);
				this.socket.Close();
				this.socket = null;
			}
			Thread.Sleep(2000);
			this.transFinish();
		}

		private static byte computeCheckSum(byte[] buff, int len)
		{
			int num = 0;
			for (int i = 0; i < len; i++)
			{
				num += (int)(buff[i] & Convert.ToByte(255));
			}
			while (num >> 8 != 0)
			{
				num = (num >> 8) + (num & (int)Convert.ToByte(255));
			}
			return (byte)(~num & (int)Convert.ToByte(255));
		}

		private void output(byte[] recBytes)
		{
			for (int i = 0; i < recBytes.Length; i++)
			{
				byte b = recBytes[i];
				Console.WriteLine("recBytes = " + b);
			}
		}
	}
}
