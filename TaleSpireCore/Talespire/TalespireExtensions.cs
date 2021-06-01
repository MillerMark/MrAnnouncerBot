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

		public static CharacterPosition GetCharacterPosition(this CreatureBoardAsset creatureAsset)
		{
			float altitude = 0;
			if (creatureAsset.FlyingIndicator?.ElevationAmount > 0)
				altitude = creatureAsset.FlyingIndicator.ElevationAmount;

			VectorDto creaturePosition = creatureAsset.GetPositionVectorDto();
			CharacterPosition characterPosition = new CharacterPosition()
			{
				Name = TaleSpireUtils.GetName(creatureAsset),
				Position = creaturePosition,
				ID = creatureAsset.BoardAssetId.ToString(),
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