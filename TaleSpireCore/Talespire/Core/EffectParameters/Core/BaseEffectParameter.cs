using System;
using System.Linq;
using UnityEngine;

namespace TaleSpireCore
{
	/// <summary>
	/// Base class for effect parameters.
	/// </summary>
	public abstract class BaseEffectParameter
	{
		public BaseEffectParameter()
		{
		}

		public abstract void Apply(GameObject gameObject, string rightSide);

		protected static Vector3 GetVector(string rightSide)
		{
			Vector3 scale = Vector3.zero;
			if (rightSide.IndexOf(',') > 0)
				scale = Talespire.Convert.ToVector3(rightSide);
			else
			{
				// Use a single number to represent a scale.
				if (float.TryParse(rightSide, out float scaleValue))
					scale = new Vector3(scaleValue, scaleValue, scaleValue);
				else
					Talespire.Log.Error($"Unable to convert \"{rightSide}\" to a float or a Vector3.");
			}

			return scale;
		}
	}
}



