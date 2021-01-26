using System;
using System.Linq;

namespace DndCore
{
	public class TrailingEffectsDto
	{
		public string Name { get; set; }
		public string OnThrowSound { get; set; }
		public string OnFirstContactSound { get; set; }
		public string OnFirstContactEffect { get; set; }
		public string NumHalos { get; set; }
		public string EffectType { get; set; }
		public string StartIndex { get; set; }
		public string FadeIn { get; set; }
		public string Lifespan { get; set; }
		public string FadeOut { get; set; }
		public string Opacity { get; set; }
		public string Scale { get; set; }
		public string ScaleWithVelocity { get; set; }
		public string MinScale { get; set; }
		public string MaxScale { get; set; }
		public string ScaleVariance { get; set; }
		public string HueShift { get; set; }
		public string HueShiftRandom { get; set; }
		public string Saturation { get; set; }
		public string Brightness { get; set; }
		public string RotationOffset { get; set; }
		public string RotationOffsetRandom { get; set; }
		public string FlipHalfTime { get; set; }
		public string LeftRightDistanceBetweenPrints { get; set; }
		public string MinForwardDistanceBetweenPrints { get; set; }
		//public string MedianSoundInterval { get; set; }
		//public string PlusMinusSoundInterval { get; set; }
		public string OnPrintPlaySound { get; set; }
		public TrailingEffectsDto()
		{

		}
	}
}
