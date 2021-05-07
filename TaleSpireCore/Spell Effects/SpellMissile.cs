using Bounce.Singletons;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace TaleSpireCore
{
	public class SpellMissile: VisualEffect
  {
    // Fields
    private BezierPath _path;
    [SerializeField]
    private ParticleSystem impact;
    [SerializeField]
    private ParticleSystem drizzle;
    [SerializeField]
    private Transform visual;
    //[SerializeField]
    //private List<DampedTransform> dampenList;
    [SerializeField]
    private AnimationCurve animCurve;
    [SerializeField]
    private Renderer[] renderers;
    [SerializeField]
    private Transform[] bones;
    [SerializeField]
    private AudioSource hitAudio;
    [SerializeField]
    private float boneDistance = 0.01f;
    //[SerializeField]
    //private Creature.Targets StartTarget;
    [SerializeField]
    private Creature.Targets EndTarget;
    //[SerializeField]
    //private AudioClip audioClipImpact;
    //[SerializeField]
    //private AudioClip audioClipThrow;
    private Vector3 _originPosition;
    private Vector3 _targetPosition;
    private Creature _castingCreature;
    private int stage;
    private float time;
    private bool _hasPlayedHit;

		protected virtual void InitializeEffects()
		{
      /* 
			 impact
      drizzle
      visual

			 
			 */

    }

    private void EnableRenderers(bool value)
    {
			for (int i = 0; i < renderers.Length; i++)
				renderers[i].enabled = value;
		}

    private void LateUpdate()
    {
      if (_path != null)
      {
        if (stage == 0)
        {
          stage++;
        }
        else if (stage == 1)
        {
          EnableRenderers(true);
          int index = 0;
          while (true)
          {
            if (index >= bones.Length)
            {
              stage++;
              break;
            }
            if (index == 0)
            {
              transform.position = _path.CalculateBezierPoint(0, animCurve.Evaluate(time));
              transform.LookAt(_path.CalculateBezierPoint(0, animCurve.Evaluate(time + 0.01f)));
            }
            bones[index].transform.position = _path.CalculateBezierPoint(0, animCurve.Evaluate(time - (index * boneDistance)));
            index++;
          }
        }
        else if (stage == 2)
        {
          stage++;
        }
        else if (stage != 3)
        {
          if (stage == 4)
          {
            Creature creature;
            EnableRenderers(false);
            impact.transform.position = _targetPosition;
            impact.transform.LookAt(impact.transform.position + transform.forward);
            impact.Play(true);
            if (_castingCreature.TryGetCreatureFromTarget(EndTarget, out creature))
            {
              creature.AttackTargetCreature(impact.transform.forward);
            }
            if (SimpleSingletonBehaviour<CameraController>.HasInstance)
            {
              TS_CameraShaker.CallPushInDirection(0.1f, impact.transform.forward);
            }
            stage++;
          }
        }
        else
        {
          int index = 0;
          while (true)
          {
            if (index >= bones.Length)
            {
              time += Time.deltaTime;
              if (time > 1f)
              {
                stage++;
              }
              if (!drizzle.isEmitting)
              {
                drizzle.Clear(true);
                drizzle.Play(true);
              }
              break;
            }
            if (index == 0)
            {
              transform.position = _path.CalculateBezierPoint(0, animCurve.Evaluate(time));
              transform.LookAt(_path.CalculateBezierPoint(0, animCurve.Evaluate(time + 0.01f)));
            }
            bones[index].transform.position = _path.CalculateBezierPoint(0, animCurve.Evaluate(time - (index * boneDistance)));
            index++;
          }
        }
        TryPlayImpactAudio(time);
      }
    }

    protected override void OnPlayFromOriginToTarget(Transform origin, Transform target)
    {
      _originPosition = origin.position;
      _targetPosition = target.position;
      transform.position = _originPosition;
      transform.forward = (_targetPosition - _originPosition).normalized;
      float maxInclusive = Mathf.Min(7f, Vector3.Distance(_originPosition, _targetPosition));
      _path = new BezierPath();
      List<Vector3> newControlPoints = new List<Vector3>();
      newControlPoints.Add(_originPosition);
      newControlPoints.Add((_originPosition + (transform.right * Random.Range(-maxInclusive, maxInclusive))) + (transform.up * Random.Range(0f, maxInclusive)));
      newControlPoints.Add((_targetPosition + (transform.right * Random.Range(-maxInclusive, maxInclusive))) + (transform.up * Random.Range(0f, maxInclusive)));
      newControlPoints.Add(_targetPosition);
      _path.SetControlPoints(newControlPoints);
      time = 0f;
      stage = 0;
      visual.gameObject.SetActive(true);
      drizzle.Clear(true);
    }

    protected override void OnPlayFromOriginToTarget(Creature creature, Creature.Targets origin, Creature.Targets target)
    {
      _castingCreature = creature;
      OnPlayFromOriginToTarget(creature.GetTransformFromTarget(origin), creature.GetTransformFromTarget(target));
    }

    protected override void OnReturnToFactory()
    {
      impact.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
      drizzle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
      _castingCreature = null;
    }

    protected override void OnSetup()
    {
      _hasPlayedHit = false;
      OnSetup();
    }

    private void TryPlayImpactAudio(float time)
    {
      if (!_hasPlayedHit && (time > 0.85f))
      {
        hitAudio.Play();
        _hasPlayedHit = true;
      }
    }

		public static void Clone(VFXMissile source, VFXMissile target)
		{
			if (target == null)
				return;
			if (source == null)
				return;

   //   source.impact = new ParticleSystem();
   //   ReflectionHelper.CloneNonPublicProperties(source, target, "impact");

   //   drizzle = new ParticleSystem();
   //   ReflectionHelper.ClonePropertiesFrom(drizzle, ReflectionHelper.GetNonPublicFieldValue(target, "drizzle"));

   //   visual = ReflectionHelper.GetNonPublicFieldValue<Transform>(target, "visual");

   //   animCurve = ReflectionHelper.GetNonPublicFieldValue<AnimationCurve>(target, "animCurve");

   //   renderers = ReflectionHelper.GetNonPublicFieldValue<Renderer[]>(target, "renderers");
   //   bones = ReflectionHelper.GetNonPublicFieldValue<Transform[]>(target, "bones");
   //   hitAudio = ReflectionHelper.GetNonPublicFieldValue<AudioSource>(target, "hitAudio");
   //   EndTarget = ReflectionHelper.GetNonPublicFieldValue<Creature.Targets>(target, "EndTarget");
   //   enabled = target.enabled;
			//hideFlags = target.hideFlags;
			//name = target.name;

		}
	}
}
