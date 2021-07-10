using System;
using System.Linq;

namespace DndCore
{
	public class SpellEffectEventArgs : BaseEffectArgs
	{
		public SpellEffectEventArgs(string effectName, string iD, string taleSpireId, EffectLocation effectLocation = EffectLocation.ActiveCreaturePosition, float lifeTime = 0, float secondsDelayStart = 0, float enlargeTime = 0) : base(effectName, iD, taleSpireId)
		{
			EnlargeTime = enlargeTime;
			SecondsDelayStart = secondsDelayStart;
			EffectLocation = effectLocation;
			LifeTime = lifeTime;
		}
		public float LifeTime { get; set; }
		public EffectLocation EffectLocation { get; set; }
		public float SecondsDelayStart { get; set; }
		public float EnlargeTime { get; set; }
	}
}
