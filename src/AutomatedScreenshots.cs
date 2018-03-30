﻿

using System;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AutomatedScreenshots
{
	
	public enum UIOnScreenshot : ushort
	{
		show = 0,
		hide,
		both
    };

	[KSPAddon (KSPAddon.Startup.MainMenu, true)]
	public partial class AS : MonoBehaviour
	{

		public const String TITLE = "Automated Screenshots and Saves";

		private float lastUpdate = 0.0f;
		private float lastPrecrashUpdate = 0.0f;
		private int cnt = 0;
		//		private float snapshotInterval = 5.0f;
		public static bool doSnapshots = false;
		private static bool doSave = false;

		private bool specialScene = false;
		private bool precrash = false;
		private bool newScene = false;
		private bool sceneReady = false;

		private bool snapshotInProgress = false;

		public bool isSceneReady ()
		{
			return sceneReady;
		}

		public bool isSpecialScene ()
		{
			return specialScene;
		}

		public bool isPreCrash ()
		{
			return precrash;
		}

		public bool isNewScene ()
		{
			return newScene;
		}

		private float lastSceneUpdate = 0.0f;
		private float sceneReadyAt = 0.0f;
		private string pngToConvert = "";
		private string jpgName = "";
		private bool screenshotTaken = false;
		private string screenshotFile = "";
		public static bool changeCallbacks;
		public static Configuration configuration = new Configuration ();
		public static KeyCode activeKeycode;
		static private UICLASS uiVisiblity;
		private bool wasUIVisible = true;
		private ushort dualScreenshots = 0;
		public MainMenuGui gui = null;
		private float lastBackup = 0.0f;
		public Thread backupThread = null;

		//int saveFileCnt = 0;

		public void DoSave ()
		{
			doSave = true;
		}

		public void DoSnapshot ()
		{
			doSnapshots = true;
		}


		static AS ()
		{
		}

		public AS ()
		{
			Log.Info ("New instance of Automated Screenshots: AS constructor");
		}

		public void Awake ()
		{
			Log.Info ("Awake");
			uiVisiblity = new UICLASS ();
			uiVisiblity.Awake ();
			Version.VerifyHistorianVersion ();
            GameEvents.onGUIApplicationLauncherUnreadifying.Add(hideNow);
        }

        public void hideNow(GameScenes scene)
        {
            if (MainMenuGui.Instance != null)
                MainMenuGui.Instance.GUIToggleFalse();
        }
		public void Start ()
		{

			Log.Info ("Start");
			DontDestroyOnLoad (this);
            FileOperations.MoveCfgToDataDir();

            configuration.Load ();
#if (DEBUG)
			Log.SetLevel (Log.LEVEL.INFO);
#else
			Log.SetLevel (configuration.logLevel);
#endif

		}

		public void Update ()
		{
			if (this.gui == null) {
				Log.Info ("this.gui == null");
				this.gui = this.gameObject.AddComponent<MainMenuGui> ();
				this.gui.SetVisible (false);
				RegisterEvents ();

			}
            gui.OnGUIApplicationLauncherReady();

			if (changeCallbacks) {
				Log.Info ("Update - changeCallbacks: " + changeCallbacks.ToString ());
				RegisterEvents ();
			}

			if ((Input.GetKey (KeyCode.RightControl) || Input.GetKey (KeyCode.LeftControl)) &&
			    Input.GetKeyDown (KeyCode.F6)) {
				AS.configuration.autoSave = !AS.configuration.autoSave;
				this.gui.set_AS_Button_active ();
				Log.Info ("AutoSave: " + AS.configuration.autoSave.ToString ());
			}

			if (Input.GetKeyDown (activeKeycode) && !(Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)))
            {
				Log.Info ("Update:     GameScene: " + HighLogic.LoadedScene.ToString ());
				if (HighLogic.LoadedScene != GameScenes.MAINMENU) {
					Log.Info ("KeyCode: " + activeKeycode.ToString () + " pressed");
					if (!doSnapshots)
                        MainMenuGui.toolbarControl.SetTexture(MainMenuGui.TEXTURE_DIR + "Auto-negative-38", MainMenuGui.TEXTURE_DIR + "Auto-negative-24");
                    else
                        MainMenuGui.toolbarControl.SetTexture(MainMenuGui.TEXTURE_DIR + "Auto-38", MainMenuGui.TEXTURE_DIR + "Auto-24");
                   						
					doSnapshots = !doSnapshots;
					if (!doSnapshots && screenshotTaken && configuration.noGUIOnScreenshot == true && wasUIVisible)
						GameEvents.onShowUI.Fire ();
					this.gui.set_AS_Button_active ();
					Log.Info ("LoadedScene   doSnapshots: " + doSnapshots.ToString ());
				} else if (HighLogic.LoadedScene == GameScenes.MAINMENU) {
					Log.Info ("LoadedScene = MAINMENU   doSnapshots: " + doSnapshots.ToString ());
					doSnapshots = false;
				}
			}
			
		}

		public void FixedUpdate()
//		public void LateUpdate ()
		{
			string pngName;

			if (doSave || (AS.configuration.autoSave && ((Time.realtimeSinceStartup - lastBackup) > AS.configuration.minBetweenSaves * 60))) {
				lastBackup = Time.realtimeSinceStartup;
				SaveFilesHandlers sfh = new SaveFilesHandlers ();
				sfh.startBackup (this);
				doSave = false;
			}
			if (doSnapshots || snapshotInProgress) {
				Log.Info ("In LateUpdate, doSnapshots");
				if (screenshotTaken && configuration.noGUIOnScreenshot == true && System.IO.File.Exists (screenshotFile) && wasUIVisible)
					GameEvents.onShowUI.Fire ();
				// If there is a png file waiting to be converted, then don't do another screenshot
				if (pngToConvert != "") {
					Log.Info ("pngToConvert: " + pngToConvert);
					if (System.IO.File.Exists (pngToConvert)) {
						Log.Info ("Converting screenshot to JPG. New name: " + jpgName);
						ConvertToJPG (pngToConvert, jpgName, configuration.JPGQuality);
						System.IO.FileInfo file = new System.IO.FileInfo (pngToConvert);
						if (!configuration.keepOrginalPNG) {
							Log.Info ("AutomatedScreenshots: Delete PNG file");
							file.Delete ();
						}
						pngToConvert = "";
						snapshotInProgress = false;
					}
				} else {
					snapshotInProgress = false;
					if (AS.configuration.precrashSnapshots) {
						if (FlightGlobals.ActiveVessel != null) {
							Vessel vessel = FlightGlobals.ActiveVessel;

							if ((-vessel.verticalSpeed > AS.configuration.hsMinVerticalSpeed) &&
							    ((FlightGlobals.ship_altitude / -vessel.verticalSpeed < AS.configuration.secondsUntilImpact) ||
							    (FlightGlobals.ship_altitude < AS.configuration.hsAltitudeLimit)
							    )) {

								if (Time.realtimeSinceStartup - lastPrecrashUpdate > configuration.hsScreenshotInterval) {
									this.precrash = true;
									lastPrecrashUpdate = Time.realtimeSinceStartup;

									Log.Info ("vessel.verticalSpeed: " + vessel.verticalSpeed.ToString ());
									Log.Info ("FlightGlobals.ship_altitude: " + FlightGlobals.ship_altitude.ToString ());
									Log.Info ("FlightGlobals.ship_altitude  / -vessel.verticalSpeed: " + (FlightGlobals.ship_altitude / -vessel.verticalSpeed).ToString ());
								}
							}
						}
					}

					if ((this.specialScene && !this.newScene) || this.precrash || dualScreenshots == 1 ||
					    ( /*AS.configuration.screenshotAtIntervals && */
							((this.newScene && (this.sceneReady && Time.realtimeSinceStartup - sceneReadyAt > 0.1)  && Time.realtimeSinceStartup - lastSceneUpdate > 1) ||
								((Time.realtimeSinceStartup - lastUpdate) > configuration.screenshotInterval && !this.newScene)
					        )
					    )) {

						Log.Info ("this.specialScene: " + this.specialScene.ToString ());
						Log.Info ("this.precrash: " + this.precrash.ToString ());
						Log.Info ("dualScreenshots: " + dualScreenshots.ToString ());
						Log.Info ("this.newScene: " + this.newScene.ToString ());
						Log.Info ("this.sceneReady: " + this.sceneReady.ToString ());
						Log.Info ("Time.realtimeSinceStartup - sceneReadyAt: " + (Time.realtimeSinceStartup - sceneReadyAt).ToString ());
						Log.Info ("Time.realtimeSinceStartup - lastSceneUpdate: " + (Time.realtimeSinceStartup - lastSceneUpdate).ToString ());
						Log.Info ("Time.realtimeSinceStartup - lastUpdate: " + (Time.realtimeSinceStartup - lastUpdate).ToString ());


						Log.Info ("Taking screenshot");
						Log.Info ("CurrentDirectory: " + System.IO.Directory.GetCurrentDirectory ());
						Log.Info ("FileOperations.ScreenshotFolder: " + FileOperations.ScreenshotFolder ());
						snapshotInProgress = true;
						newScene = false;
						this.specialScene = false;
                        
						//check if directory doesn't exist
						if (!System.IO.Directory.Exists (FileOperations.ScreenshotFolder ())) {
							Log.Info ("Directory does not exist");
							//if it doesn't, try to create it
							try {
								Log.Info ("Trying to create directory");
								System.IO.Directory.CreateDirectory (FileOperations.ScreenshotFolder ());
							} catch (Exception e) {
								Log.Error ("Exception trying to create directory: " + e);
								return;
							}
							Log.Info ("Directory created");
						} 
						do {
							cnt++;
							string s = AddInfo (configuration.filename, cnt, sceneReady, specialScene, precrash);

							pngName = System.IO.Path.GetFullPath (FileOperations.ScreenshotFolder ()) + s + ".png";
							jpgName = System.IO.Path.GetFullPath (FileOperations.ScreenshotFolder ()) + s + ".jpg";
						} while (System.IO.File.Exists (pngName) || System.IO.File.Exists (jpgName));

						this.precrash = false;

						//
						// I make the assumption that if the player wants the gui during the screenshot, then 
						// it will be left visible.
						//
						wasUIVisible = uiVisiblity.isVisible () | configuration.guiOnScreenshot;
						//Log.Info ("Update: Screenshotfolder:" + pngName);
						if (configuration.noGUIOnScreenshot == true)
							GameEvents.onHideUI.Fire ();
						if (configuration.noGUIOnScreenshot && configuration.guiOnScreenshot) {
							if (dualScreenshots == 0)
								dualScreenshots = 1;
							else if (dualScreenshots == 1) {
								dualScreenshots = 0;
								GameEvents.onShowUI.Fire ();
							}
						}
                        if (dualScreenshots == 0)
                        {
                            lastUpdate = Time.realtimeSinceStartup;
                            screenshotTaken = true;
                        }
						screenshotFile = pngName;
						//
						// If Historian is available, then tell it to activate
						//
						Version.set_m_Active ();
                        // Change second number for supersize.  If non-zero,
                        // then multiplies the resolution by that number
                        // Must be an integer
                        ScreenCapture.CaptureScreenshot (pngName, configuration.supersize);


						if (configuration.convertToJPG) {
							pngToConvert = pngName;
						}

					}
				}
			}
		}

		public void RegisterEvents ()
		{
			Log.Info ("RegisterEvents");

			RegisterSceneChanges (false);
			RegisterSpecialEvents (false);
			RegisterSceneChanges (AS.configuration.screenshotOnSceneChange);
			RegisterSpecialEvents (AS.configuration.onSpecialEvent);
			changeCallbacks = false;
			GameEvents.onGameStateLoad.Add (setAutosave);
		}

		void setAutosave(ConfigNode evt)
		{
			Log.Info ("setAutosave");
			AS.configuration.autoSave = AS.configuration.autoSaveOnGameStart;
			gui.OnGUIApplicationLauncherReady();
			gui.set_AS_Button_active ();
		}

		private void RegisterSceneChanges (bool  enable)
		{
			Log.Info ("RegisterSceneChanges: " + enable.ToString ());
			if (enable) {
				GameEvents.onGameSceneLoadRequested.Add (this.CallbackGameSceneLoadRequested);
				//GameEvents.onLevelWasLoaded.Add (this.CallbackLevelWasLoaded);
			} else {
				GameEvents.onGameSceneLoadRequested.Remove (this.CallbackGameSceneLoadRequested);
				//GameEvents.onLevelWasLoadedGUIReady.Remove (this.CallbackLevelWasLoaded);
			}
		}

        void OnEnable()
        {
            //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
            SceneManager.sceneLoaded += CallbackLevelWasLoaded;
        }

        void OnDisable()
        {
            //Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
            SceneManager.sceneLoaded -= CallbackLevelWasLoaded;
        }

        //
        // Register and unregister all the special events here
        //
        private void RegisterSpecialEvents (bool enable)
		{
			Log.Info ("RegisterSpecialEvents: " + enable.ToString ());
			if (enable) {
				GameEvents.onActiveJointNeedUpdate.Add (this.CallbackVesselEventHappened);
				GameEvents.onCollision.Add (this.CallbackEventReportHappened);

				GameEvents.onCrash.Add (this.CallbackEventReportHappened);
				GameEvents.onCrashSplashdown.Add (this.CallbackEventReportHappened);
				GameEvents.onCrewKilled.Add (this.CallbackEventReportHappened);
				GameEvents.onCrewOnEva.Add (this.CallbackCrewEvaLoaded);
				GameEvents.onFlagPlant.Add (this.CallbackVesselEventHappened);
				GameEvents.onKrakensbaneDisengage.Add (this.CallbackVector3dWasLoaded);
				GameEvents.onKrakensbaneEngage.Add (this.CallbackVector3dWasLoaded);
				GameEvents.onLaunch.Add (this.CallbackEventReportHappened);
				GameEvents.onPartCouple.Add (this.CallbackPartCouple);
				GameEvents.onPartDie.Add (this.CallbackPartDieWasLoaded);
				GameEvents.onPartExplode.Add (this.CallbackPartExplode);
				GameEvents.onPartJointBreak.Add (this.CallbackPartJointWasLoaded);

				GameEvents.onPartUndock.Add (this.CallbackPartDieWasLoaded);
				GameEvents.onStageActivate.Add (this.CallbackStageActivateWasLoaded);
				GameEvents.onStageSeparation.Add (this.CallbackEventReportHappened);

				GameEvents.onVesselChange.Add (this.CallbackOnVesselChange);
				GameEvents.onVesselOrbitClosed.Add (this.CallbackVesselEventHappened);
				GameEvents.onVesselOrbitEscaped.Add (this.CallbackVesselEventHappened);
				GameEvents.onVesselSOIChanged.Add (this.CallbackSOIChanged);
			} else {
				GameEvents.onActiveJointNeedUpdate.Remove (this.CallbackVesselEventHappened);
				GameEvents.onCollision.Remove (this.CallbackEventReportHappened);

				GameEvents.onCrash.Remove (this.CallbackEventReportHappened);
				GameEvents.onCrashSplashdown.Remove (this.CallbackEventReportHappened);
				GameEvents.onCrewKilled.Remove (this.CallbackEventReportHappened);
				GameEvents.onCrewOnEva.Remove (this.CallbackCrewEvaLoaded);
				GameEvents.onFlagPlant.Remove (this.CallbackVesselEventHappened);
				GameEvents.onKrakensbaneDisengage.Remove (this.CallbackVector3dWasLoaded);
				GameEvents.onKrakensbaneEngage.Remove (this.CallbackVector3dWasLoaded);
				GameEvents.onLaunch.Remove (this.CallbackEventReportHappened);
				GameEvents.onPartCouple.Remove (this.CallbackPartCouple);
				GameEvents.onPartDie.Remove (this.CallbackPartDieWasLoaded);
				GameEvents.onPartExplode.Remove (this.CallbackPartExplode);
				GameEvents.onPartJointBreak.Remove (this.CallbackPartJointWasLoaded);

				GameEvents.onPartUndock.Remove (this.CallbackPartDieWasLoaded);
				GameEvents.onStageActivate.Remove (this.CallbackStageActivateWasLoaded);
				GameEvents.onStageSeparation.Remove (this.CallbackEventReportHappened);

				GameEvents.onVesselChange.Remove (this.CallbackOnVesselChange);
				GameEvents.onVesselOrbitClosed.Remove (this.CallbackVesselEventHappened);
				GameEvents.onVesselOrbitEscaped.Remove (this.CallbackVesselEventHappened);
				GameEvents.onVesselSOIChanged.Remove (this.CallbackSOIChanged);
			}
		}

		private void CallbackGameSceneLoadRequested (GameScenes scene)
		{
			Log.Info ("CallbackGameSceneLoadRequested");
			if (AS.configuration.screenshotOnSceneChange) {
				this.newScene = true;
				this.sceneReady = false;

			}
		}

		private void CallbackLevelWasLoaded(Scene scene, LoadSceneMode mode)
        {
			Log.Info ("CallbackLevelWasLoaded");
			this.sceneReady = true;
			lastSceneUpdate = Time.realtimeSinceStartup;
			sceneReadyAt = Time.realtimeSinceStartup;
		}

		private void CallbackOnVesselChange (Vessel evt)
		{
			Log.Info ("CallbackOnVesselChange");
			//this.newScene = true;
			//this.sceneReady = true;
			this.specialScene = true;
			lastSceneUpdate = Time.realtimeSinceStartup;
		}

		private void CallbackVesselEventHappened (Vessel evt)
		{
			Log.Info ("CallbackVesselEventHappened");
			//this.newScene = true;
			//this.sceneReady = true;
			this.specialScene = true;
			lastSceneUpdate = Time.realtimeSinceStartup;
		}

		private void CallbackEventReportHappened (EventReport evt)
		{
			Log.Info ("CallbackEventReportHappened");
			//this.newScene = true;
			//this.sceneReady = true;
			this.specialScene = true;
			lastSceneUpdate = Time.realtimeSinceStartup;
		}

		private void CallbackVector3dWasLoaded (Vector3d vector)
		{
			Log.Info ("CallbackVector3dWasLoaded");
			//this.newScene = true;
			//this.sceneReady = true;
			this.specialScene = true;
			//lastSceneUpdate = Time.realtimeSinceStartup;
		}

		private void CallbackPartDieWasLoaded (Part part)
		{
			Log.Info ("CallbackPartDieWasLoaded");
			//this.newScene = true;
			//this.sceneReady = true;
			this.specialScene = true;
			//lastSceneUpdate = Time.realtimeSinceStartup;
		}

		private void CallbackPartJointWasLoaded (PartJoint partjoint, float f)
		{
			Log.Info ("CallbackPartJointWasLoaded");
			//this.newScene = true;
			//this.sceneReady = true;
			this.specialScene = true;
			//lastSceneUpdate = Time.realtimeSinceStartup;
		}

		private void CallbackStageActivateWasLoaded (int i)
		{
			Log.Info ("CallbackStageActivateWasLoaded");
			//this.newScene = true;
			//this.sceneReady = true;
			this.specialScene = true;
			//lastSceneUpdate = Time.realtimeSinceStartup;
		}

		private void CallbackCrewEvaLoaded (GameEvents.FromToAction<Part, Part> action)
		{
			Log.Info ("CallbackCrewEvaLoaded");
			//this.newScene = true;
			//this.sceneReady = true;
			this.specialScene = true;
			//lastSceneUpdate = Time.realtimeSinceStartup;
		}

		private void CallbackPartCouple (GameEvents.FromToAction<Part, Part> action)
		{
			Log.Info ("CallbackPartCouple");
			//this.newScene = true;
			//this.sceneReady = true;
			this.specialScene = true;
			//lastSceneUpdate = Time.realtimeSinceStartup;
		}

		private void CallbackPartExplode (GameEvents.ExplosionReaction action)
		{
			Log.Info ("CallbackPartExplode");
			//this.newScene = true;
			//this.sceneReady = true;
			this.specialScene = true;
			//lastSceneUpdate = Time.realtimeSinceStartup;
		}

		private void CallbackSOIChanged (GameEvents.HostedFromToAction<Vessel, CelestialBody >  action)
		{
			Log.Info ("CallbackSOIChanged");
			//this.newScene = true;
			//this.sceneReady = true;
			this.specialScene = true;
			//lastSceneUpdate = Time.realtimeSinceStartup;
		}

		internal void OnDestroy ()
		{
			Log.Info ("destroying Automated Screenshots");
            MainMenuGui.toolbarControl.OnDestroy();
            Destroy(MainMenuGui.toolbarControl);

            //DelToolbarButton ();
			configuration.Save ();
		}

		public void ConvertToJPG (string originalFile, string newFile, int quality = 75)
		{
			Texture2D png = new Texture2D (1, 1);

			byte[] pngData = System.IO.File.ReadAllBytes (originalFile);
			png.LoadImage (pngData);
			byte[] jpgData = png.EncodeToJPG (quality);
			var file = System.IO.File.Open (newFile, System.IO.FileMode.Create);
			var binary = new System.IO.BinaryWriter (file);
			binary.Write (jpgData);
			file.Close ();
			Destroy (png);
			//Resources.UnloadAsset(png);
		}

		public static KeyCode setActiveKeycode (string keycode)
		{
			AS.activeKeycode = (KeyCode)Enum.Parse (typeof(KeyCode), keycode);
			if (AS.activeKeycode == KeyCode.None) {
				Log.Warning ("Make sure to use the list of keys to set the key! Reverting to F6");
				AS.activeKeycode = KeyCode.F6;
			}
		
			return AS.activeKeycode;
		}

		//
		// Following taken from SensibleScreenshot
		//
		private static string ConvertDateString ()
		{
			string dateFormat = "yyyy-MM-dd--HH-mm-ss";

			return DateTime.Now.ToString (dateFormat);
		}

		private static int[] ConvertUT (double UT)
		{
			double time = UT;
			int[] ret = { 0, 0, 0, 0, 0 };

			ret [0] = (int)Math.Floor (time / (KSPUtil.dateTimeFormatter.Year)) + 1; //year
			time %= (KSPUtil.dateTimeFormatter.Year);
			ret [1] = (int)Math.Floor (time / KSPUtil.dateTimeFormatter.Day) + 1; //days
			time %= (KSPUtil.dateTimeFormatter.Day);
			ret [2] = (int)Math.Floor (time / (3600)); //hours
			time %= (3600);
			ret [3] = (int)Math.Floor (time / (60)); //minutes
			time %= (60);
			ret [4] = (int)Math.Floor (time); //seconds

			return ret; 
		}

		public static string AddInfo (string original, int cnt, bool sceneReady = false, bool specialScene = false, bool precrash = false)
		{
			string f = original;
			Log.Info ("AddInfo: original: " + original);
			if (f.Contains (":")) {
				f = f.Replace (":", "-");
			}
			if (f.Contains ("[cnt]")) {
				Log.Info ("Contains [cnt]");
				f = f.Replace ("[cnt]", cnt.ToString ());
			} else
				Log.Info ("Doesn't contain [cnt]");

			if (f.Contains ("[date]")) {
				f = f.Replace ("[date]", ConvertDateString ());
			}
			if (f.Contains ("[UT]")) {
				double UT = 0;
				if (Planetarium.fetch != null)
					UT = Planetarium.GetUniversalTime ();
				f = f.Replace ("[UT]", Math.Round (UT).ToString ());
			}
			if (f.Contains ("[save]")) {
				string save = "NA";
				if (HighLogic.SaveFolder != null && HighLogic.SaveFolder.Trim ().Length > 0)
					save = HighLogic.SaveFolder;
				f = f.Replace ("[save]", save);
			}
			//if (f.Contains("[version]"))
			//{
			//	string version = Versioning.GetVersionString();
			//	f = f.Replace("[version]", version);
			//}
			if (f.Contains ("[vessel]")) {
				string vessel = "NA";
				if (HighLogic.LoadedSceneIsFlight && FlightGlobals.ActiveVessel != null)
					vessel = FlightGlobals.ActiveVessel.vesselName;
				f = f.Replace ("[vessel]", vessel);
			}
			if (f.Contains ("[body]")) {
				string body = "NA";
				if (Planetarium.fetch != null)
					body = Planetarium.fetch.CurrentMainBody.bodyName;
				f = f.Replace ("[body]", body);
			}
			if (f.Contains ("[situation]")) {
				string sit = "NA";
				if (HighLogic.LoadedSceneIsFlight && FlightGlobals.ActiveVessel != null) {
					sit = FlightGlobals.ActiveVessel.situation.ToString ();
				}
				f = f.Replace ("[situation]", sit);
			}
			if (f.Contains ("[biome]")) {
				string biome = "NA";
				if (HighLogic.LoadedSceneIsFlight && FlightGlobals.ActiveVessel != null)
					biome = ScienceUtil.GetExperimentBiome (FlightGlobals.ActiveVessel.mainBody, FlightGlobals.ActiveVessel.latitude, FlightGlobals.ActiveVessel.longitude);
				f = f.Replace ("[biome]", biome);
			}
			int[] times = { 0, 0, 0, 0, 0 };
			if (Planetarium.fetch != null)
				times = ConvertUT (Planetarium.GetUniversalTime ());
			if (f.Contains ("[year]")) {
				string time = times [0].ToString ("D4");
				f = f.Replace ("[year]", time);
			}
			if (f.Contains ("[day]")) {
				string time = times [1].ToString ("D2");
				f = f.Replace ("[day]", time);
			}
			if (f.Contains ("[hour]")) {
				string time = times [2].ToString ("D2");
				//if (time.Length == 1)
				//	time = "0" + time;
				f = f.Replace ("[hour]", time);
			}
			if (f.Contains ("[min]")) {
				string time = times [3].ToString ("D2");
				//if (time.Length == 1)
				//	time = "0" + time;
				f = f.Replace ("[min]", time);
			}
			if (f.Contains ("[sec]")) {
				string time = times [4].ToString ("D2");
				//if (time.Length == 1)
				//	time = "0" + time;
				f = f.Replace ("[sec]", time);
			}

			// In case they don't have anything there
			if (f == original && cnt > 0) {
				Log.Info ("f == original");
				f = f + cnt.ToString ();
			}
			if (f.Contains ("[evt]")) {
				string evt = "timed";
				if (sceneReady)
					evt = "scene";
				if (specialScene)
					evt = "event";
				if (precrash)
					evt = "precrash";

				f = f.Replace ("[evt]", evt);
			}
				
			return f;
		}
	}

}