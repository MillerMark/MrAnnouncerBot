//#define profiling
using System;
using System.Linq;
using System.Collections.Generic;
using Leap;

namespace DHDM
{
	public class Hand2d
	{
		//` ![](D0721EC9212F4FB8CFAB94D72FC8257E.png;;;0.02560,0.02560)

		public float Speed { get; set; }
		public float PositionZ { get; set; }
		public HandSide Side { get; set; }
		public VectorCompassDirection PalmDirection { get; set; }
		public VectorCompassDirection FacingForwardOrBack { get; set; }
		public ScaledPoint PalmPosition { get; set; }
		public List<Finger2d> Fingers { get; set; } = new List<Finger2d>();
		public bool Throwing { get; set; }
		public int ThrownObjectIndex { get; set; }
		public VectorCompassDirection ThrowDirection { get; set; }
		public VectorCompassDirection SpeedDirection { get; set; }
		public ScaledPoint PalmAttachPoint { get; set; }
		public bool IsFist { get; set; }
		public bool IsFlat { get; set; }
		public Hand2d()
		{

		}

		public Hand2d(Hand hand)
		{
			Speed = hand.PalmVelocity.Magnitude;
			SpeedDirection = GetVectorDirection(hand.PalmVelocity);
			IsFist = GetIsFist(hand);
			IsFlat = GetIsFlat(hand);
			SetPalmProperties(hand);
			if (hand.IsLeft)
				Side = HandSide.Left;
			else
				Side = HandSide.Right;

			HandVelocityHistory.AddVelocity(hand.PalmVelocity, GetAttachPoint(hand), Side);

			PalmPosition = LeapCalibrator.ToScaledPoint(hand.PalmPosition);
			// TODO: Calculate Details!!!
			foreach (Finger finger in hand.Fingers)
			{
				Fingers.Add(new Finger2d(finger));
			}
		}

		void SetPalmProperties(Hand hand)
		{
			PalmAttachPoint = LeapCalibrator.ToScaledPoint(GetAttachPoint(hand));
			PalmDirection = GetVectorDirection(hand.PalmNormal);
			FacingForwardOrBack = GetForwardOrBack(hand.PalmNormal);
			PositionZ = hand.PalmNormal.z;
		}

		private static Vector GetAttachPoint(Hand hand)
		{
			return hand.PalmPosition + hand.PalmNormal * hand.PalmWidth;
		}

		public static VectorCompassDirection GetVectorDirection(Vector PalmNormal)
		{
			float dotUp = PalmNormal.Dot(Vector.Up);
			float dotRight = PalmNormal.Dot(Vector.Right);
			float dotForward = PalmNormal.Dot(Vector.Forward);
			float dotUpAbs = Math.Abs(dotUp);
			float dotRightAbs = Math.Abs(dotRight);
			float dotForwardAbs = Math.Abs(dotForward);
			if (dotUpAbs > dotRightAbs)
				if (dotUpAbs > dotForwardAbs)
				{
					if (dotUp > 0)
						return VectorCompassDirection.Up;
					else
						return VectorCompassDirection.Down;
				}
				else
				{
					if (dotForward > 0)
						return VectorCompassDirection.Forward;
					else
						return VectorCompassDirection.Backward;
				}
			else if (dotRightAbs > dotForwardAbs)
			{
				if (dotRight > 0)
					return VectorCompassDirection.Right;
				else
					return VectorCompassDirection.Left;
			}
			else
			{
				if (dotForward > 0)
					return VectorCompassDirection.Forward;
				else
					return VectorCompassDirection.Backward;
			}
		}

		public static VectorCompassDirection GetForwardOrBack(Vector PalmNormal)
		{
			float dotForward = PalmNormal.Dot(Vector.Forward);
			if (dotForward > 0)
				return VectorCompassDirection.Forward;
			else
				return VectorCompassDirection.Backward;
		}

		bool GetIsFist(Hand hand)
		{
			double totalLength = GetPalmFingerTravelDistance(hand);
			const double MaxLengthForFist = 170d;
			if (totalLength < MaxLengthForFist)
				return true;
			else
				return false;
		}

		bool GetIsFlat(Hand hand)
		{
			double totalLength = GetPalmFingerTravelDistance(hand);
			const double MinLengthForFlat = 350d;
			if (totalLength > MinLengthForFlat)
				return true;
			else
				return false;
		}

		private static double GetPalmFingerTravelDistance(Hand hand)
		{
			double totalLength = 0;
			double distanceFromPalmToThumb = (hand.Fingers[0].TipPosition - hand.PalmPosition).Magnitude;
			for (var i = 1; i < hand.Fingers.Count; i++)
			{
				Finger previousFinger = hand.Fingers[i - 1];
				Finger thisFinger = hand.Fingers[i];
				double distanceBetweenFingers = (thisFinger.TipPosition - previousFinger.TipPosition).Magnitude;
				totalLength += distanceBetweenFingers;
			}
			double distanceFromPinkieToPalm = (hand.Fingers[4].TipPosition - hand.PalmPosition).Magnitude;
			totalLength += distanceFromPalmToThumb + distanceFromPinkieToPalm;
			return totalLength;
		}
	}
}