using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class SelectMonsterFunction : DndFunction
	{
		public static event SelectMonsterEventHandler RequestSelectMonster;

		public static void OnRequestSelectMonster(SelectMonsterEventArgs ea)
		{
			RequestSelectMonster?.Invoke(null, ea);
		}

		public override string Name { get; set; } = "SelectMonster";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Target target, CastedSpell spell, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 0, 1);

			SelectMonsterEventArgs ea = new SelectMonsterEventArgs(player);
			if (args.Count > 0)
				ea.MaxChallengeRating = Expressions.GetDouble(args[0], player, target, spell);

			OnRequestSelectMonster(ea);

			return ea.Monster;
		}
	}
}
