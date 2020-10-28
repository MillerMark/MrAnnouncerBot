//#define profiling
using System;
using System.Linq;
using System.Collections.Generic;
using Leap;

namespace DHDM
{
	public class Hand2d
	{
		//` ![](D0721EC9212F4FB8CFAB94D72FC8257E.png;;;0.01630,0.01630)

		public float Speed { get; set; }
		public HandSide HandSide { get; set; }
		public PalmDirection PalmDirection { get; set; }
		public ScaledPoint PalmPosition { get; set; }
		public List<Finger2d> Fingers { get; set; } = new List<Finger2d>();
		public bool Throwing { get; set; }
		public int ThrownObjectIndex { get; set; }
		public Hand2d()
		{

		}

		public Hand2d(Hand hand)
		{
			Speed = hand.PalmVelocity.Magnitude;
			SetPalmDirection(hand);
			if (hand.IsLeft)
				HandSide = HandSide.Left;
			else
				HandSide = HandSide.Right;

			HandVelocityHistory.AddVelocity(hand.PalmVelocity, HandSide);
			
			PalmPosition = LeapCalibrator.ToScaledPoint(hand.PalmPosition);
			// TODO: Calculate Details!!!
			foreach (Finger finger in hand.Fingers)
			{
				Fingers.Add(new Finger2d(finger));
			}
		}

		void SetPalmDirection(Hand hand)
		{
			float dotUp = hand.PalmNormal.Dot(Vector.Up);
			float dotRight = hand.PalmNormal.Dot(Vector.Right);
			float dotForward = hand.PalmNormal.Dot(Vector.Forward);
			float dotUpAbs = Math.Abs(dotUp);
			float dotRightAbs = Math.Abs(dotRight);
			float dotForwardAbs = Math.Abs(dotForward);
			if (dotUpAbs > dotRightAbs)
				if (dotUpAbs > dotForwardAbs)
				{
					if (dotUp > 0)
						PalmDirection = PalmDirection.Up;
					else
						PalmDirection = PalmDirection.Down;
				}
				else
				{
					if (dotForwardAbs > 0)
						PalmDirection = PalmDirection.Forward;
					else
						PalmDirection = PalmDirection.Backward;
				}
			else if (dotRightAbs > dotForwardAbs)
			{
				if (dotRight > 0)
					PalmDirection = PalmDirection.Right;
				else
					PalmDirection = PalmDirection.Left;
			}
			else
			{
				if (dotForwardAbs > 0)
					PalmDirection = PalmDirection.Forward;
				else
					PalmDirection = PalmDirection.Backward;
			}
		}
	}
}