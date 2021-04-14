using System;
using System.Linq;
using System.Collections.Generic;

namespace DHDM
{
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
