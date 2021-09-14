using System;
using System.Drawing;

namespace Imaging
{
	public class VisualProcessingResults
	{
		Point HeadPoint;
		Point LeftShoulder;
		Point RightShoulder;

		/// <summary>
		/// Which camera to use (Front, Right, etc.).
		/// </summary>
		public StudioCamera Camera { get; set; } = StudioCamera.Front;

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

		/// <summary>
		/// The center anchor point of the actor.
		/// </summary>
		public Point Origin { get; set; }

		/// <summary>
		/// The length of time in seconds this data is valid.
		/// </summary>
		public double Duration { get; set; }

		public VisualProcessingResults()
		{
		}

		public bool Matches(VisualProcessingResults obj)
		{
			if (obj == null)
				return false;
			return Flipped == obj.Flipped &&
				Camera == obj.Camera &&
				Rotation == obj.Rotation &&
				Scale == obj.Scale &&
				Opacity == obj.Opacity &&
				Origin == obj.Origin;
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
			Opacity = intermediateResults.GreatestOpacity / 255.0;
			if (intermediateResults.Yellow.Count > intermediateResults.Green.Count)
			{
				Camera = StudioCamera.Profile;
				HeadPoint = GetPoint(intermediateResults.Yellow);
			}
			else
				HeadPoint = GetPoint(intermediateResults.Green);
			RightShoulder = GetPoint(intermediateResults.Red);
			LeftShoulder = GetPoint(intermediateResults.Blue);
			Origin = new Point((RightShoulder.X + LeftShoulder.X) / 2, (RightShoulder.Y + LeftShoulder.Y) / 2);
			CalculateScale();
			if (Scale == 0)
			{
				Opacity = 0;
				Scale = 1;
				Rotation = 0;
			}
			else
				CalculateRotation();
			// TODO: Flipped...
		}

		private void CalculateScale()
		{
			double deltaX = RightShoulder.X - LeftShoulder.X;
			double deltaY = RightShoulder.Y - LeftShoulder.Y;
			double distanceBetweenShoulders = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
			Scale = distanceBetweenShoulders / 119.0;
		}

		private void CalculateRotation()
		{
			double deltaX = HeadPoint.X - Origin.X;
			double deltaY = HeadPoint.Y - Origin.Y;
			Rotation = Math.Atan2(deltaY, deltaX) * 180.0 / Math.PI + 90;
		}
	}
}
