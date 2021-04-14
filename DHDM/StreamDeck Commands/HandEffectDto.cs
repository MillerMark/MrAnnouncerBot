using System;
using System.Linq;

namespace DHDM
{
	public class HandEffectDto
	{
		// TODO: Exit strategy? Lifetime?
		public string EffectName { get; set; }
		public double Scale { get; set; }
		public int HueShift { get; set; }
		public bool FollowHand { get; set; }
		public int OffsetX { get; set; }
		public int OffsetY { get; set; }
		public TargetHand TargetHand { get; set; } = TargetHand.Any;
		public HandEffectDto()
		{

		}

		public HandEffectDto(string effectName, int hueShift, int scalePercent)
		{
			EffectName = effectName;
			HueShift = hueShift;
			Scale = scalePercent / 100.0;
		}
	}
}
