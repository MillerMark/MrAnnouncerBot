using System;
using System.Linq;

namespace DndCore
{
	public class ProjectileEffectEventArgs : BaseEffectArgs
	{
		public ProjectileEffectEventArgs(string effectName, string iD, string taleSpireId, float speed, int count, ProjectileKind kind, FireCollisionEventOn fireCollisionEventOn, float launchTimeVariance, float targetVariance, Target target, ProjectileSizeOption projectileSize, float projectileSizeMultiplier) : base(effectName, iD, taleSpireId)
		{
			ProjectileSizeMultiplier = projectileSizeMultiplier;
			ProjectileSize = projectileSize;
			Target = target;
			Speed = speed;
			Count = count;
			Kind = kind;
			FireCollisionEventOn = fireCollisionEventOn;
			LaunchTimeVariance = launchTimeVariance;
			TargetVariance = targetVariance;
		}

		public float Speed { get; set; }
		public int Count { get; set; }
		public ProjectileKind Kind { get; set; }
		public FireCollisionEventOn FireCollisionEventOn { get; set; }
		public float LaunchTimeVariance { get; set; }
		public float TargetVariance { get; set; }
		public Target Target { get; set; }
		public ProjectileSizeOption ProjectileSize { get; set; }
		public float ProjectileSizeMultiplier { get; set; }
	}
}
