using System;
using System.Linq;
using DndCore;

namespace DndCore
{
	//! Synchronize with WindupData in WindupData.ts.
	public class WindupDto
	{
		public string Effect { get; set; }
		public string Name { get; set; }
		public double Scale { get; set; }
		public double Opacity { get; set; }
		public int Lifespan { get; set; }
		public int FadeIn { get; set; }
		public int FadeOut { get; set; }
		public int Hue { get; set; }
		public int Saturation { get; set; }
		public int Brightness { get; set; }
		public string StartSound { get; set; }
		public string EndSound { get; set; }
		public string Description { get; set; }
		public double Rotation { get; set; }
		public double AutoRotation { get; set; }
		public int DegreesOffset { get; set; }
		public Vector Velocity { get; set; }
		public Vector Offset { get; set; }
		public Vector Force { get; set; }
		public double ForceAmount { get; set; }
		public bool PlayToEndOnExpire { get; set; }
		public bool FlipHorizontal { get; set; }
		public bool FlipVertical { get; set; }
		public string EffectAvailableWhen { get; set; }

		public WindupDto Clone()
		{
			WindupDto result = new WindupDto();
			result.AutoRotation = this.AutoRotation;
			result.Opacity = this.Opacity;
			result.Brightness = this.Brightness;
			result.DegreesOffset = this.DegreesOffset;
			result.Effect = this.Effect;
			result.FadeIn = this.FadeIn;
			result.FadeOut = this.FadeOut;
			result.FlipHorizontal = this.FlipHorizontal;
			result.FlipVertical = this.FlipVertical;
			result.PlayToEndOnExpire = this.PlayToEndOnExpire;
			result.Force = this.Force;
			result.ForceAmount = this.ForceAmount;
			result.Hue = this.Hue;
			result.Lifespan = this.Lifespan;
			result.Name = this.Name;
			result.Offset = this.Offset;
			result.Rotation = this.Rotation;
			result.Saturation = this.Saturation;
			result.Scale = this.Scale;
			result.StartSound = this.StartSound;
			result.EndSound = this.EndSound;
			result.Description = this.Description;
			result.Velocity = this.Velocity;
			result.EffectAvailableWhen = this.EffectAvailableWhen;
			result.PrepareForSerialization();
			return result;
		}

		public WindupDto()
		{
			Opacity = 1;
			Saturation = 100;
			Brightness = 100;
			FadeIn = 400;
			Scale = 1;
			PlayToEndOnExpire = false;
		}
		public WindupDto Float()
		{
			Offset = new Vector(0, 100);
			Velocity = new Vector(0, -1.5);
			Force = new Vector(960, 20000);
			ForceAmount = 0.5;
			return Fade();
		}
		public WindupDto MoveUpDown(int deltaY)
		{
			Offset = new Vector(0, deltaY);
			return this;
		}
		public WindupDto Fade()
		{
			Lifespan = 5500;
			FadeIn = 500;
			FadeOut = 900;
			return this;
		}

		public WindupDto Necrotic()
		{
			Hue = 30;
			Saturation = 40;
			Brightness = 80;
			return this;
		}

		public WindupDto SetBright(int value)
		{
			Brightness = value;
			return this;
		}

		void PrepareForSerialization()
		{
			if (Hue == -1)
				Hue = random.Next(360);
		}

		static Random random = new Random();

		public static WindupDto From(PlayerActionShortcutDto shortcutDto, Character player)
		{
			if (string.IsNullOrEmpty(shortcutDto.effect))
				return null;
			if (shortcutDto.effect.StartsWith("//"))
				return null;
			WindupDto windupDto = new WindupDto();
			windupDto.Effect = shortcutDto.effect;
			if (!string.IsNullOrEmpty(shortcutDto.hue))
				if (shortcutDto.hue == "player")
					windupDto.Hue = player.hueShift;
				else if (int.TryParse(shortcutDto.hue, out int hue))
					windupDto.Hue = hue;
			windupDto.Brightness = MathUtils.GetInt(shortcutDto.brightness, 100);
			windupDto.Saturation = MathUtils.GetInt(shortcutDto.saturation, 100);
			windupDto.Scale = MathUtils.GetDouble(shortcutDto.scale, 1);
			windupDto.Opacity = MathUtils.GetDouble(shortcutDto.opacity, 1);
			windupDto.Rotation = MathUtils.GetInt(shortcutDto.rotation);
			windupDto.DegreesOffset = MathUtils.GetInt(shortcutDto.degreesOffset);
			windupDto.FlipHorizontal = MathUtils.IsChecked(shortcutDto.flipHorizontal);
			if (MathUtils.IsChecked(shortcutDto.fade))
				windupDto.Fade();
			if (MathUtils.IsChecked(shortcutDto.playToEndOnExpire))
				windupDto.PlayToEndOnExpire = true;

			int deltaX = MathUtils.GetInt(shortcutDto.moveLeftRight);
			int deltaY = MathUtils.GetInt(shortcutDto.moveUpDown);
			windupDto.Offset = new Vector(deltaX, deltaY);

			windupDto.StartSound = shortcutDto.startSound;
			windupDto.EndSound = shortcutDto.endSound;
			windupDto.Description = shortcutDto.description;
			return windupDto;
		}

		public static WindupDto FromItemEffect(ItemEffect itemEffect, string effectName)
		{
			WindupDto result = new WindupDto();
			result.Effect = itemEffect.effect;
			result.Name = effectName;
			result.Scale = itemEffect.scale;
			result.Opacity = itemEffect.opacity;
			if (itemEffect.fade)
			{
				result.FadeIn = 500;
				result.FadeOut = 900;
			}

			result.Hue = MathUtils.GetInt(itemEffect.hue);
			result.Saturation = itemEffect.saturation;
			result.Brightness = itemEffect.brightness;
			result.StartSound = itemEffect.startSound;
			result.EndSound = itemEffect.endSound;
			result.Rotation = itemEffect.rotation;
			result.DegreesOffset = (int)Math.Round(itemEffect.degreesOffset);
			result.Offset = new Vector(itemEffect.moveLeftRight, itemEffect.moveUpDown);
			result.PlayToEndOnExpire = itemEffect.playToEndOnExpire;
			result.FlipHorizontal = itemEffect.flipHorizontal;
			result.EffectAvailableWhen = itemEffect.effectAvailableWhen;
			return result;
		}
	}
}
