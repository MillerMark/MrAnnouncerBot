using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Clears the attached effect for the active spell on the active player's TaleSpire mini.")]
	public class ClearAttachedFunction : DndFunction
	{
		public static event SpellEffectEventHandler ClearAttached;
		public override string Name { get; set; } = "ClearAttached";

		static void OnClearAttached(string spellId, string taleSpireId)
		{
			ClearAttached?.Invoke(null, new SpellEffectEventArgs(null, spellId, taleSpireId));
		}

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, CastedSpell spell, RollResults dice = null)
		{
			ExpectingArguments(args, 0);
			if (player == null || spell == null)
				return null;

			OnClearAttached(spell.ID, player.taleSpireId);

			return null;
		}
	}
}
