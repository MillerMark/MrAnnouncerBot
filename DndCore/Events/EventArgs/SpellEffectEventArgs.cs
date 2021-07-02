using System;
using System.Linq;

namespace DndCore
{
	public class SpellEffectEventArgs : EventArgs
	{
		public SpellEffectEventArgs(string effectName, string iD, string taleSpireId, float lifeTime = 0)
		{
			LifeTime = lifeTime;
			EffectName = effectName;
			SpellId = iD;
			TaleSpireId = taleSpireId;
		}
		public string TaleSpireId { get; set; }
		public string SpellId { get; set; }
		public string EffectName { get; set; }
		public float LifeTime { get; set; }
	}
}
