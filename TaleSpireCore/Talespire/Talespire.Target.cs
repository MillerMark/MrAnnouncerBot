using System;
using System.Linq;
using System.Windows.Forms;
using UnityEngine;

namespace TaleSpireCore
{
	public static partial class Talespire
	{
		public static class Target
		{
			static CompositeEffect targetingSphereCompositeEffect;
			static GameObject targetDisk;
			static GameObject targetingSphere;
			public static void SetTargetingSphere(string sphereCompositeEffectJson)
			{
				Log.Debug($"SetTargetingSphere");
				targetingSphereCompositeEffect = CompositeEffect.CreateFrom(sphereCompositeEffectJson);
				if (targetingSphereCompositeEffect != null)
					Log.Debug($"Targeting Sphere found!");
				else
					Log.Error($"Targeting Sphere NOT found!");
			}

			public static void On(int diameter = 0)
			{
				Talespire.Log.Debug($"Flashlight.On();");
				Flashlight.On();
				Talespire.Log.Debug($"FlashLight flashlight = Flashlight.Get();");
				FlashLight flashlight = Flashlight.Get();
				if (flashlight == null)
				{
					Log.Error($"flashlight is null!");
					return;
				}
				Log.Debug($"Adding cylinders...");
				AddTargetDisk(flashlight.gameObject.transform);
				if (diameter > 0)
					AddTargetingSphere(flashlight, diameter);
			}

			private static void AddTargetDisk(Transform parent)
			{
				AddCylinder(parent, 0.25f, Color.red, 0.02f);
				AddCylinder(parent, 0.58f, Color.white, 0f);
				AddCylinder(parent, 0.92f, Color.red, -0.02f);
			}

			static void AddTargetingSphere(FlashLight flashlight, int diameterFeet)
			{
				try
				{
					targetingSphere = targetingSphereCompositeEffect?.CreateOrFind();
					if (targetingSphere == null)
					{
						Talespire.Log.Error($"targetingDome in NULL!!!");
						return;
					}

					SetSphereScale(diameterFeet);
					targetingSphere.transform.SetParent(flashlight.gameObject.transform);
					targetingSphere.transform.localPosition = new Vector3(0, 0, 0);
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, $"{ex.GetType()} in AddTargetingSphere!");
				}
			}

			public static void SetSphereScale(int diameterFeet)
			{
				if (targetingSphere == null)
					return;
				float diameterTiles = diameterFeet / 5;
				targetingSphere.transform.localScale = new Vector3(diameterTiles, diameterTiles, diameterTiles);
			}

			static void AddCylinder(Transform parent, float diameter, Color color, float yOffset)
			{
				GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
				Renderer renderer = cylinder.GetComponent<Renderer>();
				if (renderer != null)
					renderer.material.color = color;

				cylinder.transform.localScale = new Vector3(diameter, 0.07f, diameter);
				cylinder.transform.parent = parent;
				cylinder.transform.localPosition = new Vector3(0, yOffset, 0);
			}

			public static void Off()
			{
				// This works because everything we add is parented by the flashlight.
				Flashlight.Off();
				targetingSphere = null;
			}

			public static void Set()
			{
				FlashLight flashLight = Flashlight.Get();
				if (flashLight != null)
				{
					Vector3 targetPosition = Flashlight.GetPosition();
					targetDisk = new GameObject("TargetDisk");
					targetDisk.transform.position = targetPosition;
					AddTargetDisk(targetDisk.transform);
					Off();
				}
			}

			public static void CleanUp()
			{
				Off();

				if (targetDisk == null)
					return;

				UnityEngine.Object.Destroy(targetDisk);
				targetDisk = null;
			}
		}
	}
}