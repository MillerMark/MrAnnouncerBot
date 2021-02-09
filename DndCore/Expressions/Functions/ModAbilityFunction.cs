using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[ReturnType(typeof(double))]
	[Tooltip("Gets the specified ability modifier for the active player.")]
	[Param(1, typeof(Ability), "ability", "The ability to get the modifier for.", ParameterIs.Required)]
	public class ModAbilityFunction : DndFunction
	{
		public override string Name => "Mod";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, CastedSpell spell, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 1);

			Ability ability = (Ability)Expressions.GetInt(args[0], player, target, spell);

			return player.GetAbilityModifier(ability);
		}
	}
}
