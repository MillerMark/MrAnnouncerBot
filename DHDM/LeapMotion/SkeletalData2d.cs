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
		internal World3d world = new World3d();
		public List<ThrownObject> ThrownObjects { get; set; } = new List<ThrownObject>();
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
			CheckForCatch();
			RemoveOutOfBoundsInstances();
		}

		void CheckForThrow()
		{
			foreach (Hand2d hand in Hands)
				CheckForThrow(hand);
		}
		void CheckForCatch()
		{
			foreach (Hand2d hand in Hands)
				CheckForCatch(hand);
		}
		void CheckForCatch(Hand2d hand)
		{
			const float minDistanceForCatch = 240;
			ScaledPoint palmPosition2d = hand.PalmPosition3d.ToScaledPoint();
			List<Virtual3dObject> caughtInstances = new List<Virtual3dObject>();
			lock (world.Instances)
			{
				if (world.Instances.Count == 0)
					return;

				foreach (Virtual3dObject virtual3DObject in world.Instances)
				{
					if (DateTime.Now - virtual3DObject.CreationTimeMs < TimeSpan.FromSeconds(0.25))
					{
						continue;
					}
					virtual3DObject.Update(DateTime.Now);

					ScaledPoint currentPos2d = virtual3DObject.CurrentPosition.ToScaledPoint();


					float deltaX = palmPosition2d.X - currentPos2d.X;
					float deltaY = palmPosition2d.Y - currentPos2d.Y;
					float magnitudeXy = (float)Math.Sqrt((deltaX * deltaX + deltaY * deltaY));
					if (magnitudeXy < minDistanceForCatch)
					{
						caughtInstances.Add(virtual3DObject);
						if (hand.Side == HandSide.Left)
							trackingObjectInLeftHand = true;
						else
							trackingObjectInRightHand = true;
						hand.Throwing = false;
						hand.ThrownObjectIndex = -1;
						hand.JustCaught = true;
					}
				}
			}
			world.RemoveInstances(caughtInstances);
		}

		void RemoveOutOfBoundsInstances()
		{
			const float maxX = 3000;
			const float minX = -3000;
			const float minY = -3000;
			const float maxZ = 3000;
			const float minZ = -3000;
			List<Virtual3dObject> instancesToRemove = new List<Virtual3dObject>();
			lock (world.Instances)
			{
				if (world.Instances.Count == 0)
					return;

				foreach (Virtual3dObject virtual3DObject in world.Instances)
				{
					if (virtual3DObject.CurrentPosition == null)
						continue;
					if (virtual3DObject.CurrentPosition.x > maxX || virtual3DObject.CurrentPosition.x < minX || virtual3DObject.CurrentPosition.y < minY || virtual3DObject.CurrentPosition.z > maxZ || virtual3DObject.CurrentPosition.z < minZ)
					{
						instancesToRemove.Add(virtual3DObject);
					}
				}
			}

			if (instancesToRemove.Count > 0)
				world.RemoveInstances(instancesToRemove);
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

			if (!HasVirtualObjects())
				return;

			lock (world.Instances)
				foreach (Virtual3dObject virtual3DObject in world.Instances)
				{
					ThrownObjects.Add(virtual3DObject.ThrownObject);
				}
		}

		public bool HasVirtualObjects()
		{
			lock (world.Instances)
				return world.Instances.Count > 0;
		}
	}
}