using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TaleSpireCore
{
	public static partial class Talespire
	{
		public static class Instances
		{
			static Dictionary<string, GameObject> instances = new Dictionary<string, GameObject>();

			public static void Delete(string instanceId)
			{
				if (!instances.ContainsKey(instanceId))
					return;
				UnityEngine.Object.Destroy(instances[instanceId]);
				instances.Remove(instanceId);
			}

			public static void Add(string instanceId, GameObject prefab)
			{
				Delete(instanceId);  // Only allow one object with instanceId at a time.
				instances.Add(instanceId, prefab);
			}

			public static T Create<T>(string instanceId, GameObject originalAsset) where T : class
			{
				GameObject asset = UnityEngine.Object.Instantiate(originalAsset);
				if (asset == null)
					return default(T);

				Talespire.Instances.Add(instanceId, asset);
				UnityEngine.Object.DontDestroyOnLoad(asset);
				return asset as T;
			}
		}
	}
}