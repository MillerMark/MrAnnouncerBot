using System;
using System.Drawing;
using System.Collections.Generic;

namespace Imaging
{
	public class CountTotals
	{
		public int Count { get; set; }


		public double weightedSumX { get; set; }
		public double weightedSumY { get; set; }
		public double totalMass { get; set; }

		public double WeightedCenterX()
		{
			return weightedSumX / totalMass;
		}

		public double WeightedCenterY()
		{
			return weightedSumY / totalMass;
		}

		public void Add(int x, int y, byte opacity)
		{
			Count++;

			// Pudding's algorithm:
			weightedSumX += x * opacity;
			weightedSumY += y * opacity;
			totalMass += opacity;
		}
	}
}
