
using System;
using UnityEngine;
//using System.IO;
using KSP.UI.Screens;

using ClickThroughFix;
using ToolbarControl_NS;

namespace AutomatedScreenshots
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class RegisterToolbar : MonoBehaviour
    {
        void Start()
        {
            ToolbarControl.RegisterMod(MainMenuGui.MODID, MainMenuGui.MODNAME);


                    FileOperations. ROOT_PATH = KSPUtil.ApplicationRootPath;
            FileOperations.CONFIG_BASE_FOLDER = FileOperations.ROOT_PATH + "GameData/";
            FileOperations.AS_BASE_FOLDER = FileOperations.CONFIG_BASE_FOLDER + "AutomatedScreenShots/";
            FileOperations.AS_NODENAME = "AutomatedScreenShots";
            FileOperations.AS_CFG_FILE = FileOperations.AS_BASE_FOLDER + "PluginData/AS_Settings.cfg";
            FileOperations.AS_OLD_CFG_FILE = FileOperations.AS_BASE_FOLDER + "AS_Settings.cfg";

    }
}
    public class MainMenuGui : MonoBehaviour
	{
        public static MainMenuGui Instance;

		private const int WIDTH = 725;
		private const int HEIGHT = 425;
		private Rect bounds = new Rect (Screen.width / 2 - WIDTH / 2, Screen.height / 2 - HEIGHT / 2, WIDTH, HEIGHT);
		private /* volatile*/ bool visible = false;

        public static ToolbarControl toolbarControl = null;
		public  bool stockToolBarcreated = false;

		public static Texture2D AS_button_off = new Texture2D (38, 38, TextureFormat.ARGB32, false);
		public static Texture2D AS_button_config = new Texture2D (38, 38, TextureFormat.ARGB32, false);
		public static Texture2D AS_button_save = new Texture2D (38, 38, TextureFormat.ARGB32, false);
		public static Texture2D AS_button_snapshot = new Texture2D (38, 38, TextureFormat.ARGB32, false);
		public static Texture2D AS_button_snapshot_save = new Texture2D (38, 38, TextureFormat.ARGB32, false);


		//private bool AS_Texture_Load = false;

		private bool cfgWinData = false;
//		private static bool newScreenshotAtIntervals = true;
		private static float newInterval = 1.0F;
		private static string interval = "";
		private static bool newConvertToJPG;
		private static bool newKeepOrginalPNG;
		private static string newScreenshotPath = "";
		private static string newFilename = "";
		private static string JPGQuality = "";
		private static ushort newJPGQuality;
		private static bool newScreenshotOnSceneChange;
		private static bool newUseBlizzyToolbar;
		private static bool newOnSpecialEvent;
		private static bool newNoGUIOnScreenshot;
		private static bool newGUIOnScreenshot;
		private static bool newprecrashSnapshots;
		private static bool blizzyToolbarInstalled = false;
		private static bool appLaucherHidden = true;
		private static string newKeycode = "";
		private static ushort newsecondsUntilImpact;
		private static ushort newhsAltitudeLimit;
		private static ushort newhsMinVerticalSpeed;
		private static float newhsScreenshotInterval;
		private static string secondsUntilImpact = "";
		private static string hsAltitudeLimit = "";
		private static string hsMinVerticalSpeed = "";
		private static string hsScreenshotInterval = "";


		private static string strsupersize = "";
		private ushort newsupersize;

		private bool newautoSave;
		private ushort newminBetweenSaves;
		private string minBetweenSaves;
		private string newsavePrefix;
		private ushort newnumToRotate;
		private string numToRotate;
		private bool newautoSaveOnGameStart;

		internal MainMenuGui ()
		{
            Instance = this;
			blizzyToolbarInstalled = ToolbarManager.ToolbarAvailable;
		}

		public void setAppLauncherHidden()
		{
			appLaucherHidden = true;
		}

		public void OnGUIHideApplicationLauncher ()
		{
			if (!appLaucherHidden) {

                //if (AS.configuration.BlizzyToolbarIsAvailable && AS.configuration.useBlizzyToolbar) 
                {
					HideToolbarStock ();
					appLaucherHidden = true;
				}

			}
		}
			
		public void OnGUIShowApplicationLauncher ()
		{
            //if (!AS.configuration.BlizzyToolbarIsAvailable || !AS.configuration.useBlizzyToolbar) 
            {
				if (appLaucherHidden) {
					appLaucherHidden = false;
					if (MainMenuGui.toolbarControl != null)
						UpdateToolbarStock ();
				}
			}
		}

		public void OnGUIApplicationLauncherReady ()
		{
			UpdateToolbarStock ();
		}
        public const string TEXTURE_DIR = "AutomatedScreenshots/PluginData/Textures/";
        internal const string MODID = "AutomatedScreenshot_NS";
        internal const string MODNAME = "Automated Screenshot";
        private void UpdateToolbarStock ()
		{
			Log.Info ("UpdateToolbarStock, appLaucherHidden: " + appLaucherHidden.ToString());

            if (toolbarControl == null)
            {
                toolbarControl = gameObject.AddComponent<ToolbarControl>();
                toolbarControl.AddToAllToolbars(GUIToggle, GUIToggleFalse,
                    ApplicationLauncher.AppScenes.ALWAYS & ~ApplicationLauncher.AppScenes.MAINMENU,
                    MODID,
                    "automatedScreenshotButton",
                    MainMenuGui.TEXTURE_DIR + "Auto-38",
                    MainMenuGui.TEXTURE_DIR + "Auto-24",
                    MODNAME
                );
            }
        }

        private void HideToolbarStock ()
		{
			Log.Info ("HideToolbarStock");
            toolbarControl.OnDestroy();
            Destroy(toolbarControl);

            toolbarControl = null;
			appLaucherHidden = false;
		}

		public bool Visible ()
		{ 
			return this.visible;
		}

		public void SetVisible (bool visible)
		{
			this.visible = visible;
		}

		
		/////////////////////////////////////
		public void OnGUI ()
		{
            try
            {
				if (this.Visible ()) {
					this.bounds = ClickThruBlocker.GUILayoutWindow (this.GetInstanceID (), this.bounds, this.Window, AS.TITLE, HighLogic.Skin.window);
				}
			} catch (Exception e) {
				Log.Error ("exception: " + e.Message);
			}
		}

		private void Window (int id)
		{
			if (cfgWinData == false) {
				cfgWinData = true;
//				newScreenshotAtIntervals = AS.configuration.screenshotAtIntervals;
				newInterval = AS.configuration.screenshotInterval;
				interval = newInterval.ToString ();
				newConvertToJPG = AS.configuration.convertToJPG;
				newKeepOrginalPNG = AS.configuration.keepOrginalPNG;
				newNoGUIOnScreenshot = AS.configuration.noGUIOnScreenshot;
				newGUIOnScreenshot = AS.configuration.guiOnScreenshot;
					
				newScreenshotPath = AS.configuration.screenshotPath;
				newFilename = AS.configuration.filename;
				newJPGQuality = AS.configuration.JPGQuality;
				JPGQuality = newJPGQuality.ToString ();
				newScreenshotOnSceneChange = AS.configuration.screenshotOnSceneChange;
				newOnSpecialEvent = AS.configuration.onSpecialEvent;
				newKeycode = AS.configuration.keycode;

				newprecrashSnapshots = AS.configuration.precrashSnapshots;
				newsecondsUntilImpact = AS.configuration.secondsUntilImpact;
				newhsAltitudeLimit = AS.configuration.hsAltitudeLimit;
				newhsMinVerticalSpeed = AS.configuration.hsMinVerticalSpeed;
				newhsScreenshotInterval = AS.configuration.hsScreenshotInterval;

				secondsUntilImpact = AS.configuration.secondsUntilImpact.ToString();
				hsAltitudeLimit = AS.configuration.hsAltitudeLimit.ToString();
				hsMinVerticalSpeed = AS.configuration.hsMinVerticalSpeed.ToString();
				hsScreenshotInterval = AS.configuration.hsScreenshotInterval.ToString();

				newsupersize = AS.configuration.supersize;
				strsupersize = AS.configuration.supersize.ToString();

				newautoSave = AS.configuration.autoSave;
				newminBetweenSaves = AS.configuration.minBetweenSaves;
				minBetweenSaves = AS.configuration.minBetweenSaves.ToString ();
				newsavePrefix = AS.configuration.savePrefix;
				newnumToRotate = AS.configuration.numToRotate;
				numToRotate = AS.configuration.numToRotate.ToString ();
				newautoSaveOnGameStart = AS.configuration.autoSaveOnGameStart;
			} 

			SetVisible (true);
			GUI.enabled = true;

			GUILayout.BeginHorizontal ();
			GUILayout.EndHorizontal ();
			//DrawTitle ("Options");
			GUILayout.BeginArea (new Rect (10, 50, 375, 450));

			GUILayout.BeginVertical ();

			DrawTitle ("Screenshot Options");

			//GUILayout.BeginHorizontal ();
			//GUILayout.Label ("Take screenshots at specified intervals: ");
			//GUILayout.FlexibleSpace ();
			//newScreenshotAtIntervals = GUILayout.Toggle (newScreenshotAtIntervals, "");
			//GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Screenshot Interval in seconds: ");
			GUILayout.FlexibleSpace ();
			interval = GUILayout.TextField (interval, GUILayout.MinWidth (30.0F), GUILayout.MaxWidth (30.0F));
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Convert to JPG: ");
			GUILayout.FlexibleSpace ();
			newConvertToJPG = GUILayout.Toggle (newConvertToJPG, "");
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Keep original PNG: ");
			GUILayout.FlexibleSpace ();
			newKeepOrginalPNG = GUILayout.Toggle (newKeepOrginalPNG, "");
			GUILayout.EndHorizontal ();


			GUILayout.BeginHorizontal ();
			GUILayout.Label ("JPEG Quality (1-100):");
			GUILayout.FlexibleSpace ();
			JPGQuality = GUILayout.TextField (JPGQuality, GUILayout.MinWidth (30.0F), GUILayout.MaxWidth (30.0F));
			GUILayout.EndHorizontal ();


			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Supersize images (0-4):");
			GUILayout.FlexibleSpace ();
			strsupersize = GUILayout.TextField (strsupersize, GUILayout.MinWidth (30.0F), GUILayout.MaxWidth (30.0F));
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Screenshot path:");
			GUILayout.FlexibleSpace ();
//			GUILayout.EndHorizontal ();

//			GUILayout.BeginHorizontal ();
//			GUILayout.FlexibleSpace ();
			newScreenshotPath = GUILayout.TextField (newScreenshotPath, GUILayout.MinWidth (50F), GUILayout.MaxWidth (250F));
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Filename Format:");
			GUILayout.FlexibleSpace ();
			newFilename = GUILayout.TextField (newFilename, GUILayout.MinWidth (30F), GUILayout.MaxWidth (160F));
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Screenshot after scene change:");
			GUILayout.FlexibleSpace ();
			newScreenshotOnSceneChange = GUILayout.Toggle (newScreenshotOnSceneChange, "");
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Screenshot after special event:");
			GUILayout.FlexibleSpace ();
			newOnSpecialEvent = GUILayout.Toggle (newOnSpecialEvent, "");
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Use Blizzy Toolbar if available:");
			GUILayout.FlexibleSpace ();
			newUseBlizzyToolbar = GUILayout.Toggle (newUseBlizzyToolbar, "");
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Activation Keycode:");
			GUILayout.FlexibleSpace ();
			newKeycode = GUILayout.TextField (newKeycode, GUILayout.MinWidth (30F), GUILayout.MaxWidth (40F));
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("No GUI on screenshot:");
			GUILayout.FlexibleSpace ();
			newNoGUIOnScreenshot = GUILayout.Toggle (newNoGUIOnScreenshot, "");
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("GUI on screenshot:   ");
			GUILayout.FlexibleSpace ();
			newGUIOnScreenshot = GUILayout.Toggle (newGUIOnScreenshot, "");
			GUILayout.EndHorizontal ();

			GUILayout.EndVertical ();
			GUILayout.EndArea ();


			GUILayout.BeginArea (new Rect (400, 50, 300, 400));
			GUILayout.BeginVertical ();

			DrawTitle("Pre-Crash Settings");

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Take pre-crash snapshots:");
			GUILayout.FlexibleSpace ();
			newprecrashSnapshots = GUILayout.Toggle (newprecrashSnapshots, "");
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Seconds until impact:");
			secondsUntilImpact = GUILayout.TextField (secondsUntilImpact, GUILayout.MinWidth (30.0F), GUILayout.MaxWidth (30.0F));
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Altitude limit (meters):");
			hsAltitudeLimit = GUILayout.TextField (hsAltitudeLimit, GUILayout.MinWidth (30.0F), GUILayout.MaxWidth (60.0F));
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Minimum vertical speed:");
			hsMinVerticalSpeed = GUILayout.TextField (hsMinVerticalSpeed, GUILayout.MinWidth (30.0F), GUILayout.MaxWidth (30.0F));
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Screenshot interval (pre-crash):");
			hsScreenshotInterval = GUILayout.TextField (hsScreenshotInterval, GUILayout.MinWidth (30.0F), GUILayout.MaxWidth (30.0F));
			GUILayout.EndHorizontal ();


			GUILayout.BeginHorizontal ();
			GUILayout.Label ("");
			GUILayout.EndHorizontal ();

			DrawTitle("Automatic Save Settings");

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Activation Key (not configurable):");
			GUILayout.FlexibleSpace ();
			GUILayout.Label ("Ctrl-F6");
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Minutes between saves:");
			minBetweenSaves = GUILayout.TextField (minBetweenSaves, GUILayout.MinWidth (30.0F), GUILayout.MaxWidth (30.0F));
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Save file format:");
			newsavePrefix = GUILayout.TextField (newsavePrefix, GUILayout.MinWidth (30.0F), GUILayout.MaxWidth (160.0F));
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Max save files:");
			numToRotate = GUILayout.TextField (numToRotate, GUILayout.MinWidth (30.0F), GUILayout.MaxWidth (30.0F));
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Autosave on at game start:");
			GUILayout.FlexibleSpace ();
			newautoSaveOnGameStart = GUILayout.Toggle (newautoSaveOnGameStart, "");
			GUILayout.EndHorizontal ();



			GUILayout.EndVertical ();
			GUILayout.EndArea ();


			//
			// I probably don't need to have the "finally" sections, but
			// it doesn't hurt and will be there if I need it in the future
			//
			try {
				newInterval = Convert.ToSingle(Convert.ToDouble (interval));
			} catch (Exception ) {
			} finally {	}

			try {
				newJPGQuality = Convert.ToUInt16 (JPGQuality);
			} catch (Exception ) {
			} finally {	}


			try {
				newsecondsUntilImpact = Convert.ToUInt16 (secondsUntilImpact);
			} catch (Exception ) {
			} finally {	}
			try {
				newhsAltitudeLimit = Convert.ToUInt16 (hsAltitudeLimit);
			} catch (Exception ) {
			} finally {	}
			try {
				newhsMinVerticalSpeed = Convert.ToUInt16 (hsMinVerticalSpeed);
			} catch (Exception ) {
			} finally {	}
			try {
				newhsScreenshotInterval = Convert.ToSingle(Convert.ToDouble (hsScreenshotInterval));
			} catch (Exception ) {
			} finally {	}

			try {
				newsupersize = Convert.ToUInt16(strsupersize);
				if (newsupersize < 0 )
					newsupersize = 0;
				if (newsupersize > AS.configuration.MAX_SUPERSIZE )
					newsupersize = AS.configuration.MAX_SUPERSIZE;
			} catch (Exception ) {
			} finally {	}

			try {
				newminBetweenSaves = Convert.ToUInt16(minBetweenSaves);
			} catch (Exception ) {
			} finally {	}

			try {
				newnumToRotate = Convert.ToUInt16(numToRotate);
			} catch (Exception ) {
			} finally {	}
				
			GUI.DragWindow ();

		}

		private void DrawTitle (String text)
		{
			GUILayout.BeginHorizontal ();
			GUILayout.Label (text, HighLogic.Skin.label);
			GUILayout.FlexibleSpace ();
			GUILayout.EndHorizontal ();
		}

		public void GUI_SaveData ()
		{
//			AS.configuration.screenshotAtIntervals = newScreenshotAtIntervals;
			AS.configuration.screenshotInterval = newInterval;
			if (AS.configuration.screenshotInterval < Time.deltaTime)
				AS.configuration.screenshotInterval = Time.deltaTime;
			AS.configuration.convertToJPG = newConvertToJPG;
			AS.configuration.keepOrginalPNG = newKeepOrginalPNG;
			if (newScreenshotPath [newScreenshotPath.Length - 1] != '/' && newScreenshotPath [newScreenshotPath.Length - 1] != '\\')
				newScreenshotPath += '/';
			AS.configuration.screenshotPath = newScreenshotPath;
			if (!(newFilename.Contains ("/") || newFilename.Contains ("\\")))
				AS.configuration.filename = newFilename;

			AS.configuration.JPGQuality = newJPGQuality;
			if (AS.configuration.JPGQuality < 1 || AS.configuration.JPGQuality > 100)
				AS.configuration.JPGQuality = 75;
			AS.configuration.screenshotOnSceneChange = newScreenshotOnSceneChange;	
			AS.configuration.onSpecialEvent = newOnSpecialEvent;
			AS.configuration.keycode = newKeycode;
			AS.configuration.noGUIOnScreenshot = newNoGUIOnScreenshot;
			AS.configuration.guiOnScreenshot = newGUIOnScreenshot;

			AS.configuration.precrashSnapshots = newprecrashSnapshots;
			AS.configuration.secondsUntilImpact = newsecondsUntilImpact;
			AS.configuration.hsAltitudeLimit = newhsAltitudeLimit;
			AS.configuration.hsMinVerticalSpeed = newhsMinVerticalSpeed;
			AS.configuration.hsScreenshotInterval = newhsScreenshotInterval;

			AS.configuration.supersize = newsupersize;

			AS.configuration.minBetweenSaves = newminBetweenSaves;
			AS.configuration.savePrefix = newsavePrefix;
			AS.configuration.numToRotate = newnumToRotate;
			AS.configuration.autoSaveOnGameStart = newautoSaveOnGameStart;
		}

		public void set_AS_Button_active()
		{
			Log.Info ("set_AS_Button_active   AS.doSnapshots: " + AS.doSnapshots.ToString() + "   AS.configuration.autoSave: " + AS.configuration.autoSave.ToString() );
 

				if (AS.doSnapshots == false && AS.configuration.autoSave == false)
					toolbarControl.SetTexture (TEXTURE_DIR + "Auto-38", TEXTURE_DIR + "Auto-24");
				if (AS.doSnapshots == true && AS.configuration.autoSave == false)
                toolbarControl.SetTexture (TEXTURE_DIR + "Auto-snapshot-38", TEXTURE_DIR + "Auto-snapshot-24");
				if (AS.doSnapshots == false && AS.configuration.autoSave == true)
                toolbarControl.SetTexture (TEXTURE_DIR + "Auto-save-38", TEXTURE_DIR + "Auto-save-24");
				if (AS.doSnapshots == true && AS.configuration.autoSave == true)
                toolbarControl.SetTexture (TEXTURE_DIR + "Auto-snapshot-save-38", TEXTURE_DIR + "Auto-snapshot-save-24");

		}

        public void GUIToggleFalse()
        {
            if (ASInfoDisplay.infoDisplayActive)
                GUIToggle();
        }

        public void GUIToggle ()
		{
			Log.Info ("GUIToggle");
			ASInfoDisplay.infoDisplayActive = !ASInfoDisplay.infoDisplayActive;
			if (ASInfoDisplay.infoDisplayActive) {
				SetVisible (true);
				toolbarControl.SetTexture (TEXTURE_DIR + "Auto-negative-38", TEXTURE_DIR + "Auto-negative-24"); 
			} else {
				SetVisible (false);
				set_AS_Button_active ();
				cfgWinData = false;

				GUI_SaveData ();

				AS.configuration.Save ();
#if false
                if (AS.configuration.BlizzyToolbarIsAvailable && AS.configuration.useBlizzyToolbar) {
					HideToolbarStock ();
;
				} else {
#endif
					UpdateToolbarStock ();
					set_AS_Button_active();

		//		}

			}
		}
	}
}