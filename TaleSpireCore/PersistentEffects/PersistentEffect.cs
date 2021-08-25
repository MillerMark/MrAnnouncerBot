using System;
using System.Linq;

namespace TaleSpireCore
{
	public class PersistentEffect
	{
		// TODO: EffectName is going to get way more sophisticated
		public string EffectName { get; set; }
		public bool RotationLocked { get; set; }
		public bool Hidden { get; set; }
		public PersistentEffect()
		{

		}
	}
}