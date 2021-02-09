using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Gives the target full damage from the last spell/magic attack.")]
	public class GiveTargetHalfDamage : DndFunction
	{
		public override string Name { get; set; } = "GiveTargetHalfDamage";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target = null, CastedSpell spell = null, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 0);
			return GiveTargetFullDamage.ApplyDamage(args, evaluator, player, target, 0.5);
		}
		
	}
}
