using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class ModAbilityFunction : DndFunction
	{
		public override string Name => "Mod";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Creature target, CastedSpell spell)
		{
			ExpectingArguments(args, 1);

			Ability ability = (Ability)evaluator.Evaluate<int>(args[0]);

			return player.GetAbilityModifier(ability);
		}
	}
}
