using System;

namespace TPcastClassLibrary
{
	public class TPcastDeviceStatus : EventArgs
	{
		public int Count
		{
			get;
			set;
		}

		public int Status
		{
			get;
			set;
		}
	}
}
