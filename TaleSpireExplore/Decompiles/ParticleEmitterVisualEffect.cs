using System;
using System.Linq;
using UnityEngine;

namespace TaleSpireExploreDecompiles
{
	public class ParticleEmitterVisualEffect : VisualEffect
	{
		// Fields
		[SerializeField]
		private ParticleSystem _pSystem;

		// Methods
		protected override void OnEmit(int amount, Vector3 location, Quaternion rotation)
		{
			base.transform.position = location;
			base.transform.rotation = rotation;
			this._pSystem.Emit(amount);
		}

		protected override void OnPlay(Vector3 location, Quaternion rotation)
		{
			base.transform.position = location;
			base.transform.rotation = rotation;
			this._pSystem.Play(true);
		}

		protected override void OnReturnToFactory()
		{
			this._pSystem.Stop(true);
			this._pSystem.Clear(true);
		}

		protected override void OnSetup()
		{
			this._pSystem = base.GetComponent<ParticleSystem>();
			this._pSystem.Stop(true);
		}
	}
}
