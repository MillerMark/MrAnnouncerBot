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
		public bool AllowCanvasFilterCaching { get; set; }
		public int MaxFiltersOnRoll { get; set; }
		public int MaxFiltersOnWindup { get; set; }
		public int MaxFiltersOnDieCleanup { get; set; }
		public static bool GlobalShowFpsWindow { get; set; } = false;
		public static bool GlobalAllowColorShifting { get; set; } = true;
		public static bool GlobalAllowCanvasFilterCaching { get; set; } = false;
		public static int GlobalMaxFiltersOnWindup { get; set; } = 6;
		public static int GlobalMaxFiltersOnRoll { get; set; } = 6;
		public static int GlobalMaxFiltersOnDieCleanup { get; set; } = 6;

		public FrameRateChangeData()
		{
			FrameRate = -1;
			ShowFpsWindow = GlobalShowFpsWindow;
			AllowColorShifting = GlobalAllowColorShifting;
			AllowCanvasFilterCaching = GlobalAllowCanvasFilterCaching;
			MaxFiltersOnDieCleanup = GlobalMaxFiltersOnDieCleanup;
			MaxFiltersOnRoll = GlobalMaxFiltersOnRoll;
			MaxFiltersOnWindup = GlobalMaxFiltersOnWindup;
		}
	}
}
