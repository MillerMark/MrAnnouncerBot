//#define profiling
using System;
using System.Linq;
using System.Windows;

namespace DHDM
{
	public class Point2D
	{
		public int X { get; set; }
		public int Y { get; set; }

		public static Point2D From(Point position)
		{
			return new Point2D()
			{
				X = (int)Math.Round(position.X),
				Y = (int)Math.Round(position.Y)
			};
		}
	}
}