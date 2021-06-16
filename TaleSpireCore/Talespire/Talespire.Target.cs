using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
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
			static List<GameObject> targetDisks = new List<GameObject>();
			static GameObject targetingSphere;

			public static void SetInteractiveTargetingMode(InteractiveTargetingMode mode)
			{
				InteractiveTargetingMode = mode;
			}

			public static bool IsTargetingSphereSet()
			{
				return targetingSphereCompositeEffect != null;
			}

			public static void SetTargetingSphere(string sphereCompositeEffectJson)
			{
				targetingSphereCompositeEffect = CompositeEffect.CreateFrom(sphereCompositeEffectJson);
				if (targetingSphereCompositeEffect != null)
					Log.Debug($"Targeting Sphere found!");
				else
					Log.Error($"Targeting Sphere NOT found!");
			}

			public static void On(int diameter = 0)
			{
				TargetSphereDiameter = 0;

				CleanUp();
				Flashlight.On();
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

			static void AddTargetingSphere(FlashLight flashlight, int diameterFeet)
			{
				TargetSphereDiameter = diameterFeet;
				try
				{
					targetingSphere = targetingSphereCompositeEffect?.CreateOrFind();
					if (targetingSphere == null)
					{
						Log.Error($"targetingDome in NULL!!!");
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
				CreatureBoardAsset creatureBoardAsset = Minis.GetCreatureClosestTo(Flashlight.GetPosition(), maxDistanceToNearestCreatureFt);
				if (creatureBoardAsset != null)
				{
					AddTargetDiskToCreature(creatureBoardAsset);
				}
				return creatureBoardAsset;
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
					AddTargetDiskToCreature(creatureBoardAsset);
				}
			}

			private static void AddTargetDiskToCreature(CreatureBoardAsset creatureBoardAsset)
			{
				GameObject baseGameObject = GetBaseGameObject(creatureBoardAsset);
				Transform baseTransform = baseGameObject.transform;

				if (baseTransform.HasChild(STR_TargetDisk))
					return;

				Vector3 basePosition = baseTransform.position;

				// Bump the target position up a bit...
				Vector3 targetPosition = new Vector3(basePosition.x, basePosition.y + 0.05f, basePosition.z);

				AddTargetDisk(targetPosition, baseTransform);
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

				GameObject baseGameObject = GetBaseGameObject(creatureBoardAsset);

				Transform[] children = baseGameObject.GetComponentsInChildren<Transform>();
				
				foreach (Transform transform in children)
					if (transform.gameObject.name == STR_TargetDisk)
					{
						targetDisks.Remove(transform.gameObject);
						UnityEngine.Object.Destroy(transform.gameObject);
						return creatureBoardAsset;
					}
				return creatureBoardAsset;
			}

			public static CreatureBoardAsset Set()
			{
				FlashLight flashLight = Flashlight.Get();
				if (flashLight != null)
				{
					if (InteractiveTargetingMode == InteractiveTargetingMode.Creatures)
						return AddTargetToNearestCreature();
					else
					{
						DropTargetAtFlashlight();
						Off();
					}
				}
				return null;
			}

			private static void DropTargetAtFlashlight()
			{
				Vector3 targetPosition = Flashlight.GetPosition();
				AddTargetDisk(targetPosition, null);
			}

			private static void AddTargetDisk(Vector3 targetPosition, Transform parent)
			{
				GameObject targetDisk = new GameObject(STR_TargetDisk);
				targetDisk.transform.position = targetPosition;
				if (parent != null)
					targetDisk.transform.SetParent(parent);
				float percentAdjust = 0.65f;
				AddCylinder(targetDisk.transform, 0.25f * percentAdjust, Color.red, 0.02f);
				AddCylinder(targetDisk.transform, 0.58f * percentAdjust, Color.white, 0f);
				AddCylinder(targetDisk.transform, 0.92f * percentAdjust, Color.red, -0.02f);
				targetDisks.Add(targetDisk);
			}

			private static void AddTargetDisk(Transform parent)
			{
				AddTargetDisk(parent.position, parent);
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
						UnityEngine.Object.Destroy(gameObject);
					targetDisks.RemoveAt(i);
				}
			}

		}
	}
}