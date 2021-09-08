using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace TaleSpireExplore.Unmanaged
{
	public static class Native
	{
		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

		[DllImport("user32.dll")]
		public static extern bool SetForegroundWindow(IntPtr hWnd);
	}
}
