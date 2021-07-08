using System;
using System.Linq;
using System.Timers;
using DndCore;

namespace DHDM
{
	public class SpellEffectTimer : Timer
	{
		public string SpellId { get; set; }
		public string TaleSpireId { get; set; }
		public string EffectName { get; set; }
		public EffectLocation EffectLocation { get; set; }
		public float LifeTime { get; set; }
		public SpellEffectTimer()
		{

		}
	}
}


