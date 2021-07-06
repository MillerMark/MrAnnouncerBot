using System;
using System.Collections.Generic;
using System.Text;

namespace DndCore
{
	public static class Targeting
	{
		public static void Start(TargetDetails targetDetails, WhatSide whatSide)
		{
			Side = whatSide;
			Details = targetDetails;
			IsReady = false;
		}

		public static void Ready()
		{
			IsReady = true;
		}

		public static TargetDetails Details { get; set; }
		public static WhatSide Side { get; set; }
		public static bool IsReady { get; private set; }
	}
}
