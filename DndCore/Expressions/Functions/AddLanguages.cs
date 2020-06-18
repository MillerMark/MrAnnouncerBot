using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Adds the specified languages to the active player.")]
	[Param(1, typeof(string), "languageStr", "The languages to add, separated by commas.", ParameterIs.Required)]
	public class AddLanguages : DndFunction
	{
		public override string Name { get; set; } = "AddLanguages";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Target target = null, CastedSpell spell = null, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 1);
			if (player != null)
			{
				string languageStr = Expressions.GetStr(args[0], player, target, spell);
				player.AddLanguages(languageStr);
			}

			return null;
		}
	}
}
