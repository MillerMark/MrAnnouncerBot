using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Plays the specified spell effect over the active player's TaleSpire mini.")]
	[Param(1, typeof(string), "spellEffectName", "The name of the known effect or Prefab to create.", ParameterIs.Required)]
	[Param(2, typeof(float), "duration", "The duration, in seconds, for this spell effect to last before destroying it.", ParameterIs.Optional)]
	public class PlayEffectFunction : DndFunction
	{
		public static event SpellEffectEventHandler PlayEffect;
		public override string Name { get; set; } = "PlayEffect";

		static void OnPlayKnownEffect(string effectName, string spellId, string taleSpireId, float lifeTime)
		{
			PlayEffect?.Invoke(null, new SpellEffectEventArgs(effectName, spellId, taleSpireId, lifeTime));
		}

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, CastedSpell spell, RollResults dice = null)
		{
			ExpectingArguments(args, 1, 2);
			if (player == null || spell == null)
				return null;

			string effectName = Expressions.GetStr(args[0]);
			float lifeTime = 0;
			if (args.Count > 1)
				lifeTime = (float)Expressions.GetDouble(args[1]);
			OnPlayKnownEffect(effectName, spell.ID, player.taleSpireId, lifeTime);

			return null;
		}
	}
}
