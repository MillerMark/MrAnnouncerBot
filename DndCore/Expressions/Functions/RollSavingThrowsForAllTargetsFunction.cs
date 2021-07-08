using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Rolls saving throws for all targeted creatures.")]
	[Param(1, typeof(Ability), "ability", "The ability saving throw to roll.", ParameterIs.Required)]
	[Param(2, typeof(Conditions), "condition", "The condition the creature is at risk of receiving if the saving throw fails.", ParameterIs.Required)]
	[Param(3, typeof(string), "damageDie", "The damage die to include in this roll (e.g., \"3d4(fire)\".", ParameterIs.Optional)]
	public class RollSavingThrowsForAllTargetsFunction : DndFunction
	{
		public static event SavingThrowRollEventHandler SavingThrowForTargetsRequested;
		public override string Name { get; set; } = "RollSavingThrowsForAllTargets";

		public static void OnSavingThrowForTargetsRequested(object sender, SavingThrowRollEventArgs ea)
		{
			SavingThrowForTargetsRequested?.Invoke(sender, ea);
		}

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, CastedSpell spell, RollResults dice = null)
		{
			ExpectingArguments(args, 2, 3);

			Ability ability = Expressions.Get<Ability>(args[0].ToLower());
			Conditions condition = Expressions.Get<Conditions>(args[1]);
			string damageDie = "";
			if (args.Count > 2)
				damageDie = args[2];

			OnSavingThrowForTargetsRequested(null, new SavingThrowRollEventArgs(damageDie, condition, ability, spell));

			return null;
		}
	}
}
