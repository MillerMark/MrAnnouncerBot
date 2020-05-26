//#define profiling
using System;
using System.Linq;

namespace DHDM
{
	public class FrameRateChangeData
	{
		public string OverlayName { get; set; }
		public int FrameRate { get; set; }
		public bool BackgroundCanvasPainting { get; set; }
		public bool ShowFpsWindow { get; set; }
		public bool AllowColorShifting { get; set; }

		public static bool GlobalShowFpsWindow { get; set; }
		public static bool GlobalAllowColorShifting { get; set; } = true;

		public FrameRateChangeData()
		{
			ShowFpsWindow = GlobalShowFpsWindow;
			AllowColorShifting = GlobalAllowColorShifting;
		}
	}
}
