using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TaleSpireCore
{
	public class TargetingSphere : TargetingVolume
	{
		static CompositeEffect targetingSphereCompositeEffect;

		public float DiameterFeet { get; set; }
		public override CharacterPositions GetAllTargetsInVolume()
		{
			return Talespire.Minis.GetAllInSphere(Center.GetVectorDto(), DiameterFeet);
		}

		public void SetSphereScale(float diameterFeet)
		{
			if (targetingPrefab == null)
				return;
			float diameterTiles = Talespire.Convert.FeetToTiles(diameterFeet);
			targetingPrefab.transform.localScale = new Vector3(diameterTiles, diameterTiles, diameterTiles);
		}

		public TargetingSphere(float diameterFeet, Transform parent): base(parent)
		{
			DiameterFeet = diameterFeet;
			
			CreateTargetSelector(targetingSphereCompositeEffect);

			if (targetingPrefab == null)
				return;

			SetSphereScale(diameterFeet);
		}

		public static bool IsTargetingSphereSet()
		{
			return targetingSphereCompositeEffect != null;
		}

		public static void SetTargetingEffect(string effectJson)
		{
			targetingSphereCompositeEffect = CompositeEffect.CreateFrom(effectJson);
			if (targetingSphereCompositeEffect != null)
				Talespire.Log.Debug($"Targeting Sphere found!");
			else
				Talespire.Log.Error($"Targeting Sphere NOT found!");
		}

		public TargetingSphere()
		{

		}
	}
}