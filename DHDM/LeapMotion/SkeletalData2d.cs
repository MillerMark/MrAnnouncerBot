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
		int trackableEffectHueShift;
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
		public int TrackableEffectHueShiftLeft { get; set; }
		public int TrackableEffectHueShiftRight { get; set; }

		bool IsTrackableEffect(string effectName)
		{

			// TODO: If we add more trackable effects, include them here.
			// TODO: If we do this a lot, make it data-driven.
			return effectName == "FireBall" || effectName == "MagicSmokeLoop";
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
					trackableEffectHueShift = handEffectDto.HueShift;
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

		bool HasRightHand(Frame frame)
		{
			foreach (Hand hand in frame.Hands)
				if (hand.IsRight)
					return true;

			return false;
		}

		bool HasLeftHand(Frame frame)
		{
			foreach (Hand hand in frame.Hands)
				if (hand.IsLeft)
					return true;

			return false;
		}

		public object handsLock = new object();

		public void SetFromFrame(Frame frame)
		{
			lock (handsLock)
			{
				Hands.Clear();
				if (frame == null)
					return;

				foreach (Hand hand in frame.Hands)
					Hands.Add(new Hand2d(hand));
			}

			if (!HasRightHand(frame))
				Hand2d.RightHandFloatPoint = null;
			if (!HasLeftHand(frame))
				Hand2d.LeftHandFloatPoint = null;


			if (hasTrackableEffect && Hands.Count > 0)
			{
				hasTrackableEffect = false;
				trackingObjectInLeftHand = false;
				trackingObjectInRightHand = false;

				// This works, because there is only one hand up when the button is pressed.
				// TODO: Make this work for two hands up while button is pressed.
				if (Hands[0].Side == HandSide.Left)
				{
					TrackableEffectHueShiftLeft = trackableEffectHueShift;
					trackingObjectInLeftHand = true;
				}
				else
				{
					TrackableEffectHueShiftRight = trackableEffectHueShift;
					trackingObjectInRightHand = true;
				}
				trackableEffectHueShift = -1;

				Hands[0].HasLightSource = true;
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
			lock (world.Instances)
			{
				if (world.Instances.Count == 0)
					return;
			}

			const float minXyDistanceForCatch = 200;
			const float minZDistanceForCatch = 280;
			ScaledPoint palmPosition2d = hand.PalmPosition3d.ToScaledPoint();
			List<Virtual3dObject> caughtInstances = new List<Virtual3dObject>();
			lock (world.Instances)
			{
				foreach (Virtual3dObject virtual3DObject in world.Instances)
				{
					if (DateTime.Now - virtual3DObject.CreationTimeMs < TimeSpan.FromSeconds(0.6))
					{
						continue;
					}
					virtual3DObject.Update(DateTime.Now);

					ScaledPoint currentPos2d = virtual3DObject.CurrentPosition.ToScaledPoint();


					float deltaX = palmPosition2d.X - currentPos2d.X;
					float deltaY = palmPosition2d.Y - currentPos2d.Y;
					float magnitudeXy = (float)Math.Sqrt((deltaX * deltaX + deltaY * deltaY));
					float deltaZ = Math.Abs(virtual3DObject.CurrentPosition.z - hand.PalmPosition3d.z);
					if (magnitudeXy < minXyDistanceForCatch && deltaZ < minZDistanceForCatch)
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

		public Vector GetLightPosition(Hand2d hand)
		{
			lock (handsLock)
				lock (world.Instances)
				{
					if (hand.Throwing)
					{
						if (hand.ThrownObjectIndex < 0 || hand.ThrownObjectIndex >= world.Instances.Count)
							return null;
						return world.Instances[hand.ThrownObjectIndex].CurrentPosition;
					}
					else if (hand.Side == HandSide.Right && trackingObjectInRightHand)
						return hand.PalmPosition3d;
					else if (hand.Side == HandSide.Left && trackingObjectInLeftHand)
						return hand.PalmPosition3d;

					return null;
				}
		}

		bool CheckForThrow(Hand2d hand)
		{
			if (!trackingObjectInLeftHand && !trackingObjectInRightHand || hand.IsFist)
				return false;
			PositionVelocityTime throwVector = HandVelocityHistory.GetThrowVector(hand);
			bool validThrow = throwVector.Velocity != null;
			if (!validThrow)
				return false;
			return ThrowObject(hand, throwVector);
		}

		private bool ThrowObject(Hand2d hand, PositionVelocityTime throwVector)
		{
			lock (handsLock)
			{
				HandVelocityHistory.ClearHistory(hand);
				int instanceIndex = Throw(hand, throwVector);
				if (instanceIndex < 0)
					return false;
				hand.Throwing = true;
				hand.ThrownObjectIndex = instanceIndex;
				hand.ThrowDirection = Hand2d.GetVectorDirection(throwVector.Velocity);
				return true;
			}
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

		Hand2d GetTrackingHand()
		{
			HandSide handSide;
			if (trackingObjectInLeftHand)
				handSide = HandSide.Left;
			else if (trackingObjectInRightHand)
				handSide = HandSide.Right;
			else
				return null;

			foreach (Hand2d hand2D in Hands)
				if (hand2D.Side == handSide)
					return hand2D;

			return null;
		}

		public void LaunchTowardFingers(int speed)
		{
			Hand2d hand = GetTrackingHand();
			if (hand == null)
				return;

			PositionVelocityTime launchInfo = new PositionVelocityTime();
			launchInfo.Position = hand.palmAttachPoint3d;
			launchInfo.Velocity = (hand.fingerTipsAttachPoint3d - hand.palmAttachPoint3d).Normalized * speed;
			launchInfo.Time = DateTime.Now;
			ThrowObject(hand, launchInfo);
		}

		public void LaunchToCamera(int speed)
		{
			Hand2d hand = GetTrackingHand();
			if (hand == null)
				return;

			float cameraX = -125;
			float cameraY = 415;
			Vector cameraCenter = new Vector(cameraX, cameraY, (float)LeapCalibrator.frontPlaneZ);
			PositionVelocityTime launchInfo = new PositionVelocityTime();
			launchInfo.Position = hand.palmAttachPoint3d;
			launchInfo.Velocity = (cameraCenter - hand.palmAttachPoint3d).Normalized * speed;
			launchInfo.Time = DateTime.Now;
			ThrowObject(hand, launchInfo);
		}
	}
}