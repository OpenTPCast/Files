using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace TPcastClassLibrary
{
	public class TPcastDiagnose
	{
		private const string conffile = "Configuration.ini";

		public string HDMName;

		[method: CompilerGenerated]
		[CompilerGenerated]
		public event TPcastSvcStsChgEventHandler DevStsChgEvent;

		[DllImport("Advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool NotifyServiceStatusChange();

		private bool FindRemoteByBroadcast(ref string RemoteIP)
		{
			return false;
		}

		private bool CheckTPcastService()
		{
			ServiceController serviceController = null;
			bool result;
			try
			{
				serviceController = new ServiceController("TpcastService");
				if (serviceController.Status == ServiceControllerStatus.Running)
				{
					result = true;
				}
				else
				{
					result = false;
				}
			}
			catch
			{
				result = false;
			}
			finally
			{
				serviceController.Close();
			}
			return result;
		}

		private bool GetRemoteIPPort(ref string RemoteIP, ref int RemotePort)
		{
			string path = Environment.CurrentDirectory + "\\Configuration.ini";
			bool flag = false;
			try
			{
				string[] array = new StreamReader(path).ReadLine().Split(new char[]
				{
					':'
				});
				RemoteIP = array[0];
				RemotePort = int.Parse(array[1]) - 1;
				bool result = true;
				return result;
			}
			catch (FileNotFoundException ex)
			{
				Console.WriteLine("FileNotFoundException: {0}", ex.Message);
				flag = true;
			}
			catch (Exception ex2)
			{
				Console.WriteLine("Exception: {0}", ex2.Message);
				bool result = false;
				return result;
			}
			if (flag && this.FindRemoteByBroadcast(ref RemoteIP))
			{
				RemotePort = 5880;
				return true;
			}
			return false;
		}

		private bool CheckUSBCableHDM(ref string HDMType)
		{
			TcpClient tcpClient = new TcpClient();
			bool result;
			try
			{
				string host = null;
				int port = 0;
				if (!this.GetRemoteIPPort(ref host, ref port))
				{
					Console.WriteLine("{0} is missing, use default settings", "Configuration.ini");
					host = "192.168.144.88";
					port = 5880;
				}
				Task task = tcpClient.ConnectAsync(host, port);
				int num = 0;
				while (!task.IsCompleted && num++ < 60)
				{
					task.Wait(50);
				}
				if (!task.IsCompleted)
				{
					result = false;
				}
				else
				{
					byte[] array = new byte[16];
					byte[] bytes = Encoding.ASCII.GetBytes("cable_status!");
					Array.Copy(bytes, array, bytes.Length);
					NetworkStream expr_98 = tcpClient.GetStream();
					expr_98.Write(array, 0, array.Length);
					expr_98.ReadTimeout = 5000;
					byte[] array2 = new byte[16];
					int num2 = expr_98.Read(array2, 0, array2.Length);
					if (num2 != array2.Length)
					{
						HDMType = "None";
						result = false;
					}
					else
					{
						string @string = Encoding.ASCII.GetString(array2, 0, num2);
						if (@string.Contains("disconnected"))
						{
							HDMType = "None";
							result = false;
						}
						else if (@string.Contains("HTC"))
						{
							HDMType = "HTC Vive";
							result = true;
						}
						else if (@string.Contains("Ocu"))
						{
							HDMType = "Oculus Rifi";
							result = true;
						}
						else
						{
							HDMType = "Unknown";
							result = true;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception: {0}", ex.Message);
				result = false;
			}
			finally
			{
				if (tcpClient != null)
				{
					tcpClient.Close();
				}
			}
			return result;
		}

		[DllImport("TPcastTools.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool CheckTPcastDrivers();

		public bool GetComponentStatus(TPcastComponent comp)
		{
			if (comp <= TPcastComponent.TPcast_None || comp >= TPcastComponent.TPcast_Max)
			{
				return false;
			}
			bool result;
			switch (comp)
			{
			case TPcastComponent.TPcast_Driver:
				result = TPcastDiagnose.CheckTPcastDrivers();
				break;
			case TPcastComponent.TPcast_Service:
				result = false;
				break;
			case TPcastComponent.TPcast_USB_Cable:
				result = this.CheckUSBCableHDM(ref this.HDMName);
				break;
			case TPcastComponent.TPcast_Remote_Host:
				result = false;
				break;
			default:
				result = false;
				break;
			}
			return result;
		}
	}
}
