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
		public WindupDto()
		{
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

		public WindupDto Clone()
		{
			WindupDto result = new WindupDto();
			result.AutoRotation = this.AutoRotation;
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
			result.Velocity = this.Velocity;
			result.PrepareForSerialization();
			return result;
		}
		static double GetDouble(string str, double value = 0)
		{
			if (!string.IsNullOrEmpty(str) && double.TryParse(str, out double result))
				return result;
			return value;
		}
		static int GetInt(string str, int value = 0)
		{
			if (!string.IsNullOrEmpty(str) && int.TryParse(str, out int result))
				return result;
			return value;
		}
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
			windupDto.Scale = GetDouble(shortcutDto.scale, 1);
			windupDto.Opacity = GetDouble(shortcutDto.opacity, 1);
			windupDto.Rotation = GetInt(shortcutDto.rotation);
			windupDto.DegreesOffset = GetInt(shortcutDto.degreesOffset);
			windupDto.FlipHorizontal = IsChecked(shortcutDto.flipHorizontal);
			if (IsChecked(shortcutDto.fade))
				windupDto.Fade();
			if (IsChecked(shortcutDto.playToEndOnExpire))
				windupDto.PlayToEndOnExpire = true;
			if (!string.IsNullOrEmpty(shortcutDto.moveUpDown) && int.TryParse(shortcutDto.moveUpDown, out int moveUpDown))
				windupDto.MoveUpDown(moveUpDown);
			windupDto.StartSound = shortcutDto.startSound;
			windupDto.EndSound = shortcutDto.endSound;
			return windupDto;
		}

		public static bool IsChecked(string entry)
		{
			return entry.ToLower().Trim() == "x";
		}
	}
}
