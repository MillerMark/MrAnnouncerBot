using System;
using System.Linq;
using System.Drawing;
using TaleSpireExplore.Unmanaged;
using TaleSpireCore;

namespace TaleSpireExplore
{
	public static class WindowHelper
	{
		public const int TaleSpireTitleBarHeight = 56;
		public static IntPtr GetTaleSpire()
		{
			return Native.FindWindow("UnityWndClass", "TaleSpire");
		}

		public static void FocusTaleSpire()
		{
			Native.SetForegroundWindow(GetTaleSpire());
		}

		public static Point GetTaleSpireTopRight()
		{
			IntPtr hWnd = GetTaleSpire();

			if (hWnd != IntPtr.Zero)
			{
				Native.GetWindowRect(hWnd, out RECT lpRect);
				return new Point(lpRect.Right, lpRect.Top);
			}
			Talespire.Log.Error($"GetTaleSpireTopRight - Could not find TaleSpire window!!!");
			return Point.Empty;
		}

		public static Point GetTaleSpireTopLeft()
		{
			IntPtr hWnd = GetTaleSpire();

			if (hWnd != IntPtr.Zero)
			{
				Native.GetWindowRect(hWnd, out RECT lpRect);
				return new Point(lpRect.Left, lpRect.Top);
			}
			Talespire.Log.Error($"GetTaleSpireTopLeft - Could not find TaleSpire window!!!");
			return Point.Empty;
		}
	}
}
