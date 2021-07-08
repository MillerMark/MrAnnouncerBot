using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TaleSpireCore
{
	public static partial class Talespire
	{
		public static class Prefabs
		{
			static Dictionary<string, GameObject> prefabs = new Dictionary<string, GameObject>();

			public static GameObject Get(string prefabName)
			{
				if (Has(prefabName))
					return prefabs[prefabName];
				return null;
			}

			public static bool Has(string prefabName)
			{
				return prefabs.ContainsKey(prefabName);
			}

			public static void Add(GameObject prefab)
			{
				if (prefab == null)
				{
					Log.Error($"prefab in call to Talespire.Prefabs.Add is null!");
					return;
				}

				if (!prefabs.ContainsKey(prefab.name))
					prefabs.Add(prefab.name, prefab);
				else
					Log.Error($"prefabs Dictionary already contains a key named \"{prefab.name}\"");
			}

			public static GameObject Clone(string prefabName, string instanceId = null)
			{
				try
				{
					GameObject original = Get(prefabName);
					if (original == null)
						return null;

					Log.Debug($"GameObject prefab = UnityEngine.Object.Instantiate(original);");
					GameObject prefab = UnityEngine.Object.Instantiate(original);
					if (prefab == null)
						return null;

					if (instanceId != null)
						Instances.Add(instanceId, prefab);

					//Log.Debug($"UnityEngine.Object.DontDestroyOnLoad(prefab);");
					//UnityEngine.Object.DontDestroyOnLoad(prefab);
					return prefab;
				}
				catch (Exception ex)
				{
					Log.Exception(ex);
					return null;
				}
			}

			public static List<string> AllNames
			{
				get => prefabs.Keys.ToList();
			}
		}
	}
}