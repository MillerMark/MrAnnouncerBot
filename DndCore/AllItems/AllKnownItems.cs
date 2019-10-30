using System;
using System.Linq;

namespace DndCore
{
	public static class AllKnownItems
	{
		public static void Invalidate()
		{
			AllActionShortcuts.Invalidate();
			AllDieRollEffects.Invalidate();
			AllFeatures.Invalidate();
			AllFunctions.Invalidate();
			AllPlayers.Invalidate();
			AllProperties.Invalidate();
			AllSpellEffects.Invalidate();
			AllSpells.Invalidate();
			AllTables.Invalidate();
			AllTrailingEffects.Invalidate();
			AllWeaponEffects.Invalidate();
			AllWeapons.Invalidate();
		}
	}
}

