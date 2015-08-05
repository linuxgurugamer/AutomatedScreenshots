using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace AutomatedScreenshots
{
	public class Configuration
	{
		private static readonly Configuration instance = new Configuration ();

		private static readonly String FILE_NAME = "AutomatedScreenshot.dat";

		[Persistent] public Log.LEVEL logLevel { get; set; }

		public bool screenshotAtIntervals { get; set; }
		public ushort screenshotInterval { get; set; }
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
		public ushort hsScreenshotInterval { get; set; }

		internal Boolean BlizzyToolbarIsAvailable = false;

		public Configuration ()
		{
#if (DEBUG)
			logLevel = Log.LEVEL.INFO;
#else
			logLevel = Log.LEVEL.WARNING;
#endif
			Log.Info ("Configuration - Setting default config");

			screenshotAtIntervals = false;
			screenshotInterval = 5;
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
			hsScreenshotInterval = 1;
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
