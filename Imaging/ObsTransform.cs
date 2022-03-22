using CommonCore;
using System;

namespace Imaging
{
	public class ObsTransform : BaseObsTransform
	{
		const double TrackerDistanceBetweenRedAndBlue = 476;
		Point2d HeadPoint;
		Point2d LeftShoulder;
		Point2d RightShoulder;

		private const int INT_Camera0Green = 0;
		private const int INT_Camera1Yellow = 1;
		private const int INT_Camera2Cyan = 2;
		private const int INT_Camera3Magenta = 3;


		/// <summary>
		/// The length of time in seconds this data is valid.
		/// </summary>
		public double Duration { get; set; }

		public ObsTransform()
		{
		}

		public bool Matches(ObsTransform obj)
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

			return new Point2d(countTotals.WeightedCenterX() + 0.5, countTotals.WeightedCenterY() + 0.5);
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

		void SetCameraAndHeadPoint(IntermediateResults intermediateResults)
		{
			int greenCount = intermediateResults.Green.Count;
			int yellowCount = intermediateResults.Yellow.Count;
			int cyanCount = intermediateResults.Cyan.Count;
			int magentaCount = intermediateResults.Magenta.Count;

			if (cyanCount == 0 && yellowCount == 0 && greenCount == 0 && magentaCount == 0)
			{
				Camera = INT_Camera0Green;
				HeadPoint = new Point2d(-1920, 0);
				return;
			}

			if (greenCount > yellowCount)
			{
				if (greenCount > cyanCount)
				{
					if (greenCount > magentaCount)
					{
						Camera = INT_Camera0Green;
						HeadPoint = GetPoint(intermediateResults.Green);
					}
					else
					{
						Camera = INT_Camera3Magenta;
						HeadPoint = GetPoint(intermediateResults.Magenta);
					}
				}
				else // cyanCount is greater than or equal to green.
				{
					if (cyanCount > magentaCount)
					{
						Camera = INT_Camera2Cyan;
						HeadPoint = GetPoint(intermediateResults.Cyan);
					}
					else
					{
						Camera = INT_Camera3Magenta;
						HeadPoint = GetPoint(intermediateResults.Magenta);
					}
				}
			}
			else  // yellow is greater than or equal to green.
			{
				if (yellowCount > cyanCount)
				{
					if (yellowCount > magentaCount)
					{
						Camera = INT_Camera1Yellow;
						HeadPoint = GetPoint(intermediateResults.Yellow);
					}
					else
					{
						Camera = INT_Camera3Magenta;
						HeadPoint = GetPoint(intermediateResults.Magenta);
					}
				}
				else // cyanCount is greater than or equal to yellow.
				{
					if (cyanCount > magentaCount)
					{
						Camera = INT_Camera2Cyan;
						HeadPoint = GetPoint(intermediateResults.Cyan);
					}
					else
					{
						Camera = INT_Camera3Magenta;
						HeadPoint = GetPoint(intermediateResults.Magenta);
					}
				}
			}
		}

		public void Calculate(IntermediateResults intermediateResults)
		{
			Opacity = intermediateResults.GreatestOpacity / 255.0;
			SetCameraAndHeadPoint(intermediateResults);
			
			RightShoulder = GetPoint(intermediateResults.Red);
			LeftShoulder = GetPoint(intermediateResults.Blue);
			double x = Math.Round((RightShoulder.X + LeftShoulder.X) / 2, 1);
			double y = Math.Round((RightShoulder.Y + LeftShoulder.Y) / 2, 1);
			Origin = new Point2d(x, y);
			CalculateScale();
			if (Scale == 0)
			{
				Opacity = 0;
				Scale = 1;
				Rotation = 0;
			}
			else
				CalculateRotation();

			CalculateFlipped();
		}

		private void CalculateScale()
		{
			double deltaX = RightShoulder.X - LeftShoulder.X;
			double deltaY = RightShoulder.Y - LeftShoulder.Y;
			double distanceBetweenShoulders = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
			Scale = Math.Round(distanceBetweenShoulders / TrackerDistanceBetweenRedAndBlue, 3);
		}

		private void CalculateRotation()
		{
			double deltaX = HeadPoint.X - Origin.X;
			double deltaY = HeadPoint.Y - Origin.Y;
			Rotation = Math.Round(Math.Atan2(deltaY, deltaX) * 180.0 / Math.PI + 90, 1);
		}

		public ObsTransformEdit CreateLiveFeedEdit(int frameIndex)
		{
			return new ObsTransformEdit()
			{
				Camera = this.Camera,
				Flipped = this.Flipped,
				Opacity = this.Opacity,
				Origin = this.Origin,
				Rotation = this.Rotation,
				Scale = this.Scale,
				FrameIndex = frameIndex
			};
		}
	}
}
