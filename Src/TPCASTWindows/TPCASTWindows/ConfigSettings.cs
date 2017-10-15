using System;
using System.IO;
using System.Xml;

namespace TPCASTWindows
{
	internal class ConfigSettings
	{
		public static string readSteamPath()
		{
			return ConfigSettings.readSetting("steam");
		}

		public static void saveSteamPath(string steamPath)
		{
			ConfigSettings.writeSetting("steam", steamPath);
		}

		public static string readSetting(string key)
		{
			XmlNode node = ConfigSettings.loadConfigDocument().SelectSingleNode("//appSettings");
			if (node == null)
			{
				throw new InvalidOperationException("appSettings section not found in config file.");
			}
			string result;
			try
			{
				XmlElement elem = (XmlElement)node.SelectSingleNode(string.Format("//add[@key='{0}']", key));
				if (elem != null)
				{
					result = elem.GetAttribute("value");
				}
				else
				{
					result = null;
				}
			}
			catch
			{
				throw;
			}
			return result;
		}

		public static void writeSetting(string key, string value)
		{
			XmlDocument doc = ConfigSettings.loadConfigDocument();
			XmlNode node = doc.SelectSingleNode("//appSettings");
			if (node == null)
			{
				throw new InvalidOperationException("appSettings section not found in config file.");
			}
			try
			{
				XmlElement elem = (XmlElement)node.SelectSingleNode(string.Format("//add[@key='{0}']", key));
				if (elem != null)
				{
					elem.SetAttribute("value", value);
				}
				else
				{
					elem = doc.CreateElement("add");
					elem.SetAttribute("key", key);
					elem.SetAttribute("value", value);
					node.AppendChild(elem);
				}
				doc.Save(ConfigSettings.getConfigFilePath());
			}
			catch
			{
				throw;
			}
		}

		public static void RemoveSetting(string key)
		{
			XmlDocument doc = ConfigSettings.loadConfigDocument();
			XmlNode node = doc.SelectSingleNode("//appSettings");
			try
			{
				if (node == null)
				{
					throw new InvalidOperationException("appSettings section not found in config file.");
				}
				XmlNode expr_21 = node;
				expr_21.RemoveChild(expr_21.SelectSingleNode(string.Format("//add[@key='{0}']", key)));
				doc.Save(ConfigSettings.getConfigFilePath());
			}
			catch (NullReferenceException e)
			{
				throw new Exception(string.Format("The key {0} does not exist.", key), e);
			}
		}

		private static XmlDocument loadConfigDocument()
		{
			XmlDocument result;
			try
			{
				XmlDocument doc = new XmlDocument();
				string configFile = ConfigSettings.getConfigFilePath();
				if (File.Exists(configFile))
				{
					doc.Load(ConfigSettings.getConfigFilePath());
				}
				else
				{
					XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "utf-8", null);
					doc.AppendChild(dec);
					XmlElement root = doc.CreateElement("configuration");
					doc.AppendChild(root);
					XmlElement settings = doc.CreateElement("appSettings");
					XmlElement addNode = doc.CreateElement("add");
					addNode.SetAttribute("key", "steam");
					addNode.SetAttribute("value", "");
					settings.AppendChild(addNode);
					root.AppendChild(settings);
					doc.Save(configFile);
				}
				result = doc;
			}
			catch (FileNotFoundException e)
			{
				throw new Exception("No configuration file found.", e);
			}
			return result;
		}

		private static string getConfigFilePath()
		{
			return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\TPCAST\\App.config";
		}
	}
}
