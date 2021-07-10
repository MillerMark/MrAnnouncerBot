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
			//static Timer mortalityCheckTimer;
			static List<MortalGameObject> MortalGameObjects { get; set; } = new List<MortalGameObject>();
			static Dictionary<string, List<GameObject>> instances = new Dictionary<string, List<GameObject>>();
			static Dictionary<string, List<GameObject>> spells = new Dictionary<string, List<GameObject>>();

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

				foreach (GameObject spellEffect in spells[spellId])
					AddTemporal(spellEffect, 2, 2, 1);

				spells.Remove(spellId);
			}

			private static void DeleteFrom(string instanceId, Dictionary<string, List<GameObject>> instances)
			{
				if (!instances.ContainsKey(instanceId))
					return;

				foreach (GameObject effect in instances[instanceId])
				{
					DestroyEffect(effect);
				}

				instances.Remove(instanceId);
			}

			public static void Add(string instanceId, GameObject gameObject)
			{
				AddTo(instances, instanceId, gameObject);
			}

			private static void AddTo(Dictionary<string, List<GameObject>> instances, string instanceId, GameObject gameObject)
			{
				if (!instances.ContainsKey(instanceId))
					instances.Add(instanceId, new List<GameObject>());

				instances[instanceId].Add(gameObject);
			}

			public static void AddSpell(string spellId, GameObject gameObject, float enlargeTimeSeconds = 0)
			{
				EnlargeSoon(gameObject, enlargeTimeSeconds);
				AddTo(spells, spellId, gameObject);
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

			//static void MakeSureTimerIsRunning()
			//{
			//	if (mortalityCheckTimer != null)
			//		return;

			//	mortalityCheckTimer = new Timer();
			//	mortalityCheckTimer.Interval = 1000;  // Once a second.
			//	mortalityCheckTimer.Elapsed += MortalityCheckTimer_Elapsed;
			//	mortalityCheckTimer.Start();
			//}

			static void TurnOffAllParticleSystems(GameObject gameObject)
			{
				if (gameObject == null || !gameObject.activeInHierarchy)
					return;

				ParticleSystem[] particleSystems = gameObject.GetComponentsInChildren<ParticleSystem>();
				foreach (ParticleSystem particleSystem in particleSystems)
				{
					ParticleSystem.EmissionModule emission = particleSystem.emission;
					emission.enabled = false;
				}
			}

			//private static void MortalityCheckTimer_Elapsed(object sender, ElapsedEventArgs e)
			//{
			//	List<GameObject> gameObjectsToDestroy = new List<GameObject>();
			//	DateTime now = DateTime.Now;
			//	for (int i = MortalGameObjects.Count - 1; i >= 0; i--)
			//		if (MortalGameObjects[i].ExpireTime <= now)
			//		{
			//			gameObjectsToDestroy.Add(MortalGameObjects[i].GameObject);
			//			MortalGameObjects.RemoveAt(i);
			//		}
			//		else if (MortalGameObjects[i].ParticleShutoffTime <= now)
			//		{
			//			TurnOffAllParticleSystems(MortalGameObjects[i].GameObject);
			//		}

			//	if (MortalGameObjects.Count == 0 && mortalityCheckTimer != null)
			//	{
			//		mortalityCheckTimer.Stop();
			//		mortalityCheckTimer = null;
			//	}

			//	UnityMainThreadDispatcher.ExecuteOnMainThread(() =>
			//	{
			//		foreach (GameObject gameObject in gameObjectsToDestroy)
			//			GameObject.Destroy(gameObject);
			//	});
			//}

			static Dictionary<GameObject, EnlargeData> enlargingObjects = new Dictionary<GameObject, EnlargeData>();

			public static void AddTemporal(GameObject instance, double lifetimeSeconds, double particleShutoffTimeSeconds = 0, float shrinkOnDeleteTime = 0, float enlargeTimeSeconds = 0)
			{
				EnlargeSoon(instance, enlargeTimeSeconds);

				DateTime expireTime = DateTime.Now + TimeSpan.FromSeconds(lifetimeSeconds);
				MortalGameObjects.Add(new MortalGameObject(instance, expireTime, expireTime - TimeSpan.FromSeconds(particleShutoffTimeSeconds), shrinkOnDeleteTime));
				//MakeSureTimerIsRunning();
			}

			public static void EnlargeSoon(GameObject instance, float enlargeTimeSeconds)
			{
				if (enlargeTimeSeconds <= 0)
					return;

				if (instance == null || instance.transform == null || !instance.activeInHierarchy)
					return;

				enlargingObjects.Add(instance, new EnlargeData()
				{
					CreationTime = Time.time,
					EnlargeTimeSeconds = enlargeTimeSeconds,
					OriginalScale = instance.transform.localScale
				});

				instance.transform.localScale = Vector3.zero;
			}

			public static void Update()
			{
				List<GameObject> gameObjectsToDestroy = CleanUpAndScaleShrinkingObjects();

				EnlargeGrowingObjects();

				DestroyAll(gameObjectsToDestroy);
			}

			private static void DestroyAll(List<GameObject> gameObjectsToDestroy)
			{
				if (gameObjectsToDestroy != null)
					foreach (GameObject gameObject in gameObjectsToDestroy)
						DestroyEffect(gameObject);
			}

			private static List<GameObject> CleanUpAndScaleShrinkingObjects()
			{
				DateTime now = DateTime.Now;

				List<GameObject> gameObjectsToDestroy = null;

				for (int i = MortalGameObjects.Count - 1; i >= 0; i--)
				{
					MortalGameObject mortal = MortalGameObjects[i];
					GameObject gameObject = mortal.GameObject;

					if (mortal.ExpireTime <= now)
					{
						if (gameObjectsToDestroy == null)
							gameObjectsToDestroy = new List<GameObject>();
						gameObjectsToDestroy.Add(gameObject);
						MortalGameObjects.RemoveAt(i);
					}
					else
					{
						float secondsToExpiration = (float)(mortal.ExpireTime - now).TotalSeconds;
						if (secondsToExpiration < mortal.ShrinkOnDeleteTimeSeconds)
							mortal.ScaleToExpire(secondsToExpiration);

						if (!mortal.ParticlesShutDown && mortal.ParticleShutoffTime <= now)
						{
							TurnOffAllParticleSystems(gameObject);
							mortal.ParticlesShutDown = true;
						}
					}
				}

				return gameObjectsToDestroy;
			}

			private static void EnlargeGrowingObjects()
			{
				if (enlargingObjects.Count > 0)
				{
					float time = Time.time;
					List<GameObject> totallyEnlarged = new List<GameObject>();
					foreach (GameObject gameObject in enlargingObjects.Keys)
					{
						EnlargeData enlargeData = enlargingObjects[gameObject];
						if (time > enlargeData.CreationTime + enlargeData.EnlargeTimeSeconds)
						{
							gameObject.transform.localScale = enlargeData.OriginalScale;
							totallyEnlarged.Add(gameObject);
							continue;
						}

						Vector3 scale = enlargeData.OriginalScale;
						if (scale == null)
							continue;

						float percentEnlarged = (time - enlargeData.CreationTime) / enlargeData.EnlargeTimeSeconds;
						gameObject.transform.localScale = new Vector3(scale.x * percentEnlarged, scale.y * percentEnlarged, scale.z * percentEnlarged);
					}

					foreach (GameObject gameObject in totallyEnlarged)
						enlargingObjects.Remove(gameObject);
				}
			}

			private static void DestroyEffect(GameObject gameObject)
			{
				if (enlargingObjects.ContainsKey(gameObject))
					enlargingObjects.Remove(gameObject);
				UnityEngine.Object.Destroy(gameObject);
			}
		}
	}
}