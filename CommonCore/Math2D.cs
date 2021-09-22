using System;
using System.Drawing;

namespace CommonCore
{
	public static class Math2D
	{

		public static Point2d RotatePoint(Point2d pointToRotate, Point2d centerPoint, double angleInDegrees)
		{
			double angleInRadians = angleInDegrees * (Math.PI / 180);
			double cosTheta = Math.Cos(angleInRadians);
			double sinTheta = Math.Sin(angleInRadians);

			double deltaX = pointToRotate.X - centerPoint.X;
			double deltaY = pointToRotate.Y - centerPoint.Y;

			double newX = cosTheta * deltaX - sinTheta * deltaY + centerPoint.X;
			double newY = sinTheta * deltaX + cosTheta * deltaY + centerPoint.Y;

			return new Point2d(newX, newY);
		}
	}
}
