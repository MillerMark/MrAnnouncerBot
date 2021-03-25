//#define profiling
using System;
using System.Linq;

namespace DHDM
{
	public class ContestDto
	{
		public ContestGroup BottomContestants { get; set; }
		public ContestGroup TopContestants { get; set; }
		public ContestSection ActiveSection { get; set; }
		public string Command { get; set; }

		// TODO: Remove SoundEffectToPlay if not ultimately used.
		public string SoundEffectToPlay { get; set; }
		public ContestDto()
		{
			TopContestants = new ContestGroup();
			BottomContestants = new ContestGroup();
		}
	}
}
