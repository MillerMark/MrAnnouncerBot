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

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, CastedSpell spell, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 1);
			Target targetToCheck = Expressions.Get<Target>(args[0], player, target, spell);
			string result = string.Empty;
			if (targetToCheck != null)
			{
				if (player?.Game != null)
				{
					foreach (int playerId in targetToCheck.PlayerIds)
					{
						Character targetedPlayer = player.Game.GetPlayerFromId(playerId);
						if (targetedPlayer != null)
						{
							result += targetedPlayer.Name + ", ";
						}
					}

					foreach (Creature creature in targetToCheck.Creatures)
					{
						result += creature.Name + ", ";
					}

					result = result.EverythingBeforeLast(", ");
				}
			}

			return result;
		}
	}
}
