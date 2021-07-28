using System;
using System.Linq;
using UnityEngine;

namespace TaleSpireCore
{
	public class TargetingSquare : TargetingVolume
	{
		static CompositeEffect targetingSquareCompositeEffect;
		public override CharacterPositions GetAllCreaturesInVolume()
		{
			return null;
		}

		public TargetingSquare(float edgeLengthFeet, Transform parent) : base(parent)
		{
			offsetY = 0.1f;
			EdgeLengthFeet = edgeLengthFeet;
			CreateTargetSelector(targetingSquareCompositeEffect);

			if (targetingPrefab == null)
				return;

			SetSquareScale(edgeLengthFeet);
		}

		void SetSquareScale(float edgeLengthFeet)
		{
			if (targetingPrefab == null)
				return;
			float edgeLengthTiles = Talespire.Convert.FeetToTiles(edgeLengthFeet);
			targetingPrefab.transform.localScale = new Vector3(edgeLengthTiles, edgeLengthTiles, edgeLengthTiles);
		}

		public float EdgeLengthFeet { get; set; }

		public static bool IsTargetingSquareSet()
		{
			return targetingSquareCompositeEffect != null;
		}

		public static void SetTargetingEffect(string effectJson)
		{
			targetingSquareCompositeEffect = CompositeEffect.CreateFrom(effectJson);
			if (targetingSquareCompositeEffect != null)
				Talespire.Log.Debug($"Targeting Square found!");
			else
				Talespire.Log.Error($"Targeting Square NOT found!");
		}
	}
}