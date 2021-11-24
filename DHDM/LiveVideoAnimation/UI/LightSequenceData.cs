using System;
using System.Linq;
using System.Collections.Generic;

namespace DHDM
{
	public class LightSequenceData
	{
		public int Hue { get; set; } = 0;
		public int Saturation { get; set; } = 100;
		public int Lightness { get; set; } = 0;

		/// <summary>
		/// The length of time in seconds this data is valid.
		/// </summary>
		public double Duration { get; set; } = 1d / 30d; // 30 fps

		public LightSequenceData()
		{
			
		}

		public bool SameColor(LightSequenceData lightSequenceData)
		{
			return Hue == lightSequenceData.Hue ||
						 Saturation == lightSequenceData.Saturation &&
						 Lightness == lightSequenceData.Lightness;
		}

		public IEnumerable<LightSequenceData> Decompress()
		{
			const double singleFrameDuration = 1d / 30d;
			const double halfFrameDuration = singleFrameDuration / 2d;
			List<LightSequenceData> result = new List<LightSequenceData>();
			double totalDuration = Duration;
			while (totalDuration >= halfFrameDuration)
			{
				LightSequenceData lightSequenceData = new LightSequenceData() { Hue = Hue, Saturation = Saturation, Lightness = Lightness, Duration = singleFrameDuration };
				result.Add(lightSequenceData);
				totalDuration -= singleFrameDuration;
			}
			return result;
		}
	}
}
