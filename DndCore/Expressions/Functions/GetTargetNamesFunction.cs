using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Returns the names of the players or creatures targeted.")]
	[Param(1, typeof(Target), "target", "The target to check.", ParameterIs.Required)]
	public class GetTargetNamesFunction : DndFunction
	{
		public override string Name => "GetTargetNames";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, CastedSpell spell, RollResults dice = null)
		{
			ExpectingArguments(args, 1);
			Target targetToCheck = Expressions.Get<Target>(args[0], player, target, spell);
			if (targetToCheck == null || player?.Game == null)
				return string.Empty;

			var names = new List<string>();
			foreach (int playerId in targetToCheck.PlayerIds)
			{
				Character targetedPlayer = player.Game.GetPlayerFromId(playerId);
				if (targetedPlayer != null)
					names.Add(targetedPlayer.Name);
			}
			foreach (Creature creature in targetToCheck.Creatures)
				names.Add(creature.Name);

			return string.Join(", ", names);
		}
	}
}
