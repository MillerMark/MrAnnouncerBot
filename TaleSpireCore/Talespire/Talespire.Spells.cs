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

			public static void PlayEffect(string effectName, string spellId, string creatureId, float lifeTime = 0)
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
					Log.Error($"Spell effect \"{effectName}\" not found. Unable to Play the effect.");
					return;
				}

				spell.name = "Play." + spellId;
				GameObject creatureBase = creatureBoardAsset.GetBase();
				spell.transform.position = creatureBase.transform.position;
				if (lifeTime > 0)
					Instances.AddTemporal(spell, lifeTime, Math.Min(2, lifeTime / 5));
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
		}
	}
}



