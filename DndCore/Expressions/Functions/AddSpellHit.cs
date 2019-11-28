using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class AddSpellHit : DndFunction
	{
		public override string Name { get; set; } = "AddSpellHit";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Creature target = null, CastedSpell spell = null)
		{
			ExpectingArguments(args, 1, 2);
			if (player != null)
			{
				int hue = Expressions.GetInt(args[0], player, target, spell);
				int brightness = 100;
				if (args.Count == 2)
					brightness = Expressions.GetInt(args[1], player, target, spell);

				player.AddSpellEffect(hue, brightness);
			}

			return null;
		}
	}
}
