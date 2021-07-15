using System;
using System.Collections.Generic;
using System.Text;

namespace DndCore
{
	public static class Targeting
	{
		public static void Start(TargetDetails targetDetails, WhatSide whatSide)
		{
			ExpectedSide = whatSide;
			ExpectedTargetDetails = targetDetails;
			IsReady = false;
			ActualKind = TargetKind.None;
			TargetPoint = Vector.zero;
		}

		public static void Ready()
		{
			IsReady = true;
		}

		public static void SetPoint(Vector vector)
		{
			TargetPoint = vector;
			if (ExpectedTargetDetails != null && ExpectedTargetDetails.Kind.HasFlag(TargetKind.Volume))
				ActualKind = TargetKind.Volume;
			else
				ActualKind = TargetKind.Location;
		}

		public static TargetDetails ExpectedTargetDetails { get; set; }
		public static WhatSide ExpectedSide { get; set; }
		public static TargetKind ActualKind { get; set; }
		public static bool IsReady { get; private set; }
		public static Vector TargetPoint { get; set; }

	}
}
