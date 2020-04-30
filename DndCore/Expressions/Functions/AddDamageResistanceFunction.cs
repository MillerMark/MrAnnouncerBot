using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class AddDamageResistanceFunction : DndFunction
	{
		public override string Name => "AddDamageResistance";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Target target, CastedSpell spell, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 2);

			DamageType damageType = (DamageType)Expressions.Get(args[0], player, target, spell);
			AttackKind attackKind = (AttackKind)Expressions.Get(args[1], player, target, spell);
			player.AddDamageResistance(damageType, attackKind);
			return null;
		}
	}
}
