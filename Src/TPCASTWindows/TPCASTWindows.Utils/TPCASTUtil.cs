using NLog;
using System;
using TPcastClassLibrary;

namespace TPCASTWindows.Utils
{
	internal class TPCASTUtil
	{
		private static Logger log = LogManager.GetCurrentClassLogger();

		public static bool isDriverOK()
		{
			bool driverStatus = new TPcastDiagnose().GetComponentStatus(TPcastComponent.TPcast_Driver);
			TPCASTUtil.log.Trace("driver " + driverStatus.ToString());
			return driverStatus;
		}

		public static bool isServiceOK()
		{
			bool serviceStatus = new TPcastDiagnose().GetComponentStatus(TPcastComponent.TPcast_Service);
			TPCASTUtil.log.Trace("serStatus " + serviceStatus.ToString());
			return serviceStatus;
		}

		public static bool isCableConnected()
		{
			bool cableStatus = new TPcastDiagnose().GetComponentStatus(TPcastComponent.TPcast_USB_Cable);
			TPCASTUtil.log.Trace("cableStatus " + cableStatus.ToString());
			return cableStatus;
		}

		public static bool isHostConnected()
		{
			bool hostStatus = new TPcastDiagnose().GetComponentStatus(TPcastComponent.TPcast_Remote_Host);
			TPCASTUtil.log.Trace("hostStatus " + hostStatus.ToString());
			return hostStatus;
		}
	}
}
