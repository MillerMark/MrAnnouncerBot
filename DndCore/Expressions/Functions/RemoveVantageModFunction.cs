using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Removes the specified vantage mod from the active magic owner (a CreaturePlusModId from Magic set in the Expression Evaluator's custom data).")]
	public class RemoveVantageModFunction : DndFunction
	{
		public override string Name { get; set; } = "RemoveVantageMod";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, CastedSpell spell, DiceStoppedRollingData dice = null)
		{
			CreaturePlusModId recipient = Expressions.GetCustomData<CreaturePlusModId>(evaluator.Variables);
			if (recipient == null)
				throw new Exception($"CreaturePlusModId recipient must be specified before evaluating expressions containing RemovePropertyMod.");

			ExpectingArguments(args, 0);

			recipient.Creature.RemoveVantageMod(recipient.ID);
			return null;
		}
	}
}
