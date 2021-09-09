using System;
using System.Drawing;

namespace Imaging
{
	public class VisualProcessingResults
	{
		Point HeadPoint;
		Point LeftShoulder;
		Point RightShoulder;
		public StudioCamera Camera { get; set; } = StudioCamera.Front;
		/// <summary>
		/// The horizontal offset of the anchor, in pixels.
		/// </summary>
		public double X { get; set; }

		/// <summary>
		/// The vertical offset of the anchor, in pixels.
		/// </summary>
		public double Y { get; set; }

		/// <summary>
		/// The scale of the tracked dots, 1 is 100%.
		/// </summary>
		public double Scale { get; set; }

		/// <summary>
		/// Rotation in degrees
		/// </summary>
		public double Rotation { get; set; }

		/// <summary>
		/// Whether the image is flipped left/right.
		/// </summary>
		public bool Flipped { get; set; }

		/// <summary>
		/// 0..1, where 0 is transparent and 1 is fully opaque.
		/// </summary>
		public double Opacity { get; set; }

		public VisualProcessingResults()
		{

		}

		Point GetPoint(CountTotals countTotals)
		{
			if (countTotals.Count == 0)
				return Point.Empty;

			int x = countTotals.XTotal / countTotals.Count;
			int y = countTotals.YTotal / countTotals.Count;
			return new Point(x, y);
		}

		public void Calculate(IntermediateResults intermediateResults)
		{
			Opacity = intermediateResults.GreatestOpacity;
			if (intermediateResults.Yellow.Count > intermediateResults.Green.Count)
			{
				Camera = StudioCamera.Profile;
				HeadPoint = GetPoint(intermediateResults.Yellow);
			}
			else
				HeadPoint = GetPoint(intermediateResults.Green);
			RightShoulder = GetPoint(intermediateResults.Red);
			LeftShoulder = GetPoint(intermediateResults.Blue);
		}
	}
}
