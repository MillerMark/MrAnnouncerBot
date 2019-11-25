using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class AddSpells : DndFunction
	{
		public override string Name { get; set; } = "AddSpells";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Creature target = null, CastedSpell spell = null)
		{
			ExpectingArguments(args, 1);
			if (player != null)
				player.AddSpellsFrom(Expressions.GetStr(args[0], player, target, spell));
			return null;
		}
	}
}
