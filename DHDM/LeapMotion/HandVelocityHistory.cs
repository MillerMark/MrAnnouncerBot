//#define profiling
using Leap;
using System;
using System.Linq;
using System.Collections.Generic;
using C5;

namespace DHDM
{
	public static class HandVelocityHistory
	{
		private const int MaxHistoryCount = 64;
		static CircularQueue<PositionVelocityTime> leftHandVelocityHistory = new CircularQueue<PositionVelocityTime>();
		static CircularQueue<PositionVelocityTime> rightHandVelocityHistory = new CircularQueue<PositionVelocityTime>();

		public static void AddVelocity(Vector velocity, Vector position, HandSide handSide)
		{
			CircularQueue<PositionVelocityTime> queue = GetQueue(handSide);
			lock (queue)
			{
				queue.Enqueue(new PositionVelocityTime() { Velocity = velocity, Time = DateTime.Now, Position = position });
				if (queue.Count > MaxHistoryCount)
					queue.Dequeue();
			}
		}

		private static CircularQueue<PositionVelocityTime> GetQueue(HandSide handSide)
		{
			CircularQueue<PositionVelocityTime> queue;
			if (handSide == HandSide.Left)
				queue = leftHandVelocityHistory;
			else
				queue = rightHandVelocityHistory;
			return queue;
		}

		static bool IsValidThrowDirection(Vector velocity)
		{
			VectorCompassDirection vectorDirection = Hand2d.GetVectorDirection(velocity);
			switch (vectorDirection)
			{
				case VectorCompassDirection.Up:
				case VectorCompassDirection.Left:
				case VectorCompassDirection.Right:
				case VectorCompassDirection.Forward:
					return true;
			}
			return false;
		}
		static PositionVelocityTime GetThrowVector(CircularQueue<PositionVelocityTime> queue)
		{
			DateTime earliestValidTime = DateTime.Now - TimeSpan.FromSeconds(1);
			const double minVelocityForThrow = 425;
			const double minDeltaSpeedForRelease = 100;
			bool escapeVelocityReached = false;
			double fastestThrowingSpeed = 0;
			bool weAreThrowing = false;
			Vector fastestVelocity = new Vector(0, 0, 0);
			Vector throwingPoint = new Vector(0, 0, 0);
			DateTime throwingTime = DateTime.MinValue;

			lock (queue)
				for (int i = 0; i < queue.Count; i++)
				{
					PositionVelocityTime checkPt = queue[i];
					if (checkPt.Time < earliestValidTime)  // Data is too stale
						continue;

					float speed = checkPt.Velocity.Magnitude;
					if (speed > minVelocityForThrow && IsValidThrowDirection(checkPt.Velocity))
					{
						escapeVelocityReached = true;
						if (speed > fastestThrowingSpeed)
						{
							fastestThrowingSpeed = speed;
							fastestVelocity = checkPt.Velocity;
							throwingPoint = checkPt.Position;
							throwingTime = checkPt.Time;
							continue;
						}
					}

					if (!escapeVelocityReached)
						continue;

					// HACK: We're not really calculating this correctly.
					if (speed < fastestThrowingSpeed - minDeltaSpeedForRelease || checkPt.Velocity.Dot(fastestVelocity) < 0)
					{
						weAreThrowing = true;
					}
				}

			if (!weAreThrowing)
				return new PositionVelocityTime();

			return new PositionVelocityTime() { Position = throwingPoint, Velocity = fastestVelocity, Time = throwingTime };
		}

		public static PositionVelocityTime GetThrowVector(Hand2d hand)
		{
			return GetThrowVector(GetQueue(hand.Side));
		}

		static void ClearQueue(HandSide handSide)
		{
			if (handSide == HandSide.Left)
				leftHandVelocityHistory = new CircularQueue<PositionVelocityTime>();
			else
				rightHandVelocityHistory = new CircularQueue<PositionVelocityTime>();
		}

		public static void ClearHistory(Hand2d hand)
		{
			ClearQueue(hand.Side);
		}
	}
}