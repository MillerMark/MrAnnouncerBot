using System;
using System.Linq;

namespace DHDM
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
		public VisualEffectTarget target;

		// ![](8AB6F03C5C27C36A9CD817A9F4DCD645.png;;830,167,1347,560;0.04000,0.04000)

		public EmitterShape emitterShape;
		public double radius = 10;
		public double width = 10;
		public double height = 10;
		public double edgeSpread = 1;
		public double emitterGravity = 0;
		public double particleGravity = 0;
		public double particleMass = 1;
		public double minParticleSize = 0.5;
		public TargetValue particleRadius = new TargetValue(1, 0, 0, 0, 100, 0, TargetBinding.truncate);
		public TargetValue hue = new TargetValue(0, 0, 0, 0, 360, 0, TargetBinding.wrap);
		public TargetValue saturation = new TargetValue(1, 0, 0, 0, 100, 0, TargetBinding.truncate);
		public TargetValue brightness = new TargetValue(1, 0, 0, 0, 100, 0, TargetBinding.truncate);
		public double emitterAirDensity = 0;
		public TargetValue particleInitialVelocity = new TargetValue(1, 0, 0, 0, 100, 0, TargetBinding.truncate);
		public Vector particleWindDirection = Vector.zero;
		public Vector bonusVelocityVector = Vector.zero;
		public bool renderOldestParticlesLast = false;
		public double particlesPerSecond = 500;
		public double fadeInTime = 0.4;
		public double lifeSpan = 3;
		public double maxOpacity = 1;
		public double particleAirDensity = 1;
		public double maxConcurrentParticles = 4000;
		public double maxTotalParticles = double.PositiveInfinity;  // "Stop after" in UI
		public Vector emitterWindDirection = Vector.zero;
		public Vector particleInitialDirection = Vector.zero;
		public Vector gravityCenter = new Vector(960, 999999);



		public EmitterEffect()
		{

		}
	}
}
