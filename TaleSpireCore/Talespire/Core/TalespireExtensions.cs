using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

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
				if (component.GetType().Name == scriptName)
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
	}
}