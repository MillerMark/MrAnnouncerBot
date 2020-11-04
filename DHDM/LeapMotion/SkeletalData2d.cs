//#define profiling
using Leap;
using System;
using System.Linq;
using System.Text;
using System.Timers;
using System.Collections.Generic;

namespace DHDM
{
	public class SkeletalData2d
	{
		private const double fps30 = 1000 / 30;
		World3d world = new World3d();
		public List<ThrownObject> ThrownObjects { get; set; } = new List<ThrownObject>();
		public List<Hand2d> Hands { get; set; } = new List<Hand2d>();
		public ScaledPlane BackPlane { get; set; } = new ScaledPlane();
		public ScaledPlane FrontPlane { get; set; } = new ScaledPlane();
		public ScaledPlane ActivePlane { get; set; } = new ScaledPlane();
		public bool ShowBackPlane { get; set; }
		public bool ShowFrontPlane { get; set; }
		public bool ShowActivePlane { get; set; }
		public bool ShowLiveHandPosition { get; set; }
		Timer timer = new Timer(fps30) { Enabled = true };
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
			timer.Elapsed += Timer_Elapsed;
		}

		private void Timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			// TODO: Deal with this later.
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

		bool CheckForThrow(Hand2d hand)
		{
			if (!trackingObjectInLeftHand && !trackingObjectInRightHand)
				return false;
			PositionVelocityTime throwVector = HandVelocityHistory.GetThrowVector(hand);
			bool validThrow = throwVector.LeapVelocity != null;
			if (!validThrow)
				return false;
			HandVelocityHistory.ClearHistory(hand);
			int instanceIndex = Throw(hand, throwVector);
			if (instanceIndex < 0)
				return false;
			hand.Throwing = true;
			hand.ThrownObjectIndex = instanceIndex;
			hand.ThrowDirection = Hand2d.GetVectorDirection(throwVector.LeapVelocity);
			return true;
		}

		public bool ShowingDiagnostics()
		{
			return ShowBackPlane || ShowFrontPlane || ShowActivePlane;
		}

		public void UpdateVirtualObjects()
		{
			ThrownObjects.Clear();
			world.Update(DateTime.Now);

			if (world.Instances == null || world.Instances.Count == 0)
				return;

			foreach (Virtual3dObject virtual3DObject in world.Instances)
			{
				ThrownObjects.Add(virtual3DObject.ThrownObject);
			}
		}
	}
}