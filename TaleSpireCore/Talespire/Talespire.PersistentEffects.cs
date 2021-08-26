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
			public static event PersistentEffectEventHandler PersistentEffectInitialized;
			public static readonly string Folder = "TaleSpire_CustomData/PersistentEffects/";
			static List<string> effectsToInitialize = new List<string>();
			const string STR_Goat1BoardAssetId = "71a127d8-a437-414d-a845-a39606a8a2fa";
			const string STR_UninitializedMiniMeshName = "Goat_01(Clone)";
			const string STR_EffectOrb = "EffectOrb";
			const string STR_AttachedNode = "Attached";
			const string STR_SpinLockIndicator = "SpinLock";
			//const string STR_RichTextSizeModifier = "<size=0>";
			internal static readonly string STR_PersistentEffect = "$CodeRush.PersistentEffect$";

			public static void Initialize()
			{
				BoardToolManager.OnSwitchTool += BoardToolManager_OnSwitchTool;
			}

			static bool trackingDraggedMiniForRotationLock;

			private static void BoardToolManager_OnSwitchTool(BoardTool obj)
			{
				if (obj is CreatureMoveBoardTool)
				{
					CreatureBoardAsset selectedCreature = Minis.GetSelected();
					draggingPersistentEffect = selectedCreature.GetPersistentEffect();
					if (draggingPersistentEffect != null && draggingPersistentEffect.RotationLocked)
						trackingDraggedMiniForRotationLock = true;
				}
				else
				{
					if (draggingPersistentEffect != null && trackingDraggedMiniForRotationLock)
					{
						CreatureBoardAsset selectedCreature = Minis.GetSelected();
						if (selectedCreature != null)
							selectedCreature.SetRotationDegrees(draggingPersistentEffect.LockedRotation);
					}
					draggingPersistentEffect = null;
					trackingDraggedMiniForRotationLock = false;
				}
			}

			public static void Create()
			{
				float3 pointerPos = RulerHelpers.GetPointerPos();
				string creatureId = Board.InstantiateCreature(STR_Goat1BoardAssetId, new Vector3(pointerPos.x, pointerPos.y, pointerPos.z));
				effectsToInitialize.Add(creatureId);
			}

			static List<string> updatedCreatures = new List<string>();
			static float boardActivationTime;

			public static void SetSpinLockVisible(CreatureBoardAsset creatureAtMenu, bool visible)
			{
				GameObject effectOrb = GetEffectOrb(creatureAtMenu);
				SetOrbIndicatorVisible(effectOrb, visible, STR_SpinLockIndicator);
			}

			private static void SetOrbIndicatorVisible(GameObject effectOrb, bool visible, string childName)
			{
				if (effectOrb == null)
				{
					Log.Error($"effectOrb == null");
					return;
				}

				GameObject child = effectOrb.FindChild(childName, true);
				if (child != null)
				{
					if (visible)
						Log.Debug($"Showing {childName}!");
					else
						Log.Debug($"Hiding {childName}!");

					child.SetActive(visible);
				}
				else
				{
					Log.Error($"DID NOT FIND {childName}!!");
				}
			}

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

			// TODO: Refactor this. Hard to read.
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

							PersistentEffect persistentEffect = creatureAsset.GetPersistentEffect();
							if (persistentEffect == null)
								persistentEffect = new PersistentEffect()
								{
									EffectName = "R1.WaterWallSegment1"
								};

							InitializePersistentEffect(creatureAsset, persistentEffect);

							Material baseGlow = Materials.GetMaterial("Standard (Instance)");

							if (baseGlow != null)
								baseGlow.SetColor("_Color", Color.black);

							meshRenderer.material = baseGlow;  // Reassigning kills hide?
							meshRenderer.material.shader = Materials.GetShader("Taleweaver/CreatureShader");

							updatedCreatures.Add(creatureAsset.CreatureId.ToString());
							if (!StatMessaging.HasSizeZeroMarker(creatureAsset.Creature.Name))
								CreatureManager.SetCreatureName(creatureAsset.CreatureId, "Effect");

							string defaultNewEffect = JsonConvert.SerializeObject(persistentEffect);
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
						if (assetLoader.FindChild(STR_EffectOrb) != null)
						{
							if (effectsToInitialize.Count > 0)
								Log.Warning($"effectsToInitialize.Count = {effectsToInitialize.Count}");
							if (updatedCreatures.Count > 0)
								Log.Warning($"updatedCreatures.Count = {updatedCreatures.Count}");

							Log.Warning($"Already initialized. Adding {creatureAsset.CreatureId.ToString()} to updatedCreatures");

							PersistentEffect persistentEffect = creatureAsset.GetPersistentEffect();
							GameObject effectOrb = GetEffectOrb(creatureAsset);
							GameObject attachedNode = GetAttachedNode(creatureAsset);
							persistentEffectEventArgs.Set(creatureAsset, assetLoader, effectOrb, attachedNode, persistentEffect);
							OnPersistentEffectInitialized(creatureAsset, persistentEffectEventArgs);

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

			static void AddAdornments(GameObject adornmentsParent)
			{
				GameObject spinLock = Prefabs.Clone(STR_SpinLockIndicator);
				spinLock.name = STR_SpinLockIndicator;
				spinLock.transform.SetParent(adornmentsParent.transform);
				spinLock.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
				spinLock.transform.localPosition = new Vector3(0, -0.7f, 0);
				spinLock.transform.localEulerAngles =		new Vector3(0, 0, 0);
			}

			public static void OnPersistentEffectInitialized(object sender, PersistentEffectEventArgs ea)
			{
				PersistentEffectInitialized?.Invoke(sender, ea);
			}

			static PersistentEffectEventArgs persistentEffectEventArgs = new PersistentEffectEventArgs();
			static PersistentEffect draggingPersistentEffect;

			private static void InitializePersistentEffect(CreatureBoardAsset creatureAsset, PersistentEffect persistentEffect)
			{
				GameObject assetLoader = creatureAsset.GetAssetLoader();
				if (assetLoader != null)
				{
					GameObject effectOrb = assetLoader.FindChild(STR_UninitializedMiniMeshName);
					effectOrb.name = STR_EffectOrb;  // renaming Goat_01(Clone) to EffectOrb to indicate we have added effects and re-meshed the goat!
					effectOrb.transform.localEulerAngles = new Vector3(0, 0, 0);
					AddAdornments(effectOrb);
					GameObject attachedNode = AddChild(assetLoader, STR_AttachedNode);
					attachedNode.transform.localScale = new Vector3(1, 1, 1);
					attachedNode.transform.localPosition = new Vector3(0.1f, -0.2f, 0.045f);
					attachedNode.transform.localEulerAngles = new Vector3(0, 0, 0);

					persistentEffectEventArgs.Set(creatureAsset, assetLoader, effectOrb, attachedNode, persistentEffect);
					OnPersistentEffectInitialized(creatureAsset, persistentEffectEventArgs);
				}
				
			}

			private static GameObject AddChild(GameObject parent, string nodeName)
			{
				GameObject node = new GameObject();
				node.name = nodeName;
				node.transform.SetParent(parent.transform);
				return node;
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
				PersistentEffect persistentEffect = creatureAsset.GetPersistentEffect();

				if (persistentEffect != null)
				{
					Log.Warning($"Adding effect \"{persistentEffect.EffectName}\"...");
					Spells.AttachEffect(creatureAsset, persistentEffect.EffectName, creatureAsset.CreatureId.ToString(), 0, 0, 0, STR_AttachedNode);
				}
				else
				{
					Log.Warning($"Adding default \"{defaultNewEffect}\"...");
					Spells.AttachEffect(creatureAsset, defaultNewEffect, creatureAsset.CreatureId.ToString(), 0, 0, 0, STR_AttachedNode);
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

			public static bool IsRotationLocked(string creatureId)
			{
				Log.Debug($"IsRotationLocked...");
				CreatureBoardAsset creatureBoardAsset = Minis.GetCreatureBoardAsset(creatureId);
				if (creatureBoardAsset == null)
				{
					Log.Error($"creatureBoardAsset == null");
					return false;
				}

				PersistentEffect persistentEffect = creatureBoardAsset.GetPersistentEffect();
				if (persistentEffect == null)
				{
					Log.Error($"persistentEffect == null");
					return false;
				}

				Log.Debug($"persistentEffect.RotationLocked = {persistentEffect.RotationLocked}");

				return persistentEffect.RotationLocked;
			}
			public static bool IsHidden(string creatureId)
			{
				Log.Debug($"IsHidden...");
				CreatureBoardAsset creatureBoardAsset = Minis.GetCreatureBoardAsset(creatureId);
				if (creatureBoardAsset == null)
				{
					Log.Error($"creatureBoardAsset == null");
					return false;
				}

				PersistentEffect persistentEffect = creatureBoardAsset.GetPersistentEffect();
				if (persistentEffect == null)
				{
					Log.Error($"persistentEffect == null");
					return false;
				}

				Log.Debug($"persistentEffect.Hidden = {persistentEffect.Hidden}");

				return persistentEffect.Hidden;
			}

			public static void SetRotationLocked(CreatureBoardAsset creatureBoardAsset, bool locked)
			{
				Log.Debug($"SetRotationLocked - locked: {locked}");
				PersistentEffect persistentEffect = creatureBoardAsset.GetPersistentEffect();
				if (persistentEffect == null)
				{
					Log.Error($"persistentEffect == null");
					return;
				}

				Log.Debug($"SetRotationLocked - IsVisible: {creatureBoardAsset.IsVisible}");
				SetSpinLockVisible(creatureBoardAsset, locked && creatureBoardAsset.IsVisible);

				persistentEffect.RotationLocked = locked;
				if (locked)
					persistentEffect.LockedRotation = creatureBoardAsset.GetRotationDegrees();

				creatureBoardAsset.SavePersistentEffect(persistentEffect);
			}

			public static void SetHidden(CreatureBoardAsset creatureBoardAsset, bool hidden)
			{
				Log.Debug($"SetHidden - hidden: {hidden}");
				PersistentEffect persistentEffect = creatureBoardAsset.GetPersistentEffect();
				if (persistentEffect == null)
				{
					Log.Error($"persistentEffect == null");
					return;
				}

				persistentEffect.Hidden = hidden;
				CreatureManager.SetCreatureExplicitHideState(creatureBoardAsset.Creature.CreatureId, hidden);
				
				// Hide or show adornments:
				GameObject effectOrb = GetEffectOrb(creatureBoardAsset);

				foreach (string key in persistentEffect.Indicators.Keys)
				{
					bool indicatorVisible = persistentEffect.Indicators[key];
					bool shouldBeVisible = !hidden && indicatorVisible;
					SetOrbIndicatorVisible(effectOrb, shouldBeVisible, key);
				}

				// STR_SpinLockIndicator -> callback architecture???
				
				

				creatureBoardAsset.SavePersistentEffect(persistentEffect);
			}

			public static GameObject GetEffectOrb(CreatureBoardAsset creatureBoardAsset)
			{
				GameObject effectOrb = null;
				GameObject assetLoader = creatureBoardAsset.GetAssetLoader();
				if (assetLoader != null)
					effectOrb = assetLoader.FindChild(STR_EffectOrb, true);
				return effectOrb;
			}

			public static GameObject GetAttachedNode(CreatureBoardAsset creatureBoardAsset)
			{
				GameObject attachedNode = null;
				GameObject assetLoader = creatureBoardAsset.GetAssetLoader();
				if (assetLoader != null)
					attachedNode = assetLoader.FindChild(STR_AttachedNode, true);
				return attachedNode;
			}

			public static bool IsPersistentEffectRotationLocked(NGuid creatureId)
			{
				return IsRotationLocked(creatureId.ToString());
			}
			public static bool IsPersistentEffectHidden(NGuid creatureId)
			{
				return IsHidden(creatureId.ToString());
			}
		}
	}
}