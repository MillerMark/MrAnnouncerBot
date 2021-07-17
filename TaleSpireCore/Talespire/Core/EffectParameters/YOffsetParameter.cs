using System;
using System.Linq;
using UnityEngine;

namespace TaleSpireCore
{
	[EffectParameter("yOffset", WhenToApply.AfterPositioning)]
	public class YOffsetParameter : BaseEffectParameter
	{
		void Apply(GameObject gameObject, float distanceTiles)
		{
			if (distanceTiles == 0)
				return;
			Vector3 position = gameObject.transform.localPosition;
			Vector3 newPosition = new Vector3(position.x, position.y + distanceTiles, position.z);
			Talespire.Log.Warning($"Moving {gameObject.name} from {position} to {newPosition}.");
			gameObject.transform.localPosition = newPosition;
		}

		public override void Apply(GameObject gameObject, string rightSide)
		{
			float distanceTiles = Talespire.Convert.ToDistanceTiles(rightSide);
			Apply(gameObject, distanceTiles);
		}

		public YOffsetParameter()
		{
		}
	}
}



