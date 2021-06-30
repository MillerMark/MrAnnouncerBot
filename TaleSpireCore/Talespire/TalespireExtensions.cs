using System;
using System.Linq;
using UnityEngine;

namespace TaleSpireCore
{
	public static class TalespireExtensions
	{
		public static Vector3 GetVector3(this VectorDto vectorDto)
		{
			return new Vector3(vectorDto.x, vectorDto.y, vectorDto.z);
		}


		/// <summary>
		/// Returns the height of the ground at the creature's position.
		/// </summary>
		public static float GetGroundHeight(this CreatureBoardAsset creatureAsset)
		{
			float groundHeight = creatureAsset.PlacedPosition.y;
			if (creatureAsset.IsFlying)
				groundHeight -= creatureAsset.FlyingIndicator.ElevationAmount;
			return groundHeight;
		}

		public static GameObject FindChild(this GameObject gameObject, string name)
		{
			Transform[] childTransforms = gameObject.GetComponentsInChildren<Transform>();

			if (childTransforms == null)
				return null;

			foreach (Transform transform in childTransforms)
			{
				GameObject child = transform.gameObject;
				if (child?.name == name)
					return child;
			}

			return null;
		}

		public static bool HasChild(this Transform transform, string nameToFind)
		{
			Transform[] children = transform.GetComponentsInChildren<Transform>();
			foreach (Transform child in children)
				if (child.gameObject.name == nameToFind)
					return true;

			return false;
		}

		public static GameObject GetChild(this Transform transform, string nameToFind)
		{
			Transform[] children = transform.GetComponentsInChildren<Transform>();
			foreach (Transform child in children)
				if (child.gameObject.name == nameToFind)
					return child.gameObject;

			return null;
		}

		public static CharacterPosition GetCharacterPosition(this CreatureBoardAsset creatureAsset)
		{
			float altitude = 0;
			if (creatureAsset == null)
				return null;

			if (creatureAsset.FlyingIndicator?.ElevationAmount > 0)
				altitude = creatureAsset.FlyingIndicator.ElevationAmount;

			VectorDto creaturePosition = creatureAsset.GetPositionVectorDto();
			CharacterPosition characterPosition = new CharacterPosition()
			{
				Name = TaleSpireUtils.GetName(creatureAsset),
				Position = creaturePosition,
				ID = creatureAsset.WorldId.ToString(),
				FlyingAltitude = altitude
			};
			return characterPosition;
		}

		public static VectorDto GetPositionVectorDto(this CreatureBoardAsset creatureAsset)
		{
			return new VectorDto(creatureAsset.PlacedPosition.x, creatureAsset.PlacedPosition.y, creatureAsset.PlacedPosition.z);
		}
	}
}