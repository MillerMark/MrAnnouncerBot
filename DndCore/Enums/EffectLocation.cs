using System;
using System.Linq;

namespace DndCore
{
	[Flags]
	public enum EffectLocation
	{
		None = 0,
		ActiveCreaturePosition = 1,
		LastTargetPosition = 2,
		AtCollision = 3
	}
}
