using System;
using System.Linq;

namespace DndCore
{
	public class DieRollEffectDto
	{
		public string Name { get; set; }
		public string OnThrowSound { get; set; }
		public string OnFirstContactSound { get; set; }
		public string OnFirstContactEffect { get; set; }
		public string NumHalos { get; set; }
		public string Rotation { get; set; }
		public string Scale { get; set; }
		public string OnStopRollingSound { get; set; }
		public string HueShift { get; set; }
		public string Saturation { get; set; }
		public string Brightness { get; set; }
		public DieRollEffectDto()
		{

		}
	}
}
