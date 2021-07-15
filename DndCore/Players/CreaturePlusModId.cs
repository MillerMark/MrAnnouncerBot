using System;

namespace DndCore
{
	public class CreaturePlusModId
	{
		public string ID { get; set; }
		public Creature Creature { get; set; }
		public Magic Magic { get; set; }
		public string Guid { get; set; }
		public CreaturePlusModId(string iD, Creature creature, string guid)
		{
			Guid = guid;
			ID = iD;
			Creature = creature;
		}
		public CreaturePlusModId()
		{

		}
	}
}
