using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Gets the active target as set by the DM.")]
	public class GetActiveTarget : DndFunction
	{
		public override string Name { get; set; } = "GetActiveTarget";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Target target = null, CastedSpell spell = null, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 0);

			Target returnTarget = new Target();
			foreach (InGameCreature inGameCreature in AllInGameCreatures.Creatures)
			{
				if (inGameCreature.IsTargeted)
					returnTarget.AddCreature(inGameCreature.Creature);
			}

			return returnTarget;
		}
	}
}
