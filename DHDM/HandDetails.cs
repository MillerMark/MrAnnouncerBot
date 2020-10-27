//#define profiling
using System;
using System.Linq;

namespace DHDM
{
	[Flags]
	public enum HandDetails
	{
		FacingUp = 1,
		FingersClosed = 2,
		FingersOpened = 4
	}
}