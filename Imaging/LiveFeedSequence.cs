using CommonCore;
using System;

namespace Imaging
{
	public class LiveFeedSequence : LiveFeedData
	{
		Point2d HeadPoint;
		Point2d LeftShoulder;
		Point2d RightShoulder;

		/// <summary>
		/// The length of time in seconds this data is valid.
		/// </summary>
		public double Duration { get; set; }

		public LiveFeedSequence()
		{
		}

		public bool Matches(LiveFeedSequence obj)
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

		Point2d GetPoint(CountTotals countTotals)
		{
			if (countTotals.Count == 0)
				return Point2d.Empty;

			int x = countTotals.XTotal / countTotals.Count;
			int y = countTotals.YTotal / countTotals.Count;
			return new Point2d(x, y);
		}

		void CalculateFlipped()
		{
			// Rotate the red and blue shoulder points around the HeadPoint by -Rotation.
			// Red and blue will be at essentially the same y position.
			// Then see if red is right of blue ==> then we are flipped
			Point2d right = RightShoulder;  // Red
			Point2d left = LeftShoulder;
			Point2d rotatedRight = Math2D.RotatePoint(right, Origin, -Rotation);
			Point2d rotatedLeft = Math2D.RotatePoint(left, Origin, -Rotation);
			if (rotatedRight.X > rotatedLeft.X)  // Because red (on the left in the original image represents the right shoulder of the actor facing the camera).
				Flipped = true;
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
			Origin = new Point2d((RightShoulder.X + LeftShoulder.X) / 2, (RightShoulder.Y + LeftShoulder.Y) / 2);
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
			CalculateFlipped();
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

		public LiveFeedEdit CreateLiveFeedEdit()
		{
			return new LiveFeedEdit()
			{
				Camera = this.Camera,
				Flipped = this.Flipped,
				Opacity = this.Opacity,
				Origin = this.Origin,
				Rotation = this.Rotation,
				Scale = this.Scale
			};
		}
	}
}
