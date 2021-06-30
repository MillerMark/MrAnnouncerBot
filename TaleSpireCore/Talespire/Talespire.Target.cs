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

			static CompositeEffect targetingSphereCompositeEffect;
			static CompositeEffect targetingFireCompositeEffect;
			static List<GameObject> targetDisks = new List<GameObject>();
			static GameObject targetingSphere;
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

			public static TargetKind WhatSide(string worldId)
			{
				if (IsAlly(worldId))
					return TargetKind.Friendly;

				if (IsNeutral(worldId))
					return TargetKind.Neutral;

				return TargetKind.Enemy;
			}

			static TargetKind WhatSide(NGuid worldId)
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
					targetingSphere = targetingSphereCompositeEffect?.CreateOrFindUnsafe();
					if (targetingSphere == null)
					{
						Log.Error($"targetingSphere is NULL!!!");
						return;
					}

					SetSphereScale(diameterFeet);
					targetingSphere.transform.SetParent(flashlight.gameObject.transform);
					targetingSphere.transform.localPosition = new Vector3(0, 0, 0);
				}
				catch (Exception ex)
				{
					Talespire.Log.Exception(ex);
					MessageBox.Show(ex.Message, $"{ex.GetType()} in AddTargetingSphere!");
				}
			}

			public static void SetSphereScale(int diameterFeet)
			{
				if (targetingSphere == null)
					return;
				float diameterTiles = Convert.FeetToTiles(diameterFeet);
				targetingSphere.transform.localScale = new Vector3(diameterTiles, diameterTiles, diameterTiles);
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
				targetingSphere = null;
			}

			private const int maxDistanceToNearestCreatureFt = 10;
			const string STR_TargetDisk = "TargetDisk";

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
				GameObject baseGameObject = GetBaseGameObject(creatureBoardAsset);
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
				GameObject baseGameObject = GetBaseGameObject(creatureBoardAsset);
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
				GameObject baseGameObject = GetBaseGameObject(creatureBoardAsset);
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

			public static GameObject GetBaseGameObject(CreatureBoardAsset creatureBoardAsset)
			{
				GameObject baseGameObject;
				if (creatureBoardAsset.IsFlying)
					baseGameObject = creatureBoardAsset.FlyingIndicator.gameObject;
				else
					baseGameObject = creatureBoardAsset.BaseLoader.LoadedAsset;
				return baseGameObject;
			}

			static CreatureBoardAsset RemoveTargetFromNearestCreature()
			{
				CreatureBoardAsset creatureBoardAsset = Minis.GetCreatureClosestTo(Flashlight.GetPosition(), maxDistanceToNearestCreatureFt);
				if (creatureBoardAsset == null)
					return null;
				RemoveTargetFrom(creatureBoardAsset);

				return creatureBoardAsset;
			}

			public static CreatureBoardAsset StartTargeting()
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

			private static void AddTarget(Vector3 targetPosition, Transform parent, float scale = 1, TargetKind targetKind = TargetKind.Enemy)
			{
				GameObject targetDisk = new GameObject(STR_TargetDisk);

				GameObject targetDiskPrefab;
				if (targetKind == TargetKind.Friendly)
					targetDiskPrefab = Prefabs.Clone("TargetFriend");
				else if (targetKind == TargetKind.Neutral)
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
			}

			private static void AddTargetDisk(Transform parent)
			{
				AddTarget(parent.position, parent, 1);
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
		}
	}
}