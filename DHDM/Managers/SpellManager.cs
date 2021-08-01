using DndCore;
using TaleSpireCore;
using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;

namespace DHDM
{
	public static class SpellManager
	{
		static List<SpellEffectTimer> timers = new List<SpellEffectTimer>();
		public static string nextSpellIdWeAreCasting = null;
		public static string activeSpellName = null;

		public static void Initialize(DndGame game)
		{
			Game = game;
			LaunchProjectileFunction.LaunchProjectile += LaunchProjectileFunction_LaunchProjectile;
			AttachEffectFunction.AttachEffect += AttachEffectFunction_AttachEffect;
			PlayEffectFunction.PlayEffect += PlayEffectFunction_PlayEffect;
			ClearAttachedFunction.ClearAttached += ClearAttachedFunction_ClearAttached;
			ClearSpellFunction.ClearSpell += ClearSpellFunction_ClearSpell;
		}

		private static void LaunchProjectileFunction_LaunchProjectile(object sender, ProjectileEffectEventArgs ea)
		{
			List<string> targets = GetTargets(ea);

			TaleSpireClient.LaunchProjectile(ea.EffectName, ea.TaleSpireId, ea.Kind.ToString(), ea.Count,
				ea.Speed, ea.FireCollisionEventOn.ToString(), ea.LaunchTimeVariance,
				ea.TargetVariance, ea.SpellId, ea.ProjectileSize.ToString(), ea.ProjectileSizeMultiplier, ea.BezierPathMultiplier, targets);
		}

		private static List<string> GetTargets(ProjectileEffectEventArgs ea)
		{
			List<string> targets = new List<string>();
			if (Targeting.ActualKind.HasFlag(TargetKind.Volume))
				targets.Add(Targeting.TargetPoint.GetXyzStr());  // Adding a vector to the target list. Might add something like "(0, 0, 0) Square20"

			if (CastedSpell.ActiveSpells.ContainsKey(ea.SpellId))
			{
				CastedSpell castedSpell = CastedSpell.ActiveSpells[ea.SpellId];
				TargetDetails targetDetails = castedSpell.Spell.TargetDetails;
				if (targetDetails.Kind.HasFlag(TargetKind.Volume) && targetDetails.MaxCreatures == 0 && targets.Count > 0)
					return targets;  // Don't add any active.
			}

			AddActiveTargets(ea, targets);

			return targets;
		}

		private static void AddActiveTargets(ProjectileEffectEventArgs ea, List<string> targets)
		{
			if (ea.Target != null)
			{
				foreach (Creature creature in ea.Target.Creatures)
					targets.Add(creature.taleSpireId);

				if (ea.Target.PlayerIds != null)
					foreach (int playerID in ea.Target.PlayerIds)
					{
						Character player = Game.GetPlayerFromId(playerID);
						if (player != null)
							targets.Add(player.taleSpireId);
					}
			}
			else
			{
				List<Creature> allTargets = TargetManager.GetTargets();
				foreach (Creature creature in allTargets)
					targets.Add(creature.taleSpireId);
			}
		}

		/* 
				* Prepare to cast (when a spell button is pressed on the Stream Deck)
				* Dice rolled
				* Roll result received
				* Magic Given  <<<
				* Spell Expired
	  */
		public static void AttachEffect(string effectName, string spellId, string taleSpireId, float secondsDelayStart, float enlargeTime, float lifeTime, float shrinkTime, float rotationDegrees)
		{
			TaleSpireClient.AttachEffect(effectName, spellId, taleSpireId, enlargeTime, lifeTime, shrinkTime, secondsDelayStart, rotationDegrees);
		}

		static void ClearAttached(string spellId, string taleSpireId, float secondsDelayStart)
		{
			if (secondsDelayStart > 0)
			{
				CreateClearAttachedSpellTimer(spellId, taleSpireId, secondsDelayStart);
				return;
			}
			TaleSpireClient.ClearAttached(spellId, taleSpireId);
		}

		static void ClearSpell(string spellId, float secondsDelayStart, float shrinkTime)
		{
			if (secondsDelayStart > 0)
			{
				CreateClearSpellTimer(spellId, secondsDelayStart, shrinkTime);
				return;
			}
			TaleSpireClient.ClearSpell(spellId, shrinkTime);
		}

		static void PlayEffect(string effectName, string spellId, string taleSpireId, float lifeTime, EffectLocation effectLocation, float secondsDelayStart, float enlargeTime, float shrinkTime, float rotationDegrees)
		{
			if (effectLocation == EffectLocation.CreatureBase)
				TaleSpireClient.PlayEffectAtCreatureBase(effectName, spellId, taleSpireId, lifeTime, enlargeTime, secondsDelayStart, shrinkTime, rotationDegrees);
			else if (effectLocation == EffectLocation.SpellCast || effectLocation == EffectLocation.MoveableSpellCast)
			{
				bool isMoveable = effectLocation == EffectLocation.MoveableSpellCast;
				TaleSpireClient.CreatureCastSpell(effectName, spellId, taleSpireId, lifeTime, enlargeTime, secondsDelayStart, shrinkTime, rotationDegrees, isMoveable);
			}
			else if (effectLocation == EffectLocation.LastTargetPosition || effectLocation == EffectLocation.MoveableTarget)
			{
				Vector targetPoint = Targeting.TargetPoint;
				VectorDto vector = new VectorDto((float)targetPoint.x, (float)targetPoint.y, (float)targetPoint.z);
				bool isMoveable = effectLocation == EffectLocation.MoveableTarget;
				TaleSpireClient.PlayEffectAtPosition(effectName, spellId, vector, lifeTime, enlargeTime, secondsDelayStart, shrinkTime, rotationDegrees, isMoveable);
			}
			else if (effectLocation == EffectLocation.AtCollisionTarget)
				TaleSpireClient.PlayEffectOnCollision(effectName, spellId, lifeTime, enlargeTime, secondsDelayStart, true, shrinkTime, rotationDegrees);
			else if (effectLocation == EffectLocation.AtCollision)
				TaleSpireClient.PlayEffectOnCollision(effectName, spellId, lifeTime, enlargeTime, secondsDelayStart, false, shrinkTime, rotationDegrees);
			else if (effectLocation == EffectLocation.AtCollisionBase)
				TaleSpireClient.PlayEffectOnCollision(effectName, spellId, lifeTime, enlargeTime, secondsDelayStart, true, shrinkTime, rotationDegrees, true);
		}

		private static void CreateClearSpellTimer(string spellId, float secondsDelayStart, float shrinkTime)
		{
			SpellEffectTimer timer = new SpellEffectTimer();
			timer.Elapsed += ClearSpellTimer_Elapsed;
			timer.SpellId = spellId;
			timer.ShrinkTime = shrinkTime;
			timer.Interval = TimeSpan.FromSeconds(secondsDelayStart).TotalMilliseconds;
			timer.Start();
			timers.Add(timer);
		}

		private static void CreateClearAttachedSpellTimer(string spellId, string taleSpireId, float secondsDelayStart)
		{
			SpellEffectTimer timer = new SpellEffectTimer();
			timer.Elapsed += ClearAttachedSpellTimer_Elapsed;
			timer.SpellId = spellId;
			timer.TaleSpireId = taleSpireId;
			timer.Interval = TimeSpan.FromSeconds(secondsDelayStart).TotalMilliseconds;
			timer.Start();
			timers.Add(timer);
		}

		private static void ClearSpellTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if (!(sender is SpellEffectTimer spellTimer))
				return;

			spellTimer.Stop();
			TaleSpireClient.ClearSpell(spellTimer.SpellId, spellTimer.ShrinkTime);
			timers.Remove(spellTimer);
		}

		private static void ClearAttachedSpellTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if (!(sender is SpellEffectTimer spellTimer))
				return;

			spellTimer.Stop();
			TaleSpireClient.ClearAttached(spellTimer.SpellId, spellTimer.TaleSpireId);
			timers.Remove(spellTimer);
		}

		private static void AttachEffectFunction_AttachEffect(object sender, SpellEffectEventArgs ea)
		{
			AttachEffect(ea.EffectName, ea.SpellId, ea.TaleSpireId, ea.SecondsDelayStart, ea.EnlargeTime, ea.LifeTime, ea.ShrinkTime, ea.RotationDegrees);
		}

		private static void PlayEffectFunction_PlayEffect(object sender, SpellEffectEventArgs ea)
		{
			PlayEffect(ea.EffectName, ea.SpellId, ea.TaleSpireId, ea.LifeTime, ea.EffectLocation, ea.SecondsDelayStart, ea.EnlargeTime, ea.ShrinkTime, ea.RotationDegrees);
		}

		private static void ClearAttachedFunction_ClearAttached(object sender, SpellEffectEventArgs ea)
		{
			ClearAttached(ea.SpellId, ea.TaleSpireId, ea.SecondsDelayStart);
		}

		private static void ClearSpellFunction_ClearSpell(object sender, SpellEffectEventArgs ea)
		{
			ClearSpell(ea.SpellId, ea.SecondsDelayStart, ea.ShrinkTime);
		}
		public static DndGame Game { get; set; }
	}
}


