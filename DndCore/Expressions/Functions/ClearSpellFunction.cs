using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Clears the specified effect for the active spell on the active player's TaleSpire mini.")]
	[Param(1, typeof(int), "secondsDelay", "Seconds to wait before clearing the effect.", ParameterIs.Optional)]
	public class ClearSpellFunction : DndFunction
	{
		public static event SpellEffectEventHandler ClearSpell;
		public override string Name { get; set; } = "ClearSpell";

		static void OnClearSpell(string spellId, float secondsDelayStart)
		{
			ClearSpell?.Invoke(null, new SpellEffectEventArgs(null, spellId, null, EffectLocation.None, 0, secondsDelayStart));
		}

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, CastedSpell spell, RollResults dice = null)
		{
			ExpectingArguments(args, 0, 1);
			if (player == null || spell == null)
				return null;

			float secondsDelayStart = 0;
			if (args.Count > 0)
				float.TryParse(args[0], out secondsDelayStart);
			OnClearSpell(spell.ID, secondsDelayStart);

			return null;
		}
	}
}
