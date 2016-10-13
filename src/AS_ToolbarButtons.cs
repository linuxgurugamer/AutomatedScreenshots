using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

using UnityEngine;

namespace AutomatedScreenshots
{
    public partial class AS
    {
		static IButton btnReturn = null;
		private const string _tooltipOn = "Hide AutomatedScreenshots & Saves";
		private const string _tooltipOff = "Show AutomatedScreenshots & Saves";
		public const string TEXTURE_DIR = "AutomatedScreenshots/Textures/";

        public void setToolbarButtonVisibility(bool v)
		{

			btnReturn.Visible = v;
		}

		public void ToolbarToggle()
		{
			Log.Info("btnReturn.OnClick");
			if (gui.Visible ()) {
				gui.SetVisible (false);
				GUI.enabled = false;
				btnReturn.ToolTip = _tooltipOff;
				gui.GUI_SaveData ();

				if (AS.configuration.BlizzyToolbarIsAvailable && AS.configuration.useBlizzyToolbar) {
//					btnReturn.TexturePath = "AutomatedScreenshots/Textures/AS_24_white";
					btnReturn.TexturePath = TEXTURE_DIR + "Auto-24";
					gui.OnGUIHideApplicationLauncher ();
					//InitToolbarButton ();
				} else {
					Log.Info ("Trying to add stock toolbar");
					//gui.OnGUIShowApplicationLauncher ();
					ASInfoDisplay.infoDisplayActive = false;
					//GameEvents.onGUIApplicationLauncherReady.Add (gui.OnGUIApplicationLauncherReady);

					//gui.setAppLauncherHidden ();
					gui.OnGUIApplicationLauncherReady();

					//GameEvents.onGUIApplicationLauncherReady.Add (gui.OnGUIApplicationLauncherReady);
					gui.OnGUIShowApplicationLauncher ();
					// Hide blizzy toolbar button
					setToolbarButtonVisibility(false);
				}
				configuration.Save ();
				ToolBarActive (false);
			} else {
				gui.SetVisible (true);
				GUI.enabled = true;
				btnReturn.ToolTip = _tooltipOn;
//				btnReturn.TexturePath = "AutomatedScreenshots/Textures/AS_24";
				btnReturn.TexturePath = TEXTURE_DIR + "Auto-negative-24";
			}
		}

		public /*static*/ void  ToolBarActive(bool active)
		{
			if (active)
//				btnReturn.TexturePath = "AutomatedScreenshots/Textures/AS_24_green";
				btnReturn.TexturePath = TEXTURE_DIR + "Auto-snapshot-24";
			else
//				btnReturn.TexturePath = "AutomatedScreenshots/Textures/AS_24_white";
				btnReturn.TexturePath = TEXTURE_DIR + "Auto-24";
		}


		public /* static*/ void  ToolBarBusy(bool autoSave, bool doSnapshots)
		{
			Log.Info ("ToolBarBusy:  autoSave: " + autoSave.ToString () + "    doSnapshot: " + doSnapshots.ToString ());
			if (btnReturn == null)
				return;
			if (doSnapshots == false && autoSave == false)
				btnReturn.TexturePath = TEXTURE_DIR + "Auto-24";
				//AS_Button.SetTexture (AS_button_off);
			if (doSnapshots == true && autoSave == false)
			{
				//Log.Info ("Setting auto-snapshot-24");
				btnReturn.TexturePath = TEXTURE_DIR + "Auto-snapshot-24";
				//AS_Button.SetTexture (AS_button_snapshot);
			}
			if (doSnapshots == false && autoSave == true)
			{
				//Log.Info ("Setting Auto-save-24");
				btnReturn.TexturePath = TEXTURE_DIR + "Auto-save-24";
				//AS_Button.SetTexture (AS_button_save);
			}
			if (doSnapshots == true && autoSave == true)
				btnReturn.TexturePath = TEXTURE_DIR + "Auto-snapshot-save-24";
				//AS_Button.SetTexture (AS_button_snapshot_save);
		}



        /// <summary>
        /// initialises a Toolbar Button for this mod
        /// </summary>
        /// <returns>The ToolbarButtonWrapper that was created</returns>
        public void InitToolbarButton()
        {
            
            try
            {
				Log.Info("Initialising the Toolbar Icon");
                btnReturn = ToolbarManager.Instance.add("AutomatedScreenshots", "btnReturn");
//				btnReturn.TexturePath = "SpaceTux/AS/Textures/AS_24_white";
				btnReturn.TexturePath = TEXTURE_DIR + "Auto-24";
				btnReturn.ToolTip = TITLE;
				btnReturn.OnClick += e => ToolbarToggle();
            }
            catch (Exception ex)
            {
                DestroyToolbarButton(btnReturn);
				Log.Info("Error Initialising Toolbar Button: " + ex.Message);
            }
            return;
        }

		public void DelToolbarButton()
		{
			DestroyToolbarButton(btnReturn);
		}
        /// <summary>
        /// Destroys theToolbarButtonWrapper object
        /// </summary>
        /// <param name="btnToDestroy">Object to Destroy</param>
		static  void DestroyToolbarButton(IButton btnToDestroy)
        {
            if (btnToDestroy != null)
            {
				Log.Info("Destroying Toolbar Button");
                btnToDestroy.Destroy();
            }
            btnToDestroy = null;
        }

    }
}
