using System;
using System.Linq;

namespace DndCore
{
	public class SpellEffectEventArgs : EventArgs
	{
		public SpellEffectEventArgs(string effectName, string iD, string taleSpireId, EffectLocation effectLocation = EffectLocation.ActiveCreaturePosition, float lifeTime = 0)
		{
			EffectLocation = effectLocation;
			LifeTime = lifeTime;
			EffectName = effectName;
			SpellId = iD;
			TaleSpireId = taleSpireId;
		}
		public string TaleSpireId { get; set; }
		public string SpellId { get; set; }
		public string EffectName { get; set; }
		public float LifeTime { get; set; }
		public EffectLocation EffectLocation { get; set; }
	}
}
