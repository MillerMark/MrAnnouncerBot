using Leap;
using System;
using System.Linq;

namespace DHDM
{
	public class Virtual3dObject
	{
		public Vector Position { get; set; }
		public Vector Velocity { get; set; }
		public DateTime CreationTime { get; set; }
		public int Index { get; set; }

		public ScaledPoint ToScaledPoint()
		{
			return LeapCalibrator.ToScaledPoint(Position);
		}

		public Virtual3dObject()
		{
			CreationTime = DateTime.Now;
		}
	}
}