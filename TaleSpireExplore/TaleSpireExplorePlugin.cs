using BepInEx;
using ModdingTales;
using MultiMod;
using MultiMod.Interface;
using MultiMod.Shared;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TaleSpireCore;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TaleSpireExplore
{

	[BepInPlugin("org.generic.plugins.talespireexplore", "Mark's TaleSpire Explorer", "1.0.0.2")]
	class TaleSpireExplorePlugin : BaseUnityPlugin
	{
		static FrmExplorer frmExplorer;

		public void LoadMultiModAssets()
		{
			var searchDirectory = Path.Combine(Paths.PluginPath, "multimod/mods");
			var mm = ModManager.instance;
			
			mm.AddSearchDirectory(searchDirectory);
			mm.RefreshSearchDirectories();
			Debug.Log($"MultiMod search path: {searchDirectory}");
			//MessageBox.Show(searchDirectory, "New Search Directory");
			//MessageBox.Show(mm.defaultSearchDirectory, "mm.defaultSearchDirectory");
			mm.ModsChanged += () =>
			{
				try
				{
					Debug.Log("Mods changed.");
					foreach (var mod in mm.mods)
						Debug.Log(
								$"{mod.name}: {mod.assemblyNames.Count} assemblies, {mod.contentHandler.prefabs.Count} prefabs, isValid={mod.isValid}, state {mod.loadState}");
				}
				catch (Exception ex)
				{
					while (ex != null)
					{
						MessageBox.Show(ex.Message, "Exception!");
						ex = ex.InnerException;
					}
				}
			};

			mm.ModFound += mod =>
			{
				try
				{
					//MessageBox.Show($"Mod found: {mod.name}");
					Debug.Log(
							$"Mod found: {mod.name} {mod.assemblyNames.Count} assemblies, {mod.contentHandler.prefabs.Count} prefabs, isValid={mod.isValid}, state {mod.loadState}");

					foreach (var assetPath in mod.assetPaths) Debug.Log($" - {assetPath}");

					mod.Load();

					mod.LoadCancelled += (obj) => {
						if (obj != null)
							MessageBox.Show($"Mod_LoadCancelled: {obj.name}");
						else
							MessageBox.Show($"Mod_LoadCancelled: null resource");
					};

					mod.Loaded += resource => { Debug.Log($"Resource loaded? {resource.loadState} - {resource.name}"); };

					Debug.Log(
							$"Mod loaded?: {mod.name} {mod.assemblyNames.Count} assemblies, {mod.contentHandler.prefabs.Count} prefabs, isValid={mod.isValid}, state {mod.loadState}");
				}
				catch (Exception ex)
				{
					while (ex != null)
					{
						MessageBox.Show(ex.Message, "Exception!");
						ex = ex.InnerException;
					}
				}
			};

			mm.ModLoaded += mod =>
			{
				try
				{
					//MessageBox.Show($"{mod.name} loaded. Looking for ExportSettings.");
					Debug.Log($"{mod.name} loaded. Looking for ExportSettings.");
					var settings = mod.GetAsset<ExportSettings>("ExportSettings");

					if (settings == null)
					{
						Debug.LogError("Couldn't find ExportSettings in mod assetbundle.");
						return;
					}

					if (settings.StartupPrefab == null)
					{
						//MessageBox.Show(ReflectionHelper.GetAllProperties(settings), "settings");
					}
					else
					{
						// settings.StartupPrefab is always null.
						MessageBox.Show(ReflectionHelper.GetAllProperties(settings), "Prefab is good!");
						var gobj = Instantiate(settings.StartupPrefab);
						UnityEngine.Object.DontDestroyOnLoad(gobj);
						gobj.GetComponents<ModBehaviour>().ToList().ForEach(i =>
						{
							i.contentHandler = mod.contentHandler;
							i.OnLoaded(mod.contentHandler);
						});
					}
				}
				catch (Exception ex)
				{
					while (ex != null)
					{
						MessageBox.Show(ex.Message, "Exception!");
						ex = ex.InnerException;
					}
				}
			};

			mm.ModLoadCancelled += mod => { Debug.LogWarning($"Mod loading canceled: {mod.name}"); };

			mm.ModUnloaded += mod => { Debug.Log($"Mod UNLOADED: {mod.name}"); };
		}

		void Awake()
		{
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

			//ShowGameObjectsInScene(scene);

			//if (gameObjects != null)
			//	for (int i = 0; i < gameObjects.Length; i++)
			//		MessageBox.Show(ReflectionHelper.GetAllProperties(gameObjects[i]), $"GameObject {i}");

			if (frmExplorer == null)
			{
				frmExplorer = new FrmExplorer();
				frmExplorer.Show();

				//MessageBox.Show("LoadMultiModAssets...");
				try
				{
					LoadMultiModAssets();
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Exception!");
				}
			}
		}

		private static void ShowGameObjectsInScene(Scene scene)
		{
			GameObject[] gameObjects = scene.GetRootGameObjects();

			StringBuilder stringBuilder = new StringBuilder();
			foreach (GameObject gameObject in gameObjects)
			{
				stringBuilder.AppendLine($"{gameObject.name}");
			}

			MessageBox.Show(stringBuilder.ToString(), $"GameObjects in the {scene.name} scene:");
		}

		void Update()
		{
			ModdingUtils.OnUpdate();
		}
	}
}
