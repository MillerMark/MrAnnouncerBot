using System;
using System.Linq;

namespace TaleSpireCore
{
	public class EffectParameterAttribute : Attribute
	{
		/// <summary>
		/// Case-sensitive property name this effect parameter will handle.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// When to apply this effect parameter. The default value is after creation. There is also an option to wait 
		/// until the effect is positioned before applying the parameter.
		/// </summary>
		public WhenToApply WhenToApply { get; set; }
		public EffectParameterAttribute(string name, WhenToApply whenToApply = WhenToApply.AfterCreation)
		{
			Name = name;
			WhenToApply = whenToApply;
		}
	}
}



