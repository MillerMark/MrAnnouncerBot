using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class AddSound : DndFunction
	{
		public override string Name { get; set; } = "AddSound";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Creature target = null, CastedSpell spell = null)
		{
			ExpectingArguments(args, 1, 2);
			if (player != null)
			{
				string fileName = Expressions.GetStr(args[0], player, target, spell);

				int timeOffset = 0;
				if (args.Count > 1)
					timeOffset = Expressions.GetInt(args[1], player, target, spell);

				player.AddSoundEffect(fileName, timeOffset);
			}

			return null;
		}
	}
}
