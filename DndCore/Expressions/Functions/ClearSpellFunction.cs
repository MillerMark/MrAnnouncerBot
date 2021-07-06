using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Clears the specified effect for the active spell on the active player's TaleSpire mini.")]
	public class ClearSpellFunction : DndFunction
	{
		public static event SpellEffectEventHandler ClearSpell;
		public override string Name { get; set; } = "ClearSpell";

		static void OnClearSpell(string spellId)
		{
			ClearSpell?.Invoke(null, new SpellEffectEventArgs(null, spellId, null, EffectLocation.None));
		}

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, CastedSpell spell, RollResults dice = null)
		{
			ExpectingArguments(args, 0);
			if (player == null || spell == null)
				return null;

			OnClearSpell(spell.ID);

			return null;
		}
	}
}
