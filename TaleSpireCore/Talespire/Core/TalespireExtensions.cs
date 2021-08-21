using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using LordAshes;

namespace TaleSpireCore
{
	public static class TalespireExtensions
	{
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

		public static List<Transform> GetHandles(this LineRulerIndicator lineRulerIndicator)
		{
			return ReflectionHelper.GetNonPublicField<List<Transform>>(lineRulerIndicator, "_handles");
		}

		public static List<Vector3> GetPositions(this LineRulerIndicator lineRulerIndicator)
		{
			return lineRulerIndicator.GetHandles()?.ConvertAll(x => x.position);
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

		public static bool IsPersistentEffect(this CreatureBoardAsset creatureBoardAsset)
		{
			return creatureBoardAsset.HasAttachedData(Talespire.PersistentEffects.STR_PersistentEffect);
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
			Vector3 targetFloor = new Vector3(targetPosition.x, 0f, targetPosition.z);
			Vector3 creaturePosition = creature.transform.position;
			Vector3 creaturePositionFloor = new Vector3(creaturePosition.x, 0f, creaturePosition.z);
			Vector3 to = -(creaturePositionFloor - targetFloor).normalized;
			float angle = Vector3.Angle(new Vector3(-creature.Rotator.right.x, 0f, -creature.Rotator.right.z), to);
			float dotProduct = Vector3.Dot(-creature.Rotator.up, to);

			if (Vector3.Distance(creaturePositionFloor, targetFloor) <= 0.0002f)
				return;

			float correctAngle;
			if (dotProduct < 0f)
				correctAngle = -angle;
			else
				correctAngle = angle;
			float zAngle = correctAngle * Mathf.Min(Time.deltaTime, 0.08f) * multiplier;
			creature.Rotator.Rotate(0f, 0f, zAngle, Space.Self);
		}
	}
}