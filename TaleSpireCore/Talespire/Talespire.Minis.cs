using System;
using System.Collections.Generic;
using System.Linq;

namespace TaleSpireCore
{
	public static partial class Talespire
	{
		public static class Minis
		{
			public static CharacterPositions GetPositions()
			{
				IReadOnlyList<CreatureBoardAsset> allCreatureAssets = CreaturePresenter.AllCreatureAssets;
				if (allCreatureAssets == null)
					return null;

				CharacterPositions characterPositions = new CharacterPositions();

				foreach (CreatureBoardAsset creatureAsset in allCreatureAssets)
				{
					CharacterPosition characterPosition = creatureAsset.GetCharacterPosition();
					characterPositions.Characters.Add(characterPosition);
				}

				return characterPositions;
			}

			public static CharacterPosition GetPosition(string id)
			{
				IReadOnlyList<CreatureBoardAsset> allCreatureAssets = CreaturePresenter.AllCreatureAssets;
				if (allCreatureAssets == null)
					return null;

				foreach (CreatureBoardAsset creatureAsset in allCreatureAssets)
					if (creatureAsset.BoardAssetId.ToString() == id)
						return creatureAsset.GetCharacterPosition();

				return null;
				//// I think the CreatureGuid is different from the id we have for the unique minis.
				//CreatureGuid creatureGuid = new CreatureGuid(id);
				//if (CreaturePresenter.TryGetAsset(creatureGuid, out CreatureBoardAsset creatureAsset))
				//	return creatureAsset.GetCharacterPosition();

				//return null;
			}

		}
	}
}