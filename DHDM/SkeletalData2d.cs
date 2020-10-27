//#define profiling
using Leap;
using System;
using System.Linq;
using System.Collections.Generic;

namespace DHDM
{
	public class SkeletalData2d
	{
		public List<Hand2d> Hands { get; set; } = new List<Hand2d>();
		public ScaledPlane BackPlane { get; set; } = new ScaledPlane();
		public ScaledPlane FrontPlane { get; set; } = new ScaledPlane();
		public ScaledPlane ActivePlane { get; set; } = new ScaledPlane();
		public bool ShowBackPlane { get; set; }
		public bool ShowFrontPlane { get; set; }
		public bool ShowActivePlane { get; set; }

		public SkeletalData2d()
		{

		}
		public void SetFromFrame(Frame frame)
		{
			Hands.Clear();
			if (frame == null)
				return;

			foreach (Hand hand in frame.Hands)
				Hands.Add(new Hand2d(hand));
		}
		public bool ShowingDiagnostics()
		{
			return ShowBackPlane || ShowFrontPlane || ShowActivePlane;
		}
	}
}