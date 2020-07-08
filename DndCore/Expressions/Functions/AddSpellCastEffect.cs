using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Adds the specified visual effect for the active player's next spell cast.")]
	[Param(1, typeof(string), "effectName", "The name of the effect.", ParameterIs.Required, CompletionProviderNames.AnimationEffectName)]
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
	public class AddSpellCastEffect : DndFunction
	{
		public override string Name { get; set; } = "AddSpellCastEffect";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Target target = null, CastedSpell spell = null, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 1, 11);
			if (player != null)
			{
				string effectName;
				int hue, saturation, brightness, timeOffset, secondaryHue, secondarySaturation, secondaryBrightness;
				double scale, rotation, autoRotation;
				GetVisualEffectParameters(args, player, target, spell, out effectName, out hue, out saturation, out brightness, out scale, out rotation, out autoRotation, out timeOffset, out secondaryHue, out secondarySaturation, out secondaryBrightness);

				player.AddSpellCastEffect(effectName, hue, saturation, brightness,
					scale, rotation, autoRotation, timeOffset,
					secondaryHue, secondarySaturation, secondaryBrightness);
			}

			return null;
		}

		public static void GetVisualEffectParameters(List<string> args, Character player, Target target, CastedSpell spell, out string effectName, out int hue, out int saturation, out int brightness, out double scale, out double rotation, out double autoRotation, out int timeOffset, out int secondaryHue, out int secondarySaturation, out int secondaryBrightness)
		{
			effectName = Expressions.GetStr(args[0], player, target, spell);
			hue = 0;
			if (args.Count > 1)
				if (args[1].Trim() == "player")
					hue = player.hueShift;
				else
					hue = Expressions.GetInt(args[1], player, target, spell);

			saturation = 100;
			if (args.Count > 2)
				saturation = Expressions.GetInt(args[2], player, target, spell);

			brightness = 100;
			if (args.Count > 3)
				brightness = Expressions.GetInt(args[3], player, target, spell);

			scale = 1;
			if (args.Count > 4)
				scale = Expressions.GetDouble(args[4], player, target, spell);

			rotation = 0;
			if (args.Count > 5)
				rotation = Expressions.GetDouble(args[5], player, target, spell);


			autoRotation = 0;
			if (args.Count > 6)
				autoRotation = Expressions.GetDouble(args[6], player, target, spell);

			timeOffset = int.MinValue;
			if (args.Count > 7)
				timeOffset = Expressions.GetInt(args[7], player, target, spell);

			secondaryHue = 0;
			if (args.Count > 8)
				if (args[8].Trim() == "player")
					secondaryHue = player.hueShift;
				else
					secondaryHue = Expressions.GetInt(args[8], player, target, spell);

			secondarySaturation = 100;
			if (args.Count > 9)
				secondarySaturation = Expressions.GetInt(args[9], player, target, spell);

			secondaryBrightness = 100;
			if (args.Count > 10)
				secondaryBrightness = Expressions.GetInt(args[10], player, target, spell);
		}
	}
}
