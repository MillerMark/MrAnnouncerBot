using System;

namespace Imaging
{
	public class LiveFeedEdit : LiveFeedData
	{
		public int DeltaX { get; set; }
		public int DeltaY { get; set; }
		public double? RotationOverride { get; set; }
		public double? ScaleOverride { get; set; }
		public double? OpacityOverride { get; set; }

		public LiveFeedEdit()
		{

		}
		public double GetX()
		{
			return Origin.X + DeltaX;
		}

		public double GetY()
		{
			return Origin.Y + DeltaY;
		}
	}
}
