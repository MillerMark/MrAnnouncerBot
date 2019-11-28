using System;
using System.Linq;

namespace DndCore
{
	public class SpellHit
	{
		public int Hue { get; set; }
		public int Brightness { get; set; } = 100;
		public string EffectName { get; set; }
		public SpellHit(int hue, int brightness, string effectName)
		{
			EffectName = effectName;
			Hue = hue;
			Brightness = brightness;
		}
		public SpellHit()
		{

		}
	}
}