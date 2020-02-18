using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class ActivateShortcutInFunction : DndFunction
	{
		public override string Name { get; set; } = "ActivateShortcutIn";

		public static void OnActivateShortcutRequest(object sender, ShortcutEventArgs ea)
		{
			ActivateShortcutFunction.OnActivateShortcutRequest(sender, ea);
		}

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Creature target, CastedSpell spell)
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
