using System;
using System.Linq;
using System.Collections.Generic;
using DndCore;

namespace DndCore
{
	public enum AttackTargetType
	{
		Spell,
		Weapon
	}

	public class Target
	{
		public AttackTargetType Type { get; set; }
		public SpellTargetType SpellType { get; set; }
		public SpellTargetShape Shape { get; set; }
		public int CasterId { get; set; }
		public List<int> PlayerIds { get; set; }
		public Vector Location { get; set; }
		public int Range { get; set; }
		public Target()
		{

		}

		public Target(AttackTargetType attackTargetType, Creature targetCreature)
		{
			Type = attackTargetType;
			SpellType = SpellTargetType.Creatures;
			if (targetCreature is Character player)
				PlayerIds.Add(player.playerID);
		}
	}
}
