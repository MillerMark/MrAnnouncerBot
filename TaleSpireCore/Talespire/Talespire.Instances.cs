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
			static Dictionary<string, GameObject> spells = new Dictionary<string, GameObject>();

			public static void Delete(string instanceId)
			{
				DeleteFrom(instanceId, instances);
			}

			public static void DeleteSpell(string spellId)
			{
				DeleteFrom(spellId, spells);
			}

			public static void DeleteSpellSoon(string spellId)
			{
				if (!spells.ContainsKey(spellId))
					return;
				AddTemporal(spells[spellId], 2, 2);
				spells.Remove(spellId);
			}

			private static void DeleteFrom(string instanceId, Dictionary<string, GameObject> instances)
			{
				if (!instances.ContainsKey(instanceId))
					return;
				UnityEngine.Object.Destroy(instances[instanceId]);
				instances.Remove(instanceId);
			}

			public static void Add(string instanceId, GameObject gameObject)
			{
				Delete(instanceId);  // Only allow one object with instanceId at a time.
				instances.Add(instanceId, gameObject);
			}

			public static void AddSpell(string spellId, GameObject gameObject)
			{
				DeleteSpell(spellId);  // Only allow one object with instanceId at a time.
				spells.Add(spellId, gameObject);
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

			static void TurnOffAllParticleSystems(GameObject gameObject)
			{
				ParticleSystem[] particleSystems = gameObject.GetComponentsInChildren<ParticleSystem>();
				foreach (ParticleSystem particleSystem in particleSystems)
				{
					ParticleSystem.EmissionModule emission = particleSystem.emission;
					emission.enabled = false;
				}
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
					else if (MortalGameObjects[i].ParticleShutoffTime <= DateTime.Now)
					{
						TurnOffAllParticleSystems(MortalGameObjects[i].GameObject);
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

			public static void AddTemporal(GameObject instance, double lifetimeSeconds, double particleShutoffTimeSeconds = 0)
			{
				DateTime expireTime = DateTime.Now + TimeSpan.FromSeconds(lifetimeSeconds);
				MortalGameObjects.Add(new MortalGameObject(instance, expireTime, expireTime - TimeSpan.FromSeconds(particleShutoffTimeSeconds)));
				MakeSureTimerIsRunning();
			}
		}
	}
}