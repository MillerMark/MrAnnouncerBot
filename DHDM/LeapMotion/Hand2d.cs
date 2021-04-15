//#define profiling
using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Leap;

namespace DHDM
{
	public class Hand2d
	{
		internal Vector palmAttachPoint3d;
		internal Vector fingerTipsAttachPoint3d;
		public static FloatingAttachPoint RightHandFloatPoint { get; set; }
		public static FloatingAttachPoint LeftHandFloatPoint { get; set; }
		
		public ScaledPoint FloatingAttachPoint { get; set; }
		public float Speed { get; set; }
		public float PositionZ { get; set; }
		public HandSide Side { get; set; }
		public VectorCompassDirection PalmDirection { get; set; }
		public VectorCompassDirection FacingForwardOrBack { get; set; }
		public ScaledPoint PalmPosition { get; set; }
		[JsonIgnore]
		public Vector PalmPosition3d { get; set; }
		public List<Finger2d> Fingers { get; set; } = new List<Finger2d>();
		public bool Throwing { get; set; }
		public bool JustCaught { get; set; }
		public int ThrownObjectIndex { get; set; } = -1;
		public VectorCompassDirection ThrowDirection { get; set; }
		public VectorCompassDirection SpeedDirection { get; set; }
		public ScaledPoint PalmAttachPoint { get; set; }
		public bool IsFist { get; set; }
		public bool IsFlat { get; set; }
		public bool HasLightSource { get; set; }
		public DndCore.Vector PalmToFingerVector { get; set; }
		public Hand2d()
		{

		}

		public Hand2d(Hand hand)
		{
			Speed = hand.PalmVelocity.Magnitude;
			SpeedDirection = GetVectorDirection(hand.PalmVelocity);
			FloatingAttachPoint = GetFloatingAttachPoint(hand);
			IsFist = GetIsFist(hand);
			IsFlat = GetIsFlat(hand);
			SetPalmProperties(hand);
			if (hand.IsLeft)
				Side = HandSide.Left;
			else
				Side = HandSide.Right;

			 HandVelocityHistory.AddVelocity(hand.PalmVelocity, palmAttachPoint3d, Side);

			PalmPosition = LeapCalibrator.ToScaledPoint(hand.PalmPosition);
			PalmPosition3d = hand.PalmPosition;
			// TODO: Calculate Details!!!
			foreach (Finger finger in hand.Fingers)
				Fingers.Add(new Finger2d(finger));
		}

		static Vector GetFingerTipsAttachPoint3d(Hand hand)
		{
			Vector averageFingerTipPoint = new Vector();
			if (hand.Fingers.Count > 4)
				for (int i = 1; i < 4; i++)
					averageFingerTipPoint += hand.Fingers[i].TipPosition;
			return averageFingerTipPoint / 3;
		}

		void SetPalmProperties(Hand hand)
		{
			palmAttachPoint3d = GetAttachPoint3d(hand);
			fingerTipsAttachPoint3d = GetFingerTipsAttachPoint3d(hand);
			PalmAttachPoint = LeapCalibrator.ToScaledPoint(palmAttachPoint3d);
			PalmDirection = GetVectorDirection(hand.PalmNormal);
			PalmToFingerVector = GetPalmToFingerVector();
			FacingForwardOrBack = GetForwardOrBack(hand.PalmNormal);
			PositionZ = hand.PalmPosition.z;
		}

		private DndCore.Vector GetPalmToFingerVector()
		{
			Vector palmToFingerVector3D = (fingerTipsAttachPoint3d - palmAttachPoint3d).Normalized;
			return new DndCore.Vector(palmToFingerVector3D.x, palmToFingerVector3D.y);
		}

		public static Vector GetAttachPoint3d(Hand hand)
		{
			return hand.PalmPosition + hand.PalmNormal * hand.PalmWidth / 2;
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
			Vector tippingBack = new Vector(0, 1, 1);
			float dotTipBack = PalmNormal.Dot(tippingBack);
			if (dotTipBack > 0)
			{
				return VectorCompassDirection.Backward;
				// Tipping back
//				System.Diagnostics.Debugger.Break();
			}
			float dotForward = PalmNormal.Dot(Vector.Forward);
			if (dotForward > -0.5)
				return VectorCompassDirection.Forward;
			else
				return VectorCompassDirection.Backward;
		}

		bool GetIsFist(Hand hand)
		{
			double totalLength = GetPalmFingerTravelDistance(hand);
			const double MaxLengthForFist = 190d;
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

		Vector GetSpringForceVector(Vector attachPoint3D, Vector position)
		{
			Vector forceVector = attachPoint3D - position;
			const float k = 1.03f;
			return forceVector * k;

			// Spring force equation (Hook's Law):

			//` <formula 2; F = -kx>
		}

		Vector GetFloatingAttachPoint3d(Vector attachPoint3D, FloatingAttachPoint floatingAttachPoint)
		{
			Vector forceVector = GetSpringForceVector(attachPoint3D, floatingAttachPoint.Position);

			DateTime now = DateTime.Now;
			TimeSpan deltaTime = now - floatingAttachPoint.SnapshotTime;

			Vector currentPosition = floatingAttachPoint.Position.GetCurrentPosition(forceVector, floatingAttachPoint.Velocity, deltaTime);
			Vector finalVelocity = floatingAttachPoint.Velocity.GetFinalVelocity(forceVector, deltaTime) * 0.965f;

			floatingAttachPoint.Velocity = finalVelocity;
			floatingAttachPoint.Position = currentPosition;
			floatingAttachPoint.SnapshotTime = now;
			return currentPosition;
		}

		FloatingAttachPoint NewFloatingAttachPoint(Hand hand)
		{
			FloatingAttachPoint floatingAttachPoint = new FloatingAttachPoint();
			floatingAttachPoint.Position = GetAttachPoint3d(hand);
			return floatingAttachPoint;
		}
		ScaledPoint GetFloatingAttachPoint(Hand hand)
		{
			Vector attachPoint3d = GetAttachPoint3d(hand);

			Vector floatingAttachPoint3d;
			if (hand.IsLeft)
			{
				if (LeftHandFloatPoint == null)
					LeftHandFloatPoint = NewFloatingAttachPoint(hand);

				floatingAttachPoint3d = GetFloatingAttachPoint3d(attachPoint3d, LeftHandFloatPoint);
			}
			else
			{
				if (RightHandFloatPoint == null)
					RightHandFloatPoint = NewFloatingAttachPoint(hand);
				floatingAttachPoint3d = GetFloatingAttachPoint3d(attachPoint3d, RightHandFloatPoint);
			}

			return floatingAttachPoint3d.ToScaledPoint();
		}
	}
}