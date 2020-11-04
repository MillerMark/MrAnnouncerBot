//#define profiling
using Leap;
using System;
using System.Linq;

namespace DHDM
{
	public class Finger2d
	{
		public ScaledPoint TipPosition { get; set; }
		public Finger2d()
		{

		}

		public Finger2d(Finger finger)
		{
			TipPosition = LeapCalibrator.ToScaledPoint(finger.TipPosition);
		}
	}
}