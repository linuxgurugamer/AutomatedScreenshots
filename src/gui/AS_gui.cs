#define STOCKTOOLBAR
//#undef STOCKTOOLBAR
#define BLIZZYTOOLBAR
// #undef BLIZZYTOOLBAR

using System;
using UnityEngine;
using System.IO;



namespace AutomatedScreenshots
{
	public class MainMenuGui : MonoBehaviour
	{
		private const String TITLE = "Automated Screenshots";
		private const int WIDTH = 400;
		private Rect bounds = new Rect (Screen.width / 2 - WIDTH / 2, Screen.height / 4, WIDTH, 0);
		private volatile bool visible = false;
		// Stock APP Toolbar - Stavell
		public static ApplicationLauncherButton AS_Button = null;
		public  bool stockToolBarcreated = false;
		private static Texture2D AS_button_on = new Texture2D (38, 38, TextureFormat.ARGB32, false);
		public static Texture2D AS_button_off = new Texture2D (38, 38, TextureFormat.ARGB32, false);
		public static Texture2D AS_button_alert = new Texture2D (38, 38, TextureFormat.ARGB32, false);
		//		private static Texture2D AS_button_alert_1 = new Texture2D (38, 38, TextureFormat.ARGB32, false);
		//		private Texture2D AS_button_alert_2 = new Texture2D (38, 38, TextureFormat.ARGB32, false);
		//			private int AS_button_alert_anim = 0;
		#if (STOCKTOOLBAR)
		private bool AS_Texture_Load = false;
		#endif

		private bool cfgWinData = false;
		//		private bool cancel = false;
		private static bool newScreenshotAtIntervals = true;
		private static ushort newInterval = 1;
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
		private static ushort newhsScreenshotInterval;
		private static string secondsUntilImpact = "";
		private static string hsAltitudeLimit = "";
		private static string hsMinVerticalSpeed = "";
		private static string hsScreenshotInterval = "";


		internal MainMenuGui ()
		{
			
			blizzyToolbarInstalled = ToolbarManager.ToolbarAvailable;
		}

		public void setAppLauncherHidden()
		{
			appLaucherHidden = true;
		}

		public void OnGUIHideApplicationLauncher ()
		{
			if (!appLaucherHidden) {
#if (STOCKTOOLBAR)

				if (AS.configuration.BlizzyToolbarIsAvailable && AS.configuration.useBlizzyToolbar) {
					HideToolbarStock ();
					appLaucherHidden = true;
				}
#endif
			}
		}

#if (STOCKTOOLBAR)

		public void OnGUIShowApplicationLauncher ()
		{
			//Log.Info ("OnGUIShowApplicationLauncher");

			if (!AS.configuration.BlizzyToolbarIsAvailable || !AS.configuration.useBlizzyToolbar) {
				if (appLaucherHidden) {
					appLaucherHidden = false;
					if (AS_Button != null)
						UpdateToolbarStock ();
				}
			}
		}

		public void OnGUIApplicationLauncherReady ()
		{
			UpdateToolbarStock ();
		}

		private void UpdateToolbarStock ()
		{
			Log.Info ("UpdateToolbarStock, appLaucherHidden: " + appLaucherHidden.ToString());
			if (AS_Button != null)
				Log.Info ("AS_Button not null");
			// Create the button in the KSP AppLauncher
			if (!AS_Texture_Load) {
				if (GameDatabase.Instance.ExistsTexture ("AutomatedScreenShots/Textures/AS_38"))
					AS_button_on = GameDatabase.Instance.GetTexture ("AutomatedScreenShots/Textures/AS_38", false);
				if (GameDatabase.Instance.ExistsTexture ("AutomatedScreenShots/Textures/AS_38_white"))
					AS_button_off = GameDatabase.Instance.GetTexture ("AutomatedScreenShots/Textures/AS_38_white", false);
				if (GameDatabase.Instance.ExistsTexture ("AutomatedScreenShots/Textures/AS_38_green"))
					AS_button_alert = GameDatabase.Instance.GetTexture ("AutomatedScreenShots/Textures/AS_38_green", false);

				AS_Texture_Load = true;
			}
			if (AS_Button == null && !appLaucherHidden) {
				Log.Info ("AS_Button == null");
				
				AS_Button = ApplicationLauncher.Instance.AddModApplication (GUIToggle, GUIToggle,
					null, null,
					null, null,
					ApplicationLauncher.AppScenes.ALWAYS,
						//ApplicationLauncher.AppScenes.SPACECENTER | ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.MAPVIEW | ApplicationLauncher.AppScenes.VAB | ApplicationLauncher.AppScenes.SPH,
					AS_button_off);
				Log.Info ("Added");
				stockToolBarcreated = true;
			}
		}

		private void HideToolbarStock ()
		{
			Log.Info ("HideToolbarStock");
			ApplicationLauncher.Instance.RemoveModApplication (MainMenuGui.AS_Button);
			Destroy (AS_Button); // Is this necessary?
			AS_Button = null;
			appLaucherHidden = false;
		}
#endif

		public bool Visible ()
		{ 
			return visible;
		}

		public void SetVisible (bool visible)
		{
			this.visible = visible;
		}

		
		/////////////////////////////////////
		public void OnGUI ()
		{
			try {
				if (this.Visible ()) {
					this.bounds = GUILayout.Window (this.GetInstanceID (), this.bounds, this.Window, TITLE, HighLogic.Skin.window);
				}
			} catch (Exception e) {
				Log.Error ("exception: " + e.Message);
			}
		}

		private void Window (int id)
		{
			if (cfgWinData == false) {
				cfgWinData = true;
				newScreenshotAtIntervals = AS.configuration.screenshotAtIntervals;
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
				newUseBlizzyToolbar = AS.configuration.useBlizzyToolbar;
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

			} 

			SetVisible (true);
			GUI.enabled = true;

			DrawTitle ("Options");
			GUILayout.BeginVertical ();
	
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Take screenshots at specified intervals:");
			newScreenshotAtIntervals = GUILayout.Toggle (newScreenshotAtIntervals, "");
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Screenshot Interval in seconds: ");
			interval = GUILayout.TextField (interval, GUILayout.MinWidth (30.0F), GUILayout.MaxWidth (30.0F));
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Convert to JPG:");
			newConvertToJPG = GUILayout.Toggle (newConvertToJPG, "", GUILayout.MinWidth (30F));
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Keep original PNG:");
			newKeepOrginalPNG = GUILayout.Toggle (newKeepOrginalPNG, "", GUILayout.MinWidth (30F));
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Screenshot path:");

			newScreenshotPath = GUILayout.TextField (newScreenshotPath, GUILayout.MinWidth (50F), GUILayout.MaxWidth (300F));
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Filename Format:");
			newFilename = GUILayout.TextField (newFilename, GUILayout.MinWidth (30F), GUILayout.MaxWidth (160F));
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("JPEG Quality (1-100):");
			JPGQuality = GUILayout.TextField (JPGQuality, GUILayout.MinWidth (30.0F), GUILayout.MaxWidth (30.0F));
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Screenshot after scene change:");
			newScreenshotOnSceneChange = GUILayout.Toggle (newScreenshotOnSceneChange, "");
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Screenshot after special event:");
			newOnSpecialEvent = GUILayout.Toggle (newOnSpecialEvent, "");
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Use Blizzy Toolbar if available:");
			newUseBlizzyToolbar = GUILayout.Toggle (newUseBlizzyToolbar, "");
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Activation Keycode:");
			newKeycode = GUILayout.TextField (newKeycode, GUILayout.MinWidth (30F), GUILayout.MaxWidth (40F));
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("No GUI on screenshot:");
			newNoGUIOnScreenshot = GUILayout.Toggle (newNoGUIOnScreenshot, "");
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("GUI on screenshot:");
			newGUIOnScreenshot = GUILayout.Toggle (newGUIOnScreenshot, "");
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Take pre-crash snapshots:");
			newprecrashSnapshots = GUILayout.Toggle (newprecrashSnapshots, "");
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Seconds until impact:");
			secondsUntilImpact = GUILayout.TextField (secondsUntilImpact, GUILayout.MinWidth (30.0F), GUILayout.MaxWidth (30.0F));
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Altitude limit:");
			hsAltitudeLimit = GUILayout.TextField (hsAltitudeLimit, GUILayout.MinWidth (30.0F), GUILayout.MaxWidth (30.0F));
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Minimum vertical speed:");
			hsMinVerticalSpeed = GUILayout.TextField (hsMinVerticalSpeed, GUILayout.MinWidth (30.0F), GUILayout.MaxWidth (30.0F));
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Screenshot interval (pre-crash):");
			hsScreenshotInterval = GUILayout.TextField (hsScreenshotInterval, GUILayout.MinWidth (30.0F), GUILayout.MaxWidth (30.0F));
			GUILayout.EndHorizontal ();



// Following #pragma disables the warning about unused variable
#pragma warning disable 168
			try {
				newInterval = Convert.ToUInt16 (interval);
			} catch (FormatException e) {
			} catch (OverflowException e) {
			} finally {
				// AS.configuration.screenshotInterval =  newInterval;
			}

			try {
				newJPGQuality = Convert.ToUInt16 (JPGQuality);
			} catch (FormatException e) {
			} catch (OverflowException e) {
			} finally {
				// AS.configuration.screenshotInterval =  newInterval;
			}


			try {
				newsecondsUntilImpact = Convert.ToUInt16 (secondsUntilImpact);
			} catch (FormatException e) {
			} catch (OverflowException e) {
			} finally {
				// AS.configuration.screenshotInterval =  newInterval;
			}
			try {
				newhsAltitudeLimit = Convert.ToUInt16 (hsAltitudeLimit);
			} catch (FormatException e) {
			} catch (OverflowException e) {
			} finally {
				// AS.configuration.screenshotInterval =  newInterval;
			}
			try {
				newhsMinVerticalSpeed = Convert.ToUInt16 (hsMinVerticalSpeed);
			} catch (FormatException e) {
			} catch (OverflowException e) {
			} finally {
				// AS.configuration.screenshotInterval =  newInterval;
			}
			try {
				newhsScreenshotInterval = Convert.ToUInt16 (hsScreenshotInterval);
			} catch (FormatException e) {
			} catch (OverflowException e) {
			} finally {
				// AS.configuration.screenshotInterval =  newInterval;
			}





#pragma warning restore 168

			GUILayout.EndVertical ();
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
			AS.configuration.screenshotAtIntervals = newScreenshotAtIntervals;
			AS.configuration.screenshotInterval = newInterval;
			if (AS.configuration.screenshotInterval < 1)
				AS.configuration.screenshotInterval = 1;
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
			AS.configuration.useBlizzyToolbar = newUseBlizzyToolbar;
			AS.configuration.onSpecialEvent = newOnSpecialEvent;
			AS.configuration.keycode = newKeycode;
			AS.configuration.noGUIOnScreenshot = newNoGUIOnScreenshot;
			AS.configuration.guiOnScreenshot = newGUIOnScreenshot;

			AS.configuration.precrashSnapshots = newprecrashSnapshots;
			AS.configuration.secondsUntilImpact = newsecondsUntilImpact;
			AS.configuration.hsAltitudeLimit = newhsAltitudeLimit;
			AS.configuration.hsMinVerticalSpeed = newhsMinVerticalSpeed;
			AS.configuration.hsScreenshotInterval = newhsScreenshotInterval;

		}

		public void GUIToggle ()
		{
			Log.Info ("GUIToggle");
			ASInfoDisplay.infoDisplayActive = !ASInfoDisplay.infoDisplayActive;
			if (ASInfoDisplay.infoDisplayActive) {
				SetVisible (true);
				AS_Button.SetTexture (AS_button_on); 
			} else {
				SetVisible (false);
				AS_Button.SetTexture (AS_button_off);
				cfgWinData = false;

				GUI_SaveData ();

				AS.configuration.Save ();
				if (AS.configuration.BlizzyToolbarIsAvailable && AS.configuration.useBlizzyToolbar) {
					ApplicationLauncher.Instance.RemoveModApplication (AS_Button);
					OnGUIHideApplicationLauncher ();
				}

			}
		}
	}
}