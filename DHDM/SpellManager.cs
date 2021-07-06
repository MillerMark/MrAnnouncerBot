using DndCore;
using TaleSpireCore;
using System;
using System.Linq;

namespace DHDM
{
	public static class SpellManager
	{
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
		public static void AttachEffect(string effectName, string spellId, string taleSpireId)
		{
			TaleSpireClient.AttachEffect(effectName, spellId, taleSpireId);
		}

		static void ClearAttached(string spellId, string taleSpireId)
		{
			TaleSpireClient.ClearAttached(spellId, taleSpireId);
		}

		static void ClearSpell(string spellId)
		{
			TaleSpireClient.ClearSpell(spellId);
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
			AttachEffect(ea.EffectName, ea.SpellId, ea.TaleSpireId);
		}

		private static void PlayEffectFunction_PlayEffect(object sender, SpellEffectEventArgs ea)
		{
			PlayEffect(ea.EffectName, ea.SpellId, ea.TaleSpireId, ea.LifeTime, ea.EffectLocation);
		}

		private static void ClearAttachedFunction_ClearAttached(object sender, SpellEffectEventArgs ea)
		{
			ClearAttached(ea.SpellId, ea.TaleSpireId);
		}

		private static void ClearSpellFunction_ClearSpell(object sender, SpellEffectEventArgs ea)
		{
			ClearSpell(ea.SpellId);
		}


	}
}


