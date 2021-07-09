using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TaleSpireCore
{
	public class TrackedProjectile
	{
		public bool ReadyToDelete { get; set; }
		public GameObject Projectile { get; set; }
		public Vector3 TargetPosition { get; set; }
		public Vector3 SourcePosition { get; set; }
		public float SpeedFeetPerSecond { get; set; }
		public float StartTime { get; set; }
		public string EffectName { get; set; }
		public string SpellId { get; set; }
		public event EventHandler TargetReached;
		bool alreadyCreated;
		Vector3 direction;
		float totalTravelDistanceInFeet;
		BezierPath bezierPath;
		AnimationCurve animationCurve;
		float time;

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

			Projectile.transform.position = bezierPath.CalculateBezierPoint(0, animationCurve.Evaluate(percentTraveled));
			Projectile.transform.LookAt(bezierPath.CalculateBezierPoint(0, animationCurve.Evaluate(percentTraveled + 0.01f)));
			time += Time.deltaTime;
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
			time = 0;
		}

		public void Initialize()
		{
			
		}

		public TrackedProjectile()
		{
			animationCurve = new AnimationCurve();
			animationCurve.preWrapMode = WrapMode.ClampForever;
			animationCurve.postWrapMode = WrapMode.ClampForever;
			animationCurve.AddKey(new Keyframe(0, 0, 3.69767f, 3.69767f, 0, 0.2363204f));
			animationCurve.AddKey(new Keyframe(0.08789588f, 0.1448911f, 0.4504516f, 0.4504516f, 0.3333333f, 0.130629f));
			animationCurve.AddKey(new Keyframe(0.5344772f, 0.2922845f, 0.3926852f, 0.3926852f, 0.3333333f, 0.1190708f));
			animationCurve.AddKey(new Keyframe(1, 1, 3.894356f, 3.894356f, 0.03830125f, 0));
		}
	}
}
