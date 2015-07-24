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
		public string keycode { get; set; }

		internal Boolean BlizzyToolbarIsAvailable = false;

		public Configuration ()
		{
			logLevel = Log.LEVEL.INFO;
			Log.Info ("Configuration - Setting default config");

			screenshotAtIntervals = false;
			screenshotInterval = 5;
			convertToJPG = true;
			keepOrginalPNG = false;
			screenshotPath = FileOperations.ROOT_PATH + "Screenshots/";
			filename = "AS-";
			asynchronous = false;
			JPGQuality = 75;
			screenshotOnSceneChange = false;
			onSpecialEvent = false;
			useBlizzyToolbar = false;
			keycode = "F6";
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
