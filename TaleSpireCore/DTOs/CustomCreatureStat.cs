using System;
using System.Linq;

namespace TaleSpireCore
{
	public class CustomCreatureStat
	{
		public float Max;
		public float Value;

		public CustomCreatureStat(float value, float max)
		{
			this.Value = value;
			this.Max = max;
		}
	}
}
