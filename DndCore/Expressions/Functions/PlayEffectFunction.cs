using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Plays the specified spell effect over the active player's TaleSpire mini.")]
	[Param(1, typeof(string), "spellEffectName", "The name of the known effect or Prefab to create.", ParameterIs.Required)]
	[Param(2, typeof(float), "duration", "The duration, in seconds, for this spell effect to last before destroying it.", ParameterIs.Optional)]
	[Param(3, typeof(EffectLocation), "effectLocation", "The location to play this effect. Defaults to the active creature's position.", ParameterIs.Optional)]
	[Param(4, typeof(float), "secondsDelay", "The seconds to delay playing this effect.", ParameterIs.Optional)]
	public class PlayEffectFunction : DndFunction
	{
		public static event SpellEffectEventHandler PlayEffect;
		public override string Name { get; set; } = "PlayEffect";

		static void OnPlayKnownEffect(string effectName, string spellId, string taleSpireId, float lifeTime, EffectLocation effectLocation, float secondsDelayStart)
		{
			PlayEffect?.Invoke(null, new SpellEffectEventArgs(effectName, spellId, taleSpireId, effectLocation, lifeTime, secondsDelayStart));
		}

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, CastedSpell spell, RollResults dice = null)
		{
			ExpectingArguments(args, 1, 4);
			if (player == null || spell == null)
				return null;

			string effectName = Expressions.GetStr(args[0]);
			float lifeTime = 0;
			float secondsDelayStart = 0;
			EffectLocation effectLocation = EffectLocation.ActiveCreaturePosition;
			if (args.Count > 1)
			{
				lifeTime = (float)Expressions.GetDouble(args[1]);
				if (args.Count > 2)
				{
					effectLocation = Expressions.Get<EffectLocation>(args[2]);
					if (args.Count > 3)
						secondsDelayStart = Expressions.GetFloat(args[3]);
				}
			}

			OnPlayKnownEffect(effectName, spell.ID, player.taleSpireId, lifeTime, effectLocation, secondsDelayStart);

			return null;
		}
	}
}
