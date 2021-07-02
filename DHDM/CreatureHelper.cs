using DndCore;
using System;
using System.Linq;

namespace DHDM
{
	public static class CreatureHelper
	{
		public static Creature GetCreatureFromId(int creatureId)
		{
			if (creatureId >= 0)
				return AllPlayers.GetFromId(creatureId);
			else
			{
				InGameCreature inGameCreature = AllInGameCreatures.GetByIndex(-creatureId);
				if (inGameCreature != null)
					return inGameCreature.Creature;
			}
			return null;
		}
	}
}


