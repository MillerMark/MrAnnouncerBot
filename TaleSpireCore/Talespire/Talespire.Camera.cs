using System;
using System.Linq;
using UnityEngine;
using static TaleSpireCore.Talespire;

namespace TaleSpireCore
{
	public static partial class Talespire
	{
		public static class Camera
		{
			static Transform saveCameraTransform;
			static GameObject spinner;
			static Transform saveParent;
			static Vector3 saveEulerAngles;
			public static CreatureBoardAsset LookAt(string id)
			{
				CreatureBoardAsset creatureBoardAsset = Minis.GetCreatureBoardAsset(id);
				if (creatureBoardAsset != null)
				{
					CreatureGuid creatureGuid = new CreatureGuid(creatureBoardAsset.WorldId);
					CameraController.LookAtCreature(creatureGuid);
				}

				return creatureBoardAsset;
			}

			public static void RestoreCamera()
			{
				if (saveParent != null)
				{
					Vector3 position = saveCameraTransform.position;
					
					saveCameraTransform.SetParent(saveParent);

					if (saveCameraTransform.localEulerAngles == saveEulerAngles)
						Log.Warning($"saveCamera.gameObject.transform.eulerAngles == saveEulerAngles");

					saveCameraTransform.position = position;

					CreatureBoardAsset creatureBoardAsset = Minis.GetCreatureBoardAsset(targetWorldId);
					if (creatureBoardAsset != null)
					{
						GameObject baseGO = creatureBoardAsset.GetBase();
						if (baseGO != null)
							CameraController.CameraTransform.LookAt(baseGO.transform);
					}

					saveCameraTransform.localEulerAngles = saveEulerAngles;

					saveParent = null;
					saveCameraTransform = null;
					UnityEngine.Object.Destroy(spinner);
					spinner = null;
				}
			}

			public static CreatureBoardAsset SpinAround(string id)
			{
				if (saveCameraTransform != null)
					RestoreCamera();

				CreatureBoardAsset targetCreatureBoardAsset = Minis.GetCreatureBoardAsset(id);
				if (targetCreatureBoardAsset == null)
					return null;

				targetCreatureGuid = new CreatureGuid(targetCreatureBoardAsset.WorldId);
				targetWorldId = targetCreatureBoardAsset.WorldId.ToString();
				CameraController.LookAtCreature(targetCreatureGuid);

				saveCameraTransform = GetRoot()?.transform;
				saveEulerAngles = saveCameraTransform.localEulerAngles;
				//saveLocalPosition = saveCameraTransform.localPosition;

				Vector3 delta = saveCameraTransform.position - targetCreatureBoardAsset.transform.position;

				spinner = new GameObject();
				spinner.AddComponent<Scripts.CameraSpinner>();

				saveParent = saveCameraTransform.parent;

				spinner.transform.position = targetCreatureBoardAsset.transform.position;
				Scripts.CameraSpinner tSC_Spinner = spinner.GetComponent<Scripts.CameraSpinner>();
				if (tSC_Spinner != null)
					tSC_Spinner.TargetSpinRate = 11;

				saveCameraTransform.SetParent(spinner.transform);
				saveCameraTransform.localPosition = delta;

				return targetCreatureBoardAsset;
			}

			public static GameObject GetRoot()
			{
				return GameObjects.Get("CameraRoot");
			}

			static float lastX;
			static float lastY;
			static float lastZ;
			public static void ShowPosition()
			{
				Vector3 position;
				string label;
				if (saveCameraTransform != null)
				{
					position = saveCameraTransform.position;
					label = "saveCamera";
				}
				else
				{
					if (CameraController.CameraTransform == null)
						return;

					position = CameraController.CameraTransform.position;
					label = "CameraController";
				}

				if (position == null)
					return;

				float zoomTransition = (float)Math.Round(CameraController.GetZoomTransition(), 4);
				float zoomLerpValue = (float)Math.Round(CameraController.ZoomLerpValue, 4);

				if (lastX == position.x && lastY == position.y && lastZ == position.z &&
					lastZoomLerpValue == zoomLerpValue && lastZoomTransition == zoomTransition)
					return;

				float x = (float)Math.Round(position.x, 2);
				float y = (float)Math.Round(position.y, 2);
				float z = (float)Math.Round(position.z, 2);

				Log.Debug($"{label} - ({x:N}, {y:N}, {z:N}) - Zoom value = {zoomLerpValue} / transition = {zoomTransition}");
				lastX = position.x;
				lastY = position.y;
				lastZ = position.z;
				lastZoomLerpValue = zoomLerpValue;
				lastZoomTransition = zoomTransition;
			}

			//static Vector3 saveCameraPosition;
			//static Quaternion saveCameraRotation;
			static float lastZoomLerpValue;
			static float lastZoomTransition;
			static CreatureGuid targetCreatureGuid;
			static string targetWorldId;
			

			//public static void SavePosition()
			//{
			//	saveCameraPosition = CameraController.CameraTransform.position;
			//	saveCameraRotation = CameraController.CameraTransform.rotation;
			//}

			public static void SetPosition(string position)
			{
				string[] split = position.Split(',');
				if (split.Length != 3)
				{
					Log.Error($"Unable to parse \"{position}\" into vector.");
					return;
				}
				if (float.TryParse(split[0], out float x))
					if (float.TryParse(split[1], out float y))
						if (float.TryParse(split[2], out float z))
						{
							Vector3 newPosition = new Vector3(x, y, z);
							CameraController.MoveToPosition(newPosition, true);
						}
			}

			public static void SetCameraHeight(string heightStr)
			{
				if (float.TryParse(heightStr, out float height))
				{
					//CameraController.CameraHeight = height;
					CameraController.MoveToHeight(height, true);
				}
				else
					Log.Error($"Unable to parse \"{heightStr}\" into float.");
			}
		}
	}
}