using System;

namespace Imaging
{
	public class LiveFeedEdit : LiveFeedData
	{
		public double DeltaX { get; set; }
		public double DeltaY { get; set; }
		public double DeltaRotation { get; set; }
		public double DeltaScale { get; set; } = 1;
		public double DeltaOpacity { get; set; } = 1;

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

		public double GetRotation()
		{
			return Rotation + DeltaRotation;
		}

		public double GetOpacity()
		{
			return Opacity * DeltaOpacity;
		}

		public double GetScale()
		{
			return Scale * DeltaScale;
		}
	}
}
