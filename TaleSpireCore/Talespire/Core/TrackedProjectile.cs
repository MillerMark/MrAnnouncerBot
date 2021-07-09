using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TaleSpireCore
{
	public class TrackedProjectile
	{
		// AnimationCurveExplorerPrefab has the <AnimationCurveExplorer> component
		public bool ReadyToDelete { get; set; }
		public GameObject Projectile { get; set; }
		public Vector3 TargetPosition { get; set; }
		public Vector3 SourcePosition { get; set; }
		public float SpeedFeetPerSecond { get; set; }
		public float StartTime { get; set; }
		public string EffectName { get; set; }
		public string SpellId { get; set; }
		public ProjectileSizeOption ProjectileSize { get; set; }
		public float ProjectileSizeMultiplier { get; set; }
		public event EventHandler TargetReached;

		bool alreadyCreated;
		Vector3 direction;
		float totalTravelDistanceInFeet;
		BezierPath bezierPath;
		AnimationCurve pathTimeCurve;
		AnimationCurve growProjectileCurve;
		AnimationCurve shrinkProjectileCurve;
		AnimationCurve humpProjectileCurve;

		void TriggerCollision()
		{
			// TODO: Trigger collision and corresponding effects that are waiting to fire.
		}

		public void UpdatePosition()
		{
			if (StartTime > Time.time)
				return;

			if (!alreadyCreated)
				CreateProjectile();

			if (Projectile == null)
				return;

			float secondsSinceStart = Time.time - StartTime;

			float feetTraveled = SpeedFeetPerSecond * secondsSinceStart;
			
			if (feetTraveled > totalTravelDistanceInFeet)
			{
				TriggerCollision();
				UnityEngine.Object.Destroy(Projectile);
				ReadyToDelete = true;
				Projectile = null;
				return;
			}

			float percentTraveled = feetTraveled / totalTravelDistanceInFeet;
			// Linear:
			// Projectile.transform.position = SourcePosition + direction * Talespire.Convert.FeetToTiles(feetTraveled);

			// Bezier path:
			Projectile.transform.position = bezierPath.CalculateBezierPoint(0, pathTimeCurve.Evaluate(percentTraveled));
			Projectile.transform.LookAt(bezierPath.CalculateBezierPoint(0, pathTimeCurve.Evaluate(percentTraveled + 0.01f)));

			// TODO: Modify scale as it moves based on ProjectileSize and ProjectileSizeMultiplier.
			//Projectile.transform.localScale = new Vector3(ProjectileSizeMultiplier);
		}

		private void CreateProjectile()
		{
			alreadyCreated = true;
			Projectile = Talespire.Spells.PlayEffectAtPosition(EffectName, SpellId, SourcePosition.GetVectorDto());
			direction = TargetPosition - SourcePosition;
			float distanceInTiles = direction.magnitude;
			totalTravelDistanceInFeet = Talespire.Convert.TilesToFeet(distanceInTiles);
			Talespire.Log.Debug($"CreateProjectile - totalTravelDistanceInFeet: {totalTravelDistanceInFeet}");
			direction.Normalize();

			Transform transform = Projectile.transform;
			transform.position = SourcePosition;
			transform.forward = direction.normalized;
			float maxInclusive = Mathf.Min(7f, Vector3.Distance(SourcePosition, TargetPosition));
			bezierPath = new BezierPath();
			List<Vector3> newControlPoints = new List<Vector3>();
			newControlPoints.Add(SourcePosition);
			newControlPoints.Add((SourcePosition + (transform.right * UnityEngine.Random.Range(-maxInclusive, maxInclusive))) + (transform.up * UnityEngine.Random.Range(0f, maxInclusive)));
			newControlPoints.Add((TargetPosition + (transform.right * UnityEngine.Random.Range(-maxInclusive, maxInclusive))) + (transform.up * UnityEngine.Random.Range(0f, maxInclusive)));
			newControlPoints.Add(TargetPosition);
			bezierPath.SetControlPoints(newControlPoints);
		}

		public void Initialize()
		{
			
		}

		public TrackedProjectile()
		{
			// ![](FAA41F806C5DA538A88BAD89DD38C8D8.png;;0,77,584,433)
			pathTimeCurve = new AnimationCurve();
			pathTimeCurve.preWrapMode = WrapMode.ClampForever;
			pathTimeCurve.postWrapMode = WrapMode.ClampForever;
			pathTimeCurve.AddKey(new Keyframe(0, 0, 3.69767f, 3.69767f, 0, 0.2363204f));
			pathTimeCurve.AddKey(new Keyframe(0.08789588f, 0.1448911f, 0.4504516f, 0.4504516f, 0.3333333f, 0.130629f));
			pathTimeCurve.AddKey(new Keyframe(0.5344772f, 0.2922845f, 0.3926852f, 0.3926852f, 0.3333333f, 0.1190708f));
			pathTimeCurve.AddKey(new Keyframe(1, 1, 3.894356f, 3.894356f, 0.03830125f, 0));

			growProjectileCurve = new AnimationCurve();
			growProjectileCurve.preWrapMode = WrapMode.ClampForever;
			growProjectileCurve.postWrapMode = WrapMode.ClampForever;
			growProjectileCurve.AddKey(new Keyframe(0, 0, 3.69767f, 3.69767f, 0, 0.2363204f));
			growProjectileCurve.AddKey(new Keyframe(0.08789588f, 0.1448911f, 0.4504516f, 0.4504516f, 0.3333333f, 0.130629f));
			growProjectileCurve.AddKey(new Keyframe(0.5344772f, 0.2922845f, 0.3926852f, 0.3926852f, 0.3333333f, 0.1190708f));
			growProjectileCurve.AddKey(new Keyframe(1, 1, 3.894356f, 3.894356f, 0.03830125f, 0));
		}
	}
}
