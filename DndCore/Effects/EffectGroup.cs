using System;
using System.Linq;
using System.Collections.Generic;

namespace DndCore
{
	public class EffectGroup : Effect
	{
		public List<Effect> effects = new List<Effect>();

		public EffectGroup()
		{
			effectKind = EffectKind.GroupEffect;
		}

		public int effectsCount
		{
			get
			{
				return effects.Count;
			}
		}

		public void Add(Effect effect)
		{
			effects.Add(effect);
		}

		public static string GetRandomHitSpellName()
		{
			return "SpellHit" + MathUtils.RandomBetween(1, 5).ToString();
		}

		public void AddSpellEffect(VisualEffectTarget chestTarget, VisualEffectTarget bottomTarget, ref double scale, double scaleIncrement, ref double autoRotation, ref int timeOffset, SpellEffect spellHit)
		{
			string effectName;
			bool usingSpellHits = true;
			VisualEffectTarget target;

			if (!string.IsNullOrWhiteSpace(spellHit.EffectName))
			{
				effectName = spellHit.EffectName;
				usingSpellHits = false;
				target = bottomTarget;
			}
			else
			{
				effectName = GetRandomHitSpellName();
				target = chestTarget;
			}

			AnimationEffect effectBonus = AnimationEffect.CreateEffect(effectName, target,
				spellHit.Hue, spellHit.Saturation, spellHit.Brightness,
				spellHit.SecondaryHue, spellHit.SecondarySaturation, spellHit.SecondaryBrightness, spellHit.XOffset, spellHit.YOffset, spellHit.VelocityX, spellHit.VelocityY);

			if (usingSpellHits)
			{
				effectBonus.timeOffsetMs = timeOffset;
				effectBonus.scale = scale;
				effectBonus.autoRotation = autoRotation;
				autoRotation *= -1;
				scale *= scaleIncrement;
				timeOffset += 200;
			}
			else
			{
				if (spellHit.TimeOffset > int.MinValue)
					effectBonus.timeOffsetMs = spellHit.TimeOffset;
				else
				{
					effectBonus.timeOffsetMs = timeOffset;
					timeOffset += 200;
				}

				effectBonus.scale = spellHit.Scale;
				effectBonus.autoRotation = spellHit.AutoRotation;
				effectBonus.rotation = spellHit.Rotation;
			}
			this.Add(effectBonus);
		}
	}
}
