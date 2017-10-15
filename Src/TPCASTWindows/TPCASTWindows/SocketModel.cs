using NLog;
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

		private static Logger log = LogManager.GetCurrentClassLogger();

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

		private int commandPort = 5974;

		private int fileTransPort = 5973;

		private string host = ConfigureUtil.AdapterIP();

		private static Socket socket;

		private Thread connectThread;

		private Thread recieveThread;

		private const int TIMEOUT_RECEIVE = 5000;

		private const int TIMEOUT_SEND = 5000;

		private const int TIMEOUT_CONNECT = 1000;

		private const int TIMEOUT_TRANSFILE = 10000;

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

		private Socket transFileSocket;

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
			SocketModel.log.Trace("count = " + this.socketExceptionCallbacks.Count);
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
					SocketModel.log.Trace("version Count = " + this.socketConnectCallbacks.Count);
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

		private void connectCallback(IAsyncResult ar)
		{
			SocketModel.log.Trace("connectCallback" + ar);
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

		private bool Connect()
		{
			SocketModel.log.Trace("Connect");
			SocketModel.log.Trace("socket == " + SocketModel.socket);
			if (!this.isConnected())
			{
				try
				{
					SocketModel.log.Trace("connectThreadStart");
					IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(this.host), this.commandPort);
					SocketModel.log.Trace("new socket");
					SocketModel.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					SocketModel.socket.ReceiveTimeout = 5000;
					SocketModel.log.Trace("socket begin connect");
					IAsyncResult ar = SocketModel.socket.BeginConnect(ipe, new AsyncCallback(this.connectCallback), SocketModel.socket);
					ar.AsyncWaitHandle.WaitOne(1000);
					if (ar.IsCompleted)
					{
						SocketModel.log.Trace("socket connect IsCompleted");
						Logger arg_105_0 = SocketModel.log;
						string arg_100_0 = "connected = ";
						bool connected = ((Socket)ar.AsyncState).Connected;
						arg_105_0.Trace(arg_100_0 + connected.ToString());
						connected = ((Socket)ar.AsyncState).Connected;
						return connected;
					}
					SocketModel.log.Trace("socket connect notCompleted");
					if (SocketModel.socket != null)
					{
						SocketModel.socket.Close();
						SocketModel.socket = null;
					}
					return false;
				}
				catch (Exception e)
				{
					SocketModel.log.Trace("e = " + e);
					if (SocketModel.socket != null)
					{
						SocketModel.socket.Close();
						SocketModel.socket = null;
					}
					return false;
				}
				return true;
			}
			return true;
		}

		private bool isConnected()
		{
			SocketModel.log.Trace("isConnected() ");
			if (SocketModel.socket != null)
			{
				SocketModel.log.Trace("Connected = " + SocketModel.socket.Connected.ToString());
				SocketModel.log.Trace("poll = " + SocketModel.socket.Poll(1000, SelectMode.SelectRead).ToString());
				SocketModel.log.Trace("avail = " + (SocketModel.socket.Available == 0).ToString());
			}
			else
			{
				SocketModel.log.Trace("socket == null ");
			}
			return SocketModel.socket != null && (!SocketModel.socket.Poll(1000, SelectMode.SelectRead) || SocketModel.socket.Available != 0) && SocketModel.socket.Connected;
		}

		public void disconnect()
		{
			this.abortConnectThread();
			this.abortRecieveThread();
			if (SocketModel.socket != null)
			{
				SocketModel.socket.Close();
				SocketModel.socket = null;
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
			int checkSum = (int)SocketModel.computeCheckSum(recBytes, recBytes.Length - 1);
			return Convert.ToInt32(recBytes.Skip(recBytes.Length - 1).Take(1).ToArray<byte>().GetValue(0)) == checkSum;
		}

		public void GetVersion()
		{
			SocketModel.log.Trace("GetVersion");
			this.SendMessage(this.VERSION_REQUEST, "");
		}

		public void GetMac()
		{
			SocketModel.log.Trace("GetMac");
			this.SendMessage(this.MAC_REQUEST, "");
		}

		public void TransUpdateVersion(string updateVersion)
		{
			SocketModel.log.Trace("TransUpdateVersion " + updateVersion);
			this.SendMessage(this.UPDATE_VERSION, updateVersion);
		}

		public void TransUpdateMd5(string updateMd5)
		{
			SocketModel.log.Trace("TransUpdateMd5 " + updateMd5);
			this.SendMessage(this.UPDATE_MD5, updateMd5);
		}

		public void PrepareUpdate()
		{
			SocketModel.log.Trace("PrepareUpdate ");
			this.SendMessage(this.PREPARE_UPDATE, "");
		}

		private void TransFinish()
		{
			SocketModel.log.Trace("TransFinish ");
			this.SendMessage(this.TRANS_FINISH, "");
		}

		public void RecoverOriginVersion()
		{
			SocketModel.log.Trace("RecoverOriginVersion");
			this.SendMessage(this.RECOVER_ORIGIN_VERSION, "");
		}

		public void DebugMessage()
		{
			SocketModel.log.Trace("DebugMessage");
			this.SendMessage(this.DEBUG, "");
		}

		public void SendMessage(byte type, string message = "")
		{
			new Thread(delegate
			{
				if (this.Connect())
				{
					this.Send(type, message);
					return;
				}
				this.connected(false);
			}).Start();
		}

		private void Send(byte type, string message = "")
		{
			byte[] buffer = this.buildBuffer(type, message);
			if (SocketModel.socket != null)
			{
				try
				{
					SocketModel.socket.SendTimeout = 5000;
					SocketModel.socket.Send(buffer);
				}
				catch (Exception e)
				{
					SocketModel.log.Error("send e = " + e);
					this.sendFail();
				}
				SocketModel.socket.ReceiveTimeout = 5000;
				this.receiveData();
				return;
			}
			SocketModel.log.Error("socket == null");
			this.sendFail();
		}

		private byte[] buildBuffer(byte type, string message = "")
		{
			byte[] buffer = this.buildBufferWithHeader();
			buffer = this.appendData(buffer, new byte[1]);
			buffer = this.appendData(buffer, type);
			byte[] data;
			if (string.IsNullOrEmpty(message))
			{
				data = new byte[1];
			}
			else
			{
				data = Encoding.Default.GetBytes(message);
			}
			buffer = this.appendData(buffer, data.Length);
			buffer = this.appendData(buffer, data);
			byte[] arg_5A_1 = buffer;
			byte[] expr_52 = buffer;
			return this.appendData(arg_5A_1, SocketModel.computeCheckSum(expr_52, expr_52.Length));
		}

		private void receiveData()
		{
			SocketModel.log.Trace("receiveData");
			try
			{
				byte[] recBytes = new byte[64];
				Socket arg_21_0 = SocketModel.socket;
				byte[] expr_1D = recBytes;
				int count = arg_21_0.Receive(expr_1D, expr_1D.Length, SocketFlags.None);
				if (count > 0)
				{
					SocketModel.log.Trace("read bytes = " + count);
					recBytes = recBytes.Take(count).ToArray<byte>();
					SocketModel.log.Trace("cut bytes = " + recBytes.Count<byte>());
					if (this.isCheckSum(recBytes))
					{
						SocketModel.log.Trace("check sum ok");
						this.parseData(recBytes);
					}
					else
					{
						SocketModel.log.Trace("check sum no");
					}
				}
				else if (count == 0)
				{
					SocketModel.log.Error("count == 0");
					this.receiveData();
				}
				else
				{
					SocketModel.log.Error("count < 0");
					this.sendFail();
					if (SocketModel.socket != null)
					{
						SocketModel.socket.Close();
						SocketModel.socket = null;
					}
				}
			}
			catch (Exception e)
			{
				SocketModel.log.Error("error = " + e);
				this.receiveTimeout();
				if (SocketModel.socket != null)
				{
					SocketModel.socket.Close();
					SocketModel.socket = null;
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
						byte[] recBytes = new byte[64];
						Socket arg_21_0 = socket;
						byte[] expr_1D = recBytes;
						int count = arg_21_0.Receive(expr_1D, expr_1D.Length, SocketFlags.None);
						recBytes = recBytes.Take(count).ToArray<byte>();
						if (this.isCheckSum(recBytes))
						{
							SocketModel.log.Trace("check sum ok");
							this.parseData(recBytes);
						}
						else
						{
							SocketModel.log.Trace("check sum no");
						}
						continue;
					}
					catch (Exception e)
					{
						SocketModel.log.Error("error = " + e);
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
			byte dataType = Convert.ToByte(recBytes.Skip(3).Take(1).ToArray<byte>().GetValue(0));
			SocketModel.log.Trace("type = " + dataType);
			if (dataType == this.VERSION_RECEIVE)
			{
				SocketModel.log.Trace("version recieve");
				int dataCount = Convert.ToInt32(recBytes.Skip(4).Take(1).ToArray<byte>().GetValue(0));
				byte[] data = recBytes.Skip(5).Take(dataCount).ToArray<byte>();
				string version = Encoding.UTF8.GetString(data, 0, dataCount);
				this.versionReceive(version.Trim());
				SocketModel.log.Trace("version = " + version);
			}
			else if (dataType == this.MAC_RECEIVE)
			{
				SocketModel.log.Trace("mac recieve");
				int dataCount2 = Convert.ToInt32(recBytes.Skip(4).Take(1).ToArray<byte>().GetValue(0));
				byte[] data2 = recBytes.Skip(5).Take(dataCount2).ToArray<byte>();
				string mac = Encoding.UTF8.GetString(data2, 0, dataCount2);
				this.macReceive(mac);
				SocketModel.log.Trace("mac = " + mac);
			}
			else if (dataType == this.UPDATE_VERSION_RECEIVE)
			{
				SocketModel.log.Trace("update version receive");
				int dataCount3 = Convert.ToInt32(recBytes.Skip(4).Take(1).ToArray<byte>().GetValue(0));
				byte result = recBytes.Skip(5).Take(dataCount3).ToArray<byte>()[0];
				if (result == 1)
				{
					SocketModel.log.Trace("update version success");
					this.updateVersionReceive(true);
				}
				else if (result == 255)
				{
					SocketModel.log.Trace("update version fail");
					this.updateVersionReceive(false);
				}
				else
				{
					SocketModel.log.Trace("update version result = " + result);
					this.updateVersionReceive(false);
				}
			}
			else if (dataType == this.UPDATE_MD5_RECEIVE)
			{
				SocketModel.log.Trace("update md5 receive");
				int dataCount4 = Convert.ToInt32(recBytes.Skip(4).Take(1).ToArray<byte>().GetValue(0));
				byte result2 = recBytes.Skip(5).Take(dataCount4).ToArray<byte>()[0];
				if (result2 == 1)
				{
					SocketModel.log.Trace("update md5 success");
					this.updateMd5Receive(true);
				}
				else if (result2 == 255)
				{
					SocketModel.log.Trace("update md5 fail");
					this.updateMd5Receive(false);
				}
				else
				{
					SocketModel.log.Trace("update md5 result = " + result2);
					this.updateMd5Receive(false);
				}
			}
			else if (dataType == this.STADNBY)
			{
				SocketModel.log.Trace("update standby");
				int dataCount5 = Convert.ToInt32(recBytes.Skip(4).Take(1).ToArray<byte>().GetValue(0));
				SocketModel.log.Trace("dataCount = " + dataCount5);
				byte result3 = recBytes.Skip(5).Take(dataCount5).ToArray<byte>()[0];
				SocketModel.log.Trace("result = " + result3);
				if (result3 == 1)
				{
					SocketModel.log.Trace("update standby success");
					this.updateReady(true);
				}
				else if (result3 == 255)
				{
					SocketModel.log.Trace("update standby fail");
					this.updateReady(false);
				}
				else
				{
					SocketModel.log.Trace("update standby result = " + result3);
					this.updateReady(false);
				}
			}
			else if (dataType == this.MD5_CHECK_RESULT)
			{
				SocketModel.log.Trace("md5 result");
				int dataCount6 = Convert.ToInt32(recBytes.Skip(4).Take(1).ToArray<byte>().GetValue(0));
				byte result4 = recBytes.Skip(5).Take(dataCount6).ToArray<byte>()[0];
				if (result4 == 1)
				{
					SocketModel.log.Trace("md5 success");
					this.md5CheckResult(true);
					if (SocketModel.socket != null)
					{
						SocketModel.socket.ReceiveTimeout = 10000;
						this.receiveData();
					}
				}
				else if (result4 == 255)
				{
					SocketModel.log.Trace("md5 fail");
					this.md5CheckResult(false);
				}
				else
				{
					SocketModel.log.Trace("md5 result = " + result4);
					this.md5CheckResult(false);
				}
			}
			else if (dataType == this.UPDATE_RESULT)
			{
				SocketModel.log.Trace("update result");
				int dataCount7 = Convert.ToInt32(recBytes.Skip(4).Take(1).ToArray<byte>().GetValue(0));
				byte result5 = recBytes.Skip(5).Take(dataCount7).ToArray<byte>()[0];
				if (result5 == 1)
				{
					SocketModel.log.Trace("update success");
					this.updateFinish(true);
				}
				else if (result5 == 255)
				{
					SocketModel.log.Trace("update fail");
					this.updateFinish(false);
				}
				else
				{
					SocketModel.log.Trace("update result = " + result5);
					this.updateFinish(false);
				}
			}
			else if (dataType == this.RECOVER_RESULT)
			{
				SocketModel.log.Trace("recover result");
				int dataCount8 = Convert.ToInt32(recBytes.Skip(4).Take(1).ToArray<byte>().GetValue(0));
				byte result6 = recBytes.Skip(5).Take(dataCount8).ToArray<byte>()[0];
				if (result6 == 1)
				{
					SocketModel.log.Trace("recover success");
					this.recoverResult(true);
				}
				else if (result6 == 255)
				{
					SocketModel.log.Trace("recover fail");
					this.recoverResult(false);
				}
				else
				{
					SocketModel.log.Trace("recover result = " + result6);
					this.recoverResult(false);
				}
			}
			else if (dataType == this.ERROR_CODE)
			{
				SocketModel.log.Trace("error code");
				int dataCount9 = Convert.ToInt32(recBytes.Skip(4).Take(1).ToArray<byte>().GetValue(0));
				byte result7 = recBytes.Skip(5).Take(dataCount9).ToArray<byte>()[0];
				SocketModel.log.Trace("error codet = " + result7);
			}
			else if (dataType == this.NOT_SUPPORT)
			{
				SocketModel.log.Trace("not support");
			}
			Thread.Sleep(38);
		}

		public void transFile(string filePath)
		{
			SocketModel.log.Trace("transFile");
			new Thread(new ParameterizedThreadStart(this.transFileThreadStart)).Start(filePath);
		}

		private void transFileThreadStart(object obj)
		{
			if (obj is string)
			{
				string filePath = (string)obj;
				try
				{
					IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(this.host), this.fileTransPort);
					this.transFileSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					this.transFileSocket.Connect(ipe);
					FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
					string fileName = filePath.Split(new string[]
					{
						"\\"
					}, StringSplitOptions.None).Last<string>();
					byte[] buffByte = new byte[fs.Length];
					fs.Read(buffByte, 0, (int)fs.Length);
					byte[] fileNameData = Encoding.Default.GetBytes(fileName);
					SocketModel.log.Trace("send data");
					this.transFileSocket.SendTimeout = 5000;
					this.transFileSocket.Send(fileNameData);
					Thread.Sleep(1000);
					this.transFileSocket.Send(buffByte);
					SocketModel.log.Trace("send data finish");
					fs.Close();
					this.transFileSocket.Close();
					this.transFileSocket = null;
				}
				catch (Exception e)
				{
					SocketModel.log.Error("e = " + e);
					this.sendFail();
					if (this.transFileSocket != null)
					{
						this.transFileSocket.Close();
						this.transFileSocket = null;
					}
				}
				Thread.Sleep(2000);
				this.TransFinish();
			}
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

		private void output(byte[] recBytes)
		{
			for (int i = 0; i < recBytes.Length; i++)
			{
				byte d = recBytes[i];
				SocketModel.log.Trace("recBytes = " + d);
			}
		}
	}
}
