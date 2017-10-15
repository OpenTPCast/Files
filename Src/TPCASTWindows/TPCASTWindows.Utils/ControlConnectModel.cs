using NLog;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using TPcastClassLibrary;
using TPCASTWindows.Properties;

namespace TPCASTWindows.Utils
{
	internal class ControlConnectModel
	{
		public delegate void OnCheckControlCodeDelegate(ConnectCode code);

		public delegate void OnDevicesStatusChangeDelegate(int count, int state);

		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			public static readonly ControlConnectModel.<>c <>9 = new ControlConnectModel.<>c();

			public static ThreadStart <>9__48_0;

			public static ThreadStart <>9__49_0;

			internal void <Devices_DevStsChgEvent>b__48_0()
			{
				UsbIPUtil.getDevStateString();
			}

			internal void <Devices_DevStsChgEvent2>b__49_0()
			{
				UsbIPUtil.getDevStateString();
			}
		}

		private static Logger log = LogManager.GetCurrentClassLogger();

		private Control context;

		public ControlConnectModel.OnCheckControlCodeDelegate OnCheckControlCode;

		public ControlConnectModel.OnDevicesStatusChangeDelegate OnDevicesStatusChange;

		private Thread initCheckRouterSSIDThread;

		private Thread checkRouterThread;

		public bool ssidModified = true;

		private ConnectCode routerState = ConnectCode.NONE;

		private Thread checkHostThread;

		private ConnectCode hostState = ConnectCode.NONE;

		private Thread cableThread;

		private ConnectCode cableState = ConnectCode.NONE;

		private bool isListening;

		private TPcastRemoteDevices devices;

		private bool cableFirstConnect = true;

		private bool hasStartConnect;

		public ControlConnectModel(Control context)
		{
			this.context = context;
		}

		public void unInit()
		{
			this.context = null;
			this.AbortInitCheckRouterSSIDThread();
			this.AbortRouterThread();
			this.AbortHostThread();
			this.AbortCableThread();
		}

		private void CheckControlCode(ConnectCode code)
		{
			if (this.context != null && this.OnCheckControlCode != null)
			{
				if (this.context.InvokeRequired)
				{
					this.context.Invoke(this.OnCheckControlCode, new object[]
					{
						code
					});
					return;
				}
				this.OnCheckControlCode(code);
			}
		}

		private void DevicesStatusChange(int count, int state)
		{
			if (this.context != null && this.OnDevicesStatusChange != null)
			{
				if (this.context.InvokeRequired)
				{
					this.context.Invoke(this.OnDevicesStatusChange, new object[]
					{
						count,
						state
					});
					return;
				}
				this.OnDevicesStatusChange(count, state);
			}
		}

		public void addControlConnectCallback(ControlConnectCallback c)
		{
			this.OnCheckControlCode = new ControlConnectModel.OnCheckControlCodeDelegate(c.CheckControlCode);
			this.OnDevicesStatusChange = new ControlConnectModel.OnDevicesStatusChangeDelegate(c.DevicesStatusChange);
		}

		public void InitCheckRouterSSID()
		{
			this.initCheckRouterSSIDThread = new Thread(new ThreadStart(this.initCheckRouterSSIDThreadStart));
			this.initCheckRouterSSIDThread.Start();
		}

		private void AbortInitCheckRouterSSIDThread()
		{
			if (this.initCheckRouterSSIDThread != null)
			{
				this.initCheckRouterSSIDThread.Abort();
			}
		}

		private void initCheckRouterSSIDThreadStart()
		{
			string SSID = ChannelUtil.getWifiSSID();
			ControlConnectModel.log.Trace("ssid = " + SSID);
			if (string.IsNullOrEmpty(SSID))
			{
				this.CheckControlCode(ConnectCode.INIT_CHECK_ROURTER_FAIL);
				return;
			}
			if ("TPCast_AP".Equals(SSID))
			{
				this.CheckControlCode(ConnectCode.SSID_NOT_MODIFIED);
				return;
			}
			this.CheckControlCode(ConnectCode.SSID_MODIFIED);
		}

		public void CheckControl()
		{
			this.CheckControlCode(ConnectCode.BEGIN_CHECK_ROUTER);
			this.CheckRouter2();
		}

		private void CheckRouter()
		{
			this.checkRouterThread = new Thread(new ThreadStart(this.CheckRouterThreadStart));
			this.checkRouterThread.Start();
		}

		private void CheckRouter2()
		{
			if (this.checkRouterThread == null)
			{
				this.checkRouterThread = new Thread(new ThreadStart(this.CheckRouterThreadStart2));
				this.checkRouterThread.Start();
			}
		}

		private void CheckRouterThreadStart()
		{
			if (ChannelUtil.isRouterConnect())
			{
				this.CheckControlCode(ConnectCode.CHECK_ROUTER_SUCCESS);
				this.checkRouterSSIDStart();
				return;
			}
			this.CheckControlCode(ConnectCode.CHECK_ROUTER_FAIL);
			Thread.Sleep(5000);
			this.CheckRouterThreadStart();
		}

		public void AbortRouterThread()
		{
			ControlConnectModel.log.Trace("AbortRouterThread");
			if (this.checkRouterThread != null)
			{
				this.checkRouterThread.Abort();
				this.checkRouterThread = null;
			}
		}

		private void CheckRouterThreadStart2()
		{
			while (this.ssidModified)
			{
				if (ChannelUtil.isRouterConnect())
				{
					if (this.routerState != ConnectCode.CHECK_ROUTER_SUCCESS)
					{
						this.routerState = ConnectCode.CHECK_ROUTER_SUCCESS;
						this.CheckControlCode(ConnectCode.CHECK_ROUTER_SUCCESS);
					}
					this.checkRouterSSIDStart2();
					Thread.Sleep(5000);
				}
				else
				{
					this.AbortHostThread();
					this.AbortCableThread();
					this.removeDevicesStatusListener2();
					this.hostState = ConnectCode.NONE;
					this.cableState = ConnectCode.NONE;
					if (this.routerState != ConnectCode.CHECK_ROUTER_FAIL)
					{
						this.routerState = ConnectCode.CHECK_ROUTER_FAIL;
						this.CheckControlCode(ConnectCode.CHECK_ROUTER_FAIL);
					}
					Thread.Sleep(5000);
				}
			}
		}

		private void checkRouterSSIDStart()
		{
			string SSID = ChannelUtil.getWifiSSID();
			if (string.IsNullOrEmpty(SSID))
			{
				this.CheckHost();
				return;
			}
			if ("TPCast_AP".Equals(SSID))
			{
				this.CheckControlCode(ConnectCode.CHECK_ROUTER_SSID_NOT_MODIFIED);
				return;
			}
			this.CheckHost();
		}

		private void checkRouterSSIDStart2()
		{
			string SSID = ChannelUtil.getWifiSSID();
			if (string.IsNullOrEmpty(SSID))
			{
				this.CheckHost2();
				return;
			}
			if ("TPCast_AP".Equals(SSID))
			{
				this.ssidModified = false;
				this.CheckControlCode(ConnectCode.CHECK_ROUTER_SSID_NOT_MODIFIED);
				return;
			}
			this.CheckHost2();
		}

		private void CheckHost()
		{
			this.CheckHostThreadStart();
		}

		private void CheckHost2()
		{
			this.CheckControlCode(ConnectCode.BEGIN_CHECK_HOST);
			if (this.checkHostThread == null)
			{
				this.checkHostThread = new Thread(new ThreadStart(this.CheckHostThreadStart2));
				this.checkHostThread.Start();
			}
		}

		public void AbortHostThread()
		{
			ControlConnectModel.log.Trace("AbortHostThread");
			if (this.checkHostThread != null)
			{
				this.checkHostThread.Abort();
				this.checkHostThread = null;
			}
		}

		private void CheckHostThreadStart()
		{
			if (UsbIPUtil.isHostLinked())
			{
				this.CheckControlCode(ConnectCode.CHECK_HOST_SUCCESS);
				this.CheckCable();
				return;
			}
			this.CheckControlCode(ConnectCode.CHECK_HOST_FAIL);
			Thread.Sleep(5000);
			this.CheckRouterThreadStart();
		}

		private void CheckHostThreadStart2()
		{
			while (true)
			{
				if (UsbIPUtil.isHostLinked())
				{
					if (this.hostState != ConnectCode.CHECK_HOST_SUCCESS)
					{
						this.hostState = ConnectCode.CHECK_HOST_SUCCESS;
						this.CheckControlCode(ConnectCode.CHECK_HOST_SUCCESS);
					}
					this.CheckCable2();
					Thread.Sleep(5000);
				}
				else
				{
					this.AbortCableThread();
					this.removeDevicesStatusListener2();
					this.cableState = ConnectCode.NONE;
					if (this.hostState != ConnectCode.CHECK_HOST_FAIL)
					{
						this.hostState = ConnectCode.CHECK_HOST_FAIL;
						this.CheckControlCode(ConnectCode.CHECK_HOST_FAIL);
					}
					Thread.Sleep(5000);
				}
			}
		}

		private void CheckCable()
		{
			if (UsbIPUtil.isCableLinked())
			{
				this.CheckControlCode(ConnectCode.CHECK_CABLE_SUCCESS);
				this.ConnectControl();
				return;
			}
			this.CheckControlCode(ConnectCode.CHECK_CABLE_FAIL);
			Thread.Sleep(5000);
			this.CheckRouterThreadStart();
		}

		private void CheckCable2()
		{
			if (this.cableThread == null)
			{
				this.cableThread = new Thread(new ThreadStart(this.CheckCableThreadStart2));
				this.cableThread.Start();
			}
		}

		private void AbortCableThread()
		{
			ControlConnectModel.log.Trace("AbortCableThread");
			if (this.cableThread != null)
			{
				this.cableThread.Abort();
				this.cableThread = null;
			}
		}

		private void CheckCableThreadStart2()
		{
			while (true)
			{
				if (UsbIPUtil.isCableLinked())
				{
					if (this.cableState != ConnectCode.CHECK_CABLE_SUCCESS)
					{
						this.cableState = ConnectCode.CHECK_CABLE_SUCCESS;
						this.CheckControlCode(ConnectCode.CHECK_CABLE_SUCCESS);
					}
					this.ConnectControl2();
				}
				else
				{
					this.removeDevicesStatusListener2();
					if (this.cableState != ConnectCode.CHECK_CABLE_FAIL)
					{
						this.cableState = ConnectCode.CHECK_CABLE_FAIL;
						this.CheckControlCode(ConnectCode.CHECK_CABLE_FAIL);
					}
					Thread.Sleep(5000);
				}
			}
		}

		private void ConnectControl()
		{
			ControlConnectModel.log.Trace("ConnectControl");
			this.addDevicesStatusListener();
		}

		private void ConnectControl2()
		{
			ControlConnectModel.log.Trace("ConnectControl");
			this.addDevicesStatusListener2();
		}

		private void addDevicesStatusListener()
		{
			ControlConnectModel.log.Trace("addDevicesStatusListener " + this.isListening.ToString());
			if (!this.isListening)
			{
				this.devices = new TPcastRemoteDevices();
				this.devices.DevStsChgEvent += new TPcastDevStsChgEventHandler(this.Devices_DevStsChgEvent);
				ControlConnectModel.log.Trace("TPcastStartChecking");
				this.devices.TPcastStartChecking();
				this.isListening = true;
				return;
			}
			int count = -1;
			int status = -1;
			ControlConnectModel.GetTPcastDevStatus(ref count, ref status);
			this.Devices_DevStsChgEvent(null, new TPcastDeviceStatus
			{
				Count = count,
				Status = status
			});
		}

		private void addDevicesStatusListener2()
		{
			ControlConnectModel.log.Trace("addDevicesStatusListener " + this.isListening.ToString());
			if (this.devices == null)
			{
				this.devices = new TPcastRemoteDevices();
				this.devices.DevStsChgEvent += new TPcastDevStsChgEventHandler(this.Devices_DevStsChgEvent2);
			}
			if (!this.isListening)
			{
				ControlConnectModel.log.Trace("TPcastStartChecking");
				this.devices.TPcastStartChecking();
				this.isListening = true;
				return;
			}
			if (this.cableFirstConnect)
			{
				this.cableFirstConnect = false;
				int count = -1;
				int status = -1;
				ControlConnectModel.GetTPcastDevStatus(ref count, ref status);
				this.Devices_DevStsChgEvent(null, new TPcastDeviceStatus
				{
					Count = count,
					Status = status
				});
			}
		}

		private void removeDevicesStatusListener2()
		{
			if (this.devices != null && this.isListening)
			{
				this.devices.TPcastStopChecking();
				this.isListening = false;
				this.cableFirstConnect = true;
			}
		}

		private void Devices_DevStsChgEvent(object comp, EventArgs CallerContext)
		{
			TPcastDeviceStatus expr_06 = (TPcastDeviceStatus)CallerContext;
			int deviceCount = expr_06.Count;
			int deviceState = expr_06.Status;
			ControlConnectModel.log.Trace(string.Concat(new object[]
			{
				"Devices_DevStsChgEventcount = ",
				deviceCount,
				" status = ",
				deviceState
			}));
			if (deviceCount != -1 && deviceState != -1)
			{
				this.DevicesStatusChange(deviceCount, deviceState);
				if (deviceState == 15)
				{
					ControlConnectModel.log.Trace("deviceState = 0xf");
					this.hasStartConnect = true;
					this.CheckVR();
					return;
				}
				if (deviceState == 0)
				{
					ConnectCode code = this.findError();
					if (code != ConnectCode.NONE)
					{
						ControlConnectModel.log.Trace("code = " + code);
						this.CheckRouterThreadStart();
						return;
					}
					ControlConnectModel.log.Trace("code = " + code + " getDevStateString");
					if (!this.hasStartConnect)
					{
						this.hasStartConnect = true;
						ThreadStart arg_102_0;
						if ((arg_102_0 = ControlConnectModel.<>c.<>9__48_0) == null)
						{
							arg_102_0 = (ControlConnectModel.<>c.<>9__48_0 = new ThreadStart(ControlConnectModel.<>c.<>9.<Devices_DevStsChgEvent>b__48_0));
						}
						new Thread(arg_102_0).Start();
						return;
					}
				}
				else
				{
					ConnectCode code2 = this.findError();
					if (code2 != ConnectCode.NONE)
					{
						ControlConnectModel.log.Trace("code = " + code2);
						this.CheckRouterThreadStart();
					}
				}
			}
		}

		private void Devices_DevStsChgEvent2(object comp, EventArgs CallerContext)
		{
			TPcastDeviceStatus expr_06 = (TPcastDeviceStatus)CallerContext;
			int deviceCount = expr_06.Count;
			int deviceState = expr_06.Status;
			ControlConnectModel.log.Trace(string.Concat(new object[]
			{
				"Devices_DevStsChgEventcount = ",
				deviceCount,
				" status = ",
				deviceState
			}));
			if (deviceCount != -1 && deviceState != -1)
			{
				this.DevicesStatusChange(deviceCount, deviceState);
				if (deviceState == 15)
				{
					ControlConnectModel.log.Trace("deviceState = 0xf");
					this.hasStartConnect = true;
					this.CheckVR();
					return;
				}
				if (deviceState == 0 && !this.hasStartConnect)
				{
					this.hasStartConnect = true;
					ThreadStart arg_AD_0;
					if ((arg_AD_0 = ControlConnectModel.<>c.<>9__49_0) == null)
					{
						arg_AD_0 = (ControlConnectModel.<>c.<>9__49_0 = new ThreadStart(ControlConnectModel.<>c.<>9.<Devices_DevStsChgEvent2>b__49_0));
					}
					new Thread(arg_AD_0).Start();
				}
			}
		}

		private ConnectCode findError()
		{
			ConnectCode code;
			if (ChannelUtil.isRouterConnect())
			{
				string SSID = ChannelUtil.getWifiSSID();
				if (string.IsNullOrEmpty(SSID) || !"TPCast_AP".Equals(SSID))
				{
					return this.findHostError();
				}
				code = ConnectCode.CHECK_ROUTER_SSID_NOT_MODIFIED;
				this.CheckControlCode(code);
			}
			else
			{
				code = ConnectCode.CHECK_ROUTER_FAIL;
				this.CheckControlCode(code);
			}
			return code;
		}

		private ConnectCode findHostError()
		{
			ConnectCode code = ConnectCode.NONE;
			if (UsbIPUtil.isHostLinked())
			{
				if (!UsbIPUtil.isCableLinked())
				{
					code = ConnectCode.CHECK_CABLE_FAIL;
					this.CheckControlCode(code);
				}
			}
			else
			{
				code = ConnectCode.CHECK_HOST_FAIL;
				this.CheckControlCode(code);
			}
			return code;
		}

		private void CheckVR()
		{
			Thread.Sleep(2000);
			this.CheckControlCode(ConnectCode.CHECK_VR_SUCCESS);
			if (Settings.Default.displayGuide)
			{
				Thread.Sleep(2000);
				this.CheckControlCode(ConnectCode.SHOW_GUIDE_IMAGE);
			}
		}

		[DllImport("TPcastTools.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool GetTPcastDevStatus(ref int DeviceCount, ref int DeviceStatus);
	}
}
