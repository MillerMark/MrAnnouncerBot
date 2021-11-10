using System;
using System.Linq;

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
		public double Duration { get; set; } = 1000 / 30; // 30 fps

		public LightSequenceData()
		{

		}
	}
}
