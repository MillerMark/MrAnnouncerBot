using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Adds the specified vantage die the player's next roll.")]
	[Param(1, typeof(VantageKind), "vantageKind", "Either \"Advantage\" or \"Disadvantage\".", ParameterIs.Required)]
	public class AddVantage : DndFunction
	{
		public override string Name { get; set; } = "AddVantage";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, CastedSpell spell, RollResults dice = null)
		{
			ExpectingArguments(args, 1);

			VantageKind vantageKind = Expressions.Get<VantageKind>(args[0], player, target, spell);
			if (player is Character characterPlayer)
			{
				player.AddVantageThisRoll(vantageKind);
			}
			return null;
		}
	}
}
