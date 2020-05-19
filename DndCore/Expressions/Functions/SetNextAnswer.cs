using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class SetNextAnswer : DndFunction
	{
		public override string Name { get; set; } = "SetNextAnswer";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Target target = null, CastedSpell spell = null, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 1);
			if (player != null)
				player.SetNextAnswer(Expressions.GetStr(args[0], player, target, spell));
			return null;
		}
	}
}
