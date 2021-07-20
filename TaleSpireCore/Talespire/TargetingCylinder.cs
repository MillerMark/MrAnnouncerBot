using System;
using System.Linq;
using UnityEngine;

namespace TaleSpireCore
{
	public class TargetingCylinder : TargetingVolume
	{
		static CompositeEffect targetingCylinderCompositeEffect;
		public override CharacterPositions GetAllTargetsInVolume()
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
			// TODO: Scale this.
			//targetingPrefab.transform.localScale = new Vector3(edgeLengthTiles, edgeLengthTiles, edgeLengthTiles);
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