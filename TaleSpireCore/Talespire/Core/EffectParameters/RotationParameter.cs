using System;
using System.Linq;
using UnityEngine;

namespace TaleSpireCore
{
	[EffectParameter("rotation", WhenToApply.AfterPositioning)]
	public class RotationParameter : BaseEffectParameter
	{
		void Apply(GameObject gameObject, Vector3 angle)
		{
			Talespire.Log.Warning($"Rotating {gameObject.name} from {gameObject.transform.localEulerAngles} to {angle}.");
			gameObject.transform.localEulerAngles = angle;
		}

		public override void Apply(GameObject gameObject, string rightSide)
		{
			Apply(gameObject, GetVector(rightSide));
		}

		public RotationParameter()
		{
		}
	}
}



