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

		public static bool GlobalShowFpsWindow { get; set; } = false;
		public static bool GlobalAllowColorShifting { get; set; } = true;
		public static bool GlobalAllowCanvasFilterCaching { get; set; } = false;

		public FrameRateChangeData()
		{
			ShowFpsWindow = GlobalShowFpsWindow;
			AllowColorShifting = GlobalAllowColorShifting;
			AllowCanvasFilterCaching = GlobalAllowCanvasFilterCaching;
		}
	}
}
