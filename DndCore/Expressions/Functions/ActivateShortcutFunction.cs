using System;
using System.Collections.Generic;
using System.Linq;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Activates a specified Shortcut.")]
	[Param(1, typeof(string), "shortcutName", "The name of the Shortcut to activate.", ParameterIs.Required)]
	public class ActivateShortcutFunction : DndFunction
	{
		public static event ShortcutEventHandler ActivateShortcutRequest;
		public override string Name { get; set; } = "ActivateShortcut";

		public static void OnActivateShortcutRequest(object sender, ShortcutEventArgs ea)
		{
			ActivateShortcutRequest?.Invoke(sender, ea);
		}

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, CastedSpell spell, RollResults dice = null)
		{
			ExpectingArguments(args, 1);
			char[] trimChars = { '"', ' ' };
			string shortcutName = args[0].Trim(trimChars);
			if (player == null)
				return null;

			PlayerActionShortcut shortcut = AllActionShortcuts.Get(player.IntId, shortcutName).FirstOrDefault();
			if (shortcut == null)
				return null;
			OnActivateShortcutRequest(player, new ShortcutEventArgs(shortcut));
			return null;
		}
	}
}
