using System;
using System.Linq;
using UnityEngine;

namespace TaleSpireCore
{
	public interface IKnownEffectsBuilder
	{
		GameObject Create(string effectName, string instanceId = null);
	}
}
