using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using LordAshes;
using Newtonsoft.Json;
using System.Xml.Linq;
using Unity.Entities;
using static TaleSpireCore.Talespire;

namespace TaleSpireCore
{
	public static class TalespireExtensions
	{
		// ![](330E21079BE9A11BBF50634E3F861264.png)
		// ![](A4B633BD4A6C55C7C086DDE7783ADCB6.png;;;0.02569,0.02569)
		public static int GetBaseColorIndex(this CreatureBoardAsset creatureBoardAsset)
		{
			if (Guard.IsNull(creatureBoardAsset, "creatureBoardAsset")) return 0;

			MaterialPropertyBlock _matBlock = ReflectionHelper.GetNonPublicField<MaterialPropertyBlock>(creatureBoardAsset, "_matBlock");

			int BaseIndex = Shader.PropertyToID("_baseIndex");
			if (_matBlock == null)
				return 0;

			Renderer[] renderers = creatureBoardAsset.Renderers;

			if (renderers == null)
				return 0;

			if (renderers.Length <= 0 || renderers[0] == null)
				return 0;

			renderers[0].GetPropertyBlock(_matBlock);

			return _matBlock.GetInt(BaseIndex);
		}

		public static Vector3 MoveCreatureTo(this CreatureBoardAsset asset, Vector3 dropPosition)
		{
			if (dropPosition.y < 0)
				dropPosition = new Vector3(dropPosition.x, 0, dropPosition.z);

			float dropOffset = 0;
			if (!asset.IsFlying)
			{
				dropPosition = Board.GetFloorPositionClosestTo(dropPosition);
				dropOffset = 0.5f;
			}

			asset.Drop(dropPosition, dropPosition.y + dropOffset);
			return dropPosition;
		}

		public static float GetBaseDiameterFeet(this CreatureBoardAsset asset)
		{
			const float BaseDiameterFactor = 4.5f;

			return BaseDiameterFactor * asset.CreatureScale;
		}

		public static void Knockdown(this CreatureBoardAsset asset)
		{
			ActionTimeline knockdownStatusEmote = Menus.GetKnockdownStatusEmote();
			if (knockdownStatusEmote == null)
			{
				Log.Error($"knockdownStatusEmote not found.");
				return;
			}
			bool isKnockedDown = asset.HasActiveEmote(knockdownStatusEmote);
			if (isKnockedDown)
			{
				Log.Debug($"Asset {asset.GetOnlyCreatureName()} is already knocked down!");
				return;
			}
			asset.Creature.ToggleStatusEmote(knockdownStatusEmote);
		}


		/// <summary>
		/// Removes the Knockdown status emote for creatures that are knocked down (and stands the mini upright).
		/// </summary>
		public static void StandUp(this CreatureBoardAsset asset)
		{
			ActionTimeline knockdownStatusEmote = Menus.GetKnockdownStatusEmote();
			if (knockdownStatusEmote == null)
			{
				Log.Error($"knockdownStatusEmote not found.");
				return;
			}
			bool isStanding = !asset.HasActiveEmote(knockdownStatusEmote);
			if (isStanding)
			{
				Log.Debug($"Asset {asset.GetOnlyCreatureName()} is already knocked up!");
				return;
			}
			asset.Creature.ToggleStatusEmote(knockdownStatusEmote);
		}

		public static bool HasActiveEmote(this CreatureBoardAsset asset, ActionTimeline actionTimeline)
		{
			CreatureDataV2 _mostRecentCreatureData = asset.GetRecentCreatureData();

			if (_mostRecentCreatureData.ActiveEmoteIds == null)
				return false;

			return _mostRecentCreatureData.ActiveEmoteIds.Contains(actionTimeline.ActionTimelineId);
		}

		public static CreatureDataV2 GetRecentCreatureData(this CreatureBoardAsset asset)
		{
			return (CreatureDataV2)ReflectionHelper.GetNonPublicFieldValue(asset.Creature, "_mostRecentCreatureData");
		}


		public static CreatureStat GetHp(this CreatureBoardAsset asset)
		{
			return asset.GetStats(-1);
		}

		public static CreatureStat GetStats(this CreatureBoardAsset asset, int statIndex)
		{
			if (!CreatureManager.TryGetCreatureData(asset.Creature.CreatureId, out CreatureDataV2 creatureData))
				return default;

			switch (statIndex)
			{
				case -1: return new CreatureStat(asset.Creature.Hp.Value, asset.Creature.Hp.Max);
				case 0: return creatureData.Stat0;
				case 1: return creatureData.Stat1;
				case 2: return creatureData.Stat2;
				case 3: return creatureData.Stat3;
				case 4: return creatureData.Stat4;
				case 5: return creatureData.Stat5;
				case 6: return creatureData.Stat6;
				case 7: return creatureData.Stat7;
			}

			Talespire.Log.Error($"statIndex {statIndex} out of range! Must be between -1 and 7!");

			return default;
		}

		public static void SetHp(this CreatureBoardAsset creature, float currentHp, float maxHp)
		{
			CreatureStat cs = new CreatureStat(currentHp, maxHp);
			CreatureManager.SetCreatureStatByIndex(creature.Creature.CreatureId, cs, -1);
		}

		public static void SetStat(this CreatureBoardAsset creature, float current, float max, int statIndex)
		{
			CreatureStat cs = new CreatureStat(current, max);
			CreatureManager.SetCreatureStatByIndex(creature.Creature.CreatureId, cs, statIndex);
		}

		public static float GetFlyingAltitude(this CreatureBoardAsset creature)
		{
			if (!creature.IsFlying || creature.FlyingIndicator == null)
				return 0;
			return creature.FlyingIndicator.ElevationAmount;
		}

		public static Dictionary<string, string> GetAttachedData(this CreatureBoardAsset asset)
		{
			string name = asset?.Creature?.Name;
			int sizeZeroIndex = -1;
			if (name != null)
				sizeZeroIndex = name.IndexOf(Talespire.PersistentEffects.STR_RichTextSizeZero);
			if (sizeZeroIndex >= 0)
			{
				string json = name.Substring(sizeZeroIndex + Talespire.PersistentEffects.STR_RichTextSizeZero.Length);
				return JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
			}
			else
				return new Dictionary<string, string>();
		}

		public static bool HasAttachedData(this CreatureBoardAsset asset, string key, bool logErrors = false)
		{
			if (logErrors)
				Talespire.Log.Debug($"Checking attached data for \"{asset.Creature.Name}\"...");

			int sizeZeroIndex = asset.Creature.Name.IndexOf(Talespire.PersistentEffects.STR_RichTextSizeZero);
			if (sizeZeroIndex >= 0)
			{
				string json = asset.Creature.Name.Substring(sizeZeroIndex + Talespire.PersistentEffects.STR_RichTextSizeZero.Length);
				Dictionary<string, string> state = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
				if (state == null)
				{
					if (logErrors)
						Talespire.Log.Error($"Deserialization failed!");
					return false;
				}

				return state.ContainsKey(key);
			}
			else
			{
				if (logErrors)
					Talespire.Log.Error($"\"{Talespire.PersistentEffects.STR_RichTextSizeZero}\" marker not found in name!");
				return false;
			}
		}

		public static Vector3 GetVector3(this VectorDto vectorDto)
		{
			return new Vector3(vectorDto.x, vectorDto.y, vectorDto.z);
		}

		public static VectorDto GetVectorDto(this Vector3 vector)
		{
			if (vector == null)
				return null;
			return new VectorDto(vector.x, vector.y, vector.z);
		}

		public static List<Transform> GetHandles(this LineRulerMode LineRulerMode)
		{
			return ReflectionHelper.GetNonPublicField<List<Transform>>(LineRulerMode, "_handles");
		}

		public static List<Vector3> GetPositions(this LineRulerMode LineRulerMode)
		{
			return LineRulerMode.GetHandles()?.ConvertAll(x => x.position);
		}

		public static MonoBehaviour GetScript(this GameObject gameObject, string scriptName)
		{
			Component[] components = gameObject.GetComponents(typeof(MonoBehaviour));
			foreach (Component component in components)
				if (component.GetType().Name == scriptName)
					return component as MonoBehaviour;
			return null;
		}

		public static List<MonoBehaviour> GetScriptsInChildren(this GameObject gameObject, string scriptName)
		{
			List<MonoBehaviour> result = new List<MonoBehaviour>();
			Component[] components = gameObject.GetComponentsInChildren(typeof(MonoBehaviour));

			foreach (Component component in components)
				if (component?.GetType().Name == scriptName)
					if (component is MonoBehaviour monoBehaviour)
						result.Add(monoBehaviour);

			return result;
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

		/// <summary>
		/// Returns the AssetLoader GameObject for the specified creature.
		/// </summary>
		public static GameObject GetAssetLoader(this CreatureBoardAsset creatureAsset)
		{
			// ![](907E4122717C4D32C3EC6832C50B5A85.png;;13,213,283,490)
			GameObject _Rotator = creatureAsset.BaseLoader.gameObject?.transform?.parent?.parent?.gameObject;
			return _Rotator?.FindChild("AssetLoader");
		}

		/// <summary>
		/// Gets the "Attached" GameObject holding persistent effects, if any on the specified creature.
		/// </summary>
		public static GameObject GetAttachedParentGameObject(this CreatureBoardAsset creatureBoardAsset)
		{
			GameObject attachedNode = null;
			GameObject assetLoader = creatureBoardAsset.GetAssetLoader();
			if (assetLoader != null)
				attachedNode = assetLoader.FindChild(Talespire.PersistentEffects.STR_AttachedNode, true);
			return attachedNode;
		}

		public static bool IsPersistentEffect(this CreatureBoardAsset creatureBoardAsset)
		{
			return creatureBoardAsset.HasAttachedData(Talespire.PersistentEffects.STR_PersistentEffect);
		}

		public static bool MatchesAnyLower(this List<string> names, string lowerCaseName)
		{
			return names.Any(x => lowerCaseName.Contains(x));
		}

		public static string GetOnlyCreatureName(this CreatureBoardAsset creatureAsset)
		{
			if (creatureAsset == null)
				return string.Empty;

			if (creatureAsset.Creature == null)
				return string.Empty;

			string name = creatureAsset.Creature.Name;

			if (name == null && creatureAsset.CreatureLoaders != null && creatureAsset.CreatureLoaders.Length > 0)
				name = creatureAsset.CreatureLoaders[0].LoadedAsset.name;

			if (name == null)
				return string.Empty;

			int sizeIndex = name.IndexOf(Talespire.PersistentEffects.STR_RichTextSizeZero);
			return sizeIndex > 0 ? name.Substring(0, sizeIndex).Trim() : name;
		}

		public static string GetPersistentEffectData(this CreatureBoardAsset creatureAsset)
		{
			Dictionary<string, string> dictionaries = creatureAsset.GetAttachedData();

			if (dictionaries == null)
				return null;

			if (dictionaries.ContainsKey(Talespire.PersistentEffects.STR_PersistentEffect))
				return dictionaries[Talespire.PersistentEffects.STR_PersistentEffect];
			return null;
		}

		public static IOldPersistentEffect GetPersistentEffect(this CreatureBoardAsset creatureAsset)
		{
			Dictionary<string, string> dictionaries = creatureAsset.GetAttachedData();

			if (dictionaries == null)
				return null;

			IOldPersistentEffect persistentEffect = null;

			if (dictionaries.ContainsKey(Talespire.PersistentEffects.STR_PersistentEffect))
			{
				string effectData = dictionaries[Talespire.PersistentEffects.STR_PersistentEffect];
				try
				{
					persistentEffect = JsonConvert.DeserializeObject<SuperPersistentEffect>(effectData);
					if (persistentEffect != null)
						if (string.IsNullOrWhiteSpace(persistentEffect.EffectName))
							persistentEffect = ConvertOldToNewPersistentEffect(effectData);
				}
				catch (Exception ex)
				{
					persistentEffect = null;
				}

				if (persistentEffect == null)
				{
					persistentEffect = ConvertOldToNewPersistentEffect(effectData);
				}

				if (persistentEffect == null)
					persistentEffect = new OldPersistentEffect() { EffectName = effectData };
			}

			return persistentEffect;

		}

		private static IOldPersistentEffect ConvertOldToNewPersistentEffect(string effectData)
		{
			try
			{
				IOldPersistentEffect oldPersistentEffect = JsonConvert.DeserializeObject<OldPersistentEffect>(effectData);
				if (oldPersistentEffect != null)
					return new SuperPersistentEffect(oldPersistentEffect);
			}
			catch (Exception ex)
			{
				return null;
			}

			return null;
		}

		public static void SavePersistentEffect(this CreatureBoardAsset creatureAsset, IOldPersistentEffect persistentEffect)
		{
			Talespire.Log.Indent();
			Talespire.Log.Warning($"Saving Persistent Effect....");
			string newEffectData = JsonConvert.SerializeObject(persistentEffect);
			if (newEffectData.Contains(",'BaseIndex"))
			{
				Talespire.Log.Error($"Error - expecting ScriptData to have escaped slashes\\\\!");
				if (persistentEffect is SuperPersistentEffect superPersistentEffect)
				{
					foreach (string key in superPersistentEffect.ScriptData.Keys)
						Talespire.Log.Warning($"  ScriptData[{key}] == \"{superPersistentEffect.ScriptData[key]}\"");
				}
			}
			Talespire.Log.Debug($"newEffectData: {newEffectData}");
			
			StatMessaging.SetInfoNoGuard(creatureAsset.CreatureId, Talespire.PersistentEffects.STR_PersistentEffect, newEffectData);

			Talespire.Log.Unindent();
		}

		public static GameObject GetChildNodeStartingWith(this GameObject gameObject, string prefix, bool includeInactive = false)
		{
			Transform[] childTransforms = gameObject.GetComponentsInChildren<Transform>(includeInactive);
			RectTransform[] rectTransforms = gameObject.GetComponentsInChildren<RectTransform>(includeInactive);

			if (childTransforms != null)
				foreach (Transform transform in childTransforms)
				{
					GameObject child = transform.gameObject;
					if (child != null && child.name.StartsWith(prefix))
						return child;
				}

			if (rectTransforms != null)
				foreach (Transform transform in rectTransforms)
				{
					GameObject child = transform.gameObject;
					if (child != null && child.name.StartsWith(prefix))
						return child;
				}

			return null;
		}



		public static GameObject FindChild(this GameObject gameObject, string name, bool includeInactive = false)
		{
			Transform[] childTransforms = gameObject.GetComponentsInChildren<Transform>(includeInactive);
			RectTransform[] rectTransforms = gameObject.GetComponentsInChildren<RectTransform>(includeInactive);

			if (childTransforms != null)
				foreach (Transform transform in childTransforms)
				{
					GameObject child = transform.gameObject;
					if (child?.name == name)
						return child;
				}

			if (rectTransforms != null)
				foreach (Transform transform in rectTransforms)
				{
					GameObject child = transform.gameObject;
					if (child?.name == name)
						return child;
				}

			return null;
		}

		public static GameObject GetChild(this Transform transform, string nameToFind, bool includeInactive = false)
		{
			Transform[] children = transform.GetComponentsInChildren<Transform>(includeInactive);
			foreach (Transform child in children)
				if (child.gameObject.name == nameToFind)
					return child.gameObject;

			return null;
		}

		public static bool HasChild(this Transform transform, string nameToFind, bool includeInactive = false)
		{
			Transform[] children = transform.GetComponentsInChildren<Transform>(includeInactive);
			foreach (Transform child in children)
				if (child.gameObject.name == nameToFind)
					return true;

			return false;
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
				ID = creatureAsset.Creature?.CreatureId.Value.ToString(),
				FlyingAltitude = altitude
			};
			return characterPosition;
		}

		public static VectorDto GetPositionVectorDto(this CreatureBoardAsset creatureAsset)
		{
			return new VectorDto(creatureAsset.PlacedPosition.x, creatureAsset.PlacedPosition.y, creatureAsset.PlacedPosition.z);
		}

		public static GameObject GetBase(this CreatureBoardAsset creatureBoardAsset)
		{
			GameObject baseGameObject;
			if (creatureBoardAsset.IsFlying)
				baseGameObject = creatureBoardAsset.FlyingIndicator.gameObject;
			else
				baseGameObject = creatureBoardAsset.BaseLoader.LoadedAsset;
			return baseGameObject;
		}

		public static float GetRotationDegrees(this CreatureBoardAsset creatureBoardAsset)
		{
			return creatureBoardAsset.Rotator.localEulerAngles.z;
		}

		public static void SetRotationDegrees(this CreatureBoardAsset creatureBoardAsset, float rotationDegrees)
		{
			creatureBoardAsset.Rotator.localEulerAngles = new Vector3(0, 0, rotationDegrees);
		}

		public static Vector3 GetRotation(this CreatureBoardAsset creatureBoardAsset)
		{
			return creatureBoardAsset.Rotator.localEulerAngles;
		}

		public static void SetRotation(this CreatureBoardAsset creatureBoardAsset, Vector3 angle)
		{
			creatureBoardAsset.Rotator.localEulerAngles = angle;
		}

		public static void LookAt(this CreatureBoardAsset creatureBoardAsset, Vector3 position)
		{
			creatureBoardAsset.Creature.transform.LookAt(position);
		}

		public static void RotateTowards(this CreatureBoardAsset creature, Vector3 targetPosition, float multiplier = 15f)
		{
			float correctAngle = creature.GetRotationAngleToPosition(targetPosition);
			if (correctAngle == 0)
				return;

			float zAngle = correctAngle * Mathf.Min(Time.deltaTime, 0.08f) * multiplier;
			creature.Rotator.Rotate(0f, 0f, zAngle, Space.Self);
		}

		public static void RotateTowardsNow(this CreatureBoardAsset creature, Vector3 targetPosition)
		{
			float correctAngle = creature.GetRotationAngleToPosition(targetPosition);
			if (correctAngle == 0)
				return;

			creature.Rotator.Rotate(0f, 0f, correctAngle, Space.Self);
		}

		private static float GetRotationAngleToPosition(this CreatureBoardAsset creature, Vector3 targetPosition)
		{
			Vector3 targetFloor = new Vector3(targetPosition.x, 0f, targetPosition.z);
			Vector3 creaturePosition = creature.transform.position;
			Vector3 creaturePositionFloor = new Vector3(creaturePosition.x, 0f, creaturePosition.z);
			Vector3 to = -(creaturePositionFloor - targetFloor).normalized;
			float angle = Vector3.Angle(new Vector3(-creature.Rotator.right.x, 0f, -creature.Rotator.right.z), to);
			float dotProduct = Vector3.Dot(-creature.Rotator.up, to);

			float correctAngle;

			if (Vector3.Distance(creaturePositionFloor, targetFloor) <= 0.0002f)
				correctAngle = 0;
			else if (dotProduct < 0f)
				correctAngle = -angle;
			else
				correctAngle = angle;
			return correctAngle;
		}
	}
}