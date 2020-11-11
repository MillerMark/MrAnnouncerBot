//#define profiling
using Leap;
using System;
using System.Linq;

namespace DHDM
{
	public class FloatingAttachPoint
	{
		public Vector Position { get; set; } = Vector.Zero;
		public Vector LastForce { get; set; } = Vector.Zero;
		public Vector Velocity { get; set; } = Vector.Zero;
		public DateTime SnapshotTime { get; set; } = DateTime.Now;
		public FloatingAttachPoint()
		{

		}
	}
}