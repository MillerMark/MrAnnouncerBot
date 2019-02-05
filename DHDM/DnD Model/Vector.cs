using System;
using System.Linq;

namespace DHDM
{
	public struct Vector
	{
		public static readonly Vector zero = new Vector(0, 0);
		public double x;
		public double y;
		public Vector(double x, double y)
		{
			this.x = x;
			this.y = y;
		}
	}
}
