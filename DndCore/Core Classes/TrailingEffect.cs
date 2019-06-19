using System;
using System.Linq;

namespace DndCore
{
	public class TrailingEffect
	{
		public TrailingEffect()
		{

		}

		public int Index { get; set; }
		public double LeftRightDistanceBetweenPrints { get; set; }
		public double MinForwardDistanceBetweenPrints { get; set; }
		// TODO: Rename to MedianSoundInterval
		public double MinSoundInterval { get; set; }
		/// <summary>
		/// The number of sound files starting with the base file name OnPrintPlaySound that 
		/// are located in the sound effects folder (or sub-folder if specified).
		/// </summary>
		public int NumRandomSounds { get; set; }
		public string OnPrintPlaySound { get; set; }
		public double PlusMinusSoundInterval { get; set; }
		public TrailingSpriteType Type { get; set; }
	}
}
