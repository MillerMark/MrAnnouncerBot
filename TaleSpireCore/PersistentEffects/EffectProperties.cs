using System;
using System.Linq;
using System.Collections.Generic;

namespace TaleSpireCore
{
	public class EffectProperties
	{
		public string EffectName { get; set; }
		public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();

		public EffectProperties()
		{

		}

		public EffectProperties(string effectName, Dictionary<string, string> properties)
		{
			EffectName = effectName;
			Properties = properties;
		}
	}
}