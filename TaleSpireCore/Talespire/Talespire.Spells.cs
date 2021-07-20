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


				List<CollisionEffect> collisionsEffectsToRemove		= null;
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

					PlayEffectAtPosition(collisionEffect.EffectName, collisionEffect.SpellId, position, collisionEffect.LifeTime, collisionEffect.EnlargeTime, collisionEffect.SecondsDelayStart, collisionEffect.ShrinkTime, collisionEffect.Rotation);
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

			public static void AttachEffect(string effectName, string spellId, string creatureId, float secondsDelayStart, float enlargeTime, float lifeTime, float shrinkTime, float rotation)
			{
				if (secondsDelayStart > 0)
				{
					QueueEffect(new WaitingToCast(SpellLocation.Attached, secondsDelayStart, effectName, spellId, creatureId, enlargeTime, lifeTime, null, shrinkTime, rotation));
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
				//spell.transform.
				spell.transform.position = creatureBase.transform.position;

				EffectParameters.ApplyAfterPositioning(spell);
			}


			public static void PlayEffectAtCreatureBase(string effectName, string spellId, string creatureId, float lifeTime = 0, float enlargeTimeSeconds = 0, float secondsDelayStart = 0, float shrinkTime = 0, float rotationDegrees = 0)
			{
				PlayEffect(effectName, spellId, creatureId, lifeTime, enlargeTimeSeconds, secondsDelayStart, shrinkTime, SpellLocation.AtCreatureBase, rotationDegrees, false);
			}

			public static void CreatureCastSpell(string effectName, string spellId, string creatureCastSpellId, float lifeTime = 0, float enlargeTimeSeconds = 0, float secondsDelayStart = 0, float shrinkTime = 0, float rotationDegrees = 0, bool isMoveable = false)
			{
				PlayEffect(effectName, spellId, creatureCastSpellId, lifeTime, enlargeTimeSeconds, secondsDelayStart, shrinkTime, SpellLocation.CreatureCastSpell, rotationDegrees, isMoveable);
			}

			private static void PlayEffect(string effectName, string spellId, string creatureId, float lifeTime, float enlargeTimeSeconds, float secondsDelayStart, float shrinkTime, SpellLocation location, float rotationDegrees, bool isMoveable)
			{
				if (secondsDelayStart > 0)
				{
					Log.Debug($"QueueEffect \"{effectName}\" for {secondsDelayStart} seconds...");
					QueueEffect(new WaitingToCast(location, secondsDelayStart, effectName, spellId, creatureId, enlargeTimeSeconds, lifeTime, null, shrinkTime, rotationDegrees, isMoveable));
					return;
				}

				CreatureBoardAsset creatureBoardAsset = Minis.GetCreatureBoardAsset(creatureId);
				if (creatureBoardAsset == null)
				{
					Log.Error($"CreatureBoardAsset matching id {creatureId} not found!");
					return;
				}

				GameObject spell = GetSpell(effectName, spellId, lifeTime, enlargeTimeSeconds, shrinkTime, rotationDegrees, isMoveable);

				if (spell == null)
				{
					Log.Error($"Spell name \"{effectName}\" not found!");
					return;
				}

				GameObject creatureBase = creatureBoardAsset.GetBase();
				if (location == SpellLocation.CreatureCastSpell)
				{
					Log.Vector("creatureBoardAsset.HookSpellCast.position", creatureBoardAsset.HookSpellCast.position);
					spell.transform.position = creatureBoardAsset.HookSpellCast.position;
				}
				else  // Default to base position...
					spell.transform.position = creatureBase.transform.position;

				float creatureRotationDegrees = creatureBoardAsset.GetRotationDegrees();
				spell.transform.Rotate(Vector3.up, creatureRotationDegrees);
				//spell.transform.localEulerAngles = new Vector3(spell.transform.localEulerAngles.x, rotationDegrees, spell.transform.localEulerAngles.z);
				//Log.Vector("spell.transform.localEulerAngles", spell.transform.localEulerAngles);
				//spell.transform.Rotate(creatureBoardAsset.GetRotation());
				//Log.Vector("spell.transform.localEulerAngles2", spell.transform.localEulerAngles);

				EffectParameters.ApplyAfterPositioning(spell);
			}

			public static GameObject PlayEffectAtPosition(string effectName, string spellId, VectorDto position, float lifeTime = 0, float enlargeTimeSeconds = 0, float secondsDelayStart = 0, float shrinkTime = 0, float rotationDegrees = 0, bool isMoveable = false)
			{
				if (secondsDelayStart > 0)
				{
					QueueEffect(new WaitingToCast(SpellLocation.AtPosition, secondsDelayStart, effectName, spellId, null, enlargeTimeSeconds, lifeTime, position, shrinkTime, rotationDegrees, isMoveable));
					return null;
				}

				Log.Warning($"PlayEffectAtPosition(\"{effectName}\"...)");
				GameObject spell = GetSpell(effectName, spellId, lifeTime, enlargeTimeSeconds, shrinkTime, rotationDegrees, isMoveable);

				if (spell != null)
				{
					spell.transform.position = position.GetVector3();
					EffectParameters.ApplyAfterPositioning(spell);
				}

				return spell;
			}

			public static void PlayEffectOnCollision(string effectName, string spellId, float lifeTime, float enlargeTime, float secondsDelayStart, bool useIntendedTarget, float shrinkTime, float rotation)
			{
				collisionEffects.Add(new CollisionEffect(effectName, spellId, lifeTime, enlargeTime, secondsDelayStart, useIntendedTarget, shrinkTime, rotation));
			}

			private static string GetAttachedEffectName(string spellId)
			{
				return "Attached." + spellId;
			}

			static GameObject GetSpell(string effectName, string spellId, float lifeTime, float enlargeTimeSeconds, float shrinkOnDeleteTime, float rotationDegrees, bool isMoveable)
			{
				GameObject spell = GetEffect(effectName, spellId, isMoveable);

				if (spell == null)
				{
					Log.Error($"Spell effect \"{effectName}\" not found. Unable to Play the effect.");
					return null;
				}

				spell.transform.Rotate(Vector3.up, rotationDegrees);

				spell.name = GetSpellName(spellId, effectName, isMoveable);
				// TODO: Figure out how to implement Moveable spells.
				if (lifeTime > 0)
				{
					float particleShutoffTimeSeconds = Math.Min(2, lifeTime / 5);
					if (shrinkOnDeleteTime > particleShutoffTimeSeconds)
						particleShutoffTimeSeconds = shrinkOnDeleteTime;
					Instances.AddTemporal(spell, lifeTime, particleShutoffTimeSeconds, enlargeTimeSeconds, shrinkOnDeleteTime);
				}
				else
					Instances.AddSpell(spellId, spell, enlargeTimeSeconds, shrinkOnDeleteTime);

				return spell;
			}

			internal static string GetSpellName(string spellId, string effectName, bool isMoveable)
			{
				if (isMoveable)
					return $"Moveable.{spellId}.{effectName}";
				return $"Spell.{spellId}";
			}

			static void PrepareEffect(GameObject gameObject)
			{
				if (gameObject == null)
					return;
				List<MonoBehaviour> transformMotions = gameObject.GetScriptsInChildren("RFX1_TransformMotion");
				if (transformMotions.Count > 0)
					Log.Warning($"Disabling RFX1_TransformMotion Speed in {gameObject.name}...");
				foreach (MonoBehaviour monoBehaviour in transformMotions)
				{
					monoBehaviour.enabled = false;
					//ReflectionHelper.SetPublicFieldValue(monoBehaviour, "Speed", 0);
					//ReflectionHelper.SetPublicFieldValue(monoBehaviour, "Distance", 0);
				}
			}

			private static GameObject GetEffect(string effectName, string spellId = "", bool isMoveable = false)
			{
				GameObject result;

				if (isMoveable)
				{
					result = Instances.GetMoveableSpell(spellId, effectName);
					if (result == null)
						Log.Error($"Moveable spell {spellId}.{effectName} not found!");
					else
					{
						Log.Warning($"Moveable spell {spellId}.{effectName} FOUND!");
						return result;
					}
				}

				if (Prefabs.Has(EffectParameters.GetEffectNameOnly(effectName)))
					result = Prefabs.Clone(effectName);
				else
					result = CompositeEffect.CreateKnownEffect(effectName);

				PrepareEffect(result);
				return result;
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
				if (projectileOptions.targetLocations == null || projectileOptions.targetLocations.Count == 0)
				{
					missilesRemaining = 0;
					return;
				}
				TrackedProjectile lastProjectileAdded = null;
				TrackedProjectile firstProjectileAdded = null;
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
							Log.Vector("Target's intended location", location);
							lastProjectileAdded = AddProjectile(projectileOptions, startTime, sourcePosition, location);
							if (firstProjectileAdded == null)
								firstProjectileAdded = lastProjectileAdded;
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
						lastProjectileAdded = AddProjectile(projectileOptions, startTime, sourcePosition, location);
						if (firstProjectileAdded == null)
							firstProjectileAdded = lastProjectileAdded;
						startTime += projectileOptions.launchTimeVariance * TimeVariance;
						if (missilesRemaining <= 0)
							break;
					}
				}
				firstProjectileAdded.IsFirst = true;
				if (lastProjectileAdded != null)
					lastProjectileAdded.IsLast = true;
			}

			private static TrackedProjectile AddProjectile(ProjectileOptions projectileOptions, float startTime, Vector3 sourcePosition, Vector3 location)
			{
				Log.Debug($"Creating projectile ({projectileOptions.effectName})...");
				float targetVariance = projectileOptions.targetVariance;
				TrackedProjectile trackedProjectile = new TrackedProjectile();
				trackedProjectile.StartTime = startTime;
				trackedProjectile.EffectName = projectileOptions.effectName;
				trackedProjectile.SpellId = projectileOptions.spellId;
				trackedProjectile.FireCollisionEventOn = projectileOptions.fireCollisionEventOn;
				trackedProjectile.SourcePosition = sourcePosition;
				trackedProjectile.SpeedFeetPerSecond = projectileOptions.speed;
				trackedProjectile.IntendedTargetPosition = location;
				trackedProjectile.ActualTargetPosition = new Vector3(location.x + targetVariance * DistanceVariance, location.y + targetVariance * DistanceVariance, location.z + targetVariance * DistanceVariance);
				trackedProjectile.ProjectileSize = projectileOptions.projectileSize;
				trackedProjectile.ProjectileSizeMultiplier = projectileOptions.projectileSizeMultiplier;
				trackedProjectile.BezierPathMultiplier = projectileOptions.bezierPathMultiplier;

				if (!allProjectiles.ContainsKey(projectileOptions.spellId))
					allProjectiles.Add(projectileOptions.spellId, new List<TrackedProjectile>());

				trackedProjectile.Initialize();
				allProjectiles[projectileOptions.spellId].Add(trackedProjectile);
				return trackedProjectile;
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



