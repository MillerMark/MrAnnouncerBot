using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Adds the specified shortcut to the action queue.")]
	[Param(1, typeof(string), "shortcutName", "The name of the shortcut to queue.", ParameterIs.Required)]
	[Param(2, typeof(bool), "rollImmediately", "If true, the dice are immediately rolled after this shortcut is activated.", ParameterIs.Optional)]
	public class AddShortcutToQueueFunction : DndFunction
	{
		public override string Name => "AddShortcutToQueue";

		// TODO: Add customData parameter?
		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, CastedSpell spell, RollResults dice = null)
		{
			ExpectingArguments(args, 1, 2);

			string shortcutName = evaluator.Evaluate<string>(args[0]);
			bool rollImmediately = false;
			if (args.Count > 1)
			{
				rollImmediately = Expressions.GetBool(args[1], player, target, spell, dice);
			}

			player.AddShortcutToQueue(shortcutName, rollImmediately);

			return null;
		}
	}
}
