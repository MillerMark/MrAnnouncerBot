using System;
using System.Linq;
using UnityEngine;

namespace TaleSpireCore
{
	public class TargetingCylinder : TargetingVolume
	{
		static CompositeEffect targetingCylinderCompositeEffect;
		public override CharacterPositions GetAllCreaturesInVolume()
		{
			
			return null;
		}

		public TargetingCylinder()
		{

		}

		public TargetingCylinder(float diameter, float height, Transform parent) : base(parent)
		{
			Height = height;
			Diameter = diameter;
			CreateTargetSelector(targetingCylinderCompositeEffect);

			if (targetingPrefab == null)
			{
				Talespire.Log.Error($"targetingPrefab - Cylinder not found!!!");
				return;
			}

			SetCylinderScale(diameter, height);
		}

		void SetCylinderScale(float diameter, float height)
		{
			if (targetingPrefab == null)
				return;

			float diameterTiles = Talespire.Convert.FeetToTiles(diameter);
			float heightTiles = Talespire.Convert.FeetToTiles(height);

			GameObject top = targetingPrefab.FindChild("Top");
			GameObject sides = targetingPrefab.FindChild("Sides");
			GameObject bottom = targetingPrefab.FindChild("Bottom");

			if (top == null || bottom == null || sides == null)
				return;

			Vector3 topBottomScale = new Vector3(diameterTiles, diameterTiles, diameterTiles);
			Vector3 sidesScale = new Vector3(diameterTiles, heightTiles, diameterTiles);
			Vector3 position = top.transform.localPosition;
			Vector3 topPosition = new Vector3(position.x, heightTiles, position.z);

			top.transform.localPosition = topPosition;
			top.transform.localScale = topBottomScale;
			bottom.transform.localScale = topBottomScale;
			sides.transform.localScale = sidesScale;
		}

		public float Diameter { get; set; }
		public float Height { get; set; }

		public static bool IsTargetingVolumeSet()
		{
			return targetingCylinderCompositeEffect != null;
		}

		public static void SetTargetingEffect(string effectJson)
		{
			targetingCylinderCompositeEffect = CompositeEffect.CreateFrom(effectJson);
			if (targetingCylinderCompositeEffect != null)
				Talespire.Log.Debug($"Targeting Cylinder found!");
			else
				Talespire.Log.Error($"Targeting Cylinder NOT found!");
		}
	}
}