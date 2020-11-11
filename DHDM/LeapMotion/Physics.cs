//#define profiling
using System;
using System.Linq;

namespace DHDM
{
	public static class Physics

	{
		private const float pixelsPerMeter = 50;

		public static float MetersToPixels(float meters)
		{
			return meters * pixelsPerMeter;
		}

		public static float PixelsToMeters(float pixels)
		{
			return pixels / pixelsPerMeter;
		}

		public static float GetDisplacementMeters(float timeSeconds, float initialVelocity, float acceleration)
		{
			//` <formula #ffffe0;3; d = v_i t + \frac{at^2}{2}>

			return initialVelocity * timeSeconds + acceleration * timeSeconds * timeSeconds / 2.0f;
		}


		public static float GetFinalVelocityMetersPerSecond(float timeSeconds, float initialVelocity, float acceleration)
		{
			//` <formula #ffffe0;1.5; v_f = v_i + at>

			return initialVelocity + acceleration * timeSeconds;
		}

		public static float GetDropTimeSeconds(float distanceMeters, float acceleration, float initialVelocity)
		{
			//` <formula #ffffe0;3; t = \frac{-v_i + \sqrt{v_i^2 + 2ha}}{a}>

			return (-initialVelocity + (float)Math.Sqrt(initialVelocity * initialVelocity + 2 * distanceMeters * acceleration)) / acceleration;
		}
	}
}