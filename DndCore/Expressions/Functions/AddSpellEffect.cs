using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class AddSpellEffect : DndFunction
	{
		public override string Name { get; set; } = "AddSpellEffect";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Creature target = null, CastedSpell spell = null)
		{
			ExpectingArguments(args, 1, 3);
			if (player != null)
			{
				string effectName = Expressions.GetStr(args[0], player, target, spell);

				int hue = 0;
				if (args.Count > 1)
					hue = Expressions.GetInt(args[1], player, target, spell);
				int brightness = 100;
				if (args.Count > 2)
					brightness = Expressions.GetInt(args[2], player, target, spell);

				player.AddSpellEffect(hue, brightness, effectName);
			}

			return null;
		}
	}
}
