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
		public float ShrinkTime { get; set; }
		public float Rotation { get; set; }
		public bool HitFloor { get; set; }

		public CollisionEffect(string effectName, string spellId, float lifeTime, float enlargeTime, float secondsDelayStart, bool useIntendedTarget, float shrinkTime, float rotation, bool hitFloor)
		{
			HitFloor = hitFloor;
			Rotation = rotation;
			ShrinkTime = shrinkTime;
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



