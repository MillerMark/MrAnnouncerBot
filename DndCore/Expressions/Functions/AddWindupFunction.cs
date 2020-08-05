using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Sends the specified windup effect for the active player.")]
	[Param(1, typeof(string), "effect", "The effect to invoke.", ParameterIs.Required, CompletionProviderNames.WindupEffectName)]
	[Param(2, typeof(string), "hue", "The hue of the effect. Can also be the word \"player\" to use the player's hue shift value.", ParameterIs.Optional)]
	[Param(3, typeof(int), "saturation", "The saturation of the effect (0 is gray; 100 is full color). Defaults to 100 if not specified.", ParameterIs.Optional)]
	[Param(4, typeof(int), "brightness", "The brightness of the effect (0 is black; 100 is normal; 200 is white). Defaults to 100 if not specified.", ParameterIs.Optional)]
	[Param(5, typeof(double), "scale", "The scale of the effect (1 is 100%).", ParameterIs.Optional)]
	[Param(6, typeof(double), "rotation", "The degrees to rotate the effect.", ParameterIs.Optional)]
	[Param(7, typeof(double), "autoRotation", "The degrees to rotate the effect per second.", ParameterIs.Optional)]
	[Param(8, typeof(double), "degreesOffset", "The degrees to offset the effect - determines starting frame for orbiting effects.", ParameterIs.Optional)]
	[Param(9, typeof(bool), "flipHorizontal", "Left-right flip. Will effectively reverse the spin for orbiting effects.", ParameterIs.Optional)]
	[Param(10, typeof(int), "xOffset", "The amount to move this effect on the x-axis.", ParameterIs.Optional)]
	[Param(11, typeof(int), "yOffset", "The amount to move this effect on the y-axis.", ParameterIs.Optional)]
	[Param(12, typeof(double), "velocityX", "The velocity in the x direction.", ParameterIs.Optional)]
	[Param(13, typeof(double), "velocityY", "The velocity in the y direction.", ParameterIs.Optional)]
	[Param(14, typeof(double), "forceX", "The force in the x direction.", ParameterIs.Optional)]
	[Param(15, typeof(double), "forceY", "The force in the y direction.", ParameterIs.Optional)]
	[Param(16, typeof(int), "fadeIn", "The time (ms) to fade in.", ParameterIs.Optional)]
	[Param(17, typeof(int), "lifespan", "The time (ms) this effect will stay on screen. Use -1 to stay on screen until removed.", ParameterIs.Optional)]
	[Param(18, typeof(int), "fadeOut", "The time (ms) to fade out.", ParameterIs.Optional)]
	public class AddWindupFunction : DndFunction
	{
		public static event WindupEventHandler RequestAddWindup;
		public override string Name { get; set; } = "AddWindup";

		public static void OnSendWindup(object sender, WindupEventArgs ea)
		{
			RequestAddWindup?.Invoke(sender, ea);
		}

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Target target = null, CastedSpell spell = null, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 1, 18);
			if (player != null)
			{
				GetWindupParameters(args, player, target, spell, out string effectName, out int hue, out int saturation, out int brightness, out double scale,
					out double rotation, out double autoRotation, out double degreesOffset, out bool flipHorizontal, out int xOffset, out int yOffset,
					out double velocityX, out double velocityY, out double forceX, out double forceY, out int fadeIn, out int lifespan, out int fadeOut);

				WindupDto windupDto = new WindupDto(effectName, hue, saturation, brightness,
					scale, rotation, autoRotation, degreesOffset, flipHorizontal,
					xOffset, yOffset, velocityX, velocityY, forceX, forceY, fadeIn, lifespan, fadeOut);
				
				windupDto.Name = GetWindupName(player, spell);

				OnSendWindup(player, new WindupEventArgs(windupDto));
			}

			return null;
		}

		public static string GetWindupName(Character player, CastedSpell spell)
		{
			string spellName = "";
			if (spell?.Spell != null)
				spellName = spell.Spell.Name;
			string windupName = player.Name + "." + spellName;
			return windupName;
		}

		public static void GetWindupParameters(List<string> args, Character player, Target target, CastedSpell spell, 
			out string effectName, out int hue, out int saturation, out int brightness, out double scale, 
			out double rotation, out double autoRotation, out double degreesOffset, out bool flipHorizontal, 
			out int xOffset, out int yOffset, out double velocityX, out double velocityY, out double forceX, out double forceY,
			out int fadeIn, out int lifespan, out int fadeOut)
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

			degreesOffset = 0;
			if (args.Count > 7)
				degreesOffset = Expressions.GetDouble(args[7], player, target, spell);

			flipHorizontal = false;
			if (args.Count > 8)
				flipHorizontal = Expressions.GetBool(args[8], player, target, spell);

			xOffset = 0;
			if (args.Count > 9)
				xOffset = Expressions.GetInt(args[9], player, target, spell);

			yOffset = 0;
			if (args.Count > 10)
				yOffset = Expressions.GetInt(args[10], player, target, spell);

			velocityX = 0;
			if (args.Count > 11)
				velocityX = Expressions.GetDouble(args[11], player, target, spell);

			velocityY = 0;
			if (args.Count > 12)
				velocityY = Expressions.GetDouble(args[12], player, target, spell);

			forceX = 0;
			if (args.Count > 13)
				forceX = Expressions.GetDouble(args[13], player, target, spell);

			forceY = 0;
			if (args.Count > 14)
				forceY = Expressions.GetDouble(args[14], player, target, spell);

			fadeIn = 0;
			if (args.Count > 15)
				fadeIn = Expressions.GetInt(args[15], player, target, spell);

			lifespan = 0;
			if (args.Count > 16)
				lifespan = Expressions.GetInt(args[16], player, target, spell);

			fadeOut = 0;
			if (args.Count > 17)
				fadeOut = Expressions.GetInt(args[17], player, target, spell);
		}
	}
}
