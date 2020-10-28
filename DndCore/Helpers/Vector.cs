using System;
using System.Linq;
using System.Collections.Generic;

namespace DndCore
{
	public interface IPosition
	{
		Vector Location { get; set; }
	}

	//public interface ISpellShapeTarget
	//{
	//	SpellTargetShape Shape { get; set; }
	//	bool Contains(Vector vector);
	//	double DistanceTo(Vector vector);
	//}

	public struct Vector
	{
		public static readonly Vector zero = new Vector(0, 0, 0);
		public double x; // game units
		public double y; // game units
		public double z; // game units

		public Vector(double x, double y, double z = 0)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public double DistanceTo(Vector location)
		{
			double deltaX = location.x - x;
			double deltaY = location.y - y;
			double deltaZ = location.z - z;

			return Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);
		}

		public bool AtOrigin()
		{
			return x == 0 && y == 0 && z == 0;
		}
	}
}
