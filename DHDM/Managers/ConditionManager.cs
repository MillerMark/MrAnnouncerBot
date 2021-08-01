//#define profiling
using System;
using System.Linq;
using DndCore;

namespace DHDM
{
	public static class ConditionManager
	{
		public static void ApplyToCreature(Creature targetCreature, string spellId, Conditions conditions)
		{
			targetCreature.AddSpellCondition(spellId, conditions);
			if (targetCreature is Character player)
			{
				CreatureStats playerStats = PlayerStatManager.GetPlayerStats(player.playerID);
				if (playerStats != null)
					playerStats.Conditions = player.AllConditions;
			}
		}
	}
}


