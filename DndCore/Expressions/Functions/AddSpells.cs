using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Adds the specified known spells to the active player.")]
	[Param(1, typeof(string), "spellList", "The list of spells to add, separated by semicolons.", ParameterIs.Required)]
	public class AddSpells : DndFunction
	{
		public override string Name { get; set; } = "AddSpells";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target = null, CastedSpell spell = null, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 1);
			if (player != null)
			{
				string spellList = Expressions.GetStr(args[0], player, target, spell);
				player.AddSpellsFrom(spellList);
			}

			return null;
		}
	}
}
