using Unity.Mathematics;
using System;
using System.Linq;
using System.Collections.Generic;
using Bounce.Unmanaged;
using UnityEngine;
using Newtonsoft.Json;
using LordAshes;
using static TaleSpireCore.Talespire;
using System.Diagnostics;
using Steamworks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace TaleSpireCore
{
	public static partial class Talespire
	{
		public static class PersistentEffects
		{
			internal const string STR_RichTextSizeZero = "<size=0>";

			public static event PersistentEffectEventHandler PersistentEffectInitialized;
			public static readonly string Folder = "TaleSpire_CustomData/PersistentEffects/";
			static List<QueuedEffect> effectsToInitialize = new List<QueuedEffect>();
			const string STR_Goat1BoardAssetId = "71a127d8-a437-414d-a845-a39606a8a2fa";
			const string STR_UninitializedMiniMeshName = "Goat_01(Clone)";
			const string STR_EffectOrb = "EffectOrb";
			internal const string STR_AttachedNode = "Attached";
			const string STR_SpinLockIndicator = "SpinLock";
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
					if (draggingPersistentEffect != null && draggingPersistentEffect.RotationIsLocked)
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

			public static void Create(string effectName = "MediumFire")
			{
				effectsToInitialize.Add(new QueuedEffect(InstantiateGoatAtPointer(), effectName) { MiniName = effectName });
			}

			private static string InstantiateGoatAtPointer()
			{
				float3 pointerPos = RulerHelpers.GetPointerPos();
				return Board.InstantiateCreature(STR_Goat1BoardAssetId, new Vector3(pointerPos.x, pointerPos.y, pointerPos.z));
			}

			static string IncrementEndingNumber(string originalName)
			{
				string numberStr = "";
				
				int index = originalName.Length - 1;
				while (index > 0 && char.IsDigit(originalName[index]))
				{
					numberStr = originalName[index] + numberStr;
					index--;
				}
				
				if (index > 0 && index < originalName.Length - 1)
				{
					string firstPart = originalName.Substring(0, index + 1);
					int number = int.Parse(numberStr);
					number++;
					return firstPart + number.ToString();
				}
				return originalName + " 2";
			}

			public static void Duplicate(IOldPersistentEffect persistentEffect, string originalName)
			{
				//Log.Warning($"Duplicating (originalName = \"{originalName}\")...");
				persistentEffect.Hidden = false;
				string newName = IncrementEndingNumber(originalName);
				Log.Warning($"Duplicate - newName = \"{newName}\"");
				effectsToInitialize.Add(new QueuedEffect(InstantiateGoatAtPointer(), persistentEffect, newName));
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

				Log.Warning($"UpdatePersistentEffect for {GetDisplayName(creatureAsset)} - \"{value}\"");

				if (InitializeMiniAsEffect(creatureAsset))
				{
					Log.Debug($"Initialization succeeded!");
				}
				else
				{
					Log.Debug($"Initialization failed, will try to initialize later!");
					effectsToInitialize.Add(new QueuedEffect(creatureAsset.CreatureId.ToString()));
				}
			}

			private static string GetDisplayName(CreatureBoardAsset creatureAsset)
			{
				string name = creatureAsset.Creature.Name;
				int indexOfSeparator = name.IndexOf(STR_RichTextSizeZero);
				if (indexOfSeparator > 0)
					return name.Substring(0, indexOfSeparator);
				return name;
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
				Log.Warning($"BoardHasBecomeActive - minis we see now:");
				CreatureBoardAsset[] allMinis = Minis.GetAll();
				foreach (CreatureBoardAsset creatureBoardAsset in allMinis)
				{
					Creature creature = creatureBoardAsset.GetComponent<Creature>();
					if (creature != null)
						Log.Debug($"  {creature.Name} ({creatureBoardAsset.Creature.CreatureId.Value})");
					else
						Log.Warning($"  <Creature> component missing! ({creatureBoardAsset.Creature.CreatureId.Value})");
				}
				Log.Debug($"-----------------------");
				Log.Debug($"");
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
					if (IsMiniAnUninitializedEffect(creatureBoardAsset))
						InitializeMiniAsEffect(creatureBoardAsset);
			}

			internal static bool InitializeMiniAsEffect(CreatureBoardAsset creatureAsset, string effectName = null, string newCreatureName = null)
			{
				Log.Indent($"InitializeMiniAsEffect({creatureAsset.CreatureId}, \"{effectName}\", \"{newCreatureName}\")");
				try
				{
					if (effectName == null)
						effectName = "R1.WaterWallSegment1";

					// I think the problem is close to here. We got to here because the goat clone was not found in the initial update cycle, which led to putting the data in a list to try later, which led to effectName being null when passed in here.
					IOldPersistentEffect persistentEffect = creatureAsset.GetPersistentEffect();
					if (persistentEffect == null)
					{
						Log.Error($"Error - persistentEffect == null (not found by call to GetPersistentEffect()).");
						persistentEffect = new SuperPersistentEffect();
						if (persistentEffect is SuperPersistentEffect superPersistentEffect)
							superPersistentEffect.EffectProperties.Add(new EffectProperties() { EffectName = effectName });
					}

					return InitializeMiniFromPersistentEffect(creatureAsset, persistentEffect, newCreatureName);
				}
				finally
				{
					Log.Unindent();
				}
			}

			public static bool HasSizeZeroMarker(string name)
			{
				return name.Contains(STR_RichTextSizeZero);
			}

			// TODO: Refactor this. Hard to read.
			internal static bool InitializeMiniFromPersistentEffect(CreatureBoardAsset creatureAsset, IOldPersistentEffect persistentEffect, string newCreatureName)
			{
				Log.Indent();
				try
				{
					GameObject assetLoader = creatureAsset.GetAssetLoader();
					Log.Warning($"creatureAsset.Creature.Name = \"{creatureAsset.Creature.Name}\"");
					if (assetLoader != null)
					{
						GameObject goatClone = assetLoader.FindChild(STR_UninitializedMiniMeshName);
						if (goatClone != null)
						{
							MeshFilter meshFilter = goatClone.GetComponent<MeshFilter>();
							MeshRenderer meshRenderer = goatClone.GetComponent<MeshRenderer>();
							if (meshFilter != null && meshRenderer != null)
							{
								PositionOrb(goatClone);

								ReplaceMaterial(meshFilter, meshRenderer);

								if (persistentEffect == null)
								{
									persistentEffect = creatureAsset.GetPersistentEffect();
									if (persistentEffect == null)
									{
										Log.Error($"persistentEffect is null! Creating Waterfall!");
										persistentEffect = new OldPersistentEffect()
										{
											EffectName = "R1.WaterWallSegment1"
										};
									}
								}

								InitializeNewlyCreatedPersistentEffect(creatureAsset, persistentEffect, newCreatureName);

								updatedCreatures.Add(creatureAsset.CreatureId.ToString());

								Log.Debug($"returning true");
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

								persistentEffect = creatureAsset.GetPersistentEffect();
								GameObject effectOrb = GetEffectOrb(creatureAsset);
								GameObject attachedNode = creatureAsset.GetAttachedParentGameObject();
								if (attachedNode != null)
								{
									persistentEffectEventArgs.Set(creatureAsset, assetLoader, effectOrb, attachedNode, persistentEffect);
									OnPersistentEffectInitialized(creatureAsset, persistentEffectEventArgs);
								}
								else
									Log.Error($"attachedNode is null!!!");

								updatedCreatures.Add(creatureAsset.CreatureId.ToString());
							}
							else
								Log.Debug($"goatClone not found in this update cycle...");
						}
					}
					else
						Log.Debug($"Asset Loader not found in this update cycle...");
					Log.Debug($"returning false");
					return false;
				}
				finally
				{
					Log.Unindent();
				}
			}

			private static void ReplaceMaterial(MeshFilter meshFilter, MeshRenderer meshRenderer)
			{
				Log.Debug($"  meshFilter.sharedMesh = PrimitiveHelper.GetPrimitiveMesh(PrimitiveType.Sphere);");
				meshFilter.sharedMesh = PrimitiveHelper.GetPrimitiveMesh(PrimitiveType.Sphere);
				if (meshFilter.sharedMesh != null)
					Log.Warning($"  meshFilter.sharedMesh is assigned!!!");

				Material baseGlow = Materials.GetMaterial("Standard (Instance)");

				if (baseGlow != null)
					baseGlow.SetColor("_Color", Color.black);

				meshRenderer.material = baseGlow;  // Reassigning kills hide?
				meshRenderer.material.shader = Materials.GetShader("Taleweaver/CreatureShader");
			}

			private static void PositionOrb(GameObject orb)
			{
				orb.transform.localPosition = new Vector3(0.13f, 0.6f, 0.05f);
				orb.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
			}

			static void AddAdornments(GameObject adornmentsParent)
			{
				AddSpinLock(adornmentsParent);
			}

			private static void AddSpinLock(GameObject adornmentsParent)
			{
				GameObject spinLock = Prefabs.Clone(STR_SpinLockIndicator);
				spinLock.name = STR_SpinLockIndicator;
				spinLock.transform.SetParent(adornmentsParent.transform);
				spinLock.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
				spinLock.transform.localPosition = new Vector3(0, -0.7f, 0);
				spinLock.transform.localEulerAngles = new Vector3(0, 0, 0);
			}

			public static void OnPersistentEffectInitialized(object sender, PersistentEffectEventArgs ea)
			{
				PersistentEffectInitialized?.Invoke(sender, ea);
			}

			static PersistentEffectEventArgs persistentEffectEventArgs = new PersistentEffectEventArgs();
			static IOldPersistentEffect draggingPersistentEffect;

			private static void InitializeNewlyCreatedPersistentEffect(CreatureBoardAsset creatureAsset, IOldPersistentEffect persistentEffect, string newCreatureName)
			{
				Log.Indent();
				GameObject assetLoader = creatureAsset.GetAssetLoader();
				if (assetLoader != null)
				{
					GameObject effectOrb = InitializeOrb(assetLoader);
					GameObject attachedNode = AddAttachmentNode(assetLoader);

					AttachEffect(creatureAsset, persistentEffect, newCreatureName);

					persistentEffectEventArgs.Set(creatureAsset, assetLoader, effectOrb, attachedNode, persistentEffect);
					OnPersistentEffectInitialized(creatureAsset, persistentEffectEventArgs);
				}
				Log.Unindent();
			}

			private static GameObject AddAttachmentNode(GameObject assetLoader)
			{
				GameObject attachedNode = AddChild(assetLoader, STR_AttachedNode);
				attachedNode.transform.localScale = new Vector3(1, 1, 1);
				attachedNode.transform.localPosition = new Vector3(0.1f, -0.2f, 0.045f);
				attachedNode.transform.localEulerAngles = new Vector3(0, 0, 0);
				return attachedNode;
			}

			/// <summary>
			/// Initializes the effect orb (the sphere that represents the effect when it's visible).
			/// </summary>
			private static GameObject InitializeOrb(GameObject assetLoader)
			{
				GameObject effectOrb = assetLoader.FindChild(STR_UninitializedMiniMeshName);
				effectOrb.name = STR_EffectOrb;  // renaming Goat_01(Clone) to EffectOrb to indicate we have added effects and re-meshed the goat!
				effectOrb.transform.localEulerAngles = new Vector3(0, 0, 0);
				AddAdornments(effectOrb);
				return effectOrb;
			}

			static List<SavedCreatureEffect> savedEffectsData = new List<SavedCreatureEffect>();
			static List<SavedCreatureEffect> creaturesRenameData = new List<SavedCreatureEffect>();

			private static void AttachEffect(CreatureBoardAsset creatureAsset, IOldPersistentEffect persistentEffect, string newCreatureName)
			{
				Log.Indent("AttachEffect(CreatureBoardAsset, IOldPersistentEffect)");

				try
				{
					if (persistentEffect == null)
					{
						Log.Error($"AttachEffect - persistentEffect is null!!!");
						return;
					}

					if (creatureAsset == null)
					{
						Log.Error($"AttachEffect - creatureAsset is null!");
						return;
					}

					if (!creatureAsset.HasAttachedData(STR_PersistentEffect))
					{
						Log.Debug($"nameData.Add(new SavedCreatureEffect(creatureAsset.CreatureId, persistentEffect));");
						savedEffectsData.Add(new SavedCreatureEffect(creatureAsset.CreatureId, persistentEffect, newCreatureName));
					}

					AttachEffects(creatureAsset, persistentEffect);

					//SavePersistentEffect(creatureAsset, persistentEffect);
				}
				finally
				{
					Log.Unindent();
				}
			}

			private static void SavePersistentEffect(CreatureBoardAsset creatureAsset, IOldPersistentEffect persistentEffect)
			{
				Log.Indent();
				string defaultNewEffect = JsonConvert.SerializeObject(persistentEffect);
				if (!creatureAsset.HasAttachedData(STR_PersistentEffect))
				{
					Log.Warning($"StatMessaging.SetInfo({creatureAsset.CreatureId}, {defaultNewEffect})");
					StatMessaging.SetInfo(creatureAsset.CreatureId, STR_PersistentEffect, defaultNewEffect);
				}
				else
				{
					Log.Warning($"  Attached data = \"{creatureAsset.GetAttachedData()[STR_PersistentEffect]}\"");
				}
				Log.Unindent();
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

			private static void AttachEffects(CreatureBoardAsset creatureAsset, IOldPersistentEffect persistentEffect)
			{
				Log.Indent();
				if (persistentEffect != null)
				if (persistentEffect is SuperPersistentEffect superPersistentEffect)
					{
						for (int i = 0; i < superPersistentEffect.EffectProperties.Count; i++)
						{
							string prefix = i.ToString().PadLeft(2, '0');
							EffectProperties effectProperties = superPersistentEffect.EffectProperties[i];
							Log.Warning($"Adding effect \"{effectProperties.EffectName}\" with prefix \"{prefix}\"...");
							GameObject spell = Spells.AttachEffect(creatureAsset, effectProperties.EffectName, creatureAsset.CreatureId.ToString(), 0, 0, 0, STR_AttachedNode, prefix);
							
							TransferPropertiesToPersistentEffect(CompositeEffect.GetFromGameObject(spell), superPersistentEffect);

							if (superPersistentEffect.ScriptData != null && superPersistentEffect.ScriptData.Count > 0)
							{
								Log.Debug($"");
								Log.Debug($"----");
								Log.Debug($"Attaching scripts!");
								foreach (string scriptName in superPersistentEffect.ScriptData.Keys)
								{
									Log.Warning($"Adding script: {scriptName}");
									TaleSpireBehavior script = spell.AddComponent(KnownScripts.GetType(scriptName)) as TaleSpireBehavior;
									if (script != null)
									{
										script.OwnerCreated(creatureAsset.CreatureId.ToString());
										script.Initialize(superPersistentEffect.ScriptData[scriptName]);
									}
								}
							}
						}
					}
					else
					{
						Spells.AttachEffect(creatureAsset, persistentEffect.EffectName, creatureAsset.CreatureId.ToString(), 0, 0, 0, STR_AttachedNode);
					} 
				else
				{
					Log.Error($"Unable to attach any effects. {nameof(persistentEffect)} is null!");
					//Log.Warning($"Adding \"{defaultNewEffect}\"...");
					//Spells.AttachEffect(creatureAsset, defaultNewEffect, creatureAsset.CreatureId.ToString(), 0, 0, 0, STR_AttachedNode);
				}

				Log.Unindent();
			}

			private static void TransferPropertiesToPersistentEffect(CompositeEffect compositeEffect, SuperPersistentEffect superPersistentEffect)
			{
				if (compositeEffect == null)
					return;

				superPersistentEffect.VisibilityMatchesBase = compositeEffect.VisibilityMatchesBase;

				if (compositeEffect.Scripts != null)
				{
					Log.Warning($"Found a composite effect with scripts!");
					foreach (string script in compositeEffect.Scripts)
						if (!superPersistentEffect.ScriptData.ContainsKey(script))
						{
							Log.Warning($"Adding Script Data {script}!!!");
							superPersistentEffect.ScriptData[script] = "";
						}
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

				IOldPersistentEffect persistentEffect = creatureBoardAsset.GetPersistentEffect();
				if (persistentEffect == null)
				{
					Log.Error($"persistentEffect == null");
					return false;
				}

				Log.Debug($"persistentEffect.RotationLocked = {persistentEffect.RotationIsLocked}");

				return persistentEffect.RotationIsLocked;
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

				IOldPersistentEffect persistentEffect = creatureBoardAsset.GetPersistentEffect();
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
				IOldPersistentEffect persistentEffect = creatureBoardAsset.GetPersistentEffect();
				if (persistentEffect == null)
				{
					Log.Error($"persistentEffect == null");
					return;
				}

				Log.Debug($"SetRotationLocked - IsVisible: {creatureBoardAsset.IsVisible}");
				SetSpinLockVisible(creatureBoardAsset, locked && creatureBoardAsset.IsVisible);

				persistentEffect.RotationIsLocked = locked;
				if (locked)
					persistentEffect.LockedRotation = creatureBoardAsset.GetRotationDegrees();

				creatureBoardAsset.SavePersistentEffect(persistentEffect);
			}

			public static void SetHidden(CreatureBoardAsset creatureBoardAsset, bool hidden)
			{
				Log.Debug($"SetHidden - hidden: {hidden}");
				IOldPersistentEffect persistentEffect = creatureBoardAsset.GetPersistentEffect();
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

				if (persistentEffect is SuperPersistentEffect superPersistentEffect && superPersistentEffect.VisibilityMatchesBase)
				{
					GameObject attachedParentGameObject = creatureBoardAsset.GetAttachedParentGameObject();
					if (attachedParentGameObject != null)
						attachedParentGameObject.SetActive(!hidden);
					else
						Log.Error($"Unable to find attached parent game object! Cannot hide this control!");
				}

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

			public static bool IsPersistentEffectRotationLocked(NGuid creatureId)
			{
				return IsRotationLocked(creatureId.ToString());
			}

			public static bool IsPersistentEffectHidden(NGuid creatureId)
			{
				return IsHidden(creatureId.ToString());
			}

			private static void InitializeEffectsIfNecessary()
			{
				if (effectsToInitialize.Count != 0)
				{
					foreach (QueuedEffect queuedEffect in effectsToInitialize)
						queuedEffect.Initialize();

					CleanUp();
				}
			}

			private static void CleanUp()
			{
				if (updatedCreatures.Count > 0)
				{
					foreach (string creatureId in updatedCreatures)
						RemoveEffectToInitialize(creatureId);

					updatedCreatures.Clear();
				}
			}

			private static void RemoveEffectToInitialize(string creatureId)
			{
				List<QueuedEffect> matchingEffects = effectsToInitialize.Where(x => x.Id == creatureId).ToList();
				foreach (QueuedEffect miniEffectName in matchingEffects)
					effectsToInitialize.Remove(miniEffectName);
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

				if (savedEffectsData.Count > 0)
					ProcessSavedEffectsData();
				if (creaturesRenameData.Count > 0)
					ProcessCreatureRenameData();
			}

			static void ProcessCreatureRenameData()
			{
				List<SavedCreatureEffect> creaturesToRemove = new List<SavedCreatureEffect>();
				foreach (SavedCreatureEffect creatureRenameData in creaturesRenameData)
				{
					if (DateTime.Now - creatureRenameData.CreationTime > TimeSpan.FromMilliseconds(500))
					{
						// 500ms have passed. rename...
						Log.Warning($"Setting the creature's name to \"{creatureRenameData.NewCreatureName}\"!!!");
						CreatureManager.SetCreatureName(creatureRenameData.ID, creatureRenameData.NewCreatureName);

						creaturesToRemove.Add(creatureRenameData);
					}
				}

				foreach (SavedCreatureEffect creatureToRemove in creaturesToRemove)
					creaturesRenameData.Remove(creatureToRemove);
			}

			static void ProcessSavedEffectsData()
			{
				List<SavedCreatureEffect> creaturesToRemove = new List<SavedCreatureEffect>();
				foreach (SavedCreatureEffect savedCreatureEffect in savedEffectsData)
				{
					if (DateTime.Now - savedCreatureEffect.CreationTime > TimeSpan.FromMilliseconds(200))
					{
						// 200ms have passed. Initialize...
						StorePersistentData(savedCreatureEffect.ID, savedCreatureEffect.PersistentEffect);
						
						if (!string.IsNullOrWhiteSpace(savedCreatureEffect.NewCreatureName))
						{
							Log.Warning($"savedCreatureEffect.NewCreatureName is \"{savedCreatureEffect.NewCreatureName}\"");
							creaturesRenameData.Add(savedCreatureEffect);  // Need to rename it next...
						}
						else
							Log.Warning($"NewCreatureName is not set!!!");

						creaturesToRemove.Add(savedCreatureEffect);
					}
				}

				foreach (SavedCreatureEffect creatureToRemove in creaturesToRemove)
					savedEffectsData.Remove(creatureToRemove);
			}

			static void StorePersistentData(CreatureGuid iD, IOldPersistentEffect persistentEffect)
			{
				Log.Indent();
				CreatureBoardAsset creatureBoardAsset = Minis.GetCreatureBoardAsset(iD.ToString());
				if (creatureBoardAsset != null)
					SavePersistentEffect(creatureBoardAsset, persistentEffect);
				else
					Log.Error($"creatureBoardAsset is null!");

				Log.Unindent();
			}
		}
	}
}