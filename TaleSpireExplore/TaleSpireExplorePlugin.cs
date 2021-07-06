/* 
 
CubePrefab
SampleParticleSys
ConjureAmethystSea
ConjureLavaPool
ConjureWaterPool
DivineJudgment
EssenceHarvest
FireBarrier
FusionCore
HammerStrike
HealingSeal
HeartOfBattle
HoloShield
IonMarker
IonStrike
LaserCannonBarrage
MagicMissileBarrage
MeteorStrike
MineField
MoltenPillar
NeuralShield
PlasmaBarrier
ProgramShield
PulseGrenade
RocketBarrage
SpiritBomb
StarRays
StormBeacon
StormPillar
SummonStorm
VoidShield
Volley
WaterBarrier
EvilSmoke
Fire
FireBall
FireRing
Ice
MediumFire
Particle Dome
White Smoke
SpellEffects
StartupPrefab
 
 */

using BepInEx;
using BepInEx.Logging;
using MultiMod;
using MultiMod.Interface;
using MultiMod.Shared;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using TaleSpireCore;
using UnityEngine;
using UnityEngine.SceneManagement;
using GoogleHelper;

namespace TaleSpireExplore
{

	[BepInPlugin("org.generic.plugins.talespireexplore", "Mark's TaleSpire Explorer", "1.0.0.2")]
	public class TaleSpireExplorePlugin : BaseUnityPlugin
	{
		//public static T CloneChildAsset<T>(string assetName, string instanceId, string parentPrefab = "StartupPrefab") where T: class
		//{
		//	Talespire.Log.Warning("CloneAsset...");
		//	GameObject startupPrefab = Talespire.Prefabs.Get(parentPrefab);
		//	
		//	if (startupPrefab == null)
		//	{
		//		Talespire.Log.Warning($"startupPrefab is null!");
		//		return default(T);
		//	}

		//	Talespire.Log.Warning($"Children: {startupPrefab.transform.childCount}");
		//	foreach (Transform transform in startupPrefab.transform.Children())
		//	{
		//		Talespire.Log.Warning($"transform.gameObject.name: {transform.gameObject.name}, transform.name: {transform.name}");
		//		if (transform.gameObject is T && transform.gameObject is GameObject originalAsset && transform.name == assetName)
		//			return Talespire.Instances.Create<T>(instanceId, originalAsset);
		//	}

		//	return default(T);
		//}

		//public static GameObject CloneSpellEffect(string assetName, string instanceId)
		//{
		//	return CloneChildAsset<GameObject>(assetName, instanceId, "SpellEffects");
		//}

		const string STR_UnityMainThreadDispatcher = "UnityMainThreadDispatcher";
		static FrmExplorer frmExplorer;

		public void LoadMultiModAssets()
		{
			var searchDirectory = Path.Combine(Paths.PluginPath, "multimod\\mods");
			var modManager = ModManager.instance;

			modManager.AddSearchDirectory(searchDirectory);
			modManager.RefreshSearchDirectories();
			Talespire.Log.Debug($"MultiMod search path: {searchDirectory}");
			//MessageBox.Show(searchDirectory, "New Search Directory");
			//MessageBox.Show(mm.defaultSearchDirectory, "mm.defaultSearchDirectory");
			modManager.ModsChanged += () =>
			{
				try
				{
					Talespire.Log.Debug("Mods changed.");
					foreach (var mod in modManager.mods)
						Talespire.Log.Debug($"{mod.name}: {mod.assemblyNames.Count} assemblies, {mod.contentHandler.prefabs.Count} prefabs, isValid={mod.isValid}, state {mod.loadState}");
				}
				catch (Exception ex)
				{
					Talespire.Log.Exception(ex);
				}
			};

			modManager.ModFound += mod =>
			{
				try
				{
					//MessageBox.Show($"Mod found: {mod.name}");
					Talespire.Log.Debug($"Mod found: {mod.name} {mod.assemblyNames.Count} assemblies, {mod.contentHandler.prefabs.Count} prefabs, isValid={mod.isValid}, state {mod.loadState}");

					foreach (var assetPath in mod.assetPaths) Talespire.Log.Debug($" - {assetPath}");

					mod.Load();

					mod.LoadCancelled += (obj) =>
					{
						if (obj != null)
							MessageBox.Show($"Mod_LoadCancelled: {obj.name}");
						else
							MessageBox.Show($"Mod_LoadCancelled: null resource");
					};

					mod.Loaded += resource => { Talespire.Log.Debug($"Resource loaded? {resource.loadState} - {resource.name}"); };

					Talespire.Log.Debug($"Mod loaded?: {mod.name} {mod.assemblyNames.Count} assemblies, {mod.contentHandler.prefabs.Count} prefabs, isValid={mod.isValid}, state {mod.loadState}");
				}
				catch (Exception ex)
				{
					Talespire.Log.Exception(ex);
				}
			};

			modManager.ModLoaded += mod =>
			{
				try
				{
					//MessageBox.Show($"{mod.name} loaded. Looking for ExportSettings.");
					Talespire.Log.Debug($"{mod.name} loaded. Looking for ExportSettings.");
					var settings = mod.GetAsset<ExportSettings>("ExportSettings");

					if (settings == null)
					{
						Talespire.Log.Error("Couldn't find ExportSettings in mod assetbundle.");
						return;
					}

					Talespire.Log.Warning($"settings: \n{ReflectionHelper.GetAllProperties(settings)}");

					if (settings.StartupPrefab == null)
					{
						Talespire.Log.Error("settings.StartupPrefab == null!");
						//MessageBox.Show(ReflectionHelper.GetAllProperties(settings), "settings");
						Talespire.Log.Warning($"mod.contentHandler.prefabs.Count: {mod.contentHandler.prefabs.Count}");
						foreach (GameObject prefab in mod.contentHandler.prefabs)
						{
							Talespire.Log.Warning($"  Adding \"{prefab.name}\" to the dictionary.");

							Talespire.Prefabs.Add(prefab);

							if (prefab.name == "StartupPrefab")  // This is our startup prefab!
							{
								startupPrefab = Instantiate(prefab);

								UnityEngine.Object.DontDestroyOnLoad(startupPrefab);
								startupPrefab.GetComponents<ModBehaviour>().ToList().ForEach(modBehavior =>
								{
									modBehavior.contentHandler = mod.contentHandler;
									modBehavior.OnLoaded(mod.contentHandler);
								});
								//startupPrefab.SetActive(false);
							}
						}
					}
					else
					{
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
					Talespire.Log.Exception(ex);
				}
			};

			modManager.ModLoadCancelled += mod => { Talespire.Log.Warning($"Mod loading canceled: {mod.name}"); };

			modManager.ModUnloaded += mod => { Talespire.Log.Debug($"Mod UNLOADED: {mod.name}"); };
		}

		void Awake()
		{
			Logger.LogInfo("In Awake for SocketAPI Plug-in");
			UnityEngine.Debug.Log("SocketAPI Plug-in loaded");
			ModdingUtils.Initialize(this, this.Logger, true);

			// TODO: Remove SceneManager.sceneLoaded event and the SceneManager_sceneLoaded handler when we have the explorer driver by commands.
			SceneManager.sceneLoaded += SceneManager_sceneLoaded;
			CreatureManager.OnLineOfSightUpdated += CreatureManager_OnLineOfSightUpdated;
		}

		DateTime lastLineOfSightUpdateTime = DateTime.Now;
		public static GameObject startupPrefab;

		bool initializedTarget;
		
		void InitializeTarget()
		{
			LoadKnownEffects();
			Talespire.Log.Debug($"InitializeTarget / Toggling Flashlight...");
			Talespire.Log.Debug($"  Talespire.Target.On();");
			Talespire.Target.On();
			Talespire.Log.Debug($"  Talespire.Target.Off();");
			Talespire.Target.Off();
		}

		private void CreatureManager_OnLineOfSightUpdated(CreatureGuid arg1, LineOfSightManager.LineOfSightResult arg2)
		{
			if (!initializedTarget)
			{
				initializedTarget = true;
				InitializeTarget();
				CreatureManager.OnLineOfSightUpdated -= CreatureManager_OnLineOfSightUpdated;
			}
			//if ((DateTime.Now - lastLineOfSightUpdateTime).TotalSeconds > 3)
			//{
			//	lastLineOfSightUpdateTime = DateTime.Now;
			//	DndControllerAppClient.SendEventToServer("LineOfSightUpdated");
			//}
		}

		private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode arg1)
		{
			if (scene.name == "Startup" || scene.name == "Systems" || scene.name == "Login")
				return;
			//ShowGameObjectsInScene(scene);
			//if (gameObjects != null)
			//	for (int i = 0; i < gameObjects.Length; i++)
			//		MessageBox.Show(ReflectionHelper.GetAllProperties(gameObjects[i]), $"GameObject {i}");

			AddDispatcherToScene();

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
					Talespire.Log.Exception(ex);
				}
			}
		}

		private static void AddDispatcherToScene()
		{
			GameObject dispatcher = GameObject.CreatePrimitive(PrimitiveType.Cube);
			dispatcher.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
			dispatcher.name = STR_UnityMainThreadDispatcher;
			dispatcher.AddComponent<UnityMainThreadDispatcher>();
			//UnityMainThreadDispatcher dispatcherBehavior = dispatcher.GetComponent<UnityMainThreadDispatcher>();
			//if (!UnityMainThreadDispatcher.Exists())
			//	dispatcherBehavior.WakeUp();
		}

		static TaleSpireExplorePlugin()
		{
			GoogleSheets.RegisterSpreadsheetID("TaleSpire Effects", "1bF3zEg4c1YUv6BvZ8ru-JhfUn0tFu1HVpe6F267SQvI");
		}

		public static void LoadKnownEffects()
		{
			Talespire.Log.Debug($"Loading Known Effects...");
			KnownEffects.Invalidate();
			SetTargetingSphere();
			SetTargetingSquare();
			SetTargetingFire();
		}

		private static void SetTargetingSphere()
		{
			string targetingSphereJson = KnownEffects.Get("TargetingSphere")?.Effect;
			if (string.IsNullOrWhiteSpace(targetingSphereJson))
			{
				Talespire.Log.Error($"TargetingSphere effect not found!!!");
				return;
			}
			Talespire.Target.SetTargetingSphere(targetingSphereJson);
		}

		private static void SetTargetingSquare()
		{
			string targetingSquareJson = KnownEffects.Get("TargetSquare")?.Effect;
			if (string.IsNullOrWhiteSpace(targetingSquareJson))
			{
				Talespire.Log.Error($"TargetingSquare effect not found!!!");
				return;
			}
			Talespire.Target.SetTargetingSquare(targetingSquareJson);
		}

		private static void SetTargetingFire()
		{
			string targetingFireJson = KnownEffects.Get("SelectionFire")?.Effect;
			if (string.IsNullOrWhiteSpace(targetingFireJson))
			{
				Talespire.Log.Error($"BlueFireSmoke effect not found!!!");
				return;
			}
			Talespire.Target.SetTargetingFire(targetingFireJson);
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
			//Talespire.Camera.ShowPosition();
			ModdingUtils.OnUpdate();
			Talespire.Update();
		}
	}
}
