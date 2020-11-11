using Leap;
using System;
using System.Linq;

namespace DHDM
{
	public class Virtual3dObject
	{
		public ThrownObject ThrownObject { get; set; } = new ThrownObject();
		public Vector StartPosition { get; set; }
		public Vector InitialVelocity { get; set; }
		public Vector CurrentPosition { get; set; }
		public DateTime CreationTimeMs { get; set; }

		public int Index
		{
			get => ThrownObject.Index; set => ThrownObject.Index = value;
		}

		public ScaledPoint ToScaledPoint()
		{
			return LeapCalibrator.ToScaledPoint(StartPosition);
		}

		public void Update(DateTime time)
		{
			Vector gravity = new Vector(0, -9.8f, 0); // Meters per second
			CurrentPosition = StartPosition.GetCurrentPosition(gravity, InitialVelocity, time - CreationTimeMs);
			ScaledPoint newPosition = CurrentPosition.ToScaledPoint();
			ThrownObject.Position = newPosition;
			ThrownObject.PositionZ = CurrentPosition.z;
		}

		public Virtual3dObject()
		{
			CreationTimeMs = DateTime.Now;
		}
	}
}