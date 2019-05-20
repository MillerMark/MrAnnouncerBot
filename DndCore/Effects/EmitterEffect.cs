using System;
using System.Linq;

namespace DndCore
{
	public enum TargetBinding
	{
		truncate,
		wrap,
		rock
	}

	public enum EmitterShape
	{
		Circular = 1,
		Rectangular = 2
	}

	public class EmitterEffect : Effect
	{
		public Vector bonusVelocityVector = Vector.zero;
		public TargetValue brightness = new TargetValue(0.5, 0, 0, 0, 100, 0, TargetBinding.truncate);
		public double edgeSpread = 1;
		public double emitterAirDensity = 0;
		public double emitterGravity = 0;
		public Vector emitterInitialVelocity = Vector.zero;

		// ![](8AB6F03C5C27C36A9CD817A9F4DCD645.png;;830,138,1347,560)

		public EmitterShape emitterShape;
		public Vector emitterWindDirection = Vector.zero;
		public double fadeInTime = 0.4;
		public Vector gravityCenter = new Vector(960, 999999);
		public double height = 10;
		public TargetValue hue = new TargetValue(0, 0, 0, 0, 360, 0, TargetBinding.wrap);
		public double lifeSpan = 3;
		public double maxConcurrentParticles = 4000;
		public double maxOpacity = 100;
		public double maxTotalParticles = double.PositiveInfinity;  // "Stop after" in UI
		public double minParticleSize = 0.5;
		public double particleAirDensity = 1;
		public double particleGravity = 0;
		public Vector particleGravityCenter = Vector.zero;
		public Vector particleInitialDirection = Vector.zero;
		public TargetValue particleInitialVelocity = new TargetValue(1, 0, 0, 0, 100, 0, TargetBinding.truncate);
		public double particleMass = 1;
		public TargetValue particleRadius = new TargetValue(1, 0, 0, 0, 100, 0, TargetBinding.truncate);
		public double particlesPerSecond = 500;
		public Vector particleWindDirection = Vector.zero;
		public double radius = 10;
		public bool renderOldestParticlesLast = false;
		public TargetValue saturation = new TargetValue(1, 0, 0, 0, 100, 0, TargetBinding.truncate);
		public VisualEffectTarget target;
		public double width = 10;



		public EmitterEffect()
		{
			target = new VisualEffectTarget();
			effectKind = EffectKind.Emitter;
		}
	}
}
