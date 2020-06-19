//#define profiling
using System;
using System.Linq;

namespace DHDM
{
	[Flags]
	public enum KeyboardModifiers
	{
		None = 0,
		Ctrl = 1,
		Shift = 2,
		Alt = 4
	}
}
