using System;
using System.Linq;
using UnityEngine;

namespace TaleSpireCore
{
	[EffectParameter("scale", WhenToApply.AfterPositioning)]
	public class ScaleParameter : BaseEffectParameter
	{
		void Apply(GameObject gameObject, Vector3 scale)
		{
			if (scale == Vector3.zero)
				return;
			Talespire.Log.Warning($"Scaling {gameObject.name} from {gameObject.transform.localScale} to {scale}.");
			gameObject.transform.localScale = scale;
		}

		public override void Apply(GameObject gameObject, string rightSide)
		{
			Apply(gameObject, GetVector(rightSide));
		}

		public ScaleParameter()
		{
		}
	}
}



