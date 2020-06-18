using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Adds a random spell hit effect in the specified hue for the next die roll for the active player.")]
	[Param(1, typeof(int), "hue", "The hue of the effect.", ParameterIs.Required)]
	[Param(2, typeof(int), "saturation", "The saturation of the effect (0 is gray; 100 is full color). Defaults to 100 if not specified.", ParameterIs.Optional)]
	[Param(3, typeof(int), "brightness", "The brightness of the effect (0 is black; 100 is normal; 200 is white). Defaults to 100 if not specified.", ParameterIs.Optional)]
	public class AddSpellHit : DndFunction
	{
		public override string Name { get; set; } = "AddSpellHit";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Target target = null, CastedSpell spell = null, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 1, 3);
			if (player != null)
			{
				int hue = Expressions.GetInt(args[0], player, target, spell);

				int saturation = 100;
				if (args.Count == 2)
					saturation = Expressions.GetInt(args[1], player, target, spell);

				int brightness = 100;
				if (args.Count == 3)
					brightness = Expressions.GetInt(args[2], player, target, spell);

				player.AddSpellEffect(hue: hue, saturation: saturation, brightness: brightness);
			}

			return null;
		}
	}
}
