using System;

namespace DndCore
{
	public class CreaturePlusModId
	{
		public string ID { get; set; }
		public Creature Creature { get; set; }
		public CreaturePlusModId(string iD, Creature creature)
		{
			ID = iD;
			Creature = creature;
		}
		public CreaturePlusModId()
		{

		}
	}
}
