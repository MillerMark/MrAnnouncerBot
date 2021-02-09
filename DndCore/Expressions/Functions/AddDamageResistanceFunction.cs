using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Adds damage resistance of the specified damage type and attack kind to the active player.")]
	[Param(1, typeof(DamageType), "damageType", "The type of damage the player has resistance to.", ParameterIs.Required)]
	[Param(2, typeof(AttackKind), "attackKind", "The kind of attack (magical, non-magical) the player has resistance to.", ParameterIs.Required)]
	public class AddDamageResistanceFunction : DndFunction
	{
		public override string Name => "AddDamageResistance";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, CastedSpell spell, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 2);

			DamageType damageType = (DamageType)Expressions.Get(args[0], player, target, spell);
			AttackKind attackKind = (AttackKind)Expressions.Get(args[1], player, target, spell);
			player.AddDamageResistance(damageType, attackKind);
			return null;
		}
	}
}
