//#define profiling
using Leap;
using System;
using System.Linq;

namespace DHDM
{
	public class PositionVelocityTime
	{
		public Vector LeapVelocity { get; set; }
		public Vector Position { get; set; }
		public DateTime Time { get; set; }
		public PositionVelocityTime()
		{

		}
	}
}