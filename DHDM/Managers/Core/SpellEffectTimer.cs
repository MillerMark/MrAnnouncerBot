using System;
using System.Linq;
using System.Timers;

namespace DHDM
{
	public class SpellEffectTimer : Timer
	{
		public string SpellId { get; set; }
		public string TaleSpireId { get; set; }
		public string EffectName { get; set; }
		public SpellEffectTimer()
		{

		}
	}
}


