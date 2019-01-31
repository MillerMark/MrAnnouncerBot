using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHDM
{
	public struct Vector
	{
		public double x;
		public double y;
		public Vector(double x, double y)
		{
			this.x = x;
			this.y = y;
		}
	}

	class SpritesEffectDto
	{
		public string spriteName;
		public Vector center;
		public int startFrameIndex;
		public double hueShift;
		public double saturation;
		public double brightness;

		public SpritesEffectDto(string spriteName, Vector center, int startFrameIndex, double hueShift = 0, double saturation = -1, double brightness = -1)
		{
			this.spriteName = spriteName;
			this.center = center;
			this.startFrameIndex = startFrameIndex;
			this.hueShift = hueShift;
			this.saturation = saturation;
			this.brightness = brightness;
		}
	}
}
