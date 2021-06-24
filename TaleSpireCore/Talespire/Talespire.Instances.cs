using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using UnityEngine;

namespace TaleSpireCore
{
	public static partial class Talespire
	{
		public static class Instances
		{
			static Timer mortalityCheckTimer;
			static List<MortalGameObject> MortalGameObjects { get; set; } = new List<MortalGameObject>();
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

				Add(instanceId, asset);
				UnityEngine.Object.DontDestroyOnLoad(asset);
				return asset as T;
			}

			static void MakeSureTimerIsRunning()
			{
				if (mortalityCheckTimer != null)
					return;

				mortalityCheckTimer = new Timer();
				mortalityCheckTimer.Interval = 1000;  // Once a second.
				mortalityCheckTimer.Elapsed += MortalityCheckTimer_Elapsed;
				mortalityCheckTimer.Start();
			}

			private static void MortalityCheckTimer_Elapsed(object sender, ElapsedEventArgs e)
			{
				List<GameObject> gameObjectsToDestroy = new List<GameObject>();
				for (int i = MortalGameObjects.Count - 1; i >= 0; i--)
					if (MortalGameObjects[i].ExpireTime <= DateTime.Now)
					{
						gameObjectsToDestroy.Add(MortalGameObjects[i].GameObject);
						MortalGameObjects.RemoveAt(i);
					}

				if (MortalGameObjects.Count == 0 && mortalityCheckTimer != null)
				{
					mortalityCheckTimer.Stop();
					mortalityCheckTimer = null;
				}

				UnityMainThreadDispatcher.ExecuteOnMainThread(() =>
				{
					foreach (GameObject gameObject in gameObjectsToDestroy)
						GameObject.Destroy(gameObject);
				});
			}

			public static void AddTemporal(GameObject instance, double lifetimeSeconds)
			{
				MortalGameObjects.Add(new MortalGameObject(instance, DateTime.Now + TimeSpan.FromSeconds(lifetimeSeconds)));
				MakeSureTimerIsRunning();
			}
		}
	}
}