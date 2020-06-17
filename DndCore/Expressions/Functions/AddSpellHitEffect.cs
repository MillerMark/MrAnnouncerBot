using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Adds the specified spell hit effect for the active player's next roll.")]
	[Param(1, typeof(string), "effectName", "The name of the effect.")]
	[Param(2, typeof(int), "hue", "The hue of the effect. Can also be the word \"player\" to use the player's hue shift value.", ParameterIs.Optional)]
	[Param(3, typeof(int), "saturation", "The saturation of the effect (0 is gray; 100 is full color). Defaults to 100 if not specified.", ParameterIs.Optional)]
	[Param(4, typeof(int), "brightness", "The brightness of the effect (0 is black; 100 is normal; 200 is white). Defaults to 100 if not specified.", ParameterIs.Optional)]
	[Param(5, typeof(double), "scale", "The scale of the effect (1 is 100%).", ParameterIs.Optional)]
	[Param(6, typeof(double), "rotation", "The degrees to rotate the effect.", ParameterIs.Optional)]
	[Param(7, typeof(double), "autoRotation", "The degrees to rotate the effect per second.", ParameterIs.Optional)]
	[Param(8, typeof(int), "timeOffset", "The time to wait before showing the effect.", ParameterIs.Optional)]
	[Param(9, typeof(int), "secondaryHue", "The secondary hue for effects that have two components (like fireball). Can also be the word \"player\" to use the player's hue shift value.", ParameterIs.Optional)]
	[Param(10, typeof(int), "secondarySaturation", "The secondary saturation for effects that have two animated components (like fireball).", ParameterIs.Optional)]
	[Param(11, typeof(int), "secondaryBrightness", "The secondary brightness for effects that have two animated components (like fireball).", ParameterIs.Optional)]
	public class AddSpellHitEffect : DndFunction
	{
		public override string Name { get; set; } = "AddSpellHitEffect";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Target target = null, CastedSpell spell = null, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 1, 11);
			if (player != null)
			{
				string effectName = Expressions.GetStr(args[0], player, target, spell);

				int hue = 0;
				if (args.Count > 1)
					if (args[1].Trim() == "player")
						hue = player.hueShift;
					else
						hue = Expressions.GetInt(args[1], player, target, spell);

				int saturation = 100;
				if (args.Count > 2)
					saturation = Expressions.GetInt(args[2], player, target, spell);

				int brightness = 100;
				if (args.Count > 3)
					brightness = Expressions.GetInt(args[3], player, target, spell);

				double scale = 1;
				if (args.Count > 4)
					scale = Expressions.GetDouble(args[4], player, target, spell);

				double rotation = 0;
				if (args.Count > 5)
					rotation = Expressions.GetDouble(args[5], player, target, spell);


				double autoRotation = 0;
				if (args.Count > 6)
					autoRotation = Expressions.GetDouble(args[6], player, target, spell);

				int timeOffset = int.MinValue;
				if (args.Count > 7)
					timeOffset = Expressions.GetInt(args[7], player, target, spell);

				int secondaryHue = 0;
				if (args.Count > 8)
					if (args[8].Trim() == "player")
						secondaryHue = player.hueShift;
					else
						secondaryHue = Expressions.GetInt(args[8], player, target, spell);

				int secondarySaturation = 100;
				if (args.Count > 9)
					secondarySaturation = Expressions.GetInt(args[9], player, target, spell);

				int secondaryBrightness = 100;
				if (args.Count > 10)
					secondaryBrightness = Expressions.GetInt(args[10], player, target, spell);

				player.AddSpellEffect(effectName, hue, saturation, brightness,
					scale, rotation, autoRotation, timeOffset,
					secondaryHue, secondarySaturation, secondaryBrightness);
			}

			return null;
		}
	}
}
