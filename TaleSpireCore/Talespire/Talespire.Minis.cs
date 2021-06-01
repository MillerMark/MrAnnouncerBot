using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TaleSpireCore
{
	public static partial class Talespire
	{
		public static class Minis
		{
			const string NameIdSeparator = " - ";
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

			public static CreatureBoardAsset GetCreatureBoardAsset(string id)
			{
				IReadOnlyList<CreatureBoardAsset> allCreatureAssets = CreaturePresenter.AllCreatureAssets;
				if (allCreatureAssets == null)
					return null;

				int lastIndexOfSeparator = id.LastIndexOf(NameIdSeparator);

				if (lastIndexOfSeparator >= 0)
					id = id.Substring(lastIndexOfSeparator + NameIdSeparator.Length);

				foreach (CreatureBoardAsset creatureAsset in allCreatureAssets)
					if (creatureAsset.BoardAssetId.ToString() == id)
						return creatureAsset;
				return null;
			}

			public static CharacterPosition GetPosition(string id)
			{
				CreatureBoardAsset creatureAsset = GetCreatureBoardAsset(id);
				if (creatureAsset == null)
					return null;

				return creatureAsset.GetCharacterPosition();
			}

			public static void IndicatorChangeColor(string id, Color newColor, float multiplier = 1)
			{
				IndicatorGlowfader indicatorGlowfader = GetIndicatorGlowFader(id);

				Color color = new Color(newColor.r * multiplier, newColor.g * multiplier, newColor.b * multiplier);

				if (indicatorGlowfader != null)
					indicatorGlowfader.Glow(true, color);
			}

			public static void IndicatorTurnOff(string id)
			{
				IndicatorGlowfader indicatorGlowfader = GetIndicatorGlowFader(id);

				if (indicatorGlowfader != null)
					indicatorGlowfader.Glow(false, Color.black);
			}

			static IndicatorGlowfader GetIndicatorGlowFader(string id)
			{
				return GetCreatureBoardAsset(id)?.BaseLoader?.GetComponentInChildren<IndicatorGlowfader>();
			}

			public static IReadOnlyList<CreatureBoardAsset> GetAll()
			{
				return CreaturePresenter.AllCreatureAssets;
			}

			public static List<string> GetAllNamesAndIds()
			{
				IReadOnlyList<CreatureBoardAsset> allCreatureAssets = CreaturePresenter.AllCreatureAssets;
				if (allCreatureAssets == null)
					return null;
				List<string> result = new List<string>();
				
				foreach (CreatureBoardAsset creatureBoardAsset in allCreatureAssets)
				{
					Creature creature = creatureBoardAsset.GetComponent<Creature>();
					if (creature != null)
						result.Add($"{creature.Name}{NameIdSeparator}{creatureBoardAsset.BoardAssetId}");
				}
				return result;
			}

			static GameObject GetContainer(CreatureBoardAsset creatureBoardAsset)
			{
				return creatureBoardAsset?.gameObject?.FindChild("Container");
			}

			/// <summary>
			/// Gets the asset's GameObject that has been loaded for the specified creature id.
			/// </summary>
			/// <param name="id">The id of the creature with the asset to load.</param>
			public static GameObject GetLoadedAsset(string id)
			{
				CreatureBoardAsset creatureBoardAsset = GetCreatureBoardAsset(id);
				if (creatureBoardAsset == null)
				{
					Log.Error($"GetLoadedAsset: Creature with id \"{id}\" not found.");
					return null;
				}

				return creatureBoardAsset?.CreatureLoader?.LoadedAsset;
			}

			public static void SpeakClientOnly(string id, string message)
			{
				CreatureBoardAsset asset = GetCreatureBoardAsset(id);
				if (asset == null)
				{
					Log.Error($"Speak: Creature with id \"{id}\" not found.");
					return;
				}
				asset.Creature.Speak(message);
			}

			public static void Speak(string id, string message)
			{
				CreatureBoardAsset asset = GetCreatureBoardAsset(id);
				if (asset == null)
				{
					Log.Error($"Speak: Creature with id \"{id}\" not found.");
					return;
				}
				ChatManager.SendChatMessage(message, asset.Creature.CreatureId.Value);
			}

			public static void SetCreatureScale(string id, float scale)
			{
				CreatureBoardAsset creatureBoardAsset = GetCreatureBoardAsset(id);
				if (creatureBoardAsset == null)
				{
					Log.Error($"SetCreatureScale: Creature with id \"{id}\" not found.");
					return;
				}

				Transform transform = GetContainer(creatureBoardAsset)?.GetComponent<Transform>();
				if (transform == null)
				{
					Log.Error($"SetCreatureScale: Transform not found!");
					return;
				}

				transform.localScale = new Vector3(scale, scale, scale);
			}
		}
	}
}