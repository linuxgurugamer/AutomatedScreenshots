//using UnityEngine;
using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;


namespace AutomatedScreenshots
{
	public class Configuration
	{
		private static readonly Configuration instance = new Configuration ();

		private static readonly String FILE_NAME = "AutomatedScreenshot.dat";
		public  ushort MAX_SUPERSIZE = 4;

		[Persistent] public Log.LEVEL logLevel { get; set; }

//		public bool screenshotAtIntervals { get; set; }
		public float screenshotInterval { get; set; }
		public bool convertToJPG { get; set; }
		public bool keepOrginalPNG { get; set; }
		public string screenshotPath { get; set; }
		public string filename { get; set; }
		public bool asynchronous { get; set; }
		public ushort JPGQuality { get; set; }
		public bool screenshotOnSceneChange { get; set; }
		public bool onSpecialEvent { get; set; }
		public bool useBlizzyToolbar { get; set; }
		public bool noGUIOnScreenshot { get; set; }
		public bool guiOnScreenshot { get; set; }
		public string keycode { get; set; }

		public bool precrashSnapshots { get; set; }
		public ushort secondsUntilImpact {get; set; }
		public ushort hsAltitudeLimit { get; set; }
		public ushort hsMinVerticalSpeed { get; set; }
		public float hsScreenshotInterval { get; set; }

		public ushort supersize { get; set; }

		internal Boolean BlizzyToolbarIsAvailable = false;

		//
		// Automated saves info
		//
		public bool autoSave;
		public ushort minBetweenSaves;
		public string savePrefix;
		public ushort numToRotate;
	//	public string toggleAutoSave;

		public Configuration ()
		{
#if (DEBUG)
			logLevel = Log.LEVEL.INFO;
#else
			logLevel = Log.LEVEL.WARNING;
#endif
			Log.Info ("Configuration - Setting default config");

//			screenshotAtIntervals = false;
			screenshotInterval = 5.0F;
			convertToJPG = true;
			keepOrginalPNG = false;
			screenshotPath = FileOperations.ROOT_PATH + "Screenshots/";
			noGUIOnScreenshot = false;
			guiOnScreenshot = true;
			filename = "AS-[cnt]";
			asynchronous = false;
			JPGQuality = 75;
			screenshotOnSceneChange = false;
			onSpecialEvent = false;
			useBlizzyToolbar = false;
			keycode = "F6";

			precrashSnapshots = false;
			secondsUntilImpact = 5;
			hsAltitudeLimit = 25;
			hsMinVerticalSpeed = 3;
			hsScreenshotInterval = 0.2F;

			supersize = 0;

			autoSave = false;
			minBetweenSaves = 5;
			savePrefix = "rotate-[cnt]";
			numToRotate = 15;
			//toggleAutoSave = "Ctrl-F5";
		}

		public static Configuration Instance {
			get {
				return instance; 
			}
		}
			
		public void Save ()
		{
			Log.Info ("Configuration.Save");
			FileOperations.SaveConfiguration (this, FILE_NAME);
			AS.changeCallbacks = true;
		}

		public void Load ()
		{
			Log.Info ("Configuration.Load");
			FileOperations.LoadConfiguration (this, FILE_NAME);
		}

	}
}
