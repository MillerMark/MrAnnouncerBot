using System;
using System.Linq;
using UnityEngine;

namespace TaleSpireCore
{
	public class ParticleSystemDetails
	{
		readonly ParticleSystem particleSystem;
		public float OriginalStartSize { get; set; }
		public float OriginalStartSizeCurveMultiplier { get; set; }
		public float OriginalRadius { get; set; }

		public ParticleSystemDetails()
		{

		}

		public ParticleSystemDetails(ParticleSystem particleSystem)
		{
			this.particleSystem = particleSystem;
			OriginalStartSize = particleSystem.startSize;  // Yes obsolete, and still needed.
			OriginalStartSizeCurveMultiplier = particleSystem.main.startSize.curveMultiplier;
			OriginalRadius = particleSystem.shape.radius;
		}

		public void Scale(float factor)
		{
			particleSystem.startSize = OriginalStartSize * factor;  // Yes obsolete, and still needed.
			ParticleSystem.MainModule main = particleSystem.main;
			ParticleSystem.MinMaxCurve startSize = main.startSize;
			startSize.curveMultiplier = OriginalStartSizeCurveMultiplier * factor;
			ParticleSystem.ShapeModule shape = particleSystem.shape;
			shape.radius = OriginalRadius * factor;
		}
	}
}
