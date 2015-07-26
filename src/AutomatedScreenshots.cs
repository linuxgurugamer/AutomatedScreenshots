#define STOCKTOOLBAR
// #undef STOCKTOOLBAR
#define BLIZZYTOOLBAR
// #undef BLIZZYTOOLBAR

using System;
using UnityEngine;
using KSP.IO;



namespace AutomatedScreenshots
{
	[KSPAddon (KSPAddon.Startup.MainMenu, true)]
	public partial class AS : MonoBehaviour
	{
		//			public Configuration configuration = Configuration.Instance;
		//			public static AS Instance { get; private set;}
		private float lastUpdate = 0.0f;
		private int cnt = 0;
//		private float snapshotInterval = 5.0f;
		static bool doSnapshots = false;
		private bool specialScene = false;
		private bool newScene = false;
		private bool sceneReady = false;
		private float lastSceneUpdate = 0.0f;
		private string pngToConvert = "";
		public static bool changeCallbacks;
		public static Configuration configuration = new Configuration ();
		public static KeyCode activeKeycode;

		public MainMenuGui gui = null;

		static AS ()
		{
				
		}

		public void activateSnapshots (bool val)
		{
			doSnapshots = val;
		}

		public AS ()
		{
			Log.Info ("New instance of Automated Screenshots: AS constructor");

		}

		public void Awake ()
		{
			Log.Info ("Awake");
		}

		public void Start ()
		{
			Log.SetLevel (Log.LEVEL.INFO);
			Log.Info ("Start");
			DontDestroyOnLoad (this);
			configuration.Load ();
			Log.SetLevel (configuration.logLevel);

			configuration.BlizzyToolbarIsAvailable = ToolbarManager.ToolbarAvailable;
			Log.Info ("BlizzyToolbarIsAvailable: " + configuration.BlizzyToolbarIsAvailable.ToString ());

			if (configuration.BlizzyToolbarIsAvailable) {
				InitToolbarButton ();
				if (configuration.useBlizzyToolbar) {
					setToolbarButtonVisibility(true);
				} else {
					setToolbarButtonVisibility(false);
				}
			}

		}

		public void Update ()
		{
			if (this.gui == null) {
				Log.Info ("this.gui == null");
				this.gui = this.gameObject.AddComponent<MainMenuGui> ();
				// 	this.gui.OnGUIApplicationLauncherReady ();
				this.gui.SetVisible (false);
				RegisterEvents ();
				gui.OnGUIApplicationLauncherReady ();
#if (STOCKTOOLBAR)
				{
					GameEvents.onGUIApplicationLauncherReady.Add (gui.OnGUIApplicationLauncherReady);
				}
#endif
			}

			if (HighLogic.LoadedScene == GameScenes.MAINMENU) {

#if (STOCKTOOLBAR)

				if (!configuration.BlizzyToolbarIsAvailable || !configuration.useBlizzyToolbar)
					gui.OnGUIHideApplicationLauncher ();
#endif
			} else {
#if (STOCKTOOLBAR)
				if (!configuration.BlizzyToolbarIsAvailable || !configuration.useBlizzyToolbar)
				{					
					GameEvents.onGUIApplicationLauncherReady.Add (gui.OnGUIApplicationLauncherReady);
					gui.OnGUIShowApplicationLauncher ();
				}
				else{
					setToolbarButtonVisibility(true);
				}
#endif
			}

			if (changeCallbacks) {
				Log.Info ("Update - changeCallbacks: " + changeCallbacks.ToString ());
				RegisterEvents ();
			}

			if (Input.GetKeyDown (activeKeycode)) {
				Log.Info ("Update:     GameScene: " + HighLogic.LoadedScene.ToString ());
				if (HighLogic.LoadedScene != GameScenes.MAINMENU) {
					Log.Info ("KeyCode: " + activeKeycode.ToString() + " pressed");
					if (!doSnapshots) {
						if (!configuration.BlizzyToolbarIsAvailable || !configuration.useBlizzyToolbar) {					
							MainMenuGui.AS_Button.SetTexture (MainMenuGui.AS_button_alert);
						} else {
							ToolBarActive (true);
						}
					} else {
						if (!configuration.BlizzyToolbarIsAvailable || !configuration.useBlizzyToolbar) {					
							MainMenuGui.AS_Button.SetTexture (MainMenuGui.AS_button_off);
						} else {
							ToolBarActive (false);
						}
					}
						
					doSnapshots = !doSnapshots;
				} else if (HighLogic.LoadedScene == GameScenes.MAINMENU)
					doSnapshots = false;
			}
			
		}

		public void LateUpdate ()
		{
			string pngName;
			string jpgName;
			//		Log.Info ("LateUpdate");
			if (doSnapshots) {
				// If there is a png file waiting to be converted, then don't do another screenshot
				if (pngToConvert != "") {
					Log.Info ("pngToConvert: " + pngToConvert);
					if (System.IO.File.Exists (pngToConvert)) {
						jpgName = FileOperations.ScreenshotFolder () + configuration.filename + cnt.ToString () + ".jpg";
						Log.Info ("Converting screenshot to JPG. New name: " + jpgName);
						ConvertToJPG (pngToConvert, jpgName, configuration.JPGQuality);
						System.IO.FileInfo file = new System.IO.FileInfo (pngToConvert);
						if (!configuration.keepOrginalPNG) {
							Log.Info ("AutomatedScreenshots: Delete PNG file");
							file.Delete ();
						}
						//						else
						//						{
						//							string finalPngName = finalName.Replace(".jpg", ".png");
						//							file.MoveTo(ssfolder + finalPngName);
						//							ScreenShotsFolder.Add(pngName);
						//						}

						pngToConvert = "";
					}
				} else {
					if ( this.specialScene || 
						(AS.configuration.screenshotAtIntervals && 
							((this.newScene && this.sceneReady && Time.time - lastSceneUpdate > 1) || 
								(Time.time - lastUpdate > configuration.screenshotInterval)))) {
						newScene = false;
						this.specialScene = false;
						lastUpdate = Time.time;
						//check if directory doesn't exist
						if (!System.IO.Directory.Exists (FileOperations.ScreenshotFolder ())) {    
							//if it doesn't, try to create it
							try {
								System.IO.Directory.CreateDirectory (FileOperations.ScreenshotFolder ());
							} catch (Exception e) {
								Log.Error ("Exception trying to create directory: " + e);
								return;
							}
						}
						do {
							cnt++;
							string s = AddInfo(configuration.filename, cnt);
							//pngName = FileOperations.ScreenshotFolder () + configuration.filename + cnt.ToString () + ".png";
							//jpgName = FileOperations.ScreenshotFolder () + configuration.filename + cnt.ToString () + ".jpg";
							pngName = configuration.screenshotPath + s + ".png";
							jpgName = configuration.screenshotPath + s + ".jpg";
						} while (System.IO.File.Exists (pngName) || System.IO.File.Exists (jpgName));
							
						Log.Info ("Update: Screenshotfolder:" + pngName);
						Application.CaptureScreenshot (pngName);
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
		}

		private void RegisterSceneChanges (bool  enable)
		{
			Log.Info ("RegisterSceneChanges: " + enable.ToString ());
			if (enable) {
				GameEvents.onGameSceneLoadRequested.Add (this.CallbackGameSceneLoadRequested);
				GameEvents.onLevelWasLoaded.Add (this.CallbackLevelWasLoaded);
			} else {
				GameEvents.onGameSceneLoadRequested.Remove (this.CallbackGameSceneLoadRequested);
				GameEvents.onLevelWasLoaded.Remove (this.CallbackLevelWasLoaded);
			}
		}

		private  void RegisterSpecialEvents (bool enable)
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
			// this.gui.SetVisible (scene == GameScenes.MAINMENU);
			if (AS.configuration.screenshotOnSceneChange) {
				this.newScene = true;
				this.sceneReady = false;

			}
		}

		private void CallbackLevelWasLoaded (GameScenes scene)
		{
			Log.Info ("CallbackLevelWasLoaded");
			this.sceneReady = true;
			lastSceneUpdate = Time.time;
		}

		private void CallbackOnVesselChange (Vessel evt)
		{
			Log.Info ("CallbackOnVesselChange");
			//this.newScene = true;
			//this.sceneReady = true;
			this.specialScene = true;
			lastSceneUpdate = Time.time;
		}

		private void CallbackVesselEventHappened (Vessel evt)
		{
			Log.Info ("CallbackVesselEventHappened");
			//this.newScene = true;
			//this.sceneReady = true;
			this.specialScene = true;
			lastSceneUpdate = Time.time;
		}

		private void CallbackEventReportHappened (EventReport evt)
		{
			Log.Info ("CallbackEventReportHappened");
			//this.newScene = true;
			//this.sceneReady = true;
			this.specialScene = true;
			lastSceneUpdate = Time.time;
		}

		private void CallbackVector3dWasLoaded (Vector3d vector)
		{
			Log.Info ("CallbackVector3dWasLoaded");
			//this.newScene = true;
			//this.sceneReady = true;
			this.specialScene = true;
			//lastSceneUpdate = Time.time;
		}

		private void CallbackPartDieWasLoaded (Part part)
		{
			Log.Info ("CallbackPartDieWasLoaded");
			//this.newScene = true;
			//this.sceneReady = true;
			this.specialScene = true;
			//lastSceneUpdate = Time.time;
		}

		private void CallbackPartJointWasLoaded (PartJoint partjoint)
		{
			Log.Info ("CallbackPartJointWasLoaded");
			//this.newScene = true;
			//this.sceneReady = true;
			this.specialScene = true;
			//lastSceneUpdate = Time.time;
		}

		private void CallbackStageActivateWasLoaded (int i)
		{
			Log.Info ("CallbackStageActivateWasLoaded");
			//this.newScene = true;
			//this.sceneReady = true;
			this.specialScene = true;
			//lastSceneUpdate = Time.time;
		}

		private void CallbackCrewEvaLoaded (GameEvents.FromToAction<Part, Part> action)
		{
			Log.Info ("CallbackCrewEvaLoaded");
			//this.newScene = true;
			//this.sceneReady = true;
			this.specialScene = true;
			//lastSceneUpdate = Time.time;
		}

		private void CallbackPartCouple (GameEvents.FromToAction<Part, Part> action)
		{
			Log.Info ("CallbackPartCouple");
			//this.newScene = true;
			//this.sceneReady = true;
			this.specialScene = true;
			//lastSceneUpdate = Time.time;
		}

		private void CallbackPartExplode (GameEvents.ExplosionReaction action)
		{
			Log.Info ("CallbackPartExplode");
			//this.newScene = true;
			//this.sceneReady = true;
			this.specialScene = true;
			//lastSceneUpdate = Time.time;
		}

		private void CallbackSOIChanged (GameEvents.HostedFromToAction<Vessel, CelestialBody >  action)
		{
			Log.Info ("CallbackSOIChanged");
			//this.newScene = true;
			//this.sceneReady = true;
			this.specialScene = true;
			//lastSceneUpdate = Time.time;
		}

		internal void OnDestroy ()
		{
			Log.Info ("destroying Automated Screenshots");
			DelToolbarButton ();
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

		public static KeyCode setActiveKeycode(string keycode)
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
		public string ConvertDateString()
		{
			string dateFormat = "yyyy-MM-dd--HH-mm-ss";

			return DateTime.Now.ToString(dateFormat);
		}

		public int[] ConvertUT(double UT)
		{
			double time = UT;
			int[] ret = {0, 0, 0, 0, 0};
			ret[0] = (int)Math.Floor(time / (KSPUtil.Year))+1; //year
			time %= (KSPUtil.Year);
			ret[1] = (int)Math.Floor(time / KSPUtil.Day)+1; //days
			time %= (KSPUtil.Day);
			ret[2] = (int)Math.Floor(time / (3600)); //hours
			time %= (3600);
			ret[3] = (int)Math.Floor(time / (60)); //minutes
			time %= (60);
			ret[4] = (int)Math.Floor(time); //seconds

			return ret; 
		}

		public string AddInfo(string original, int cnt)
		{
			string f = original;
			if (f.Contains ("[cnt]")) {
				f = f.Replace ("[cnt]", cnt.ToString ());
			}

			if (f.Contains("[date]"))
			{
				f = f.Replace("[date]", ConvertDateString());
			}
			if (f.Contains("[UT]"))
			{
				double UT = 0;
				if (Planetarium.fetch != null)
					UT = Planetarium.GetUniversalTime();
				f = f.Replace("[UT]", Math.Round(UT).ToString());
			}
			//if (f.Contains("[save]"))
			//{
			//	string save = "NA";
			//	if (HighLogic.SaveFolder != null && HighLogic.SaveFolder.Trim().Length > 0)
			//		save = HighLogic.SaveFolder;
			//	f = f.Replace("[save]", save);
			//}
			//if (f.Contains("[version]"))
			//{
			//	string version = Versioning.GetVersionString();
			//	f = f.Replace("[version]", version);
			//}
			if (f.Contains("[vessel]"))
			{
				string vessel = "NA";
				if (HighLogic.LoadedSceneIsFlight && FlightGlobals.ActiveVessel != null)
					vessel = FlightGlobals.ActiveVessel.vesselName;
				f = f.Replace("[vessel]", vessel);
			}
			if (f.Contains("[body]"))
			{
				string body = "NA";
				if (Planetarium.fetch != null)
					body = Planetarium.fetch.CurrentMainBody.bodyName;
				f = f.Replace("[body]", body);
			}
			if (f.Contains("[situation]"))
			{
				string sit = "NA";
				if (HighLogic.LoadedSceneIsFlight && FlightGlobals.ActiveVessel != null)
				{
					sit = FlightGlobals.ActiveVessel.situation.ToString();
				}
				f = f.Replace("[situation]", sit);
			}
			if (f.Contains("[biome]"))
			{
				string biome = "NA";
				if (HighLogic.LoadedSceneIsFlight && FlightGlobals.ActiveVessel != null)
					biome = ScienceUtil.GetExperimentBiome(FlightGlobals.ActiveVessel.mainBody, FlightGlobals.ActiveVessel.latitude, FlightGlobals.ActiveVessel.longitude);
				f = f.Replace("[biome]", biome);
			}
			int[] times = { 0, 0, 0, 0, 0 };
			if (Planetarium.fetch != null)
				times = ConvertUT(Planetarium.GetUniversalTime());
			if (f.Contains("[year]"))
			{
				string time = times[0].ToString();
				f = f.Replace("[year]", time);
			}
			if (f.Contains("[day]"))
			{
				string time = times[1].ToString();
				f = f.Replace("[day]", time);
			}
			if (f.Contains("[hour]"))
			{
				string time = times[2].ToString();
				f = f.Replace("[hour]", time);
			}
			if (f.Contains("[min]"))
			{
				string time = times[3].ToString();
				f = f.Replace("[min]", time);
			}
			if (f.Contains("[sec]"))
			{
				string time = times[4].ToString();
				f = f.Replace("[sec]", time);
			}
			// In case they don't have anything there
			if (f == original) {
				f = f = cnt.ToString ();
			}
			return f;
		}
	}
}

