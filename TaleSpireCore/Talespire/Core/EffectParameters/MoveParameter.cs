using System;
using System.Linq;
using UnityEngine;

namespace TaleSpireCore
{
	[EffectParameter("move", WhenToApply.AfterPositioning)]
	public class MoveParameter : BaseEffectParameter
	{
		void Apply(GameObject gameObject, Vector3 position)
		{
			Talespire.Log.Warning($"Moving {gameObject.name} from {gameObject.transform.localPosition} to {gameObject.transform.localPosition + position}.");
			gameObject.transform.localPosition += position;
		}

		public override void Apply(GameObject gameObject, string rightSide)
		{
			Apply(gameObject, GetVector(rightSide));
		}

		public MoveParameter()
		{
		}
	}
}



