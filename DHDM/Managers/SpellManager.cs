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

		public static void Initialize()
		{
			AttachEffectFunction.AttachEffect += AttachEffectFunction_AttachEffect;
			PlayEffectFunction.PlayEffect += PlayEffectFunction_PlayEffect;
			ClearAttachedFunction.ClearAttached += ClearAttachedFunction_ClearAttached;
			ClearSpellFunction.ClearSpell += ClearSpellFunction_ClearSpell;
		}

		/* 
				* Prepare to cast (when a spell button is pressed on the Stream Deck)
				* Dice rolled
				* Roll result received
				* Magic Given
				* Spell Expired
	  */
		public static void AttachEffect(string effectName, string spellId, string taleSpireId, float secondsDelayStart)
		{
			if (secondsDelayStart > 0)
			{
				CreateAttachEffectSpellTimer(effectName, spellId, taleSpireId, secondsDelayStart);
				return;
			}
			TaleSpireClient.AttachEffect(effectName, spellId, taleSpireId);
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

		static void ClearSpell(string spellId, float secondsDelayStart)
		{
			if (secondsDelayStart > 0)
			{
				CreateClearSpellTimer(spellId, secondsDelayStart);
				return;
			}
			TaleSpireClient.ClearSpell(spellId);
		}

		static void CreateAttachEffectSpellTimer(string effectName, string spellId, string taleSpireId, float secondsDelayStart)
		{
			SpellEffectTimer timer = new SpellEffectTimer();
			timer.Elapsed += AttachEffectTimer_Elapsed;
			timer.SpellId = spellId;
			timer.EffectName = effectName;
			timer.TaleSpireId = taleSpireId;
			timer.Interval = TimeSpan.FromSeconds(secondsDelayStart).TotalMilliseconds;
			timers.Add(timer);
		}

		private static void CreateClearSpellTimer(string spellId, float secondsDelayStart)
		{
			SpellEffectTimer timer = new SpellEffectTimer();
			timer.Elapsed += ClearSpellTimer_Elapsed;
			timer.SpellId = spellId;
			timer.Interval = TimeSpan.FromSeconds(secondsDelayStart).TotalMilliseconds;
			timers.Add(timer);
		}

		private static void CreateClearAttachedSpellTimer(string spellId, string taleSpireId, float secondsDelayStart)
		{
			SpellEffectTimer timer = new SpellEffectTimer();
			timer.Elapsed += ClearAttachedSpellTimer_Elapsed;
			timer.SpellId = spellId;
			timer.TaleSpireId = taleSpireId;
			timer.Interval = TimeSpan.FromSeconds(secondsDelayStart).TotalMilliseconds;
			timers.Add(timer);
		}

		private static void ClearSpellTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if (!(sender is SpellEffectTimer spellTimer))
				return;

			spellTimer.Stop();
			TaleSpireClient.ClearSpell(spellTimer.SpellId);
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

		private static void AttachEffectTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if (!(sender is SpellEffectTimer spellTimer))
				return;

			spellTimer.Stop();
			TaleSpireClient.AttachEffect(spellTimer.EffectName, spellTimer.SpellId, spellTimer.TaleSpireId);
			timers.Remove(spellTimer);
		}

		static void PlayEffect(string effectName, string spellId, string taleSpireId, float lifeTime, EffectLocation effectLocation)
		{
			if (effectLocation == EffectLocation.ActiveCreaturePosition)
				TaleSpireClient.PlayEffectOverCreature(effectName, spellId, taleSpireId, lifeTime);
			else if (effectLocation == EffectLocation.LastTargetPosition)
			{
				Vector targetPoint = Targeting.TargetPoint;
				VectorDto vector = new VectorDto((float)targetPoint.x, (float)targetPoint.y, (float)targetPoint.z);
				TaleSpireClient.PlayEffectAtPosition(effectName, spellId, vector, lifeTime);
			}
		}

		static SpellManager()
		{
			
		}

		private static void AttachEffectFunction_AttachEffect(object sender, SpellEffectEventArgs ea)
		{
			AttachEffect(ea.EffectName, ea.SpellId, ea.TaleSpireId, ea.SecondsDelayStart);
		}

		private static void PlayEffectFunction_PlayEffect(object sender, SpellEffectEventArgs ea)
		{
			PlayEffect(ea.EffectName, ea.SpellId, ea.TaleSpireId, ea.LifeTime, ea.EffectLocation);
		}

		private static void ClearAttachedFunction_ClearAttached(object sender, SpellEffectEventArgs ea)
		{
			ClearAttached(ea.SpellId, ea.TaleSpireId, ea.SecondsDelayStart);
		}

		private static void ClearSpellFunction_ClearSpell(object sender, SpellEffectEventArgs ea)
		{
			ClearSpell(ea.SpellId, ea.SecondsDelayStart);
		}
	}
}


