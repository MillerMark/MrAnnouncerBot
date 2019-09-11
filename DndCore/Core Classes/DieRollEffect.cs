using System;
using System.Linq;

namespace DndCore
{
	public class DieRollEffect
	{
		public string Name { get; set; }
		public string OnThrowSound { get; set; }
		public string OnFirstContactSound { get; set; }
		public string OnFirstContactEffect { get; set; }
		public int NumHalos { get; set; }
		public int Rotation { get; set; }
		public double Scale { get; set; }
		public string OnStopRollingSound { get; set; }
		public string HueShift { get; set; }
		public int Saturation { get; set; }
		public int Brightness { get; set; }
		public DieRollEffect()
		{

		}

		public static DieRollEffect From(DieRollEffectDto dto)
		{
			DieRollEffect result = new DieRollEffect();
			result.Name = dto.Name;
			result.OnThrowSound = dto.OnThrowSound;
			result.OnFirstContactSound = dto.OnFirstContactSound;
			result.OnFirstContactEffect = dto.OnFirstContactEffect;
			result.NumHalos = MathUtils.GetInt(dto.NumHalos);
			result.Rotation = MathUtils.GetInt(dto.Rotation);
			result.Scale = MathUtils.GetDouble(dto.Scale, 1);
			result.OnStopRollingSound = dto.OnStopRollingSound;
			result.HueShift = dto.HueShift;
			result.Saturation = MathUtils.GetInt(dto.Saturation, 100);
			result.Brightness = MathUtils.GetInt(dto.Brightness, 100);
			

			return result;
		}
	}
}
