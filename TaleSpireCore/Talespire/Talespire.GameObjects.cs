﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TaleSpireCore
{
	public static partial class Talespire
	{
		public static class GameObjects
		{
			static Dictionary<string, GameObject> allFoundGameObjects;
			static Dictionary<GameObject, string> allFoundGameObjectNames;

			public static GameObject Get(string name)
			{
				if (name == "$Camera$")
					return CameraController.GetCamera().gameObject;
				if (allFoundGameObjects == null)
					Refresh();
				if (allFoundGameObjects.TryGetValue(name, out GameObject gameObject))
					return gameObject;
				return null;
			}

			public static GameObject Clone(string name, string instanceId = null)
			{
				GameObject originalName = Get(name);
				if (originalName == null)
					return null;

				GameObject original = UnityEngine.Object.Instantiate(originalName);
				if (original == null)
					return null;

				if (instanceId != null)
					Instances.Add(instanceId, original);

				UnityEngine.Object.DontDestroyOnLoad(original);
				return original;
			}

			public static List<GameObject> GetAll()
			{
				if (allFoundGameObjects == null)
					Refresh();
				return allFoundGameObjects.Values.ToList();
			}

			public static List<string> GetAllNames(bool topLevelOnly = false)
			{
				if (allFoundGameObjects == null)
					Refresh();
				if (topLevelOnly)
				{
					List<string> results = new List<string>();

					try
					{
						foreach (string key in allFoundGameObjects.Keys)
						{
							GameObject gameObject = allFoundGameObjects[key];
							if (gameObject.transform?.parent == null)
								results.Add(gameObject.name);
							else if (gameObject.name == "Sphere" || gameObject.name == "SphereGraphic")
							{
								while (gameObject != null && gameObject.transform?.parent != null)
								{
									Log.Warning($"{gameObject.name} is parented by: {gameObject.transform.parent.name}");
									gameObject = gameObject.transform?.parent?.gameObject;
								}
							}
						}
					}
					catch (Exception ex)
					{
						Talespire.Log.Exception(ex);
						System.Windows.Forms.MessageBox.Show(ex.Message, ex.GetType().Name);
					}

					return results;
				}
				return allFoundGameObjects.Keys.ToList();
			}

			public static void InvalidateFound()
			{
				allFoundGameObjects = null;
				allFoundGameObjectNames = null;
			}

			public static void Refresh()
			{
				allFoundGameObjects = new Dictionary<string, GameObject>();
				allFoundGameObjectNames = new Dictionary<GameObject, string>();
				GameObject[] allGameObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
				Transform[] allTransforms = UnityEngine.Object.FindObjectsOfType<Transform>();
				if (allTransforms != null)
					foreach (Transform transform in allTransforms)
						AddGameObject(transform.gameObject);

				if (allGameObjects != null)
					foreach (GameObject gameObject in allGameObjects)
						AddGameObject(gameObject);
			}

			private static void AddGameObject(GameObject gameObject)
			{
				int index = 1;
				string indexStr = "";
				if (gameObject != null && !allFoundGameObjectNames.ContainsKey(gameObject))
				{
					while (allFoundGameObjects.ContainsKey(gameObject.name + indexStr))
					{
						index++;
						indexStr = "." + index.ToString();
					}
					string key = gameObject.name + indexStr;
					allFoundGameObjects.Add(key, gameObject);
					allFoundGameObjectNames.Add(gameObject, key);

					//"5d3e85d0-e2e8-436a-9c16-f35a493bc854"
					//PropPreviewBoardAsset.Spawn(boardAssetGuid, position, rotation)
					//new PropPreviewBoardAsset().AssetId; /*  */
					//TsAssetResources - where is this?
				}
			}

			public static GameObject TryAddProp()
			{
				// "Bottle_TallRoundCorkTop_lp" - "5d3e85d0-e2e8-436a-9c16-f35a493bc854"
				PropPreviewBoardAsset spawn = PropPreviewBoardAsset.Spawn(new Bounce.Unmanaged.NGuid("5d3e85d0-e2e8-436a-9c16-f35a493bc854"), Vector3.zero, Quaternion.identity);
				if (spawn != null)
				{
					spawn.name = "_Added Prop Bottle";
					spawn.gameObject.name = spawn.name;
				}
				return spawn.gameObject;
			}
		}
	}
}