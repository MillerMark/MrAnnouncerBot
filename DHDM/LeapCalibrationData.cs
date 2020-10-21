using System;
using System.Linq;
using System.Windows;

namespace DHDM
{
	public class LeapCalibrationData
	{
		public int X { get; set; }
		public int Y { get; set; }
		public double Scale { get; set; } = 1;
		public int DiscoverabilityIndex { get; set; }
		public DndCore.Vector FingertipPosition { get; set; }
		public LeapCalibrationData()
		{

		}
		public void SetDiscoverabilityIndex(LeapMotionCalibrationStep leapMotionCalibrationStep)
		{
			DiscoverabilityIndex = (int)leapMotionCalibrationStep - 1;
		}
		public void SetXY(Point position)
		{
			X = (int)position.X;
			Y = (int)position.Y;
		}
		public void ChangeScale(int delta)
		{
			Scale += delta / 1200.0;
		}
		public void SetFingertipPosition(DndCore.Vector vector)
		{
			FingertipPosition = vector;
		}
	}
}
