using System;
using System.Linq;

namespace DndCore
{
	public class SpellEffectEventArgs : BaseEffectArgs
	{
		public SpellEffectEventArgs(string effectName, string iD, string taleSpireId, EffectLocation effectLocation = EffectLocation.CreatureBase, 
																float lifeTime = 0, float secondsDelayStart = 0, float enlargeTime = 0, float shrinkTime = 0, float rotationDegrees = 0, float wallLength = 0) : base(effectName, iD, taleSpireId)
		{
			RotationDegrees = rotationDegrees;
			ShrinkTime = shrinkTime;
			EnlargeTime = enlargeTime;
			SecondsDelayStart = secondsDelayStart;
			EffectLocation = effectLocation;
			LifeTime = lifeTime;
			WallLength = wallLength;
		}
		public float LifeTime { get; set; }
		public EffectLocation EffectLocation { get; set; }
		public float SecondsDelayStart { get; set; }
		public float EnlargeTime { get; set; }
		public float ShrinkTime { get; set; }
		public float RotationDegrees { get; set; }
		public float WallLength { get; set; }
	}
}
