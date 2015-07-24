
// just uncomment this line to restrict file access to KSP installation folder
#define _UNLIMITED_FILE_ACCESS
// for debugging
// #define _DEBUG

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;



namespace AutomatedScreenshots
{
	public  class FileOperations
	{
		public static readonly String ROOT_PATH = KSPUtil.ApplicationRootPath;
		private static readonly String CONFIG_BASE_FOLDER = ROOT_PATH + "GameData/";
		private static String AS_BASE_FOLDER = CONFIG_BASE_FOLDER + "SpaceTux/AS/";
		private static String AS_NODENAME = "AutomatedScreenshots";
		private static String AS_CFG_FILE = AS_BASE_FOLDER + "AS_Settings.cfg";

		private static ConfigNode configFile = null;
		private static ConfigNode configFileNode = null;


		public static String ScreenshotFolder ()
		{
			String folder = AS.configuration.screenshotPath;
			return folder;
		}

		public static void SaveConfiguration (Configuration configuration, String file)
		{
			if (configFile == null) {
				Log.Info ("Creating configFile node");
				configFile = new ConfigNode ();
			}
			if (!configFile.HasNode (AS_NODENAME)) {
				Log.Info ("Creating configFileNode");
				configFileNode = new ConfigNode (AS_NODENAME);
				Log.Info ("node created");
				configFile.SetNode (AS_NODENAME, configFileNode, true);
			} else {
				Log.Info ("Reading node");
				if (configFileNode == null) {
					configFileNode = configFile.GetNode (AS_NODENAME);
					Log.Info ("Node read");
					if (configFileNode == null)
						Log.Info ("configFileNode is null");
					
				}
			}
				
			configFileNode.SetValue ("logLevel", ((ushort)configuration.logLevel).ToString (), true);
			configFileNode.SetValue ("screenshotPath", configuration.screenshotPath.ToString (), true);
			configFileNode.SetValue ("filenamePrefix", configuration.filename.ToString (), true);
			configFileNode.SetValue ("screenshotAtIntervals", configuration.screenshotAtIntervals.ToString (), true);
			configFileNode.SetValue ("screenshotInterval", configuration.screenshotInterval.ToString (), true);
			configFileNode.SetValue ("convertToJPG", configuration.convertToJPG.ToString (), true);
			configFileNode.SetValue ("keepOrginalPNG", configuration.keepOrginalPNG.ToString (), true);
			configFileNode.SetValue ("JPGQuality", configuration.JPGQuality.ToString (), true);
			configFileNode.SetValue ("asynchronous", configuration.asynchronous.ToString (), true);
			configFileNode.SetValue ("screenshotOnSceneChange", configuration.screenshotOnSceneChange.ToString (), true);
			configFileNode.SetValue ("useBlizzyToolbar", configuration.useBlizzyToolbar.ToString (), true);
			configFileNode.SetValue ("onSpecialEvent", configuration.onSpecialEvent.ToString (), true);
			configFileNode.SetValue ("keycode", configuration.keycode.ToString (), true);
		
			configuration.keycode = AS.setActiveKeycode (configuration.keycode.ToString()).ToString();
			configFile.Save (AS_CFG_FILE);
		}

		public static void LoadConfiguration (Configuration configuration, String file)
		{
			configFile = ConfigNode.Load (AS_CFG_FILE);
			if (configFile != null) {
				Log.Info ("configFile loaded,file: " + AS_CFG_FILE);
				configFileNode = configFile.GetNode (AS_NODENAME);
				if (configFileNode != null) {
					configuration.logLevel = (Log.LEVEL)int.Parse (configFileNode.GetValue ("logLevel"));
					configuration.screenshotPath = configFileNode.GetValue ("screenshotPath");
					if (configuration.screenshotPath [configuration.screenshotPath.Length - 1] != '/' && configuration.screenshotPath [configuration.screenshotPath.Length - 1] != '\\')
						configuration.screenshotPath += '/';
					if (!(configuration.filename.Contains ("/") || configuration.filename.Contains ("\\")))
						configuration.filename = configFileNode.GetValue ("filenamePrefix");
					else
						configuration.filename = "AS-";
					configuration.screenshotAtIntervals = bool.Parse (configFileNode.GetValue ("screenshotAtIntervals"));
					configuration.screenshotInterval = ushort.Parse (configFileNode.GetValue ("screenshotInterval"));
					configuration.convertToJPG = bool.Parse (configFileNode.GetValue ("convertToJPG"));
					configuration.keepOrginalPNG = bool.Parse (configFileNode.GetValue ("keepOrginalPNG"));
					configuration.JPGQuality = ushort.Parse (configFileNode.GetValue ("JPGQuality"));
					configuration.asynchronous = bool.Parse (configFileNode.GetValue ("asynchronous"));
					configuration.screenshotOnSceneChange = bool.Parse (configFileNode.GetValue ("screenshotOnSceneChange"));
					configuration.useBlizzyToolbar = bool.Parse (configFileNode.GetValue ("useBlizzyToolbar"));
					configuration.onSpecialEvent = bool.Parse (configFileNode.GetValue ("onSpecialEvent"));
					configuration.keycode = configFileNode.GetValue ("keycode");
					if (configuration.keycode == null)
						configuration.keycode = "F6";
					configuration.keycode= AS.setActiveKeycode (configuration.keycode.ToString()).ToString();
				}
			}
		}
	}
}
