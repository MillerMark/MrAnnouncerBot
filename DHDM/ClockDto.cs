using System;
using System.Linq;

namespace DHDM
{
	public class ClockDto
	{
		public string Message { get; set; }
		public string Time { get; set; }
		public bool BigUpdate { get; set; }
		public double Rotation { get; set; }
		public bool InCombat { get; set; }
		public bool InTimeFreeze { get; set; }
		public double FullSpins { get; set; }
		public string AfterSpinMp3 { get; set; }
		public bool HideClock { get; set; }
		public ClockDto()
		{

		}
	}
}
