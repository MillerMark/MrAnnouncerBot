using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	/// <summary>
	/// If the first argument equals 1, returns the second argument.
	/// Otherwise returns the third argument.
	/// Used in D & D documentation to create singular/plurals
	/// </summary>
	public class PluralFunction : DndFunction
	{
		public override string Name => "Plural";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Target target, CastedSpell spell, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 3);

			int value = Expressions.GetInt(args[0], player, target, spell);
			if (value == 1)
				return Expressions.GetStr(args[1], player, target, spell);	// singular.
			else
				return Expressions.GetStr(args[2], player, target, spell);  // plural
		}
	}
}
