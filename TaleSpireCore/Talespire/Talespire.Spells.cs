using System;
using System.Linq;
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

			public static void PlayEffectAtPosition(string effectName, string spellId, VectorDto vector, float lifeTime = 0)
			{
				GameObject spell = GetSpell(effectName, spellId, lifeTime);

				if (spell != null)
					spell.transform.position = vector.GetVector3();
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
		}
	}
}



