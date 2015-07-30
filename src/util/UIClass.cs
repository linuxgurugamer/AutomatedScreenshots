
using System;
using UnityEngine;
using KSP.IO;

namespace AutomatedScreenshots
{
	public class UICLASS: MonoBehaviour
	{
		private bool uiVisible = true;

		public UICLASS ()
		{
			Log.Info ("New instance of UICLASS: UICLASS constructor");

		}
		public void Start ()
		{
	
			Log.Info ("UICLASS: Start");
			DontDestroyOnLoad (this);
		}

		public void Awake ()
		{
			Log.Info ("UICLASS Awake");
			Log.Test ("UICLASS Awake");
			GameEvents.onShowUI.Add(onShowUI);
			GameEvents.onHideUI.Add(onHideUI);
		}

		private void onShowUI ()
		{
			Log.Info ("UICLASS onShowUI");
			Log.Test ("UICLASS onShowUI");
			uiVisible = true;
		}

		private void onHideUI ()
		{
			Log.Info ("UICLASS onHideUI");
			Log.Test ("UICLASS onHideUI");
			uiVisible = false;
		}

		public void OnDestroy ()
		{
			GameEvents.onShowUI.Remove(onShowUI);
			GameEvents.onHideUI.Remove(onHideUI);
		}

		public bool isVisible()
		{
			return uiVisible;
		}

		public void setVisible(bool b)
		{
			uiVisible = b;
		}
	}
}

