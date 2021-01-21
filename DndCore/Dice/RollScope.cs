using System;
using System.Linq;

namespace DndCore
{
	public enum RollScope
	{
		ActivePlayer,
		Viewer,
		Individuals,  // Everyone is included in this scope.
		ActiveInGameCreature,
		TargetedInGameCreatures
	}
}
