using System;
using System.Linq;

namespace DndCore
{
	public enum ItemEffectKind
	{
		None,
		Windup,   // Removed when attack/damage/health/extra/hp roll ends (e.g., spell cast completes), player selects another weapon, or another player becomes activated.
		Spell,    // Removed when a spell expires or is dispelled. Effect surrounds caster.
		Strike,   // Removed automatically (e.g., because spell is cast immediately and the effects have a duration)
		Target    // Effect surrounds a specified target, and ends when the spell expires or is dispelled.
	}
}
