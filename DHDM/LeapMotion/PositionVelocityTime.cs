//#define profiling
using Leap;
using System;
using System.Diagnostics;
using System.Linq;

namespace DHDM
{
	[DebuggerDisplay("ms: {Time.TimeOfDay.TotalMilliseconds}, Speed: {LeapVelocity.Magnitude}")]
	public class PositionVelocityTime
	{
		public Vector Position { get; set; }
		public Vector Velocity { get; set; }
		public DateTime Time { get; set; }
		public PositionVelocityTime()
		{
			
		}
	}
}