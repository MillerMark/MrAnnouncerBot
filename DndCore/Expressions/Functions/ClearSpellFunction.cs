using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Clears the specified effect for the active spell cast by the active player's TaleSpire mini.")]
	[Param(1, typeof(int), "shrinkOnDeleteTime", "The amount of time to shrink the effect.", ParameterIs.Optional)]

	public class ClearSpellFunction : DndFunction
	{
		public static event SpellEffectEventHandler ClearSpell;
		public override string Name { get; set; } = "ClearSpell";

		static void OnClearSpell(string spellId, float shrinkOnDeleteTime)
		{
			SpellEffectEventArgs spellEffectEventArgs = new SpellEffectEventArgs(null, spellId, null, EffectLocation.None, 0, 0, 0, shrinkOnDeleteTime);
			ClearSpell?.Invoke(null, spellEffectEventArgs);
		}

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, CastedSpell spell, RollResults dice = null)
		{
			ExpectingArguments(args, 0, 1);
			if (player == null || spell == null)
				return null;

			float shrinkOnDeleteTime = 0;
			if (args.Count > 0)
				float.TryParse(args[0], out shrinkOnDeleteTime);
			OnClearSpell(spell.ID, shrinkOnDeleteTime);

			return null;
		}
	}
}
