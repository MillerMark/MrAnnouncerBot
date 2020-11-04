//#define profiling
using System;
using System.Linq;
using DndCore;

namespace DHDM
{
	public struct ScaledPoint
	{
		public int X { get; set; }
		public int Y { get; set; }
		public double Scale { get; set; }

		public static ScaledPoint From(Vector point3D, double scale)
		{
			return new ScaledPoint() { X = (int)Math.Round(point3D.x), Y = (int)Math.Round(point3D.y), Scale = scale };
		}
	}
}