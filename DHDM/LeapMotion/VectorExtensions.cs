using Leap;
using System;
using System.Linq;

namespace DHDM
{
	public static class VectorExtensions
	{
		public static Vector ToMetersPerSecond(this Vector leapVectorMmPerSecond)
		{
			const int mmPerMeter = 100;
			return new Vector(leapVectorMmPerSecond.x / mmPerMeter, leapVectorMmPerSecond.y / mmPerMeter, leapVectorMmPerSecond.z / mmPerMeter);
		}

		public static Vector GetCurrentPosition(this Vector vector, Vector gravity, Vector initialVelocity, TimeSpan deltaTime)
		{
			float deltaSeconds = (float)deltaTime.TotalSeconds;
			float displacementX = Physics.MetersToPixels(Physics.GetDisplacementMeters(deltaSeconds, initialVelocity.x, gravity.x));
			float displacementY = Physics.MetersToPixels(Physics.GetDisplacementMeters(deltaSeconds, initialVelocity.y, gravity.y));
			float displacementZ = Physics.MetersToPixels(Physics.GetDisplacementMeters(deltaSeconds, initialVelocity.z, gravity.z));
			return new Vector(vector.x + displacementX, vector.y + displacementY, vector.z + displacementZ);
		}

		public static ScaledPoint ToScaledPoint(this Vector vector)
		{
			return LeapCalibrator.ToScaledPoint(vector);
		}
	}
}