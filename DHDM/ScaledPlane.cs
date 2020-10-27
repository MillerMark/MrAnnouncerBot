//#define profiling
using System;
using System.Linq;
using System.Windows;

namespace DHDM
{
	public class ScaledPlane
	{
		public ScaledPoint UpperLeft { get; set; }
		public ScaledPoint LowerRight { get; set; }
		public Point2D UpperLeft2D { get; set; }
		public Point2D LowerRight2D { get; set; }
	}
}