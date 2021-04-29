using System;
using System.Linq;
using UnityEngine;

namespace TaleSpireExploreDecompiles
{
	public class VisualEffect : FactoryItemBehaviour
	{
		// Fields
		[SerializeField]
		protected float _length;
		private float _timer;
		private bool _isPlaying;

		// Methods
		protected void Awake()
		{
			this.OnSetup();
		}

		public void Emit(int amount, Vector3 location)
		{
			this.Emit(amount, location, base.transform.rotation);
		}

		public void Emit(int amount, Vector3 location, Quaternion rotation)
		{
			this._isPlaying = true;
			this._timer = 0f;
			this.OnEmit(amount, location, rotation);
		}

		protected virtual void OnEmit(int amount, Vector3 location, Quaternion rotation)
		{
		}

		protected override void OnHire()
		{
			this.OnSetup();
			this._isPlaying = false;
			this._timer = 0f;
			base.OnHire();
		}

		protected virtual void OnPlay(Vector3 location, Quaternion rotation)
		{
		}

		protected virtual void OnPlayFromOriginToTarget(Transform origin, Transform target)
		{
		}

		protected virtual void OnPlayFromOriginToTarget(Creature creature, Creature.Targets origin, Creature.Targets target)
		{
		}

		protected virtual void OnSetup()
		{
		}

		public void Play(Vector3 location)
		{
			this.Play(location, base.transform.rotation);
		}

		public void Play(Vector3 location, Quaternion rotation)
		{
			this._isPlaying = true;
			this._timer = 0f;
			this.OnPlay(location, rotation);
		}

		public void PlayFromOriginToTarget(Transform origin, Transform target)
		{
			this._isPlaying = true;
			this._timer = 0f;
			this.OnPlayFromOriginToTarget(origin, target);
		}

		public void PlayFromOriginToTarget(Creature creature, Creature.Targets origin, Creature.Targets target)
		{
			this._isPlaying = true;
			this._timer = 0f;
			this.OnPlayFromOriginToTarget(creature, origin, target);
		}

		protected void Update()
		{
			if (this._isPlaying)
			{
				this._timer += Time.deltaTime;
				if (this._timer > this._length)
				{
					base.ReturnToFactory();
				}
			}
		}
	}
