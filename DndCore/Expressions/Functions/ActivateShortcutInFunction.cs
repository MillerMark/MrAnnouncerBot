using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Activates a specified Shortcut after the specified number of milliseconds elapses.")]
	[Param(1, typeof(string), "shortcutName", "The name of the Shortcut to activate.")]
	[Param(2, typeof(int), "delayMs", "The delay in ms to wait until activating the shortcut.")]
	public class ActivateShortcutInFunction : DndFunction
	{
		public override string Name { get; set; } = "ActivateShortcutIn";

		public static void OnActivateShortcutRequest(object sender, ShortcutEventArgs ea)
		{
			ActivateShortcutFunction.OnActivateShortcutRequest(sender, ea);
		}

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Target target, CastedSpell spell, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 2);
			char[] trimChars = { '"', ' ' };
			string shortcutName = args[1].Trim(trimChars);
			if (player == null)
				return null;

			PlayerActionShortcut shortcut = AllActionShortcuts.Get(player.playerID, shortcutName).FirstOrDefault();
			if (shortcut == null)
				return null;

			int delayMs = MathUtils.GetInt(args[0].Trim());
			OnActivateShortcutRequest(player, new ShortcutEventArgs(shortcut, delayMs));
			return null;
		}
	}
}
