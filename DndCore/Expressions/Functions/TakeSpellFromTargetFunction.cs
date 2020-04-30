using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class TakeSpellFromTargetFunction : DndFunction
	{
		public override string Name { get; set; } = "TakeSpellFromTarget";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Target target, CastedSpell spell, DiceStoppedRollingData dice = null)
		{
			if (target == null)
				return null;
			ExpectingArguments(args, 1);
			string spellName = args[0];

			if (target == null || player.Game == null)
				return null;

			foreach (int playerId in target.PlayerIds)
			{
				Character recipient = player.Game.GetPlayerFromId(playerId);
				if (recipient != null)
					recipient.TakeSpell(spellName);
			}

			return null;
		}
	}
}
