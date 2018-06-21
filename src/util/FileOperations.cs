﻿
// just uncomment this line to restrict file access to KSP installation folder
#define _UNLIMITED_FILE_ACCESS

using System;
using System.IO;
using UnityEngine;
//using System.Collections.Generic;
//using System.IO;
//using System.Threading;



namespace AutomatedScreenshots
{
	public  class FileOperations
	{
		public static readonly String ROOT_PATH = KSPUtil.ApplicationRootPath;
		private static readonly String CONFIG_BASE_FOLDER = ROOT_PATH + "GameData/";
		private static String AS_BASE_FOLDER = CONFIG_BASE_FOLDER + "AutomatedScreenShots/";
		private static String AS_NODENAME = "AutomatedScreenShots";
        private static String AS_CFG_FILE = AS_BASE_FOLDER + "PluginData/AS_Settings.cfg";
        private static String AS_OLD_CFG_FILE = AS_BASE_FOLDER + "AS_Settings.cfg";


        private static ConfigNode configFile = null;
		private static ConfigNode configFileNode = null;


        public static void MoveCfgToDataDir()
        {
            if (File.Exists(AS_OLD_CFG_FILE))
            {
                if (!File.Exists(AS_CFG_FILE))
                {
                    try
                    {
                        File.Copy(AS_OLD_CFG_FILE, AS_CFG_FILE);
                        File.Delete(AS_OLD_CFG_FILE);
                    }
                    catch (Exception e)
                    { }
                }
            }
            else
                File.Delete(AS_OLD_CFG_FILE);
        }


        public static String ScreenshotFolder ()
		{
			String folder = AS.configuration.screenshotPath;
			return folder;
		}

#if (!_UNLIMITED_FILE_ACCESS)
		public static bool InsideApplicationRootPath(String path)
		{
			if (path == null) return false;
			try
			{
				String fullpath = Path.GetFullPath(path);
				return fullpath.StartsWith(Path.GetFullPath(ROOT_PATH));
			}
			catch
			{
				return false;
			}
		}
#endif
		public static bool ValidPathForWriteOperation(String path)
		{
#if (_UNLIMITED_FILE_ACCESS)
			return true;
#else
			String fullpath = Path.GetFullPath(path);
			return InsideApplicationRootPath(fullpath);
#endif
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
			if (!ValidPathForWriteOperation(configuration.screenshotPath))
				configuration.screenshotPath = FileOperations.ROOT_PATH + "Screenshots/";
			configFileNode.SetValue ("screenshotPath", configuration.screenshotPath.ToString (), true);
			configFileNode.SetValue ("filenameFormat", configuration.filename.ToString (), true);
//			configFileNode.SetValue ("screenshotAtIntervals", configuration.screenshotAtIntervals.ToString (), true);
			configFileNode.SetValue ("screenshotInterval", configuration.screenshotInterval.ToString (), true);
			configFileNode.SetValue ("convertToJPG", configuration.convertToJPG.ToString (), true);
			configFileNode.SetValue ("keepOrginalPNG", configuration.keepOrginalPNG.ToString (), true);
			configFileNode.SetValue ("JPGQuality", configuration.JPGQuality.ToString (), true);
			configFileNode.SetValue ("asynchronous", configuration.asynchronous.ToString (), true);
			configFileNode.SetValue ("screenshotOnSceneChange", configuration.screenshotOnSceneChange.ToString (), true);
			configFileNode.SetValue ("onSpecialEvent", configuration.onSpecialEvent.ToString (), true);
			configFileNode.SetValue ("keycode", configuration.keycode.ToString (), true);
			configFileNode.SetValue ("noGUIOnScreenshot", configuration.noGUIOnScreenshot.ToString (), true);
			configFileNode.SetValue ("guiOnScreenshot", configuration.guiOnScreenshot.ToString (), true);
		
			configFileNode.SetValue ("precrashSnapshots", configuration.precrashSnapshots.ToString (), true);
			configFileNode.SetValue ("secondsUntilImpact", configuration.secondsUntilImpact.ToString (), true);
			configFileNode.SetValue ("hsAltitudeLimit", configuration.hsAltitudeLimit.ToString (), true);
			configFileNode.SetValue ("hsMinVerticalSpeed", configuration.hsMinVerticalSpeed.ToString (), true);
			configFileNode.SetValue ("hsScreenshotInterval", configuration.hsScreenshotInterval.ToString (), true);

			configFileNode.SetValue ("supersize", configuration.supersize.ToString (), true);

			configFileNode.SetValue ("minBetweenSaves", configuration.minBetweenSaves.ToString (), true);
			configFileNode.SetValue ("saveFilePrefix", configuration.savePrefix, true);
			configFileNode.SetValue ("maxSaveFiles", configuration.numToRotate.ToString (), true);

			configFileNode.SetValue ("autoSaveOnGameStart", configuration.autoSaveOnGameStart.ToString (), true);

			configuration.keycode = AS.setActiveKeycode (configuration.keycode.ToString ()).ToString ();

			configFile.Save (AS_CFG_FILE);
		}

		//
		// The following functions are used when loading data from the config file
		// They make sure that if a value is missing, that the old value will be used.
		// 
		static string SafeLoad (string value, string oldvalue)
		{
			if (value == null)
				return oldvalue;
			return value;
		}
		static string SafeLoad (string value, bool oldvalue)
		{
			if (value == null)
				return oldvalue.ToString();
			return value;
		}
		static string SafeLoad (string value, ushort oldvalue)
		{
			if (value == null)
				return oldvalue.ToString();
			return value;
		}
		static string SafeLoad (string value, float oldvalue)
		{
			if (value == null)
				return oldvalue.ToString();
			return value;
		}

		public static void LoadConfiguration (Configuration configuration, String file)
		{
			configFile = ConfigNode.Load (AS_CFG_FILE);
	
			if (configFile != null) {
				configFileNode = configFile.GetNode (AS_NODENAME);
				if (configFileNode != null) {
					
					configuration.logLevel = (Log.LEVEL)int.Parse (SafeLoad (configFileNode.GetValue ("logLevel"), configuration.logLevel.ToString ()));
					configuration.screenshotPath = SafeLoad (configFileNode.GetValue ("screenshotPath"), configuration.screenshotPath);
					if (configuration.screenshotPath [configuration.screenshotPath.Length - 1] != '/' && configuration.screenshotPath [configuration.screenshotPath.Length - 1] != '\\')
						configuration.screenshotPath += '/';
					if (!ValidPathForWriteOperation(configuration.screenshotPath))
								configuration.screenshotPath = FileOperations.ROOT_PATH + "Screenshots/";

					configuration.filename = SafeLoad (configFileNode.GetValue ("filenameFormat"), configuration.filename);
					if ((configuration.filename.Contains ("/") || configuration.filename.Contains ("\\")))
						configuration.filename = "AS-[cnt]";

					configuration.screenshotInterval = float.Parse (SafeLoad(configFileNode.GetValue ("screenshotInterval"),configuration.screenshotInterval));
					if (configuration.screenshotInterval < Time.deltaTime)
						configuration.screenshotInterval = Time.deltaTime;
					configuration.convertToJPG = bool.Parse (SafeLoad(configFileNode.GetValue ("convertToJPG"),configuration.convertToJPG));
					configuration.keepOrginalPNG = bool.Parse (SafeLoad(configFileNode.GetValue ("keepOrginalPNG"),configuration.keepOrginalPNG));
					configuration.JPGQuality = ushort.Parse (SafeLoad(configFileNode.GetValue ("JPGQuality"),configuration.JPGQuality));
					configuration.asynchronous = bool.Parse (SafeLoad(configFileNode.GetValue ("asynchronous"),configuration.asynchronous));
					configuration.screenshotOnSceneChange = bool.Parse (SafeLoad(configFileNode.GetValue ("screenshotOnSceneChange"),configuration.screenshotOnSceneChange));
					configuration.onSpecialEvent = bool.Parse (SafeLoad(configFileNode.GetValue ("onSpecialEvent"),configuration.onSpecialEvent));
					configuration.keycode = SafeLoad(configFileNode.GetValue ("keycode"),configuration.keycode);
					if (configuration.keycode == null)
						configuration.keycode = "F6";
					configuration.keycode = AS.setActiveKeycode (configuration.keycode.ToString ()).ToString ();
					configuration.noGUIOnScreenshot = bool.Parse(SafeLoad(configFileNode.GetValue ("noGUIOnScreenshot"),configuration.noGUIOnScreenshot));
					configuration.guiOnScreenshot = bool.Parse(SafeLoad(configFileNode.GetValue ("guiOnScreenshot"),configuration.guiOnScreenshot));

					configuration.precrashSnapshots = bool.Parse (SafeLoad(configFileNode.GetValue ("precrashSnapshots"),configuration.precrashSnapshots));
					configuration.secondsUntilImpact = ushort.Parse (SafeLoad(configFileNode.GetValue ("secondsUntilImpact"),configuration.secondsUntilImpact));
					configuration.hsAltitudeLimit = ushort.Parse (SafeLoad(configFileNode.GetValue ("hsAltitudeLimit"),configuration.hsAltitudeLimit));
					configuration.hsMinVerticalSpeed = ushort.Parse (SafeLoad(configFileNode.GetValue ("hsMinVerticalSpeed"),configuration.hsMinVerticalSpeed));
					configuration.hsScreenshotInterval = float.Parse (SafeLoad(configFileNode.GetValue ("hsScreenshotInterval"),configuration.hsScreenshotInterval));
					if (configuration.hsScreenshotInterval < Time.deltaTime)
						configuration.hsScreenshotInterval = Time.deltaTime;

					configuration.supersize = ushort.Parse(SafeLoad(configFileNode.GetValue ("supersize"),configuration.supersize));
					if (configuration.supersize < 0 )
						configuration.supersize = 0;
					if (configuration.supersize > configuration.MAX_SUPERSIZE )
						configuration.supersize = configuration.MAX_SUPERSIZE;
					
					configuration.minBetweenSaves = ushort.Parse(SafeLoad(configFileNode.GetValue ("minBetweenSaves"),configuration.minBetweenSaves));
					if (configuration.minBetweenSaves <= 0)
						configuration.minBetweenSaves = 5;
					configuration.savePrefix = SafeLoad (configFileNode.GetValue ("saveFilePrefix"), configuration.savePrefix);
					if ((configuration.savePrefix.Contains ("/") || configuration.savePrefix.Contains ("\\")))
						configuration.savePrefix = "rotate-[cnt]";
					configuration.numToRotate = ushort.Parse(SafeLoad(configFileNode.GetValue ("maxSaveFiles"),configuration.numToRotate));
					if (configuration.numToRotate <= 0)
						configuration.numToRotate = 15;
					
					configuration.autoSaveOnGameStart =
						bool.Parse (SafeLoad(configFileNode.GetValue ("autoSaveOnGameStart"),configuration.autoSaveOnGameStart));
				}
			}
		}
	}
}
