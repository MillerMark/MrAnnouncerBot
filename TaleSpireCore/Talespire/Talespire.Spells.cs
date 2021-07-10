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
			static Spells()
			{
				TrackedProjectile.TargetReached += TrackedProjectile_TargetReached;
			}

			private static void TrackedProjectile_TargetReached(object sender, EventArgs e)
			{
				if (!(sender is TrackedProjectile trackedProjectile))
					return;

				List<CollisionEffect> collisionsEffectsToCreate = collisionEffects.Where(x => x.SpellId == trackedProjectile.SpellId).ToList();
				if (!collisionsEffectsToCreate.Any())
					return;


				List<CollisionEffect> collisionsEffectsToRemove = null;
				foreach (CollisionEffect collisionEffect in collisionsEffectsToCreate)
				{
					VectorDto position;
					if (collisionEffect.UseIntendedTarget)
					{
						position = trackedProjectile.IntendedTargetPosition.GetVectorDto();
						if (collisionsEffectsToRemove == null)
							collisionsEffectsToRemove = new List<CollisionEffect>();
						collisionsEffectsToRemove.Add(collisionEffect);  // Only allow one of these per spell effect.
					}
					else  // Use actual target...
						position = trackedProjectile.ActualTargetPosition.GetVectorDto();

					PlayEffectAtPosition(collisionEffect.EffectName, collisionEffect.SpellId, position, collisionEffect.LifeTime, collisionEffect.EnlargeTime, collisionEffect.SecondsDelayStart);
				}

				if (collisionsEffectsToRemove != null && collisionsEffectsToRemove.Any())
					collisionEffects = collisionEffects.Except(collisionsEffectsToRemove).ToList();
			}

			static object queuedEffectsLock = new object();
			static List<WaitingToCast> queuedEffects = new List<WaitingToCast>();
			static List<CollisionEffect> collisionEffects = new List<CollisionEffect>();

			static void QueueEffect(WaitingToCast waitingToCast)
			{
				lock (queuedEffectsLock)
					queuedEffects.Add(waitingToCast);
			}

			public static void AttachEffect(string effectName, string spellId, string creatureId, float enlargeTime, float secondsDelayStart)
			{
				if (secondsDelayStart > 0)
				{
					QueueEffect(new WaitingToCast(SpellLocation.Attached, secondsDelayStart, effectName, spellId, creatureId, enlargeTime));
					return;
				}

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

				if (enlargeTime > 0)
					Instances.EnlargeSoon(spell, enlargeTime);

				spell.name = GetAttachedEffectName(spellId);
				GameObject creatureBase = creatureBoardAsset.GetBase();
				spell.transform.SetParent(creatureBase.transform);
				spell.transform.position = creatureBase.transform.position;
			}

			public static void PlayEffectOverCreature(string effectName, string spellId, string creatureId, float lifeTime = 0, float enlargeTimeSeconds = 0, float secondsDelayStart = 0)
			{
				if (secondsDelayStart > 0)
				{
					QueueEffect(new WaitingToCast(SpellLocation.OverCreature, secondsDelayStart, effectName, spellId, creatureId, enlargeTimeSeconds, lifeTime));
					return;
				}

				CreatureBoardAsset creatureBoardAsset = Minis.GetCreatureBoardAsset(creatureId);
				if (creatureBoardAsset == null)
				{
					Log.Error($"CreatureBoardAsset matching id {creatureId} not found!");
					return;
				}

				GameObject spell = GetSpell(effectName, spellId, lifeTime, enlargeTimeSeconds);

				if (spell == null)
					return;

				GameObject creatureBase = creatureBoardAsset.GetBase();
				spell.transform.position = creatureBase.transform.position;
			}

			public static GameObject PlayEffectAtPosition(string effectName, string spellId, VectorDto position, float lifeTime = 0, float enlargeTimeSeconds = 0, float secondsDelayStart = 0)
			{
				if (secondsDelayStart > 0)
				{
					QueueEffect(new WaitingToCast(SpellLocation.AtPosition, secondsDelayStart, effectName, spellId, null, enlargeTimeSeconds, lifeTime, position));
					return null;
				}

				GameObject spell = GetSpell(effectName, spellId, lifeTime, enlargeTimeSeconds);

				if (spell != null)
					spell.transform.position = position.GetVector3();

				return spell;
			}

			public static void PlayEffectOnCollision(string effectName, string spellId, float lifeTime, float enlargeTime, float secondsDelayStart, bool useIntendedTarget)
			{
				collisionEffects.Add(new CollisionEffect(effectName, spellId, lifeTime, enlargeTime, secondsDelayStart, useIntendedTarget));
			}

			private static string GetAttachedEffectName(string spellId)
			{
				return "Attached." + spellId;
			}

			static GameObject GetSpell(string effectName, string spellId, float lifeTime, float enlargeTimeSeconds)
			{
				GameObject spell = GetEffect(effectName);

				if (spell == null)
				{
					Log.Error($"Spell effect \"{effectName}\" not found. Unable to Play the effect.");
					return null;
				}

				spell.name = GetSpellName(spellId);
				if (lifeTime > 0)
					Instances.AddTemporal(spell, lifeTime, Math.Min(2, lifeTime / 5), 0, enlargeTimeSeconds);
				else
					Instances.AddSpell(spellId, spell, enlargeTimeSeconds);

				return spell;
			}

			private static string GetSpellName(string spellId)
			{
				return "Spell." + spellId;
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

			public static void Clear(string spellId, float shrinkOnDeleteTime = 1)
			{
				Instances.DeleteSpellSoon(spellId, shrinkOnDeleteTime);
				RemoveCollisionEffects(spellId);
			}

			public static void RemoveCollisionEffects(string spellId)
			{
				List<CollisionEffect> collisionsEffectsToRemove = collisionEffects.Where(x => x.SpellId == spellId).ToList();
				if (collisionsEffectsToRemove.Any())
					collisionEffects = collisionEffects.Except(collisionsEffectsToRemove).ToList();
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
				Log.Debug($"Creating projectile ({projectileOptions.effectName})...");
				float targetVariance = projectileOptions.targetVariance;
				TrackedProjectile trackedProjectile = new TrackedProjectile();
				trackedProjectile.StartTime = startTime;
				trackedProjectile.EffectName = projectileOptions.effectName;
				trackedProjectile.Parameters = projectileOptions.parameters;
				trackedProjectile.SpellId = projectileOptions.spellId;
				trackedProjectile.SourcePosition = sourcePosition;
				trackedProjectile.SpeedFeetPerSecond = projectileOptions.speed;
				trackedProjectile.IntendedTargetPosition = location;
				trackedProjectile.ActualTargetPosition = new Vector3(location.x + targetVariance * DistanceVariance, location.y + targetVariance * DistanceVariance, location.z + targetVariance * DistanceVariance);
				trackedProjectile.ProjectileSize = projectileOptions.projectileSize;
				trackedProjectile.ProjectileSizeMultiplier = projectileOptions.projectileSizeMultiplier;

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
				UpdateProjectiles();
				UpdateQueuedEffects();
			}

			private static void UpdateQueuedEffects()
			{
				List<WaitingToCast> deleteTheseNow = null;
				lock (queuedEffectsLock)
				{
					foreach (WaitingToCast waitingToCast in queuedEffects)
					{
						if (waitingToCast.ShouldCreateNow)
						{
							waitingToCast.CreateNow();

							if (deleteTheseNow == null)
								deleteTheseNow = new List<WaitingToCast>();
							deleteTheseNow.Add(waitingToCast);
						}
					}

					if (deleteTheseNow != null)
						foreach (WaitingToCast waitingToCast in deleteTheseNow)
							queuedEffects.Remove(waitingToCast);
				}
			}

			private static void UpdateProjectiles()
			{
				lock (lockTrackedProjectiles)
				{
					foreach (string spellId in allProjectiles.Keys)
					{
						List<TrackedProjectile> deleteThese = new List<TrackedProjectile>();
						foreach (TrackedProjectile trackedProjectile in allProjectiles[spellId])
						{
							trackedProjectile.UpdatePosition();
							if (trackedProjectile.readyToDelete)
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



