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
			bool driverStatus = new TPcastDiagnose().GetComponentStatus(1);
			TPCASTUtil.log.Trace("driver " + driverStatus.ToString());
			return driverStatus;
		}

		public static bool isServiceOK()
		{
			bool serviceStatus = new TPcastDiagnose().GetComponentStatus(2);
			TPCASTUtil.log.Trace("serStatus " + serviceStatus.ToString());
			return serviceStatus;
		}

		public static bool isCableConnected()
		{
			bool cableStatus = new TPcastDiagnose().GetComponentStatus(3);
			TPCASTUtil.log.Trace("cableStatus " + cableStatus.ToString());
			return cableStatus;
		}

		public static bool isHostConnected()
		{
			bool hostStatus = new TPcastDiagnose().GetComponentStatus(4);
			TPCASTUtil.log.Trace("hostStatus " + hostStatus.ToString());
			return hostStatus;
		}
	}
}
