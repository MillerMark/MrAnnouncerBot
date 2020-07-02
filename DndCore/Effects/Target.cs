using System;
using System.Linq;
using System.Collections.Generic;
using DndCore;
using Newtonsoft.Json;

namespace DndCore
{
	public class Target : IPosition
	{
		public AttackTargetType Type { get; set; }
		public SpellTargetType SpellType { get; set; }
		public SpellTargetShape Shape { get; set; }
		public int CasterId { get; set; }
		public List<int> PlayerIds { get; set; }

		List<Creature> creatures;
		[JsonIgnore]
		public List<Creature> Creatures
		{
			get => creatures ??= new List<Creature>();
		}

		public void AddCreature(Creature creature)
		{
			Creatures.Add(creature);
		}

		public Vector Location { get; set; }
		public CarriedWeapon Weapon { get; set; }
		public Target()
		{

		}

		public Target(AttackTargetType attackTargetType, Creature targetCreature)
		{
			Type = attackTargetType;
			SpellType = SpellTargetType.Creatures;
			if (targetCreature is Character player)
				PlayerIds.Add(player.playerID);
			else
				Creatures.Add(targetCreature);
		}

		public Target(CarriedWeapon weapon)
		{
			Type = AttackTargetType.Weapon;
			SpellType = SpellTargetType.None;
			Weapon = weapon;
		}
	}
}
