using System;
using System.Drawing;

namespace Imaging
{
	public class Math2D
	{
		public static Point RotatePoint(Point pointToRotate, Point centerPoint, double angleInDegrees)
		{
			double angleInRadians = angleInDegrees * (Math.PI / 180);
			double cosTheta = Math.Cos(angleInRadians);
			double sinTheta = Math.Sin(angleInRadians);
			return new Point
			{
				X =
							(int)
							(cosTheta * (pointToRotate.X - centerPoint.X) -
							sinTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.X),
				Y =
							(int)
							(sinTheta * (pointToRotate.X - centerPoint.X) +
							cosTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.Y)
			};
		}
	}
}
