using System;
using System.Linq;

namespace DHDM
{
	public class ClockDto
	{
		public string Time { get; set; }
		public bool BigUpdate { get; set; }
		public double Rotation { get; set; }
		public bool InCombat { get; set; }
		public double FullSpins { get; set; }
		public string AfterSpinMp3 { get; set; }
		public ClockDto()
		{

		}
	}
}
