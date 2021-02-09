using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Floats the specified text above the active player.")]
	[Param(1, typeof(string), "message", "The text to show.", ParameterIs.Required)]
	[Param(2, typeof(string), "fillColor", "The fill color for the text. Can be an HTML color or the word \"player\" for the player's fill color.", ParameterIs.Optional)]
	[Param(3, typeof(string), "outlineColor", "The outline color for the text. Can be an HTML color or the word \"player\" for the player's outline color.", ParameterIs.Optional)]
	[Param(4, typeof(int), "delayMs", "The time to wait before showing this message, in milliseconds.", ParameterIs.Optional)]
	public class FloatPlayerText : DndFunction
	{
		public override string Name { get; set; } = "FloatPlayerText";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target = null, CastedSpell spell = null, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 1, 4);
			string fillColor = "player";
			string outlineColor = "player";
			int delayMs = 0;
			if (args.Count > 1)
			{
				fillColor = args[1].Trim();
				if (args.Count > 2)
				{
					outlineColor = args[2].Trim();
					if (args.Count > 3)
						delayMs = Expressions.GetInt(args[3], player, target, spell, dice);
				}
			}

			CreaturePlusModId recipient = Expressions.GetCustomData<CreaturePlusModId>(evaluator.Variables);
			if (recipient != null && recipient.Creature is Character recipientPlayer)
				recipientPlayer.ShowState(Expressions.GetStr(args[0], player, target, spell), fillColor, outlineColor, delayMs);
			else if (player != null)
				player.ShowState(Expressions.GetStr(args[0], player, target, spell), fillColor, outlineColor, delayMs);
			else if (recipient.Creature != null)
			{
				// TODO: Implement FloatCreatureText
				//FloatCreatureText(recipient.Creature, args[0]);
			}
			return null;
		}
	}
}
