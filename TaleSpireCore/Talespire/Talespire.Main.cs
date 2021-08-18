using System;
using System.Linq;

namespace TaleSpireCore
{
	public static partial class Talespire
	{
		public static void Update()
		{
			Target.Update();
			Spells.Update();
			Instances.Update();
			CompositeEffect.Update();
			PersistentEffects.Update();
		}
	}
}