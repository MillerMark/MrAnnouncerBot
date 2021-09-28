using Newtonsoft.Json;
using System;

namespace CommonCore
{
	[JsonConverter(typeof(Point2dConverter))]
	public struct Point2d
	{
		public double X { get; set; }
		public double Y { get; set; }
		public static Point2d Empty => new Point2d(0, 0);

		public Point2d(double x, double y)
		{
			X = x;
			Y = y;
		}

		public static bool operator ==(Point2d left, Point2d right)
		{
			if ((object)left == null)
				return (object)right == null;
			else
				return left.Equals(right);
		}

		public static bool operator !=(Point2d left, Point2d right)
		{
			return !(left == right);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj is Point2d)
				return Equals((Point2d)obj);
			else
				return base.Equals(obj);
		}

		public bool Equals(Point2d obj)
		{
			return obj.X == X && obj.Y == Y;
		}
	}
}
