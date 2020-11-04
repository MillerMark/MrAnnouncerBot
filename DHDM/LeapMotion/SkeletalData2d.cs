//#define profiling
using Leap;
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace DHDM
{
	public class SkeletalData2d
	{
		World3d world = new World3d();
		public List<Hand2d> Hands { get; set; } = new List<Hand2d>();
		public ScaledPlane BackPlane { get; set; } = new ScaledPlane();
		public ScaledPlane FrontPlane { get; set; } = new ScaledPlane();
		public ScaledPlane ActivePlane { get; set; } = new ScaledPlane();
		public bool ShowBackPlane { get; set; }
		public bool ShowFrontPlane { get; set; }
		public bool ShowActivePlane { get; set; }
		public bool ShowLiveHandPosition { get; set; }
		bool hasTrackableEffect;
		bool trackingObjectInLeftHand;
		bool trackingObjectInRightHand;
		HandFxDto handEffect;
		public HandFxDto HandEffect
		{
			get => handEffect; 
			set
			{
				if (handEffect == value)
					return;
				handEffect = value;
				AnalyzeNewHandEffects();
			}
		}

		bool IsTrackableEffect(string effectName)
		{
			// TODO: If we add more trackable effects, include them here.
			// TODO: If we do this a lot, make it data-driven.
			return effectName == "FireBall";
		}

		void AnalyzeNewHandEffects()
		{
			if (handEffect == null)
				return;
			foreach (HandEffectDto handEffectDto in handEffect.HandEffects)
			{
				if (IsTrackableEffect(handEffectDto.EffectName))
				{
					hasTrackableEffect = true;
					return;
				}
			}
		}

		public SkeletalData2d()
		{

		}

		public void SetFromFrame(Frame frame)
		{
			Hands.Clear();
			if (frame == null)
				return;

			foreach (Hand hand in frame.Hands)
				Hands.Add(new Hand2d(hand));

			if (hasTrackableEffect && Hands.Count > 0)
			{
				hasTrackableEffect = false;
				if (Hands[0].Side == HandSide.Left)
					trackingObjectInLeftHand = true;
				else
					trackingObjectInRightHand = true;
			}

			CheckForThrow();
		}

		void CheckForThrow()
		{
			foreach (Hand2d hand in Hands)
				CheckForThrow(hand);
		}

		/// <summary>
		/// Throws the tracked object and returns the index of that object. Returns -1 if no objects found in hand.
		/// </summary>
		int Throw(Hand2d hand, PositionVelocityTime throwVector)
		{
			if (hand.Side == HandSide.Left)
			{
				if (!trackingObjectInLeftHand)
					return -1;
				trackingObjectInLeftHand = false;
			}
			else  // Right hand.
			{
				if (!trackingObjectInRightHand)
					return -1;
				trackingObjectInRightHand = false;
			}

			return world.AddInstance(throwVector);
		}

		void CheckForThrow(Hand2d hand)
		{
			if (!trackingObjectInLeftHand && !trackingObjectInRightHand)
				return;
			PositionVelocityTime throwVector = HandVelocityHistory.GetThrowVector(hand);
			bool validThrow = throwVector.Velocity != null;
			if (!validThrow)
				return;
			HandVelocityHistory.ClearHistory(hand);
			int instanceIndex = Throw(hand, throwVector);
			if (instanceIndex < 0)
				return;
			hand.Throwing = true;
			hand.ThrownObjectIndex = instanceIndex;
			hand.ThrowDirection = Hand2d.GetVectorDirection(throwVector.Velocity);
		}

		public bool ShowingDiagnostics()
		{
			return ShowBackPlane || ShowFrontPlane || ShowActivePlane;
		}
	}
}