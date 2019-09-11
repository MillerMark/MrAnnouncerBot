using System;
using System.Linq;

namespace DndCore
{
	public class TrailingEffect
	{
		public TrailingEffect()
		{

		}

		public int LeftRightDistanceBetweenPrints { get; set; }
		public int MinForwardDistanceBetweenPrints { get; set; }

		public int MedianSoundInterval { get; set; }
		/// <summary>
		/// The number of sound files starting with the base file name OnPrintPlaySound that 
		/// are located in the sound effects folder (or sub-folder if specified).
		/// </summary>
		public string OnPrintPlaySound { get; set; }
		public double PlusMinusSoundInterval { get; set; }

		
		// TODO: Remove TrailingSpriteType and work with strings.
		public TrailingSpriteType Type { get; set; }


		public string Name { get; set; }
		public string OnThrowSound { get; set; }
		public string OnFirstContactSound { get; set; }
		public string OnFirstContactEffect { get; set; }
		public int NumHalos { get; set; }
		public string EffectType { get; set; }
		public int StartIndex { get; set; }
		public int FadeIn { get; set; }
		public int Lifespan { get; set; }
		public int FadeOut { get; set; }
		public double Opacity { get; set; }
		public double Scale { get; set; }
		public string HueShift { get; set; }
		public int HueShiftRandom { get; set; }
		public int Saturation { get; set; }
		public int Brightness { get; set; }
		public int RotationOffset { get; set; }
		public int RotationOffsetRandom { get; set; }
		public bool FlipHalfTime { get; set; }

		public static TrailingEffect From(TrailingEffectsDto dto)
		{
			TrailingEffect trailingEffect = new TrailingEffect();
			trailingEffect.Brightness = MathUtils.GetInt(dto.Brightness, 100);
			trailingEffect.FlipHalfTime = MathUtils.IsChecked(dto.FlipHalfTime);
			trailingEffect.EffectType = dto.EffectType;
			trailingEffect.FadeIn = MathUtils.GetInt(dto.FadeIn);
			trailingEffect.FadeOut = MathUtils.GetInt(dto.FadeOut);
			trailingEffect.HueShift = dto.HueShift;
			trailingEffect.HueShiftRandom = MathUtils.GetInt(dto.HueShiftRandom);
			trailingEffect.LeftRightDistanceBetweenPrints = MathUtils.GetInt(dto.LeftRightDistanceBetweenPrints);
			trailingEffect.Lifespan = MathUtils.GetInt(dto.Lifespan);
			trailingEffect.MedianSoundInterval = MathUtils.GetInt(dto.MedianSoundInterval);
			trailingEffect.MinForwardDistanceBetweenPrints = MathUtils.GetInt(dto.MinForwardDistanceBetweenPrints);
			trailingEffect.Name = dto.Name;
			trailingEffect.OnPrintPlaySound = dto.OnPrintPlaySound;
			trailingEffect.Opacity = MathUtils.GetDouble(dto.Opacity, 1);
			trailingEffect.Saturation = MathUtils.GetInt(dto.Saturation, 100);
			trailingEffect.PlusMinusSoundInterval = MathUtils.GetInt(dto.PlusMinusSoundInterval);
			trailingEffect.RotationOffset = MathUtils.GetInt(dto.RotationOffset);
			trailingEffect.RotationOffsetRandom = MathUtils.GetInt(dto.RotationOffsetRandom);
			trailingEffect.StartIndex = MathUtils.GetInt(dto.StartIndex);
			trailingEffect.Scale = MathUtils.GetDouble(dto.Scale, 1);

			// These are for the higher level roll.... Not sure what to do with these.
			trailingEffect.OnThrowSound = dto.OnThrowSound;
			trailingEffect.NumHalos = MathUtils.GetInt(dto.NumHalos);
			trailingEffect.OnFirstContactEffect = dto.OnFirstContactEffect;
			trailingEffect.OnFirstContactSound = dto.OnFirstContactSound;

			return trailingEffect;
		}

	}
}
