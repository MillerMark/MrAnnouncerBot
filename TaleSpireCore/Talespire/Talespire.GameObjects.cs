using System;
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

			public static GameObject Get(string name)
			{
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
						System.Windows.Forms.MessageBox.Show(ex.Message, ex.GetType().Name);
					}

					return results;
				}
				return allFoundGameObjects.Keys.ToList();
			}

			public static void InvalidateFound()
			{
				allFoundGameObjects = null;
			}

			public static void Refresh()
			{
				allFoundGameObjects = new Dictionary<string, GameObject>();
				Transform[] allTransforms = UnityEngine.Object.FindObjectsOfType<Transform>();
				if (allTransforms != null)
					foreach (Transform transform in allTransforms)
					{
						GameObject gameObject = transform.gameObject;
						if (gameObject != null)
							if (!allFoundGameObjects.ContainsKey(gameObject.name))
								allFoundGameObjects.Add(gameObject.name, gameObject);
					}
			}
		}
	}
}