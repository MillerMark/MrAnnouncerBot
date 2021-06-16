using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Bounce.Unmanaged;

namespace TaleSpireCore
{
	public static partial class Talespire
	{
		public static class Minis
		{
			const string NameIdSeparator = " - ";
			public static CharacterPositions GetPositions()
			{
				CreatureBoardAsset[] allCreatureAssets = GetAll();
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
				CreatureBoardAsset[] allCreatureAssets = GetAll();
				if (allCreatureAssets == null)
					return null;

				int lastIndexOfSeparator = id.LastIndexOf(NameIdSeparator);

				if (lastIndexOfSeparator >= 0)
					id = id.Substring(lastIndexOfSeparator + NameIdSeparator.Length);

				foreach (CreatureBoardAsset creatureAsset in allCreatureAssets)
					if (creatureAsset.WorldId.ToString() == id)
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

			public static CreatureBoardAsset GetSelected()
			{
				CreatureBoardAsset[] assets = GetAll();
				foreach (CreatureBoardAsset asset in assets)
					if (LocalClient.SelectedCreatureId.Value == asset.Creature.CreatureId.Value)
						return asset;

				return null;
			}

			public static CreatureBoardAsset[] GetAll()
			{
				return CreaturePresenter.AllCreatureAssets.ToArray();
			}

			public static List<string> GetAllNamesAndIds()
			{
				CreatureBoardAsset[] allCreatureAssets = GetAll();
				if (allCreatureAssets == null)
					return null;
				List<string> result = new List<string>();
				
				foreach (CreatureBoardAsset creatureBoardAsset in allCreatureAssets)
				{
					Creature creature = creatureBoardAsset.GetComponent<Creature>();
					if (creature != null)
						result.Add($"{creature.Name}{NameIdSeparator}{creatureBoardAsset.WorldId}");
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

			public static CreatureBoardAsset GetCreatureClosestTo(Vector3 position, int maxRadiusFt)
			{
				CreatureBoardAsset closestCreature = null;
				float shortestDistanceSoFar = float.MaxValue;
				CreatureBoardAsset[] allCreatureAssets = GetAll();
				if (allCreatureAssets == null)
					return null;
				foreach (CreatureBoardAsset creatureBoardAsset in allCreatureAssets)
				{
					float distanceFeet = GetDistanceInFeet(position, creatureBoardAsset);
					if (distanceFeet < shortestDistanceSoFar)
					{
						shortestDistanceSoFar = distanceFeet;
						closestCreature = creatureBoardAsset;
					}
				}
				if (shortestDistanceSoFar > maxRadiusFt)
					return null;

				return closestCreature;
			}

			private static float GetDistanceInFeet(Vector3 position, CreatureBoardAsset creatureBoardAsset)
			{
				GameObject gameObject = Target.GetBaseGameObject(creatureBoardAsset);
				if (gameObject == null)
					return float.MaxValue;

				float distanceTiles = (gameObject.transform.position - position).magnitude;
				return Convert.TilesToFeet(distanceTiles);
			}

			public static List<CreatureBoardAsset> GetCreaturesInsideSphere(Vector3 position, float targetSphereDiameterFeet)
			{
				CreatureBoardAsset[] allCreatureAssets = GetAll();
				if (allCreatureAssets == null)
					return null;

				float radiusFeet = targetSphereDiameterFeet / 2f;
				List<CreatureBoardAsset> result = new List<CreatureBoardAsset>();

				foreach (CreatureBoardAsset creatureBoardAsset in allCreatureAssets)
				{
					float distanceFeet = GetDistanceInFeet(position, creatureBoardAsset);
					if (distanceFeet <= radiusFeet)
						result.Add(creatureBoardAsset);
				}

				return result;
			}
		}
	}
}