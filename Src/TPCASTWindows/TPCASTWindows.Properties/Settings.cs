using System;
using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace TPCASTWindows.Properties
{
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0"), CompilerGenerated]
	internal sealed class Settings : ApplicationSettingsBase
	{
		private static Settings defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());

		public static Settings Default
		{
			get
			{
				return Settings.defaultInstance;
			}
		}

		[DefaultSettingValue("True"), UserScopedSetting, DebuggerNonUserCode]
		public bool displayGuide
		{
			get
			{
				return (bool)this["displayGuide"];
			}
			set
			{
				this["displayGuide"] = value;
			}
		}

		[DefaultSettingValue("True"), UserScopedSetting, DebuggerNonUserCode]
		public bool showGuideDialog
		{
			get
			{
				return (bool)this["showGuideDialog"];
			}
			set
			{
				this["showGuideDialog"] = value;
			}
		}
	}
}
