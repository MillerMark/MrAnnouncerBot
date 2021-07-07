using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using Bounce.Unmanaged;
using UnityEngine;

namespace TaleSpireCore
{
	public static partial class Talespire
	{
		public static class Target
		{
			public static InteractiveTargetingMode InteractiveTargetingMode { get; set; }
			public static int TargetSphereDiameter { get; set; }
			public static int TargetSquareEdgeLength { get; set; }

			static CompositeEffect targetingSphereCompositeEffect;
			static CompositeEffect targetingSquareCompositeEffect;
			static CompositeEffect targetingFireCompositeEffect;
			static List<GameObject> targetDisks = new List<GameObject>();
			static GameObject targetingPrefab;
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

			public static bool IsTargetingSphereSet()
			{
				return targetingSphereCompositeEffect != null;
			}

			public static bool IsTargetingSquareSet()
			{
				return targetingSquareCompositeEffect != null;
			}

			public static bool IsTargetingFireSet()
			{
				return targetingFireCompositeEffect != null;
			}

			public static void SetTargetingSphere(string sphereCompositeEffectJson)
			{
				targetingSphereCompositeEffect = CompositeEffect.CreateFrom(sphereCompositeEffectJson);
				if (targetingSphereCompositeEffect != null)
					Log.Debug($"Targeting Sphere found!");
				else
					Log.Error($"Targeting Sphere NOT found!");
			}

			public static void SetTargetingSquare(string squareCompositeEffectJson)
			{
				targetingSquareCompositeEffect = CompositeEffect.CreateFrom(squareCompositeEffectJson);
				if (targetingSquareCompositeEffect != null)
					Log.Debug($"Targeting Square found!");
				else
					Log.Error($"Targeting Square NOT found!");
			}

			public static void SetTargetingFire(string fireCompositeEffectJson)
			{
				targetingFireCompositeEffect = CompositeEffect.CreateFrom(fireCompositeEffectJson);
				if (targetingFireCompositeEffect != null)
					Log.Debug($"Targeting Fire found!");
				else
					Log.Error($"Targeting Fire NOT found!");
			}

			public static void On(int diameter = 0)
			{
				PrepareForSelection();
				FlashLight flashlight = Flashlight.Get();
				if (flashlight == null)
				{
					Log.Error($"flashlight is null!");
					return;
				}
				AddTargetDisk(flashlight.gameObject.transform);
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
				TargetSphereDiameter = 0;
				CleanUp();
				Flashlight.On();
			}

			static void AddTargetingSphere(FlashLight flashlight, int diameterFeet)
			{
				TargetSphereDiameter = diameterFeet;
				try
				{
					targetingPrefab = targetingSphereCompositeEffect?.CreateOrFindUnsafe();
					if (targetingPrefab == null)
					{
						Log.Error($"targetingSphere is NULL!!!");
						return;
					}

					SetSphereScale(diameterFeet);
					targetingPrefab.transform.SetParent(flashlight.gameObject.transform);
					targetingPrefab.transform.localPosition = new Vector3(0, 0, 0);
				}
				catch (Exception ex)
				{
					Log.Exception(ex);
					MessageBox.Show(ex.Message, $"{ex.GetType()} in AddTargetingSphere!");
				}
			}

			static void AddTargetingSquare(FlashLight flashlight, int edgeLengthFeet)
			{
				TargetSquareEdgeLength = edgeLengthFeet;
				try
				{
					targetingPrefab = targetingSquareCompositeEffect?.CreateOrFindUnsafe();
					if (targetingPrefab == null)
					{
						Log.Error($"targetingPrefab is NULL!!!");
						return;
					}

					SetSquareScale(edgeLengthFeet);
					targetingPrefab.transform.SetParent(flashlight.gameObject.transform);
					targetingPrefab.transform.localPosition = new Vector3(0, 0.1f, 0);
				}
				catch (Exception ex)
				{
					Log.Exception(ex);
					MessageBox.Show(ex.Message, $"{ex.GetType()} in AddTargetingSquare!");
				}
			}

			static void SetSquareScale(int edgeLengthFeet)
			{
				if (targetingPrefab == null)
					return;
				float edgeLengthTiles = Convert.FeetToTiles(edgeLengthFeet);
				targetingPrefab.transform.localScale = new Vector3(edgeLengthTiles, 0.4f, edgeLengthTiles);
			}

			public static void SetSphereScale(int diameterFeet)
			{
				if (targetingPrefab == null)
					return;
				float diameterTiles = Convert.FeetToTiles(diameterFeet);
				targetingPrefab.transform.localScale = new Vector3(diameterTiles, diameterTiles, diameterTiles);

			}

			static void AddSelectionIndicator(FlashLight flashlight)
			{
				try
				{
					targetingFire = targetingFireCompositeEffect?.CreateOrFindUnsafe();
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
				TargetSphereDiameter = 0;
				// This works because everything we add is parented by the flashlight.
				Flashlight.Off();
				targetingPrefab = null;
				activeTargetDisk = null;
			}

			private const int maxDistanceToNearestCreatureFt = 10;
			const string STR_TargetDisk = "TargetDisk";
			static GameObject activeTargetDisk;
			static string targetAnchorId;
			static int targetAnchorRange;
			static float lastTargetTetherUpdateTime;

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
					Vector3 position = flashLight.transform.position;
					VectorDto result = new VectorDto(position.x, position.y, position.z);
					Off();
					InteractiveTargetingMode = InteractiveTargetingMode.None;
					return result;
				}
				return null;
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

				targetDisk.name = STR_TargetDisk;
				const float percentAdjust = 0.6f;
				Property.Modify(targetDiskPrefab, "<TargetScale>.Scale", scale * percentAdjust);

				targetDisk.transform.position = targetPosition;
				if (parent != null)
					targetDisk.transform.SetParent(parent);

				targetDiskPrefab.transform.parent = targetDisk.transform;

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

			public static void CleanUp()
			{
				Off();

				foreach (GameObject gameObject in targetDisks)
					UnityEngine.Object.Destroy(gameObject);

				targetDisks.Clear();
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
				Transform flashLightTransform = flashLight.transform;
				for (int i = targetDisks.Count - 1; i >= 0; i--)
				{
					GameObject gameObject = targetDisks[i];
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

			public static void StartTargeting(string shapeName, int dimensions, string id, int rangeInFeet)
			{
				targetAnchorId = null;

				PrepareForSelection();
				FlashLight flashlight = Flashlight.Get();
				if (flashlight == null)
				{
					Log.Error($"flashlight is null!");
					return;
				}
				activeTargetDisk = AddTargetDisk(flashlight.gameObject.transform);

				if (shapeName == "Sphere" && dimensions > 0)
					AddTargetingSphere(flashlight, dimensions);
				else if (shapeName == "Square" && dimensions > 0)
					AddTargetingSquare(flashlight, dimensions);
				else
					Log.Error($"Unsupported targeting shape: {shapeName} at {dimensions} feet.");

				if (rangeInFeet > 0)
				{
					targetAnchorId = id;
					targetAnchorRange = rangeInFeet;
					// TODO: Tether the target to the mini's location. Use Update method in plugin to communicate with fields in this class.
				}
			}

			public static void Update()
			{
				if (activeTargetDisk != null && !string.IsNullOrWhiteSpace(targetAnchorId))
				{
					if (Time.time - lastTargetTetherUpdateTime > 0.25)  // Limit updates to four times a second.
					{
						lastTargetTetherUpdateTime = Time.time;
						CharacterPosition position = Minis.GetPosition(targetAnchorId);
						if (position != null)
						{
							float distanceTiles = (activeTargetDisk.transform.position - new Vector3(position.Position.x, position.Position.y, position.Position.z)).magnitude;
							float distanceFeet = Convert.TilesToFeet(distanceTiles);
							bool shouldBeActive = distanceFeet < targetAnchorRange;
							if (activeTargetDisk.activeSelf != shouldBeActive)
							{
								if (!shouldBeActive)
									Log.Warning($"Target is too far. Limited to {targetAnchorRange} feet.");
								else
									Log.Warning($"Target is back in range (within {targetAnchorRange} feet of {position.Name}).");

								activeTargetDisk.SetActive(shouldBeActive);
								if (targetingPrefab != null)
									targetingPrefab.SetActive(shouldBeActive);
							}
						}
					}
				}
			}
		}
	}
}