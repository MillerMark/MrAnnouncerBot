using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace TaleSpireCore
{
	public static partial class Talespire
	{
		public static class Spells
		{
			public static void AttachEffect(string effectName, string spellId, string creatureId)
			{
				CreatureBoardAsset creatureBoardAsset = Minis.GetCreatureBoardAsset(creatureId);
				if (creatureBoardAsset == null)
				{
					Log.Error($"CreatureBoardAsset matching id {creatureId} not found!");
					return;
				}

				GameObject spell = GetEffect(effectName);

				if (spell == null)
				{
					Log.Error($"Spell effect \"{effectName}\" not found. Unable to Attach the effect.");
					return;
				}

				spell.name = GetAttachedEffectName(spellId);
				GameObject creatureBase = creatureBoardAsset.GetBase();
				spell.transform.SetParent(creatureBase.transform);
				spell.transform.position = creatureBase.transform.position;
			}

			private static string GetAttachedEffectName(string spellId)
			{
				return "Attached." + spellId;
			}

			static GameObject GetSpell(string effectName, string spellId, float lifeTime)
			{
				GameObject spell = GetEffect(effectName);

				if (spell == null)
				{
					Log.Error($"Spell effect \"{effectName}\" not found. Unable to Play the effect.");
					return null;
				}

				spell.name = GetSpellName(spellId);
				if (lifeTime > 0)
					Instances.AddTemporal(spell, lifeTime, Math.Min(2, lifeTime / 5));
				else
					Instances.AddSpell(spellId, spell);

				return spell;
			}

			private static string GetSpellName(string spellId)
			{
				return "Spell." + spellId;
			}

			public static void PlayEffectOverCreature(string effectName, string spellId, string creatureId, float lifeTime = 0)
			{
				CreatureBoardAsset creatureBoardAsset = Minis.GetCreatureBoardAsset(creatureId);
				if (creatureBoardAsset == null)
				{
					Log.Error($"CreatureBoardAsset matching id {creatureId} not found!");
					return;
				}

				GameObject spell = GetSpell(effectName, spellId, lifeTime);

				if (spell == null)
					return;
				
				GameObject creatureBase = creatureBoardAsset.GetBase();
				spell.transform.position = creatureBase.transform.position;
			}

			public static GameObject PlayEffectAtPosition(string effectName, string spellId, VectorDto vector, float lifeTime = 0)
			{
				GameObject spell = GetSpell(effectName, spellId, lifeTime);

				if (spell != null)
					spell.transform.position = vector.GetVector3();

				return spell;
			}

			private static GameObject GetEffect(string effectName)
			{
				if (Prefabs.Has(effectName))
					return Prefabs.Clone(effectName);
				else
					return CompositeEffect.CreateKnownEffect(effectName);
			}

			public static void ClearAttached(string spellId, string creatureId)
			{
				CreatureBoardAsset creatureBoardAsset = Minis.GetCreatureBoardAsset(creatureId);
				if (creatureBoardAsset == null)
				{
					Log.Error($"CreatureBoardAsset matching id {creatureId} not found!");
					return;
				}

				GameObject creatureBase = creatureBoardAsset.GetBase();
				string attachedEffectName = GetAttachedEffectName(spellId);
				GameObject childEffect = creatureBase.FindChild(attachedEffectName);
				if (childEffect == null)
				{
					Log.Error($"Child effect {attachedEffectName} not found.");
					return;
				}

				childEffect.transform.SetParent(null);
				Instances.AddTemporal(childEffect, 2, 2);
			}

			public static void Clear(string spellId)
			{
				Instances.DeleteSpellSoon(spellId);
			}

			static Dictionary<string, List<TrackedProjectile>> allProjectiles = new Dictionary<string, List<TrackedProjectile>>();

			static object lockTrackedProjectiles = new object();

			public static void LaunchProjectile(ProjectileOptions projectileOptions)
			{
				Log.Warning($"------------------------------------------------------------------");
				Log.Debug($"LaunchProjectile:");
				float startTime = Time.time;
				UnityEngine.Random.InitState((int)startTime * 1000);
				Vector3 sourcePosition = Minis.GetHitTargetVector(projectileOptions.taleSpireId);

				int missilesRemaining = projectileOptions.count;
				Log.Debug($"missilesRemaining: {missilesRemaining}");
				lock (lockTrackedProjectiles)
				{
					LaunchMissiles(projectileOptions, ref startTime, sourcePosition, ref missilesRemaining);
					while (projectileOptions.kind == ProjectileKind.DistributeAmongAllTargets && missilesRemaining > 0)
						LaunchMissiles(projectileOptions, ref startTime, sourcePosition, ref missilesRemaining);
				}
			}

			private static void LaunchMissiles(ProjectileOptions projectileOptions, ref float startTime, Vector3 sourcePosition, ref int missilesRemaining)
			{
				foreach (Vector3 location in projectileOptions.targetLocations)
				{
					//  projectileOptions.kind (ToVolume, DistributeAmongAllTargets, or EachTarget)
					//	projectileOptions.fireCollisionEventOn (FirstImpact, EachImpact, LastImpact)
					if (projectileOptions.kind == ProjectileKind.EachTarget)
					{
						Log.Debug($"ProjectileKind.EachTarget...");
						float saveStartTime = startTime;
						for (int i = 0; i < projectileOptions.count; i++)
						{
							AddProjectile(projectileOptions, startTime, sourcePosition, location);
							startTime += projectileOptions.launchTimeVariance * TimeVariance;
						}
						startTime = (startTime + saveStartTime) / 2;
					}
					else if (projectileOptions.kind == ProjectileKind.ToVolume)
					{
						Log.Debug($"ProjectileKind.ToVolume...");
						Log.Error($"Cannot project to volume yet - we need to know the shape!");
					}
					else if (projectileOptions.kind == ProjectileKind.DistributeAmongAllTargets)
					{
						Log.Debug($"ProjectileKind.DistributeAmongAllTargets...");
						missilesRemaining--;
						AddProjectile(projectileOptions, startTime, sourcePosition, location);
						startTime += projectileOptions.launchTimeVariance * TimeVariance;
						if (missilesRemaining <= 0)
							break;
					}
				}
			}

			private static void AddProjectile(ProjectileOptions projectileOptions, float startTime, Vector3 sourcePosition, Vector3 location)
			{
				// TODO: Work with these:
				//  projectileOptions.count

				// TODO: Figure out easing.
				Log.Debug($"Creating projectile ({projectileOptions.effectName})...");
				float targetVariance = projectileOptions.targetVariance;
				TrackedProjectile trackedProjectile = new TrackedProjectile();
				trackedProjectile.StartTime = startTime;
				trackedProjectile.EffectName = projectileOptions.effectName;
				trackedProjectile.SpellId = projectileOptions.spellId;
				trackedProjectile.SourcePosition = sourcePosition;
				trackedProjectile.SpeedFeetPerSecond = projectileOptions.speed;
				trackedProjectile.TargetPosition = new Vector3(location.x + targetVariance * DistanceVariance, location.y + targetVariance * DistanceVariance, location.z + targetVariance * DistanceVariance);

				if (!allProjectiles.ContainsKey(projectileOptions.spellId))
					allProjectiles.Add(projectileOptions.spellId, new List<TrackedProjectile>());


				trackedProjectile.Initialize();
				allProjectiles[projectileOptions.spellId].Add(trackedProjectile);
			}

			/// <summary>
			/// Returns a number between 80% and 120%
			/// </summary>
			private static float TimeVariance
			{
				get
				{
					return RandomRange(0.8f, 1.2f);
				}
			}
			/// <summary>
			/// Returns a number between -100% and 100%
			/// </summary>
			private static float DistanceVariance
			{
				get
				{
					return RandomRange(-1f, 1f);
				}
			}

			//
			private static float RandomRange(float minInclusive, float maxInclusive)
			{
				return UnityEngine.Random.Range(minInclusive, maxInclusive);
			}

			public static void Update()
			{
				lock (lockTrackedProjectiles)
				{
					foreach (string spellId in allProjectiles.Keys)
					{
						List<TrackedProjectile> deleteThese = new List<TrackedProjectile>();
						foreach (TrackedProjectile trackedProjectile in allProjectiles[spellId])
						{
							trackedProjectile.UpdatePosition();
							if (trackedProjectile.ReadyToDelete)
								deleteThese.Add(trackedProjectile);
						}

						if (deleteThese.Count > 0)
							foreach (TrackedProjectile trackedProjectile in deleteThese)
							{
								Log.Debug($"Removing projectile... ({trackedProjectile.EffectName})");
								allProjectiles[spellId].Remove(trackedProjectile);
							}
					}
				}
			}
		}
	}
}



