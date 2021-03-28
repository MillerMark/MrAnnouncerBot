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
	[Tooltip("Returns a plural or singular string.")]
	[Param(1, typeof(int), "amount", "The amount to check for singular/plural.", ParameterIs.Required)]
	[Param(2, typeof(string), "singularStr", "The string to return if the amount equals one.", ParameterIs.Required)]
	[Param(3, typeof(string), "pluralStr", "The string to return if the amount is not equal to one.", ParameterIs.Required)]
	public class PluralFunction : DndFunction
	{
		public override string Name => "Plural";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, CastedSpell spell, RollResults dice = null)
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
