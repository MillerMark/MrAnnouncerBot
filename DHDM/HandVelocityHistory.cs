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
		private const int MaxHistoryCount = 16;
		static CircularQueue<VelocityTime> leftHandVelocityHistory = new CircularQueue<VelocityTime>();
		static CircularQueue<VelocityTime> rightHandVelocityHistory = new CircularQueue<VelocityTime>();

		public static void AddVelocity(Vector velocity, HandSide handSide)
		{
			CircularQueue<VelocityTime> queue = GetQueue(handSide);

			queue.Enqueue(new VelocityTime() { Velocity = velocity, Time = DateTime.Now });
			if (queue.Count > MaxHistoryCount)
				queue.Dequeue();
		}

		private static CircularQueue<VelocityTime> GetQueue(HandSide handSide)
		{
			CircularQueue<VelocityTime> queue;
			if (handSide == HandSide.Left)
				queue = leftHandVelocityHistory;
			else
				queue = rightHandVelocityHistory;
			return queue;
		}

		static Vector GetThrowVector(CircularQueue<VelocityTime> queue)
		{
			DateTime earliestValidTime = DateTime.Now - TimeSpan.FromSeconds(0.2);
			const double minVelocityForThrow = 430;
			const double maxVelocityForRelease = 100;
			bool escapeVelocityReached = false;
			double fastestThrowingVelocity = 0;
			bool weAreThrowing = false;
			Vector fastestVelocity = new Vector(0, 0, 0);
			for (int i = 0; i < queue.Count; i++)
			{
				if (queue[i].Time < earliestValidTime)  // Data is too stale
					continue;

				if (queue[i].Velocity.Magnitude > minVelocityForThrow)
				{
					if (queue[i].Velocity.Magnitude > fastestThrowingVelocity)
					{
						fastestThrowingVelocity = queue[i].Velocity.Magnitude;
						fastestVelocity = queue[i].Velocity;
					}
					escapeVelocityReached = true;
				}

				if (escapeVelocityReached)
					if (queue[i].Velocity.Magnitude < maxVelocityForRelease)
					{
						weAreThrowing = true;
					}
			}

			if (weAreThrowing)
				return new Vector(fastestVelocity.x, fastestVelocity.y, fastestVelocity.z);
			else
				return new Vector(0, 0, 0);
		}

		public static Vector GetThrowVector(Hand2d hand)
		{
			return GetThrowVector(GetQueue(hand.HandSide));
		}

		static void ClearQueue(HandSide handSide)
		{
			if (handSide == HandSide.Left)
				leftHandVelocityHistory = new CircularQueue<VelocityTime>();
			else
				rightHandVelocityHistory = new CircularQueue<VelocityTime>();
		}

		public static void ClearHistory(Hand2d hand)
		{
			ClearQueue(hand.HandSide);
		}
	}
}