using System;
using System.Linq;

namespace DndCore
{
	[Flags]
	public enum EffectLocation
	{
		None = 0,
		CreatureBase = 1,
		SpellCast = 2,
		LastTargetPosition = 3,
		AtCollision = 4,
		AtCollisionTarget = 5,
		MoveableTarget = 6,
		MoveableSpellCast = 7,
		AtCollisionBase = 8
	}
}
