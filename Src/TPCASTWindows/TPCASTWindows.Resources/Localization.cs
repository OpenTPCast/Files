using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace TPCASTWindows.Resources
{
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0"), DebuggerNonUserCode, CompilerGenerated]
	internal class Localization
	{
		private static ResourceManager resourceMan;

		private static CultureInfo resourceCulture;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (Localization.resourceMan == null)
				{
					Localization.resourceMan = new ResourceManager("TPCASTWindows.Resources.Localization", typeof(Localization).Assembly);
				}
				return Localization.resourceMan;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return Localization.resourceCulture;
			}
			set
			{
				Localization.resourceCulture = value;
			}
		}

		internal static string alreadyNewest
		{
			get
			{
				return Localization.ResourceManager.GetString("alreadyNewest", Localization.resourceCulture);
			}
		}

		internal static string battery
		{
			get
			{
				return Localization.ResourceManager.GetString("battery", Localization.resourceCulture);
			}
		}

		internal static string ControlReconnecting
		{
			get
			{
				return Localization.ResourceManager.GetString("ControlReconnecting", Localization.resourceCulture);
			}
		}

		internal static string currentVersion
		{
			get
			{
				return Localization.ResourceManager.GetString("currentVersion", Localization.resourceCulture);
			}
		}

		internal static string faqLink
		{
			get
			{
				return Localization.ResourceManager.GetString("faqLink", Localization.resourceCulture);
			}
		}

		internal static string firmwareCurrentVersion
		{
			get
			{
				return Localization.ResourceManager.GetString("firmwareCurrentVersion", Localization.resourceCulture);
			}
		}

		internal static string firmwareDonloadFinish
		{
			get
			{
				return Localization.ResourceManager.GetString("firmwareDonloadFinish", Localization.resourceCulture);
			}
		}

		internal static string firmwareUnconnected
		{
			get
			{
				return Localization.ResourceManager.GetString("firmwareUnconnected", Localization.resourceCulture);
			}
		}

		internal static string firmwareVersion
		{
			get
			{
				return Localization.ResourceManager.GetString("firmwareVersion", Localization.resourceCulture);
			}
		}

		internal static string guide_image
		{
			get
			{
				return Localization.ResourceManager.GetString("guide_image", Localization.resourceCulture);
			}
		}

		internal static string interrupt_battery
		{
			get
			{
				return Localization.ResourceManager.GetString("interrupt_battery", Localization.resourceCulture);
			}
		}

		internal static string interrupt_router
		{
			get
			{
				return Localization.ResourceManager.GetString("interrupt_router", Localization.resourceCulture);
			}
		}

		internal static string launch_background
		{
			get
			{
				return Localization.ResourceManager.GetString("launch_background", Localization.resourceCulture);
			}
		}

		internal static string messageSuccess
		{
			get
			{
				return Localization.ResourceManager.GetString("messageSuccess", Localization.resourceCulture);
			}
		}

		internal static string noInternet
		{
			get
			{
				return Localization.ResourceManager.GetString("noInternet", Localization.resourceCulture);
			}
		}

		internal static string raspberryChecking
		{
			get
			{
				return Localization.ResourceManager.GetString("raspberryChecking", Localization.resourceCulture);
			}
		}

		internal static string raspberryConnecting
		{
			get
			{
				return Localization.ResourceManager.GetString("raspberryConnecting", Localization.resourceCulture);
			}
		}

		internal static string raspberryFail
		{
			get
			{
				return Localization.ResourceManager.GetString("raspberryFail", Localization.resourceCulture);
			}
		}

		internal static string RaspberryRebootFinish
		{
			get
			{
				return Localization.ResourceManager.GetString("RaspberryRebootFinish", Localization.resourceCulture);
			}
		}

		internal static string raspberrySuccess
		{
			get
			{
				return Localization.ResourceManager.GetString("raspberrySuccess", Localization.resourceCulture);
			}
		}

		internal static string routerChecking
		{
			get
			{
				return Localization.ResourceManager.GetString("routerChecking", Localization.resourceCulture);
			}
		}

		internal static string routerFail
		{
			get
			{
				return Localization.ResourceManager.GetString("routerFail", Localization.resourceCulture);
			}
		}

		internal static string routerRebootFinish
		{
			get
			{
				return Localization.ResourceManager.GetString("routerRebootFinish", Localization.resourceCulture);
			}
		}

		internal static string routerRebooting
		{
			get
			{
				return Localization.ResourceManager.GetString("routerRebooting", Localization.resourceCulture);
			}
		}

		internal static string routerSuccess
		{
			get
			{
				return Localization.ResourceManager.GetString("routerSuccess", Localization.resourceCulture);
			}
		}

		internal static string softwareDownloadFinish
		{
			get
			{
				return Localization.ResourceManager.GetString("softwareDownloadFinish", Localization.resourceCulture);
			}
		}

		internal static string softwareVersion
		{
			get
			{
				return Localization.ResourceManager.GetString("softwareVersion", Localization.resourceCulture);
			}
		}

		internal static string systemChecking
		{
			get
			{
				return Localization.ResourceManager.GetString("systemChecking", Localization.resourceCulture);
			}
		}

		internal static string systemFail
		{
			get
			{
				return Localization.ResourceManager.GetString("systemFail", Localization.resourceCulture);
			}
		}

		internal static string systemSuccess
		{
			get
			{
				return Localization.ResourceManager.GetString("systemSuccess", Localization.resourceCulture);
			}
		}

		internal static string tutorialLink
		{
			get
			{
				return Localization.ResourceManager.GetString("tutorialLink", Localization.resourceCulture);
			}
		}

		internal Localization()
		{
		}
	}
}
