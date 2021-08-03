using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using Bounce.Unmanaged;
using UnityEngine;
using TMPro;

namespace TaleSpireCore
{
	public static partial class Talespire
	{
		public static class Target
		{
			public static InteractiveTargetingMode InteractiveTargetingMode { get; set; }
			public static TargetingVolume TargetingVolume { get; set; }
			public static float TargetSquareEdgeLength { get; set; }
			public static MiniRotator MiniRotator { get; set; } = new MiniRotator();

			static CompositeEffect targetingFireCompositeEffect;
			static GameObject savedTargetingUI;
			static List<GameObject> targetDisks = new List<GameObject>();
			static GameObject targetingFire;

			static List<string> allies = new List<string>();
			static List<string> neutrals = new List<string>();

			public static void RegisterAllies(string[] allyList)
			{
				allies.Clear();
				allies.AddRange(allyList);
			}

			public static void RegisterNeutrals(string[] neutralList)
			{
				neutrals.Clear();
				neutrals.AddRange(neutralList);
			}

			public static bool IsAlly(string worldId)
			{
				return allies.Contains(worldId);
			}

			public static bool IsNeutral(string worldId)
			{
				return neutrals.Contains(worldId);
			}

			public static bool IsEnemy(string worldId)
			{
				return !IsAlly(worldId) && !IsNeutral(worldId);
			}

			public static WhatSide WhatSide(string worldId)
			{
				if (IsAlly(worldId))
					return TaleSpireCore.WhatSide.Friendly;

				if (IsNeutral(worldId))
					return TaleSpireCore.WhatSide.Neutral;

				return TaleSpireCore.WhatSide.Enemy;
			}

			static WhatSide WhatSide(NGuid worldId)
			{
				return WhatSide(worldId.ToString());
			}


			public static void SetInteractiveTargetingMode(InteractiveTargetingMode mode)
			{
				InteractiveTargetingMode = mode;
			}

			public static bool IsTargetingFireSet()
			{
				return targetingFireCompositeEffect != null;
			}

			public static void SetTargetingFire(string fireCompositeEffectJson)
			{
				targetingFireCompositeEffect = CompositeEffect.CreateFrom(fireCompositeEffectJson);
				if (targetingFireCompositeEffect != null)
					Log.Debug($"Targeting Fire found!");
				else
					Log.Error($"Targeting Fire NOT found!");
			}

			static void InitializeTargeting()
			{
				MiniRotator.Initialize();
			}

			// TODO: Refactor this into multiple dedicated methods with the best names ever!!!
			public static void On(int diameter = 0, string creatureId = null)
			{
				PrepareForSelection();
				FlashLight flashlight = Flashlight.Get();
				if (flashlight == null)
				{
					Log.Error($"flashlight is null!");
					return;
				}
				LockTo(creatureId);
				GameObject targetDisk = AddTargetDisk(flashlight.gameObject.transform);
				
				//if (!string.IsNullOrWhiteSpace(creatureId))
				activeTargetDisk = targetDisk;

				InitializeTargeting();

				if (diameter > 0)
					AddTargetingSphere(flashlight, diameter);
			}

			public static void SelectCreature(int diameter = 0)
			{
				PrepareForSelection();
				FlashLight flashlight = Flashlight.Get();
				if (flashlight == null)
				{
					Log.Error($"flashlight is null!");
					return;
				}
				AddSelectionIndicator(flashlight);
			}

			private static void PrepareForSelection()
			{
				TargetingVolume = null;
				CleanUp(false);
				Flashlight.On();
			}

			static void AddTargetingSphere(FlashLight flashlight, float diameterFeet)
			{
				TargetingVolume = new TargetingSphere(diameterFeet, flashlight.gameObject.transform);
			}

			static void AddTargetingSquare(FlashLight flashlight, float edgeLengthFeet)
			{
				TargetingVolume = new TargetingSquare(edgeLengthFeet, flashlight.gameObject.transform);
			}


			static void AddTargetingCylinder(FlashLight flashlight, float diameter, float height)
			{
				TargetingVolume = new TargetingCylinder(diameter, height, flashlight.gameObject.transform);
			}

			static void AddSelectionIndicator(FlashLight flashlight)
			{
				try
				{
					targetingFire = targetingFireCompositeEffect?.CreateOrFindUnsafe();
					targetingFireCompositeEffect?.RefreshIfNecessary(targetingFire);
					if (targetingFire == null)
					{
						Log.Error($"targetingFire is NULL!!!");
						return;
					}

					targetingFire.transform.SetParent(flashlight.gameObject.transform);
					targetingFire.transform.localPosition = new Vector3(0, 0, 0);
				}
				catch (Exception ex)
				{
					Log.Exception(ex);
				}
			}

			static void AddCylinder(Transform parent, float diameter, Color color, float yOffset)
			{
				GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
				Renderer renderer = cylinder.GetComponent<Renderer>();
				if (renderer != null)
					renderer.material.color = color;

				cylinder.transform.localScale = new Vector3(diameter, 0.05f, diameter);
				cylinder.transform.parent = parent;
				cylinder.transform.localPosition = new Vector3(0, yOffset, 0);
			}

			public static void Off()
			{
				LockTo(null);
				TargetingVolume = null;
				// This works because everything we add is parented by the flashlight.
				Flashlight.Off();
				activeTargetDisk = null;
				activeTargetRangeText = null;
				activeTargetRangeIndicator = null;
				activeTargetRangeIndicatorRedX = null;
				MiniRotator.Done();
				//activeTargetRangeIndicator = null;
			}

			private const int maxDistanceToNearestCreatureFt = 10;
			const string STR_TargetDisk = "TargetDisk";
			const string STR_TargetRangeIndicator = "TargetRangeIndicator";
			const string STR_TargetDiskPrefab = "TargetDiskPrefab";
			static GameObject activeTargetDisk;
			static GameObject activeTargetRangeIndicator;
			static GameObject activeTargetRangeIndicatorRedX;
			static string targetAnchorId;
			static int targetAnchorRange;
			static float lastTargetTetherUpdateTime;
			static TextMeshPro activeTargetRangeText;
			
			static CreatureBoardAsset AddTargetToNearestCreature()
			{
				CreatureBoardAsset creatureBoardAsset = GetNearestCreature();
				AddTargetTo(creatureBoardAsset);
				return creatureBoardAsset;
			}

			static CreatureBoardAsset GetNearestCreature()
			{
				return Minis.GetCreatureClosestTo(Flashlight.GetPosition(), maxDistanceToNearestCreatureFt);
			}

			/// <summary>
			/// Drops a target on the base of the specified creature.
			/// </summary>
			/// <param name="creatureId"></param>
			public static void Drop(string creatureId)
			{
				Log.Debug($"Target.Drop {creatureId}!");
				CreatureBoardAsset creatureBoardAsset = Minis.GetCreatureBoardAsset(creatureId);
				if (creatureBoardAsset != null)
				{
					Log.Debug($"creature found!");
					AddTargetTo(creatureBoardAsset);
				}
			}

			static bool IsTargeted(CreatureBoardAsset creatureBoardAsset)
			{
				GameObject baseGameObject = creatureBoardAsset.GetBase();
				if (baseGameObject == null)
					return false;
				Transform baseTransform = baseGameObject.transform;
				if (baseTransform == null)
					return false;

				return baseTransform.HasChild(STR_TargetDisk);
			}


			private static void RemoveTargetFrom(CreatureBoardAsset creatureBoardAsset)
			{
				if (creatureBoardAsset == null)
					return;
				GameObject baseGameObject = creatureBoardAsset.GetBase();
				GameObject targetDisk = baseGameObject.transform.GetChild(STR_TargetDisk);
				if (targetDisk != null)
				{
					targetDisks.Remove(targetDisk);
					UnityEngine.Object.Destroy(targetDisk);
				}
			}

			private static void AddTargetTo(CreatureBoardAsset creatureBoardAsset)
			{
				if (creatureBoardAsset == null)
					return;
				GameObject baseGameObject = creatureBoardAsset.GetBase();
				Transform baseTransform = baseGameObject.transform;

				if (baseTransform.HasChild(STR_TargetDisk))
				{
					Log.Warning($"{creatureBoardAsset.name}'s base transform already has a child STR_TargetDisk!");
					return;
				}

				Minis.Surprise(creatureBoardAsset);

				Vector3 basePosition = baseTransform.position;

				// Bump the target position up a bit...
				Vector3 targetPosition = new Vector3(basePosition.x, basePosition.y + 0.05f, basePosition.z);

				AddTarget(targetPosition, baseTransform, creatureBoardAsset.CreatureScale, WhatSide(creatureBoardAsset.WorldId));
			}

			static CreatureBoardAsset RemoveTargetFromNearestCreature()
			{
				CreatureBoardAsset creatureBoardAsset = Minis.GetCreatureClosestTo(Flashlight.GetPosition(), maxDistanceToNearestCreatureFt);
				if (creatureBoardAsset == null)
					return null;
				RemoveTargetFrom(creatureBoardAsset);

				return creatureBoardAsset;
			}

			public static VectorDto GetTargetedPoint()
			{
				FlashLight flashLight = Flashlight.Get();
				if (flashLight != null)
				{
					if (TargetingVolume != null)
						MoveTargetingPrefabToWorld(flashLight);
					Vector3 position = flashLight.transform.position;
					VectorDto result = new VectorDto(position.x, position.y, position.z);
					Off();
					InteractiveTargetingMode = InteractiveTargetingMode.None;
					return result;
				}
				return null;
			}

			private static void MoveTargetingPrefabToWorld(FlashLight flashLight)
			{
				if (TargetingVolume == null)
					return;
				Transform transform = TargetingVolume.PrefabTransform;
				float saveY = transform.localPosition.y;
				transform.SetParent(null);
				transform.position = flashLight.transform.position;
				Vector3 localPosition = flashLight.transform.localPosition;
				transform.localPosition = new Vector3(localPosition.x, localPosition.y + saveY, localPosition.z);
				savedTargetingUI = TargetingVolume?.targetingPrefab;
				AddTargetDisk(transform);
			}

			public static CreatureBoardAsset GetTargetedCreature()
			{
				Log.Debug($"StartTargeting - InteractiveTargetingMode = {InteractiveTargetingMode}");
				FlashLight flashLight = Flashlight.Get();
				if (flashLight != null)
				{
					if (InteractiveTargetingMode == InteractiveTargetingMode.Creatures)
						return AddTargetToNearestCreature();
					else if (InteractiveTargetingMode == InteractiveTargetingMode.CreatureSelect)
					{
						CreatureBoardAsset nearestCreature = GetNearestCreature();
						Log.Debug($"nearestCreature: {nearestCreature}");
						Off();
						InteractiveTargetingMode = InteractiveTargetingMode.None;
						return nearestCreature;
					}
					else
					{
						DropTargetAtFlashlight();
						Off();
						InteractiveTargetingMode = InteractiveTargetingMode.None;
					}
				}
				return null;
			}

			private static void DropTargetAtFlashlight()
			{
				Vector3 targetPosition = Flashlight.GetPosition();
				AddTarget(targetPosition, Flashlight.Get().transform, 1);
			}

			private static GameObject AddTarget(Vector3 targetPosition, Transform parent, float scale = 1, WhatSide whatSide = TaleSpireCore.WhatSide.Enemy)
			{
				GameObject targetDisk = new GameObject(STR_TargetDisk);

				GameObject targetDiskPrefab;
				if (whatSide == TaleSpireCore.WhatSide.Friendly)
					targetDiskPrefab = Prefabs.Clone("TargetFriend");
				else if (whatSide == TaleSpireCore.WhatSide.Neutral)
					targetDiskPrefab = Prefabs.Clone("TargetNeutral");
				else
					targetDiskPrefab = Prefabs.Clone("TargetEnemy");

				targetDiskPrefab.name = STR_TargetDiskPrefab;

				GameObject targetRangeIndicator = targetDiskPrefab.FindChild(STR_TargetRangeIndicator);
				if (targetRangeIndicator == null)
				{
					Log.Error($"targetRangeIndicator NOT found!!!");
				}
				else
				{
					Log.Warning($"targetRangeIndicator FOUND!!!");
					targetRangeIndicator.SetActive(false);
				}

				targetDisk.name = STR_TargetDisk;
				const float percentAdjust = 0.6f;
				Property.Modify(targetDiskPrefab, "<TargetScale>.Scale", scale * percentAdjust);

				targetDisk.transform.position = targetPosition;
				if (parent != null)
					targetDisk.transform.SetParent(parent);

				targetDiskPrefab.transform.SetParent(targetDisk.transform);

				targetRangeIndicator = targetDiskPrefab.FindChild(STR_TargetRangeIndicator);
				if (targetRangeIndicator != null)
				{
					targetRangeIndicator = targetDisk.FindChild(STR_TargetRangeIndicator);
					if (targetRangeIndicator != null)
						Log.Warning($"targetRangeIndicator was found! What IS GOING ON???");
					else
					{
						Log.Error($"targetRangeIndicator was NOT found. Why not?");
					}
				}

				float yOffset = 0;
				if (scale == 0.5)
					yOffset = -0.89f;
				else if (scale == 1)
					yOffset = -0.86f;
				else if (scale == 2)
					yOffset = -0.765f;
				else if (scale == 3)
					yOffset = -0.674f;
				else if (scale == 4)
					yOffset = -0.5725f;
				targetDiskPrefab.transform.localPosition = new Vector3(0, 0.9f + yOffset, 0);

				targetDisks.Add(targetDisk);
				return targetDisk;
			}

			private static GameObject AddTargetDisk(Transform parent)
			{
				return AddTarget(parent.position, parent, 1);
			}

			public static void CleanUp(bool removeTargetDisks = true)
			{
				Off();

				if (removeTargetDisks)
				{
					foreach (GameObject gameObject in targetDisks)
						UnityEngine.Object.Destroy(gameObject);
					targetDisks.Clear();
				}

				RemoveTargetingUI();
			}

			public static void RemoveTargetingUI()
			{
				if (savedTargetingUI != null)
				{
					UnityEngine.Object.Destroy(savedTargetingUI);
					savedTargetingUI = null;
				}
			}

			public static void Ready()
			{
				Off();
			}

			public static CreatureBoardAsset Clear()
			{
				return RemoveTargetFromNearestCreature();
			}

			public static void PickupAllDroppedTargets()
			{
				FlashLight flashLight = Flashlight.Get();
				if (flashLight == null)
					return;
				Transform flashLightTransform = flashLight.transform;
				for (int i = targetDisks.Count - 1; i >= 0; i--)
				{
					GameObject gameObject = targetDisks[i];
					if (gameObject != null)
						if (gameObject.transform.parent == flashLightTransform)
							continue;
						else
						{
							gameObject.transform.SetParent(null);
							UnityEngine.Object.Destroy(gameObject);
						}

					targetDisks.RemoveAt(i);
				}
			}

			public static CreatureBoardAsset Set(string creatureId, bool targeted)
			{
				if (string.IsNullOrWhiteSpace(creatureId))
					return null;
				Log.Debug($"Target.Set {creatureId}!");
				CreatureBoardAsset creatureBoardAsset = Minis.GetCreatureBoardAsset(creatureId);
				if (creatureBoardAsset != null)
				{
					if (IsTargeted(creatureBoardAsset) == targeted)
					{
						if (targeted)
							Log.Warning($"creature {creatureId} is already targeted.");
						else
							Log.Warning($"creature {creatureId} is already NOT targeted.");

						return creatureBoardAsset;
					}

					if (targeted)
						AddTargetTo(creatureBoardAsset);
					else
						RemoveTargetFrom(creatureBoardAsset);
				}
				return creatureBoardAsset;
			}

			public static FlashLight StartTargeting(string id, int rangeInFeet)
			{
				targetAnchorId = null;

				RemoveTargetingUI();

				PrepareForSelection();
				FlashLight flashlight = Flashlight.Get();
				if (flashlight != null)
					activeTargetDisk = AddTargetDisk(flashlight.gameObject.transform);

				InitializeTargeting();

				if (rangeInFeet > 0)
				{
					targetAnchorId = id;
					targetAnchorRange = rangeInFeet;
				}
				return flashlight;
			}

			public static void StartVolumeTargeting(string shapeName, string id, int rangeInFeet, float dimension1Feet, float dimension2Feet = 0, float dimension3Feet = 0)
			{
				FlashLight flashlight = StartTargeting(id, rangeInFeet);

				if (flashlight == null)
				{
					Log.Error($"flashlight is null!");
					return;
				}

				if (shapeName == "Sphere" && dimension1Feet > 0)
					AddTargetingSphere(flashlight, dimension1Feet);
				else if (shapeName == "Square" && dimension1Feet > 0)
					AddTargetingSquare(flashlight, dimension1Feet);
				else if (shapeName == "Cylinder" && dimension1Feet > 0 && dimension2Feet > 0)
					AddTargetingCylinder(flashlight, dimension1Feet, dimension2Feet);
				else
					Log.Error($"Unsupported targeting shape: {shapeName} at {dimension1Feet} feet.");
			}

			public static void Update()
			{
				if (IsTargetingWithAnchor)
					UpdateTarget();
				MiniRotator.Update();
			}

			private static bool IsTargetingWithAnchor => activeTargetDisk != null && !string.IsNullOrWhiteSpace(targetAnchorId);

			private static void UpdateTarget()
			{
				if (Time.time - lastTargetTetherUpdateTime <= 0.1)  // Limit updates to ten times a second.
					return;

				lastTargetTetherUpdateTime = Time.time;

				if (activeTargetRangeIndicator == null)
					ActivateTargetRangeIndicator();

				KeepTargetTextFacingCamera();

				CharacterPosition position = Minis.GetPosition(targetAnchorId);
				if (position != null)
					UpdateTargetDistanceFrom(position);
			}

			private static void UpdateTargetDistanceFrom(CharacterPosition position)
			{
				float distanceFeet = GetTargetingCursorDistanceTo(position);

				if (activeTargetRangeText != null)
					activeTargetRangeText.text = $"{distanceFeet:F}ft";

				bool isInRange = distanceFeet < targetAnchorRange;

				if (TargetingVolume?.activeSelf != isInRange)
					ToggleTargetIndicatorState(isInRange);
			}

			private static void ToggleTargetIndicatorState(bool isInRange)
			{
				GameObject disk = activeTargetDisk.FindChild("Disk", true);
				GameObject disabled = activeTargetDisk.FindChild("Disabled", true);
				if (disk != null && disabled != null)
				{
					disk.SetActive(isInRange);
					disabled.SetActive(!isInRange);
				}
				if (!isInRange)
					TargetIsOutOfRange();
				else
					TargetIsInRange();

				if (TargetingVolume != null)
					TargetingVolume.SetActive(isInRange);
			}

			private static void TargetIsInRange()
			{
				activeTargetRangeText.color = new Color32(255, 255, 255, 255);
				activeTargetRangeText.fontSize = 2;
				activeTargetRangeText.transform.localPosition = new Vector3(0, 0.433f, 0);
			}

			private static void TargetIsOutOfRange()
			{
				activeTargetRangeText.color = new Color32(255, 99, 99, 255);
				activeTargetRangeText.fontSize = 4;
				activeTargetRangeText.transform.localPosition = new Vector3(0, 0.8f, 0);
			}

			public static float GetTargetingCursorDistanceTo(CharacterPosition position)
			{
				return GetTargetingCursorDistanceTo(new Vector3(position.Position.x, position.Position.y, position.Position.z));
			}

			public static float GetTargetingCursorDistanceTo(Vector3 position)
			{
				float distanceTiles = (TargetingPosition - position).magnitude;
				return Convert.TilesToFeet(distanceTiles);
			}

			public static Vector3 TargetingPosition => activeTargetDisk?.transform != null ? activeTargetDisk.transform.position : Vector3.zero;

			private static void KeepTargetTextFacingCamera()
			{
				Vector3 cameraPosition = Camera.GetPosition();

				if (activeTargetRangeIndicator != null)
				{
					activeTargetRangeIndicator.transform.LookAt(cameraPosition);
					activeTargetRangeIndicator.transform.Rotate(Vector3.up, 180);
				}

				if (activeTargetRangeIndicatorRedX != null)
				{
					activeTargetRangeIndicatorRedX.transform.LookAt(cameraPosition);
					activeTargetRangeIndicatorRedX.transform.Rotate(Vector3.right, 45);
				}
			}

			private static void ActivateTargetRangeIndicator()
			{
				activeTargetRangeIndicator = activeTargetDisk.FindChild(STR_TargetRangeIndicator, true);
				activeTargetRangeIndicatorRedX = activeTargetDisk.FindChild("Red X", true);
				if (activeTargetRangeIndicator != null)
				{
					activeTargetRangeIndicator.SetActive(true);
					activeTargetRangeText = activeTargetRangeIndicator.GetComponent<TextMeshPro>();
				}
			}

			public static void LockTo(string creatureId)
			{
				targetAnchorId = creatureId;
			}
			public static CharacterPositions GetCreaturesInsideTargetingVolume()
			{	
				if (TargetingVolume != null)
					return TargetingVolume.GetAllCreaturesInVolume();
				return null;
			}
		}
	}
}