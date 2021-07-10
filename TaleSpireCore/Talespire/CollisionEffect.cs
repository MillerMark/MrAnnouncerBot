using System;
using System.Linq;

namespace TaleSpireCore
{
	public class CollisionEffect
	{
		public string EffectName { get; set; }
		public string SpellId { get; set; }
		public float LifeTime { get; set; }
		public float EnlargeTime { get; set; }
		public float SecondsDelayStart { get; set; }
		public bool UseIntendedTarget { get; set; }

		public CollisionEffect(string effectName, string spellId, float lifeTime, float enlargeTime, float secondsDelayStart, bool useIntendedTarget)
		{
			UseIntendedTarget = useIntendedTarget;
			EffectName = effectName;
			SpellId = spellId;
			LifeTime = lifeTime;
			EnlargeTime = enlargeTime;
			SecondsDelayStart = secondsDelayStart;
		}
		public CollisionEffect()
		{

		}
	}
}



