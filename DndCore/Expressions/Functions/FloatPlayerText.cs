using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class FloatPlayerText : DndFunction
	{
		public override string Name { get; set; } = "FloatPlayerText";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Target target = null, CastedSpell spell = null, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 1, 3);
			string fillColor = "player";
			string outlineColor = "player";
			if (args.Count > 1)
			{
				fillColor = args[1];
				if (args.Count > 2)
					outlineColor = args[2];
			}
			if (player != null)
				player.ShowState(Expressions.GetStr(args[0], player, target, spell), fillColor, outlineColor);
			return null;
		}
	}
}
