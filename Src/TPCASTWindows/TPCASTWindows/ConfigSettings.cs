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
			XmlNode xmlNode = ConfigSettings.loadConfigDocument().SelectSingleNode("//appSettings");
			if (xmlNode == null)
			{
				throw new InvalidOperationException("appSettings section not found in config file.");
			}
			string result;
			try
			{
				XmlElement xmlElement = (XmlElement)xmlNode.SelectSingleNode(string.Format("//add[@key='{0}']", key));
				if (xmlElement != null)
				{
					result = xmlElement.GetAttribute("value");
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
			XmlDocument xmlDocument = ConfigSettings.loadConfigDocument();
			XmlNode xmlNode = xmlDocument.SelectSingleNode("//appSettings");
			if (xmlNode == null)
			{
				throw new InvalidOperationException("appSettings section not found in config file.");
			}
			try
			{
				XmlElement xmlElement = (XmlElement)xmlNode.SelectSingleNode(string.Format("//add[@key='{0}']", key));
				if (xmlElement != null)
				{
					xmlElement.SetAttribute("value", value);
				}
				else
				{
					xmlElement = xmlDocument.CreateElement("add");
					xmlElement.SetAttribute("key", key);
					xmlElement.SetAttribute("value", value);
					xmlNode.AppendChild(xmlElement);
				}
				xmlDocument.Save(ConfigSettings.getConfigFilePath());
			}
			catch
			{
				throw;
			}
		}

		public static void RemoveSetting(string key)
		{
			XmlDocument xmlDocument = ConfigSettings.loadConfigDocument();
			XmlNode xmlNode = xmlDocument.SelectSingleNode("//appSettings");
			try
			{
				if (xmlNode == null)
				{
					throw new InvalidOperationException("appSettings section not found in config file.");
				}
				XmlNode expr_21 = xmlNode;
				expr_21.RemoveChild(expr_21.SelectSingleNode(string.Format("//add[@key='{0}']", key)));
				xmlDocument.Save(ConfigSettings.getConfigFilePath());
			}
			catch (NullReferenceException innerException)
			{
				throw new Exception(string.Format("The key {0} does not exist.", key), innerException);
			}
		}

		private static XmlDocument loadConfigDocument()
		{
			XmlDocument result;
			try
			{
				XmlDocument xmlDocument = new XmlDocument();
				string configFilePath = ConfigSettings.getConfigFilePath();
				if (File.Exists(configFilePath))
				{
					xmlDocument.Load(ConfigSettings.getConfigFilePath());
				}
				else
				{
					XmlDeclaration newChild = xmlDocument.CreateXmlDeclaration("1.0", "utf-8", null);
					xmlDocument.AppendChild(newChild);
					XmlElement xmlElement = xmlDocument.CreateElement("configuration");
					xmlDocument.AppendChild(xmlElement);
					XmlElement xmlElement2 = xmlDocument.CreateElement("appSettings");
					XmlElement xmlElement3 = xmlDocument.CreateElement("add");
					xmlElement3.SetAttribute("key", "steam");
					xmlElement3.SetAttribute("value", "");
					xmlElement2.AppendChild(xmlElement3);
					xmlElement.AppendChild(xmlElement2);
					xmlDocument.Save(configFilePath);
				}
				result = xmlDocument;
			}
			catch (FileNotFoundException innerException)
			{
				throw new Exception("No configuration file found.", innerException);
			}
			return result;
		}

		private static string getConfigFilePath()
		{
			return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\TPCAST\\App.config";
		}
	}
}
