using System;
using System.Collections.Generic;

namespace DndCore
{
	public class EffectEventArgs : EventArgs
	{
		public string EffectName { get; set; }
		public int Hue { get; set; }
		public int Saturation { get; set; }
		public int Brightness { get; set; }
		public int TimeOffset { get; set; }
		public int SecondaryHue { get; set; }
		public int SecondarySaturation { get; set; }
		public int SecondaryBrightness { get; set; }
		public int OffsetX { get; set; }
		public int OffsetY { get; set; }
		public double Scale { get; set; }
		public double Rotation { get; set; }
		public double AutoRotation { get; set; }
		public double VelocityX { get; set; }
		public double VelocityY { get; set; }

		public EffectEventArgs(List<string> args, Character player, Target target, CastedSpell spell)
		{
			string effectName;
			int hue, saturation, brightness, timeOffset, secondaryHue, secondarySaturation, secondaryBrightness, offsetX, offsetY;
			double scale, rotation, autoRotation, velocityX, velocityY;

			AddSpellCastEffect.GetVisualEffectParameters(args, player, target, spell, out effectName, out hue, out saturation, out brightness, out scale, out rotation, out autoRotation, out timeOffset, out secondaryHue, out secondarySaturation, out secondaryBrightness, out offsetX, out offsetY, out velocityX, out velocityY);

			EffectName = effectName;
			Hue = hue;
			Saturation = saturation;
			Brightness = brightness;
			TimeOffset = timeOffset;
			SecondaryHue = secondaryHue;
			SecondarySaturation = secondarySaturation;
			SecondaryBrightness = secondaryBrightness;
			OffsetX = offsetX;
			OffsetY = offsetY;
			Scale = scale;
			Rotation = rotation;
			AutoRotation = autoRotation;
			VelocityX = velocityX;
			VelocityY = velocityY;
		}
	}
}
