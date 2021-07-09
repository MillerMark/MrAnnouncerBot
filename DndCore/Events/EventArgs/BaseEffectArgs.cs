using System;
using System.Linq;

namespace DndCore
{
	public class BaseEffectArgs : EventArgs
	{
		public string EffectName { get; set; }

		public BaseEffectArgs(string effectName, string iD, string taleSpireId)
		{
			EffectName = effectName;
			SpellId = iD;
			TaleSpireId = taleSpireId;
		}
		public string SpellId { get; set; }
		public string TaleSpireId { get; set; }
	}
}
