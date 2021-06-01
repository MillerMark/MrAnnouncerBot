using System;
using System.Linq;
using UnityEngine;
using TaleSpireCore;

namespace TaleSpireExplore
{
	public class KnownEffectsFactory : IKnownEffectsBuilder
	{
		public GameObject Create(string effectName, string instanceId = null)
		{
			return KnownEffects.Create(effectName, instanceId);
		}
	}
}
