using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class GetSpellRawDieStr : DndFunction
	{
		public override string Name => "GetSpellRawDieStr";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Target target, CastedSpell spell, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 1);

			if (spell == null)
				return null;
			return spell.GetSpellRawDieStr(args[0]);
		}
	}
}
