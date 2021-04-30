using BepInEx;
using ModdingTales;
using System;
using System.Reflection;
using System.Windows.Forms;
using TaleSpireCore;
using UnityEngine.SceneManagement;

namespace TaleSpireExplore
{
	
	[BepInPlugin("org.generic.plugins.talespireexplore", "Mark's TaleSpire Explorer", "1.0.0.0")]
	class TaleSpireExplorePlugin : BaseUnityPlugin
	{
		static FrmExplorer frmExplorer;

		void Awake()
		{
			//MessageBox.Show("Awake...");
			Logger.LogInfo("In Awake for SocketAPI Plug-in");
			UnityEngine.Debug.Log("SocketAPI Plug-in loaded");
			ModdingUtils.Initialize(this, this.Logger, true);

			// TODO: Remove SceneManager.sceneLoaded event and the SceneManager_sceneLoaded handler when we have the explorer driver by commands.
			SceneManager.sceneLoaded += SceneManager_sceneLoaded;
			//CreatureManager.OnLineOfSightUpdated += CreatureManager_OnLineOfSightUpdated;
		}

		DateTime lastLineOfSightUpdateTime = DateTime.Now;
		private void CreatureManager_OnLineOfSightUpdated(CreatureGuid arg1, LineOfSightManager.LineOfSightResult arg2)
		{
			if ((DateTime.Now - lastLineOfSightUpdateTime).TotalSeconds > 3)
			{
				lastLineOfSightUpdateTime = DateTime.Now;
				DndControllerAppClient.SendEventToServer("LineOfSightUpdated");
			}
		}

		private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode arg1)
		{
			if (scene.name == "Startup" || scene.name == "Systems" || scene.name == "Login")
				return;
			
			// MessageBox.Show(scene.name, "Scene Loaded");
			
			if (frmExplorer == null)
			{
				frmExplorer = new FrmExplorer();
				frmExplorer.Show();
			}
		}

		void Update()
		{
			ModdingUtils.OnUpdate();
		}
	}
}
