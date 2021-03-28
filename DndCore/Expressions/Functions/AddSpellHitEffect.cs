using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Adds the specified spell hit effect for the active player's next roll.")]
	[Param(1, typeof(string), "effectName", "The name of the effect.", ParameterIs.Required, CompletionProviderNames.AnimationEffectName)]
	[Param(2, typeof(string), "hue", "The hue of the effect. Can also be the word \"player\" to use the player's hue shift value.", ParameterIs.Optional)]
	[Param(3, typeof(int), "saturation", "The saturation of the effect (0 is gray; 100 is full color). Defaults to 100 if not specified.", ParameterIs.Optional)]
	[Param(4, typeof(int), "brightness", "The brightness of the effect (0 is black; 100 is normal; 200 is white). Defaults to 100 if not specified.", ParameterIs.Optional)]
	[Param(5, typeof(double), "scale", "The scale of the effect (1 is 100%).", ParameterIs.Optional)]
	[Param(6, typeof(double), "rotation", "The degrees to rotate the effect.", ParameterIs.Optional)]
	[Param(7, typeof(double), "autoRotation", "The degrees to rotate the effect per second.", ParameterIs.Optional)]
	[Param(8, typeof(int), "timeOffset", "The time to wait before showing the effect.", ParameterIs.Optional)]
	[Param(9, typeof(int), "secondaryHue", "The secondary hue for effects that have two components (like fireball). Can also be the word \"player\" to use the player's hue shift value.", ParameterIs.Optional)]
	[Param(10, typeof(int), "secondarySaturation", "The secondary saturation for effects that have two animated components (like fireball).", ParameterIs.Optional)]
	[Param(11, typeof(int), "secondaryBrightness", "The secondary brightness for effects that have two animated components (like fireball).", ParameterIs.Optional)]
	[Param(12, typeof(int), "xOffset", "The amount to move this effect on the x-axis.", ParameterIs.Optional)]
	[Param(13, typeof(int), "yOffset", "The amount to move this effect on the y-axis.", ParameterIs.Optional)]
	[Param(14, typeof(double), "velocityX", "The velocity in the x direction.", ParameterIs.Optional)]
	[Param(15, typeof(double), "velocityY", "The velocity in the y direction.", ParameterIs.Optional)]
	public class AddSpellHitEffect : DndFunction
	{
		public override string Name { get; set; } = "AddSpellHitEffect";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target = null, CastedSpell spell = null, RollResults dice = null)
		{
			ExpectingArguments(args, 1, 15);
			if (player != null)
			{
				string effectName;
				int hue, saturation, brightness, timeOffset, secondaryHue, secondarySaturation, secondaryBrightness, yOffset, xOffset;
				double scale, rotation, autoRotation, velocityX, velocityY;
				AddSpellCastEffect.GetVisualEffectParameters(args, player, target, spell, out effectName, out hue, out saturation, out brightness, out scale, out rotation, out autoRotation, out timeOffset, out secondaryHue, out secondarySaturation, out secondaryBrightness, out xOffset, out yOffset, out velocityX, out velocityY);

				player.AddSpellHitEffect(effectName, hue, saturation, brightness,
					scale, rotation, autoRotation, timeOffset,
					secondaryHue, secondarySaturation, secondaryBrightness, xOffset, yOffset, velocityX, velocityY);
			}

			return null;
		}
	}
}
