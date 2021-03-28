using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Takes a specified temporary spell from the players targeted by the last spell.")]
	[Param(1, typeof(string), "spellName", "The name of the spell to take.", ParameterIs.Required)]
	public class TakeSpellFromTargetFunction : DndFunction
	{
		public override string Name { get; set; } = "TakeSpellFromTarget";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, CastedSpell spell, RollResults dice = null)
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
