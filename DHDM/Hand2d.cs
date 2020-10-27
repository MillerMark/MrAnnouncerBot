//#define profiling
using System;
using System.Linq;
using System.Collections.Generic;
using Leap;

namespace DHDM
{
	public class Hand2d
	{
		public HandDetails Details { get; set; }
		public ScaledPoint PalmPosition { get; set; }
		public List<Finger2d> Fingers { get; set; } = new List<Finger2d>();
		public Hand2d()
		{

		}

		public Hand2d(Hand hand)
		{
			PalmPosition = LeapCalibrator.ToScaledPoint(hand.PalmPosition);
			// TODO: Calculate Details!!!
			foreach (Finger finger in hand.Fingers)
			{
				Fingers.Add(new Finger2d(finger));
			}
		}
	}
}