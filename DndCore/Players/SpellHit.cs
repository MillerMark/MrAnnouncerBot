using System;
using System.Linq;

namespace DndCore
{
	public class SpellHit
	{
		public int TimeOffset { get; set; } = int.MinValue;
		public string EffectName { get; set; }
		public int Hue { get; set; }
		public int Saturation { get; set; } = 100;
		public int Brightness { get; set; } = 100;
		public double Scale { get; set; } = 1;
		public double Rotation { get; set; } = 0;
		public double AutoRotation { get; set; } = 0;
		public SpellHit(string effectName, int hue, int saturation = 100, int brightness = 100, 
			double scale = 1, double rotation = 0, double autoRotation = 0, int timeOffset = int.MinValue)
		{
			EffectName = effectName;
			Hue = hue;
			Brightness = brightness;
			Saturation = saturation;
			Scale = scale;
			Rotation = rotation;
			AutoRotation = autoRotation;
			TimeOffset = timeOffset;
		}
		public SpellHit()
		{

		}
	}
}