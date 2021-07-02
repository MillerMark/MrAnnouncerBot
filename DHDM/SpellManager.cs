using DndCore;
using TaleSpireCore;
using System;
using System.Linq;

namespace DHDM
{
	public static class SpellManager
	{
		public static void Initialize()
		{
			AttachEffectFunction.AttachEffect += AttachEffectFunction_AttachEffect;
			PlayEffectFunction.PlayEffect += PlayEffectFunction_PlayEffect;
			ClearAttachedFunction.ClearAttached += ClearAttachedFunction_ClearAttached;
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

		static void PlayEffect(string effectName, string spellId, string taleSpireId, float lifeTime)
		{
			TaleSpireClient.PlayEffect(effectName, spellId, taleSpireId, lifeTime);
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
			PlayEffect(ea.EffectName, ea.SpellId, ea.TaleSpireId, ea.LifeTime);
		}

		private static void ClearAttachedFunction_ClearAttached(object sender, SpellEffectEventArgs ea)
		{
			ClearAttached(ea.SpellId, ea.TaleSpireId);
		}


	}
}


