using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHDM
{
	public class AnimationEffect : Effect
	{
		public string name;
		public string spriteName;
		public VisualEffectTarget target;
		public int startFrameIndex;
		public double hueShift;
		public double saturation;
		public double brightness;

		public AnimationEffect(string spriteName, VisualEffectTarget target, int startFrameIndex, double hueShift = 0, double saturation = -1, double brightness = -1)
		{
			this.spriteName = spriteName;
			this.target = target;
			this.startFrameIndex = startFrameIndex;
			this.hueShift = hueShift;
			this.saturation = saturation;
			this.brightness = brightness;
			effectKind = EffectKind.Animation;
		}
	}
}
