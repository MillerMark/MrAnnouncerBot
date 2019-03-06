using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DndCore
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
		public double secondaryHueShift = 0;
		public double secondarySaturation = 100;
		public double secondaryBrightness = 100;

		public AnimationEffect()
		{
			spriteName = string.Empty;
			target = new VisualEffectTarget(960, 540);
			startFrameIndex = 0;
			hueShift = 0;
			saturation = 100;
			brightness = 100;
			effectKind = EffectKind.Animation;
			target = new VisualEffectTarget();
		}

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
