using Unity.Mathematics;
using System;
using System.Linq;
using System.Collections.Generic;
using Bounce.Unmanaged;
using UnityEngine;
using Newtonsoft.Json;
using LordAshes;

namespace TaleSpireCore
{
	public static partial class Talespire
	{
		public static class PersistentEffects
		{
			public static readonly string Folder = "TaleSpire_CustomData/PersistentEffects/";
			static List<string> effectsToInitialize = new List<string>();
			const string STR_Goat1BoardAssetId = "71a127d8-a437-414d-a845-a39606a8a2fa";
			const string STR_UninitializedMiniMeshName = "Goat_01(Clone)";
			const string STR_InitializedMiniMeshName = "EffectOrb";
			//const string STR_RichTextSizeModifier = "<size=0>";
			internal static readonly string STR_PersistentEffect = "$CodeRush.PersistentEffect$";
			public static void Create()
			{
				float3 pointerPos = RulerHelpers.GetPointerPos();
				string creatureId = Board.InstantiateCreature(STR_Goat1BoardAssetId, new Vector3(pointerPos.x, pointerPos.y, pointerPos.z));
				effectsToInitialize.Add(creatureId);
			}

			static List<string> updatedCreatures = new List<string>();
			static float boardActivationTime;

			static void UpdatePersistentEffect(CreatureBoardAsset creatureAsset, string value)
			{
				if (creatureAsset == null)
					return;
				Log.Warning($"UpdatePersistentEffect for {creatureAsset.Creature.Name} - \"{value}\"");
				if (InitializeMiniAsEffect(creatureAsset))
				{
					Log.Debug($"Initialization succeeded!");
				}
				else
				{
					Log.Debug($"Initialization failed, will try to initialize later!");
					effectsToInitialize.Add(creatureAsset.CreatureId.ToString());
				}
			}

			static void DataChangeCallback(StatMessaging.Change[] obj)
			{
				foreach (StatMessaging.Change change in obj)
					if (change.action == StatMessaging.ChangeType.added || change.action == StatMessaging.ChangeType.modified)
					{
						CreaturePresenter.TryGetAsset(change.cid, out CreatureBoardAsset creatureAsset);
						UpdatePersistentEffect(creatureAsset, change.value);
					}
			}

			static PersistentEffects()
			{
				StatMessaging.Subscribe(STR_PersistentEffect, DataChangeCallback);
				BoardSessionManager.OnStateChange += BoardSessionManager_OnStateChange;
			}

			static void BoardHasBecomeActive()
			{
				Log.Warning($"BoardHasBecomeActive - these are the minis we see now:");
				CreatureBoardAsset[] allMinis = Minis.GetAll();
				foreach (CreatureBoardAsset creatureBoardAsset in allMinis)
				{
					Log.Debug($"  {creatureBoardAsset.name} - {creatureBoardAsset.Creature.name}");
				}
				Log.Debug($"-----------------------");
				boardActivationTime = Time.time;
			}

			private static void BoardSessionManager_OnStateChange(PhotonSimpleSingletonStateMBehaviour<BoardSessionManager>.State obj)
			{
				if (obj?.Name == "Active")
					BoardHasBecomeActive();
			}

			static int frameCount;
			static int lastMiniCount;

			static void CheckForNewMinis()
			{
				CreatureBoardAsset[] allMinis = Minis.GetAll();
				if (lastMiniCount == allMinis.Length)
					return;
				Log.Debug($"CheckForNewMinis()...");
				lastMiniCount = allMinis.Length;
				foreach (CreatureBoardAsset creatureBoardAsset in allMinis)
				{
					if (IsMiniAnUninitializedEffect(creatureBoardAsset))
						InitializeMiniAsEffect(creatureBoardAsset);
				}
			}
			public static void Update()
			{
				InitializeEffectsIfNecessary();
				if (Time.time - boardActivationTime < 60)
				{
					frameCount++;
					if (frameCount > 5) // Every five frames we will check for new minis...
					{
						frameCount = 0;
						CheckForNewMinis();
					}
				}
			}

			private static void InitializeEffectsIfNecessary()
			{
				if (effectsToInitialize.Count != 0)
				{
					foreach (string creatureId in effectsToInitialize)
						InitializeEffect(creatureId);

					CleanUp();
				}
			}

			private static void CleanUp()
			{
				if (updatedCreatures.Count > 0)
				{
					foreach (string creatureId in updatedCreatures)
						effectsToInitialize.Remove(creatureId);

					updatedCreatures.Clear();
				}
			}

			private static void InitializeEffect(string creatureId)
			{
				CreaturePresenter.TryGetAsset(new CreatureGuid(creatureId), out CreatureBoardAsset creatureAsset);

				if (creatureAsset != null)
				{
					InitializeMiniAsEffect(creatureAsset);
				}
				else
				{
					Log.Debug($"creatureAsset is null this update cycle....");
				}
			}

			private static bool InitializeMiniAsEffect(CreatureBoardAsset creatureAsset)
			{
				GameObject assetLoader = creatureAsset.GetAssetLoader();
				if (assetLoader != null)
				{
					GameObject goatClone = assetLoader.FindChild(STR_UninitializedMiniMeshName);
					if (goatClone != null)
					{
						MeshFilter meshFilter = goatClone.GetComponent<MeshFilter>();
						MeshRenderer meshRenderer = goatClone.GetComponent<MeshRenderer>();
						if (meshFilter != null && meshRenderer != null)
						{
							Log.Warning($"Name = \"{creatureAsset.Creature.Name}\"");
							Log.Debug($"  meshFilter.sharedMesh = PrimitiveHelper.GetPrimitiveMesh(PrimitiveType.Sphere);");
							meshFilter.sharedMesh = PrimitiveHelper.GetPrimitiveMesh(PrimitiveType.Sphere);
							if (meshFilter.sharedMesh != null)
								Log.Warning($"  meshFilter.sharedMesh is assigned!!!");

							goatClone.transform.localPosition = new Vector3(0.1f, 0.6f, 0);
							goatClone.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
							MarkMiniAsInitializedEffect(creatureAsset);

							Material baseGlow = Materials.GetMaterial("Standard (Instance)");

							if (baseGlow != null)
								baseGlow.SetColor("_Color", Color.black);

							meshRenderer.material = baseGlow;  // Reassigning kills hide?
							meshRenderer.material.shader = Materials.GetShader("Taleweaver/CreatureShader");

							updatedCreatures.Add(creatureAsset.CreatureId.ToString());
							if (!StatMessaging.HasSizeZeroMarker(creatureAsset.Creature.Name))
								CreatureManager.SetCreatureName(creatureAsset.CreatureId, "Effect");

							string defaultNewEffect = "R1.WaterWallSegment1";
							if (!creatureAsset.HasAttachedData(STR_PersistentEffect))
							{
								Log.Warning($"  Creature has no attached data - Adding {defaultNewEffect}!");
								StatMessaging.SetInfo(creatureAsset.CreatureId, STR_PersistentEffect, defaultNewEffect);
							}
							else
							{
								Log.Warning($"  Attached data = \"{creatureAsset.GetAttachedData()[STR_PersistentEffect]}\"");
							}

							AttachEffect(creatureAsset, defaultNewEffect);
							Log.Debug($"---");
							Log.Debug($"");
							return true;
						}
						else
							Log.Debug($"Mesh Filter or Mesh Renderer not found in this update cycle...");
					}
					else
					{
						if (assetLoader.FindChild(STR_InitializedMiniMeshName) != null)
						{
							if (effectsToInitialize.Count > 0)
								Log.Warning($"effectsToInitialize.Count = {effectsToInitialize.Count}");
							if (updatedCreatures.Count > 0)
								Log.Warning($"updatedCreatures.Count = {updatedCreatures.Count}");

							Log.Warning($"Whoa! We already initialized this? Adding {creatureAsset.CreatureId.ToString()} to updatedCreatures");
							updatedCreatures.Add(creatureAsset.CreatureId.ToString());
						}
						else
							Log.Debug($"goatClone not found in this update cycle...");

					}
				}
				else
					Log.Debug($"Asset Loader not found in this update cycle...");
				return false;
			}

			private static void MarkMiniAsInitializedEffect(CreatureBoardAsset creatureAsset)
			{
				GameObject assetLoader = creatureAsset.GetAssetLoader();
				if (assetLoader != null)
				{
					GameObject goatClone = assetLoader.FindChild(STR_UninitializedMiniMeshName);
					goatClone.name = STR_InitializedMiniMeshName;  // renaming Goat_01(Clone) to EffectOrb to indicate we have added effects and re-meshed the goat!
				}
			}

			private static bool IsMiniAnUninitializedEffect(CreatureBoardAsset creatureAsset)
			{
				if (!creatureAsset.HasAttachedData(STR_PersistentEffect))
					return false;

				GameObject assetLoader = creatureAsset.GetAssetLoader();
				if (assetLoader == null)
					return false;

				GameObject goatClone = assetLoader.FindChild(STR_UninitializedMiniMeshName);
				return goatClone != null;
			}

			private static void AttachEffect(CreatureBoardAsset creatureAsset, string defaultNewEffect = "MediumFire")
			{
				Dictionary<string, string> dictionaries = creatureAsset.GetAttachedData();
				if (dictionaries.ContainsKey(STR_PersistentEffect))
				{
					Log.Warning($"Adding effect \"{dictionaries[STR_PersistentEffect]}\"...");
					Spells.AttachEffect(creatureAsset, dictionaries[STR_PersistentEffect], creatureAsset.CreatureId.ToString(), 0, 0, 0);
				}
				else
				{
					Log.Warning($"Adding default \"{defaultNewEffect}\"...");
					Spells.AttachEffect(creatureAsset, defaultNewEffect, creatureAsset.CreatureId.ToString(), 0, 0, 0);
				}
			}

			public static bool IsPersistentEffect(string creatureId)
			{
				CreatureBoardAsset creatureBoardAsset = Minis.GetCreatureBoardAsset(creatureId);
				if (creatureBoardAsset != null)
					return creatureBoardAsset.IsPersistentEffect();
				return false;
			}

			public static bool IsPersistentEffect(NGuid creatureId)
			{
				return IsPersistentEffect(creatureId.ToString());
			}
		}
	}
}