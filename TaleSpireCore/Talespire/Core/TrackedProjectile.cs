using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TaleSpireCore
{
	public class TrackedProjectile
	{
		// AnimationCurveExplorerPrefab has the <AnimationCurveExplorer> component
		public Vector3 ActualTargetPosition { get; set; }
		public Vector3 SourcePosition { get; set; }
		public float SpeedFeetPerSecond { get; set; }
		public float StartTime { get; set; }
		public string EffectName { get; set; }
		public string SpellId { get; set; }
		public ProjectileSizeOption ProjectileSize { get; set; }
		public float ProjectileSizeMultiplier { get; set; }
		GameObject projectileGameObject { get; set; }
		public bool readyToDelete { get; set; }
		public string Parameters { get; set; }
		public Vector3 IntendedTargetPosition { get; set; }

		public static event EventHandler TargetReached;

		bool alreadyCreated;
		Vector3 direction;
		float totalTravelDistanceInFeet;
		BezierPath bezierPath;
		AnimationCurve pathTimeCurve;
		AnimationCurve growProjectileCurve;
		AnimationCurve shrinkProjectileCurve;
		AnimationCurve humpProjectileCurve;
		AnimationCurve sizeCurve;

		void TriggerCollision()
		{
			TargetReached?.Invoke(this, EventArgs.Empty);
		}

		AnimationCurve GetAnimationCurve()
		{
			switch (ProjectileSize)
			{
				case ProjectileSizeOption.GrowProjectile:
					return growProjectileCurve;
				case ProjectileSizeOption.ShrinkProjectile:
					return shrinkProjectileCurve;
				case ProjectileSizeOption.HumpProjectile:
					return humpProjectileCurve;
			}
			return null;
		}

		public void UpdatePosition()
		{
			if (StartTime > Time.time)
				return;

			if (!alreadyCreated)
				CreateProjectile();

			if (projectileGameObject == null)
				return;

			float secondsSinceStart = Time.time - StartTime;

			float feetTraveled = SpeedFeetPerSecond * secondsSinceStart;

			if (feetTraveled > totalTravelDistanceInFeet)
			{
				TriggerCollision();
				UnityEngine.Object.Destroy(projectileGameObject);
				readyToDelete = true;
				projectileGameObject = null;
				return;
			}

			float percentTraveled = feetTraveled / totalTravelDistanceInFeet;
			// Linear:
			// Projectile.transform.position = SourcePosition + direction * Talespire.Convert.FeetToTiles(feetTraveled);

			// Bezier path:
			projectileGameObject.transform.position = bezierPath.CalculateBezierPoint(0, pathTimeCurve.Evaluate(percentTraveled));
			projectileGameObject.transform.LookAt(bezierPath.CalculateBezierPoint(0, pathTimeCurve.Evaluate(percentTraveled + 0.01f)));

			float scaleAtTime = 1f;
			if (sizeCurve != null)
				scaleAtTime = sizeCurve.Evaluate(percentTraveled);

			float newScale = scaleAtTime * ProjectileSizeMultiplier;
			if (newScale != projectileGameObject.transform.localScale.x && newScale != 0)
			{
				projectileGameObject.transform.localScale = new Vector3(newScale, newScale, newScale);  // Assuming a uniformly-scaled effect!
				if (particleSystems != null)
					foreach (ParticleSystemDetails particleSystemDetails in particleSystems)
						particleSystemDetails.Scale(newScale);
			}
		}

		List<ParticleSystemDetails> particleSystems;
		void RegisterParticleSystem(ParticleSystem particleSystem)
		{
			if (particleSystems == null)
				particleSystems = new List<ParticleSystemDetails>();
			particleSystems.Add(new ParticleSystemDetails(particleSystem));
		}

		void ApplyColorOverride(Color colorOverride)
		{
			Talespire.Log.Debug($"colorOverride: {colorOverride}");
			Component script = projectileGameObject.GetScript("RFX4_EffectSettings");
			if (script != null)
			{
				Talespire.Log.Warning($"Found script!");
				ReflectionHelper.SetPublicFieldValue(script, "UseCustomColor", true);
				ReflectionHelper.SetPublicFieldValue(script, "EffectColor", colorOverride);
				if (script is MonoBehaviour monoBehaviour)
				{
					monoBehaviour.enabled = false;
					monoBehaviour.enabled = true;
				}
			}
		}

		void ApplyParameter(string leftSide, string rightSide)
		{
			if (leftSide == "color")
			{
				Color colorOverride = new HueSatLight(rightSide).AsRGB.ToUnityColor();
				ApplyColorOverride(colorOverride);
			}
		}

		void ApplyParameter(string parameter)
		{
			int indexOfEquals = parameter.IndexOf('=');
			if (indexOfEquals > 0)
			{
				string leftSide = parameter.Substring(0, indexOfEquals).Trim();
				string rightSide = parameter.Substring(indexOfEquals + 1).Trim();
				ApplyParameter(leftSide, rightSide);
			}
		}
		void ApplyParameters()
		{
			if (string.IsNullOrWhiteSpace(Parameters))
				return;
			string[] parameters = Parameters.Split(';');
			foreach (string parameter in parameters)
				ApplyParameter(parameter);
		}
		private void CreateProjectile()
		{
			alreadyCreated = true;

			projectileGameObject = Talespire.Spells.PlayEffectAtPosition(EffectName, SpellId, SourcePosition.GetVectorDto());
			direction = ActualTargetPosition - SourcePosition;
			float distanceInTiles = direction.magnitude;
			totalTravelDistanceInFeet = Talespire.Convert.TilesToFeet(distanceInTiles);
			Talespire.Log.Debug($"CreateProjectile - totalTravelDistanceInFeet: {totalTravelDistanceInFeet}");
			direction.Normalize();

			Transform transform = projectileGameObject.transform;
			transform.position = SourcePosition;
			transform.forward = direction.normalized;
			float maxInclusive = Mathf.Min(7f, Vector3.Distance(SourcePosition, ActualTargetPosition));
			bezierPath = new BezierPath();
			List<Vector3> newControlPoints = new List<Vector3>();
			newControlPoints.Add(SourcePosition);
			newControlPoints.Add((SourcePosition + (transform.right * UnityEngine.Random.Range(-maxInclusive, maxInclusive))) + (transform.up * UnityEngine.Random.Range(0f, maxInclusive)));
			newControlPoints.Add((ActualTargetPosition + (transform.right * UnityEngine.Random.Range(-maxInclusive, maxInclusive))) + (transform.up * UnityEngine.Random.Range(0f, maxInclusive)));
			newControlPoints.Add(ActualTargetPosition);
			bezierPath.SetControlPoints(newControlPoints);

			ParticleSystem[] particleSystems = projectileGameObject.GetComponentsInChildren<ParticleSystem>();
			if (particleSystems != null)
				foreach (ParticleSystem particleSystem in particleSystems)
					RegisterParticleSystem(particleSystem);

			sizeCurve = GetAnimationCurve();

			ApplyParameters();
		}

		public void Initialize()
		{

		}

		public TrackedProjectile()
		{
			// ![](FAA41F806C5DA538A88BAD89DD38C8D8.png;;0,77,584,433;0.03447,0.03566)
			pathTimeCurve = new AnimationCurve();
			pathTimeCurve.preWrapMode = WrapMode.ClampForever;
			pathTimeCurve.postWrapMode = WrapMode.ClampForever;
			pathTimeCurve.AddKey(new Keyframe(0, 0, 3.69767f, 3.69767f, 0, 0.2363204f));
			pathTimeCurve.AddKey(new Keyframe(0.08789588f, 0.1448911f, 0.4504516f, 0.4504516f, 0.3333333f, 0.130629f));
			pathTimeCurve.AddKey(new Keyframe(0.5344772f, 0.2922845f, 0.3926852f, 0.3926852f, 0.3333333f, 0.1190708f));
			pathTimeCurve.AddKey(new Keyframe(1, 1, 3.894356f, 3.894356f, 0.03830125f, 0));


			//` ![](2B037123FD57230209B54252004354FA.png;;0,62,453,338)
			growProjectileCurve = new AnimationCurve();
			growProjectileCurve.preWrapMode = WrapMode.ClampForever;
			growProjectileCurve.postWrapMode = WrapMode.ClampForever;
			growProjectileCurve.AddKey(new Keyframe(0, 0, 1.146773f, 1.146773f, 0, 0.1319095f));
			growProjectileCurve.AddKey(new Keyframe(1, 1, 3.038543f, 3.038543f, 0.0671643f, 0));

			//` ![](C0B10318CCD1214602C9C295C219DF34.png;;5,66,450,334)
			shrinkProjectileCurve = new AnimationCurve();
			shrinkProjectileCurve.preWrapMode = WrapMode.ClampForever;
			shrinkProjectileCurve.postWrapMode = WrapMode.ClampForever;
			shrinkProjectileCurve.AddKey(new Keyframe(0, 1.002274f, -0.01778922f, -0.01778922f, 0, 0.1278055f));
			shrinkProjectileCurve.AddKey(new Keyframe(0.9937744f, 0.02050018f, -1.995528f, -1.995528f, 0.07466312f, 0));

			//` ![](1DC93544FE420CC37E6BCB1D04536A92.png;;2,63,453,338)
			humpProjectileCurve = new AnimationCurve();
			humpProjectileCurve.preWrapMode = WrapMode.ClampForever;
			humpProjectileCurve.postWrapMode = WrapMode.ClampForever;
			humpProjectileCurve.AddKey(new Keyframe(0, 0, 4.002508f, 4.002508f, 0, 0.07001848f));
			humpProjectileCurve.AddKey(new Keyframe(0.9843445f, 0.02298737f, -4.017225f, -4.017225f, 0.07702912f, 0));
		}
	}
}
