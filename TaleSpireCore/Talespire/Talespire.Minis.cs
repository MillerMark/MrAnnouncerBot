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

				string searchId = GetSearch(id);

				foreach (CreatureBoardAsset creatureAsset in allCreatureAssets)
					if (creatureAsset.WorldId.ToString() == searchId)
						return creatureAsset;
				return null;
			}

			public static CreatureBoardAsset[] GetAllCreatureBoardAssetsExcept(string id)
			{
				List<CreatureBoardAsset> results = new List<CreatureBoardAsset>();
				CreatureBoardAsset[] allCreatureAssets = GetAll();
				if (allCreatureAssets == null)
					return null;
				
				string searchId = GetSearch(id);

				foreach (CreatureBoardAsset creatureAsset in allCreatureAssets)
					if (creatureAsset.WorldId.ToString() != searchId)
						results.Add(creatureAsset);

				return results.ToArray();
			}

			private static string GetSearch(string id)
			{
				int lastIndexOfSeparator = id.LastIndexOf(NameIdSeparator);

				if (lastIndexOfSeparator >= 0)
					id = id.Substring(lastIndexOfSeparator + NameIdSeparator.Length);
				return id;
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
				IndicatorGlowfader indicatorGlowFader = GetIndicatorGlowFader(id);

				Color color = new Color(newColor.r * multiplier, newColor.g * multiplier, newColor.b * multiplier);

				if (indicatorGlowFader != null)
					indicatorGlowFader.Glow(true, color);
			}

			public static void IndicatorTurnOff(string id)
			{
				IndicatorGlowfader indicatorGlowFader = GetIndicatorGlowFader(id);

				if (indicatorGlowFader != null)
					indicatorGlowFader.Glow(false, Color.black);
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
					{
						Log.Warning($"Selected asset found!");
						return asset;
					}

				Log.Error($"Selected asset NOT found!");
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

				AssetLoader[] creatureLoaders = creatureBoardAsset?.CreatureLoaders;
				if (creatureLoaders.Length > 0)
					return creatureLoaders[0].LoadedAsset;
				return null;
			}

			public static void SpeakOnlyOnClientMachine(string id, string message)
			{
				CreatureBoardAsset asset = GetCreatureBoardAsset(id);
				if (asset == null)
				{
					Log.Error($"Speak: Creature with id \"{id}\" not found.");
					return;
				}
				asset.Creature.Speak(message);
			}

			public static CreatureBoardAsset SelectOne(string id)
			{
				CreatureBoardAsset[] otherCreatureAssets = GetAllCreatureBoardAssetsExcept(id);
				foreach (CreatureBoardAsset creatureBoardAsset in otherCreatureAssets)
					creatureBoardAsset.Creature.Deselect();

				return Select(id);
			}

			public static CreatureBoardAsset Select(string id)
			{
				CreatureBoardAsset asset = GetCreatureBoardAsset(id);
				if (asset == null)
				{
					Log.Error($"Select: Creature with id \"{id}\" not found.");
					return new CreatureBoardAsset();
				}
				asset.Creature.Select();
				return asset;
			}

			public static CreatureBoardAsset Speak(string id, string message)
			{
				CreatureBoardAsset asset = GetCreatureBoardAsset(id);
				if (asset == null)
				{
					Log.Error($"Speak: Creature with id \"{id}\" not found.");
					return null;
				}
				ChatManager.SendChatMessage(message, asset.Creature.CreatureId.Value);
				return asset;
			}

			public static void Delete(string id)
			{
				CreatureBoardAsset asset = GetCreatureBoardAsset(id);
				if (asset == null)
				{
					Log.Error($"{nameof(Delete)}: Creature with id \"{id}\" not found.");
					return;
				}
				asset.RequestDelete();
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
				GameObject gameObject = creatureBoardAsset.GetBase();
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

			static void SetTurnIndicatorColor(GameObject gameObject, string htmlColorStr, float creatureScale)
			{
				float multiplier = 1;
				Log.Debug($"htmlColorStr = \"{htmlColorStr}\"");
				HueSatLight hueSatLight;
				try
				{
					hueSatLight = new HueSatLight(htmlColorStr);
				}
				catch (Exception ex)
				{
					Log.Exception(ex);
					htmlColorStr = "#ff0000";
					hueSatLight = new HueSatLight("#ff0000");
				}
				double relativeLuminance = hueSatLight.GetRelativeLuminance();
				Log.Debug($"hueSatLight.GetRelativeLuminance: {relativeLuminance}");
				if (relativeLuminance > 0.24)
					multiplier = 0.5f;
				else if (relativeLuminance > 0.5)
					multiplier = 0.4f;
				if (creatureScale < 1)
					multiplier *= 0.75f;
				// relativeLuminance 1
				Property.ModifyColor(gameObject, "Particle System", "<ParticleSystemRenderer>.material._TintColor", $"{htmlColorStr} x{multiplier * 4f}");
				Property.ModifyColor(gameObject, "Decal_FireRing", "<MeshRenderer>.material._TintColor", $"{htmlColorStr} x{multiplier * 7f}");
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

				GameObject baseGameObject = creatureAsset.GetBase();
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

			static Unity.Mathematics.Random random = new Unity.Mathematics.Random((uint)DateTime.Now.Ticks);
			
			static string GetRandomSmallBloodPrefab()
			{
				int[] smallIndices = new int[] { 4, 5, 6, 7 };
				int index = random.NextInt(smallIndices.Length);
				return $"Blood{smallIndices[index]}";
			}

			static string GetRandomLargeBloodPrefab()
			{
				int[] largeIndices = new int[] { 1, 2, 3, 8, 9, 10, 11, 12, 13, 14, 15 };
				int index = random.NextInt(largeIndices.Length);
				return $"Blood{largeIndices[index]}";
			}

			static void ChangeBloodEffectColor(GameObject bloodEffect, string bloodColor)
			{
				Log.Debug($"ChangeBloodEffectColor to {bloodColor}...");
				Transform[] componentsInChildren = bloodEffect.transform.GetComponentsInChildren<Transform>();
				foreach (Transform transform in componentsInChildren)
				{
					MeshRenderer meshRenderer = transform.gameObject.GetComponent<MeshRenderer>();
					if (meshRenderer == null)
						continue;

					//Log.Debug($"Found a GameObject (\"{transform.gameObject.name}\") with a MeshRenderer!!!");
					
					Shader shader = meshRenderer.material?.shader;
					if (shader == null)
						continue;

					int propertyCount = shader.GetPropertyCount();
					for (int i = 0; i < propertyCount; i++)
						if (shader.GetPropertyType(i) == UnityEngine.Rendering.ShaderPropertyType.Color)
						{
							//Log.Debug($"  Color prop name = \"{shader.GetPropertyName(i)}\"");
							Property.ModifyColor(transform.gameObject, null, $"<MeshRenderer>.material.{shader.GetPropertyName(i)}", bloodColor);
						}
				}
			}
			public static void ShowDamage(string creatureId, int damageAmount, string bloodColor, float rotationOffset = 0)
			{
				CreatureBoardAsset creatureAsset = GetCreatureBoardAsset(creatureId);
				if (creatureAsset == null)
				{
					Log.Error($"ShowDamage - creatureId {creatureId} not found.");
					return;
				}

				if (damageAmount > 30)
				{
					ShowDamage(creatureId, damageAmount - 30, bloodColor, random.NextInt(60) - 30);
					damageAmount = 30;
				}

				float scale = 0.65f * creatureAsset.CreatureScale;  // 0.5, 1, 2, 3, 4

				string prefabName;
				float scaleMultiplier;

				if (damageAmount < 15)
				{
					prefabName = GetRandomSmallBloodPrefab();
					scaleMultiplier = 1;
				}
				else
				{
					prefabName = GetRandomLargeBloodPrefab();
					scaleMultiplier = 1 + damageAmount / 70f;
				}

				Log.Debug($"prefabName = \"{prefabName}\"");
				GameObject bloodPrefab = Prefabs.Get(prefabName);

				if (bloodPrefab == null)
				{
					Log.Error($"Prefab \"{prefabName}\" not found!");
					bloodPrefab = Prefabs.Get("Blood4");
				}

				// TODO: Change the blood color... bloodColor

				scale *= scaleMultiplier;

				GameObject bloodEffect = null;
				float groundHeight = creatureAsset.GetGroundHeight();

				//Log.Debug($"groundHeight = {groundHeight}");

				bloodEffect = UnityEngine.Object.Instantiate(bloodPrefab, creatureAsset.HookHitTarget.position, creatureAsset.HookHitTarget.rotation);
				Property.ModifyFloat(bloodEffect, null, "<BFX_BloodSettings>.GroundHeight", groundHeight);
				ChangeBloodEffectColor(bloodEffect, bloodColor);
				bloodEffect.transform.Rotate(Vector3.up, 180 + rotationOffset);
				bloodEffect.transform.localScale = new Vector3(scale, scale, scale);

				Instances.AddTemporal(bloodEffect, 16);
			}

			static void ShowHealth(string creatureId, int healthAmount, string effectName)
			{
				CreatureBoardAsset creatureAsset = GetCreatureBoardAsset(creatureId);
				if (creatureAsset == null)
				{
					Log.Error($"AddHitPoints - creatureId {creatureId} not found.");
					return;
				}

				GameObject heal = CompositeEffect.CreateKnownEffect(effectName);
				heal.transform.position = creatureAsset.transform.position;
				float scale = creatureAsset.CreatureScale / 2.0f;
				heal.transform.localScale = new Vector3(scale, scale, scale);
				Property.Modify(heal, "ParticlesHeal", "<ParticleSystem>.startSize", 0.4f * creatureAsset.CreatureScale);
				Property.Modify(heal, "ParticlesHealPlus", "<ParticleSystem>.startSize", 0.3f * creatureAsset.CreatureScale);

				double lifetimeSeconds = Math.Max(5, Math.Min(healthAmount, 16));
				double particleFadeOutTime = Math.Max(lifetimeSeconds / 2.0, 3);
				Instances.AddTemporal(heal, lifetimeSeconds, particleFadeOutTime);
			}

			public static void AddHitPoints(string creatureId, int healthAmount)
			{
				ShowHealth(creatureId, healthAmount, "Heal");
			}

			public static void AddTempHitPoints(string creatureId, int healthAmount)
			{
				ShowHealth(creatureId, healthAmount, "TempHitPoints");
			}

			static CharacterPositions GetAllInSphere(List<CharacterPosition> characterPositions, VectorDto volumeCenter, float diameter)
			{
				CharacterPositions result = new CharacterPositions();
				foreach (CharacterPosition characterPosition in characterPositions)
					if (characterPosition.IsInsideSphere(volumeCenter, diameter / 2.0f))
						result.Characters.Add(characterPosition);

				return result;
			}

			static CharacterPositions GetAllInCube(List<CharacterPosition> characterPositions, VectorDto volumeCenter, float sideEdgeLength)
			{
				CharacterPositions result = new CharacterPositions();
				foreach (CharacterPosition characterPosition in characterPositions)
					if (characterPosition.IsInsideCube(volumeCenter, sideEdgeLength))
						result.Characters.Add(characterPosition);
				return result;
			}

			static CharacterPositions GetAllInCircle(List<CharacterPosition> characterPositions, VectorDto volumeCenter, float diameter)
			{
				CharacterPositions result = new CharacterPositions();
				foreach (CharacterPosition characterPosition in characterPositions)
					if (characterPosition.IsInsideCircle(volumeCenter, diameter / 2.0f))
						result.Characters.Add(characterPosition);
				return result;
			}

			static CharacterPositions GetAllInCylinder(List<CharacterPosition> characterPositions, VectorDto volumeCenter, float dimension1, float dimension2)
			{
				// TODO: Implement this.
				return null;
			}

			static CharacterPositions GetAllInCone(List<CharacterPosition> characterPositions, VectorDto volumeCenter, float dimension1, float dimension2)
			{
				// TODO: Implement this.
				return null;
			}

			static CharacterPositions GetAllInSquare(List<CharacterPosition> characterPositions, VectorDto volumeCenter, float sideEdgeLength)
			{
				CharacterPositions result = new CharacterPositions();
				foreach (CharacterPosition characterPosition in characterPositions)
					if (characterPosition.IsInsideSquare(volumeCenter, sideEdgeLength))
						result.Characters.Add(characterPosition);

				return result;
			}

			public static CharacterPositions GetAllInSquare(VectorDto volumeCenter, float sideEdgeLengthFeet)
			{
				float sideEdgeLengthTiles = Convert.FeetToTiles(sideEdgeLengthFeet);
				return GetAllInSquare(GetAllCharacterPositions(), volumeCenter, sideEdgeLengthTiles);
			}

			public static CharacterPositions GetAllInCircle(VectorDto volumeCenter, float diameterFeet)
			{
				float diameterTiles = Convert.FeetToTiles(diameterFeet);
				return GetAllInCircle(GetAllCharacterPositions(), volumeCenter, diameterTiles);
			}

			public static CharacterPositions GetAllInSphere(VectorDto volumeCenter, float diameterFeet)
			{
				float diameterTiles = Convert.FeetToTiles(diameterFeet);
				return GetAllInSphere(GetAllCharacterPositions(), volumeCenter, diameterTiles);
			}

			public static CharacterPositions GetAllInCube(VectorDto volumeCenter, float sideEdgeLengthFeet)
			{
				float sideEdgeLengthTiles = Convert.FeetToTiles(sideEdgeLengthFeet);
				return GetAllInCube(GetAllCharacterPositions(), volumeCenter, sideEdgeLengthTiles);
			}

			static List<CharacterPosition> GetAllCharacterPositions()
			{
				return GetAll().ToList().ConvertAll(x => x.GetCharacterPosition());
			}

			public static CharacterPositions GetAllInVolume(VectorDto volumeCenter, TargetVolume volume, float dimensionFeet1, float dimensionFeet2 = 0, float dimensionFeet3 = 0, float dimensionFeet4 = 0)
			{
				float dimensionTiles1 = Convert.FeetToTiles(dimensionFeet1);
				float dimensionTiles2 = Convert.FeetToTiles(dimensionFeet2);
				float dimensionTiles3 = Convert.FeetToTiles(dimensionFeet3);
				float dimensionTiles4 = Convert.FeetToTiles(dimensionFeet4);

				List<CharacterPosition> characterPositions = GetAll().ToList().ConvertAll(x => x.GetCharacterPosition());
				switch (volume)
				{
					case TargetVolume.Sphere:
						return GetAllInSphere(characterPositions, volumeCenter, dimensionTiles1);
					case TargetVolume.Cube:
						return GetAllInCube(characterPositions, volumeCenter, dimensionTiles1);
					case TargetVolume.Circle:
						return GetAllInCircle(characterPositions, volumeCenter, dimensionTiles1);
					case TargetVolume.Cylinder:
						return GetAllInCylinder(characterPositions, volumeCenter, dimensionTiles1, dimensionTiles2);
					case TargetVolume.Cone:
						return GetAllInCone(characterPositions, volumeCenter, dimensionTiles1, dimensionTiles2);
					case TargetVolume.Square:
						return GetAllInSquare(characterPositions, volumeCenter, dimensionTiles1);
				}
				return null;
			}
		}
	}
}