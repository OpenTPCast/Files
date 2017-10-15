using System;
using System.Drawing;
using TPCASTWindows.Properties;

namespace TPCASTWindows
{
	internal class LocalizeUtil
	{
		public static Bitmap getImageFromResource(string localizedResourceString)
		{
			return (Bitmap)Resources.ResourceManager.GetObject(localizedResourceString);
		}
	}
}
