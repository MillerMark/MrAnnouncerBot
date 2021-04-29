using Bounce.Singletons;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace TaleSpireExploreDecompiles
{
  public class VFXMissileDecompile: VisualEffect
  {
    // Fields
    private BezierPath _path;
    [SerializeField]
    private ParticleSystem impact;
    [SerializeField]
    private ParticleSystem drizzle;
    [SerializeField]
    private Transform visual;
    [SerializeField]
    private List<DampedTransform> dampenList;
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
    [SerializeField]
    private Creature.Targets StartTarget;
    [SerializeField]
    private Creature.Targets EndTarget;
    [SerializeField]
    private AudioClip audioClipImpact;
    [SerializeField]
    private AudioClip audioClipThrow;
    private Vector3 _originPosition;
    private Vector3 _targetPosition;
    private Creature _castingCreature;
    private int stage;
    private float time;
    private bool _hasPlayedHit;

    // Methods
    private void EnableRenderers(bool value)
    {
      for (int i = 0; i < this.renderers.Length; i++)
      {
        this.renderers[i].enabled = value;
      }
    }

    private void LateUpdate()
    {
      if (this._path != null)
      {
        if (this.stage == 0)
        {
          this.stage++;
        }
        else if (this.stage == 1)
        {
          this.EnableRenderers(true);
          int index = 0;
          while (true)
          {
            if (index >= this.bones.Length)
            {
              this.stage++;
              break;
            }
            if (index == 0)
            {
              base.transform.position = this._path.CalculateBezierPoint(0, this.animCurve.Evaluate(this.time));
              base.transform.LookAt(this._path.CalculateBezierPoint(0, this.animCurve.Evaluate(this.time + 0.01f)));
            }
            this.bones[index].transform.position = this._path.CalculateBezierPoint(0, this.animCurve.Evaluate(this.time - (index * this.boneDistance)));
            index++;
          }
        }
        else if (this.stage == 2)
        {
          this.stage++;
        }
        else if (this.stage != 3)
        {
          if (this.stage == 4)
          {
            Creature creature;
            this.EnableRenderers(false);
            this.impact.transform.position = this._targetPosition;
            this.impact.transform.LookAt(this.impact.transform.position + base.transform.forward);
            this.impact.Play(true);
            if (this._castingCreature.TryGetCreatureFromTarget(this.EndTarget, out creature))
            {
              creature.AttackTargetCreature(this.impact.transform.forward);
            }
            if (SimpleSingletonBehaviour<CameraController>.HasInstance)
            {
              TS_CameraShaker.CallPushInDirection(0.1f, this.impact.transform.forward);
            }
            this.stage++;
          }
        }
        else
        {
          int index = 0;
          while (true)
          {
            if (index >= this.bones.Length)
            {
              this.time += Time.deltaTime;
              if (this.time > 1f)
              {
                this.stage++;
              }
              if (!this.drizzle.isEmitting)
              {
                this.drizzle.Clear(true);
                this.drizzle.Play(true);
              }
              break;
            }
            if (index == 0)
            {
              base.transform.position = this._path.CalculateBezierPoint(0, this.animCurve.Evaluate(this.time));
              base.transform.LookAt(this._path.CalculateBezierPoint(0, this.animCurve.Evaluate(this.time + 0.01f)));
            }
            this.bones[index].transform.position = this._path.CalculateBezierPoint(0, this.animCurve.Evaluate(this.time - (index * this.boneDistance)));
            index++;
          }
        }
        this.TryPlayImpactAudio(this.time);
      }
    }

    protected override void OnPlayFromOriginToTarget(Transform origin, Transform target)
    {
      this._originPosition = origin.position;
      this._targetPosition = target.position;
      base.transform.position = this._originPosition;
      base.transform.forward = (this._targetPosition - this._originPosition).normalized;
      float maxInclusive = Mathf.Min(7f, Vector3.Distance(this._originPosition, this._targetPosition));
      this._path = new BezierPath();
      List<Vector3> newControlPoints = new List<Vector3>();
      newControlPoints.Add(this._originPosition);
      newControlPoints.Add((this._originPosition + (base.transform.right * Random.Range(-maxInclusive, maxInclusive))) + (base.transform.up * Random.Range(0f, maxInclusive)));
      newControlPoints.Add((this._targetPosition + (base.transform.right * Random.Range(-maxInclusive, maxInclusive))) + (base.transform.up * Random.Range(0f, maxInclusive)));
      newControlPoints.Add(this._targetPosition);
      this._path.SetControlPoints(newControlPoints);
      this.time = 0f;
      this.stage = 0;
      this.visual.gameObject.SetActive(true);
      this.drizzle.Clear(true);
    }

    protected override void OnPlayFromOriginToTarget(Creature creature, Creature.Targets origin, Creature.Targets target)
    {
      this._castingCreature = creature;
      this.OnPlayFromOriginToTarget(creature.GetTransformFromTarget(origin), creature.GetTransformFromTarget(target));
    }

    protected override void OnReturnToFactory()
    {
      this.impact.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
      this.drizzle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
      this._castingCreature = null;
    }

    protected override void OnSetup()
    {
      this._hasPlayedHit = false;
      base.OnSetup();
    }

    private void TryPlayImpactAudio(float time)
    {
      if (!this._hasPlayedHit && (time > 0.85f))
      {
        this.hitAudio.Play();
        this._hasPlayedHit = true;
      }
    }
  }
}
