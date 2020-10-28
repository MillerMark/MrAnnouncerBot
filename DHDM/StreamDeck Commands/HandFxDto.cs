using System;
using System.Linq;
using System.Collections.Generic;

namespace DHDM
{
	public enum TargetHand
	{
		None,
		Any,
		Left,
		Right,
		Both
	}
	
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
	public class HandFxDto
	{
		public List<HandEffectDto> HandEffects { get; set; }
		public TargetHand KillFollowEffects { get; set; } = TargetHand.None;
		public HandFxDto()
		{

		}
		public void AddHandEffect(string effectName, int hueShift, int scalePercent)
		{
			if (HandEffects == null)
				HandEffects = new List<HandEffectDto>();
			HandEffects.Add(new HandEffectDto(effectName, hueShift, scalePercent));
		}
	}
}
