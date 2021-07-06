﻿using Leap;
using System;
using System.Linq;
using TaleSpireCore;

namespace DHDM
{
	public static class VectorExtensions
	{
		public static VectorDto ToVectorDto(this DndCore.Vector vector)
		{
			return new VectorDto((float)vector.x, (float)vector.y, (float)vector.z);
		}

		public static DndCore.Vector ToVector(this VectorDto vector)
		{
			return new DndCore.Vector(vector.x, vector.y, vector.z);
		}

		public static Vector ToMetersPerSecond(this Vector leapVectorMmPerSecond)
		{
			const int mmPerMeter = 100;
			return new Vector(leapVectorMmPerSecond.x / mmPerMeter, leapVectorMmPerSecond.y / mmPerMeter, leapVectorMmPerSecond.z / mmPerMeter);
		}

		public static Vector GetCurrentPosition(this Vector vector, Vector force, Vector initialVelocity, TimeSpan deltaTime)
		{
			float deltaSeconds = (float)deltaTime.TotalSeconds;
			float displacementX = Physics.MetersToPixels(Physics.GetDisplacementMeters(deltaSeconds, initialVelocity.x, force.x));
			float displacementY = Physics.MetersToPixels(Physics.GetDisplacementMeters(deltaSeconds, initialVelocity.y, force.y));
			float displacementZ = Physics.MetersToPixels(Physics.GetDisplacementMeters(deltaSeconds, initialVelocity.z, force.z));
			return new Vector(vector.x + displacementX, vector.y + displacementY, vector.z + displacementZ);
		}


		public static Vector GetFinalVelocity(this Vector initialVelocity, Vector force, TimeSpan deltaTime)
		{
			float deltaSeconds = (float)deltaTime.TotalSeconds;
			float velocityX = Physics.GetFinalVelocityMetersPerSecond(deltaSeconds, initialVelocity.x, force.x);
			float velocityY = Physics.GetFinalVelocityMetersPerSecond(deltaSeconds, initialVelocity.y, force.y);
			float velocityZ = Physics.GetFinalVelocityMetersPerSecond(deltaSeconds, initialVelocity.z, force.z);
			return new Vector(velocityX, velocityY, velocityZ);
		}

		public static ScaledPoint ToScaledPoint(this Vector vector)
		{
			return LeapCalibrator.ToScaledPoint(vector);
		}
	}
}