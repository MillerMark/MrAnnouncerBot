using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DndCore
{
	public class AnimationEffect : Effect
	{
		public double brightness;
		public double hueShift;
		public string name;
		public double saturation;
		public double scale = 1;
		public double rotation = 0;
		public double autoRotation = 0;
		public bool horizontalFlip = false;
		public bool verticalFlip = false;
		public double secondaryBrightness = 100;
		public double xOffset = 0;
		public double yOffset = 0;
		public double velocityX = 0;
		public double velocityY = 0;
		public double secondaryHueShift = 0;
		public double secondarySaturation = 100;
		public string spriteName;
		public int startFrameIndex;
		public VisualEffectTarget target;

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

		public AnimationEffect(string spriteName, VisualEffectTarget target, int startFrameIndex,
			double hueShift = 0, double saturation = 100, double brightness = 100,
			double secondaryHueShift = 0, double secondarySaturation = 100, double secondaryBrightness = 100)
		{
			this.spriteName = spriteName;
			this.target = target;
			this.startFrameIndex = startFrameIndex;
			this.hueShift = hueShift;
			this.saturation = saturation;
			this.brightness = brightness;
			this.secondaryHueShift = secondaryHueShift;
			this.secondarySaturation = secondarySaturation;
			this.secondaryBrightness = secondaryBrightness;
			effectKind = EffectKind.Animation;
		}

		public static AnimationEffect CreateEffect(string spriteName, VisualEffectTarget target,
																							 int hueShift = 0, int saturation = 100, int brightness = 100,
																							 int secondaryHueShift = 0, int secondarySaturation = 100, int secondaryBrightness = 100,
																							 int xOffset = 0, int yOffset = 0, double velocityX = 0, double velocityY = 0)
		{
			AnimationEffect spellEffect = new AnimationEffect();
			spellEffect.spriteName = spriteName;
			spellEffect.hueShift = hueShift;
			spellEffect.saturation = saturation;
			spellEffect.brightness = brightness;
			spellEffect.secondaryHueShift = secondaryHueShift;
			spellEffect.secondarySaturation = secondarySaturation;
			spellEffect.secondaryBrightness = secondaryBrightness;
			spellEffect.xOffset = xOffset;
			spellEffect.yOffset = yOffset;
			spellEffect.velocityX = velocityX;
			spellEffect.velocityY = velocityY;
			spellEffect.target = target;
			return spellEffect;
		}
	}
}
