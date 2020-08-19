using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Sets the nextAnswer (for the next interactive question) to the specified answer. Useful for Stream Deck buttons that specify spell levels or targets.")]
	[Param(1, typeof(string), "nextAnswer", "The answer to set.", ParameterIs.Required)]
	[Param(2, typeof(string), "playerName", "The name of the player to provide the answer for.", ParameterIs.Optional)]
	public class SetNextAnswer : DndFunction
	{
		public override string Name { get; set; } = "SetNextAnswer";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Target target = null, CastedSpell spell = null, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 1, 2);
			if (args.Count > 1)
				player = AllPlayers.GetFromName(args[1].Trim());
			if (player != null)
			{
				player.SetNextAnswer(Expressions.GetStr(args[0], player, target, spell));
			}
			return null;
		}
	}
}
