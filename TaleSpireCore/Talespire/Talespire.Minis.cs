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
			static GameObject activeTurnIndicator;
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

			public static void PlayEmote(string creatureId, string emote)
			{
				CreatureBoardAsset creatureBoardAsset = GetCreatureBoardAsset(creatureId);
				if (creatureBoardAsset == null)
				{
					Log.Error($"{nameof(PlayEmote)}: Creature with id \"{creatureId}\" not found.");
					return;
				}

				creatureBoardAsset.Creature.PlayEmote(emote);
			}

			public static void Wiggle(string creatureId)
			{
				PlayEmote(creatureId, "TLA_Wiggle");
			}

			public static void Twirl(string creatureId)
			{
				PlayEmote(creatureId, "TLA_Twirl");
			}

			public static void KnockDown(string creatureId)
			{
				PlayEmote(creatureId, "TLA_Action_KnockDown");
			}

			public static void Surprise(string creatureId)
			{
				PlayEmote(creatureId, "TLA_Surprise");
			}

			static void ModifyColor(GameObject gameObject, string childName, string propertyName, string valueStr)
			{
				GameObject child = gameObject.FindChild(childName);
				if (child == null)
				{
					Log.Error($"Unable to find child named \"{childName}\".");
					return;
				}
				Log.Debug($"ModifyColor - ChangeColor changeColor = new ChangeColor(propertyName, valueStr);");
				ChangeColor changeColor = new ChangeColor(propertyName, valueStr);
				Log.Debug($"changeColor.ModifyProperty(child);");
				changeColor.ModifyProperty(child);
			}

			static void SetTurnIndicatorColor(GameObject gameObject, string htmlColorStr, float creatureScale)
			{
				float multiplier = 1;
				HueSatLight hueSatLight = new HueSatLight(htmlColorStr);
				double relativeLuminance = hueSatLight.GetRelativeLuminance();
				Talespire.Log.Debug($"hueSatLight.GetRelativeLuminance: {relativeLuminance}");
				if (relativeLuminance > 0.24)
					multiplier = 0.5f;
				else if (relativeLuminance > 0.5)
					multiplier = 0.4f;
				if (creatureScale < 1)
					multiplier *= 0.75f;
				// relativeLuminance 1
				ModifyColor(gameObject, "Particle System", "<ParticleSystemRenderer>.material._TintColor", $"{htmlColorStr} x{multiplier * 4f}");
				ModifyColor(gameObject, "Decal_FireRing", "<MeshRenderer>.material._TintColor", $"{htmlColorStr} x{multiplier * 7f}");
			}

			public static CreatureBoardAsset SetActiveTurn(string creatureId, string color)
			{
				Log.Debug($"SetActiveTurn...");

				ClearActiveTurnIndicator();

				CreatureBoardAsset creatureAsset = GetCreatureBoardAsset(creatureId);
				if (creatureAsset == null)
				{
					Log.Error($"SetActiveTurn - creatureId {creatureId} not found.");
					return null;
				}

				GameObject baseGameObject = Target.GetBaseGameObject(creatureAsset);
				if (baseGameObject == null)
				{
					Log.Error($"SetActiveTurn - baseGameObject for {creatureId} not found.");
					return null;
				}

				string effectName;
				if (creatureAsset.CreatureScale == 2)
					effectName = "ActiveTurn2x2";
				else if (creatureAsset.CreatureScale == 3)
					effectName = "ActiveTurn3x3";
				else if (creatureAsset.CreatureScale == 4)
					effectName = "ActiveTurn4x4";
				else if (creatureAsset.CreatureScale == 0.5)
					effectName = "ActiveTurn0.5x0.5";
				else
					effectName = "ActiveTurn1x1";

				Log.Debug($"activeTurnIndicator = Prefabs.Clone(\"{effectName}\");");
				activeTurnIndicator = Prefabs.Clone(effectName);

				if (activeTurnIndicator == null)
				{
					Log.Error($"SetActiveTurn - Prefabs.Clone(\"{effectName}\") returned null.");
					return null;
				}

				SetTurnIndicatorColor(activeTurnIndicator, color, creatureAsset.CreatureScale);

				Log.Debug($"activeTurnIndicator.transform.SetParent(baseGameObject.transform);");
				activeTurnIndicator.transform.SetParent(baseGameObject.transform);
				activeTurnIndicator.transform.position = creatureAsset.transform.position;
				return creatureAsset;
			}

			// TODO: We might need a universal cleanup mechanism to call this when we unload the game board.
			public static void ClearActiveTurnIndicator()
			{
				if (activeTurnIndicator != null)
				{
					GameObject.Destroy(activeTurnIndicator);
					activeTurnIndicator = null;
				}
			}
		}
	}
}