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
			if (creature == null)
				return;
			if (creature is Character player)
			{
				if (PlayerIds == null)
					PlayerIds = new List<int>();
				PlayerIds.Add(player.playerID);
			}
			else
				Creatures.Add(creature);
		}

		public static Target FromMagic(Magic magic)
		{
			Target target = new Target();
			target.Type = AttackTargetType.Spell;
			target.SpellType = SpellTargetType.Creatures;
			foreach (Creature creature in magic.Targets)
				target.Creatures.Add(creature);
			return target;
		}

		public Vector Location { get; set; }
		public CarriedWeapon Weapon { get; set; }

		public int Count
		{
			get
			{
				int count = 0;
				if (PlayerIds != null)
					count += PlayerIds.Count;
				if (Creatures != null)
					count += Creatures.Count;
				return count;
			}
		}
		public Target()
		{

		}

		public Target(AttackTargetType attackTargetType, Creature targetCreature)
		{
			Type = attackTargetType;
			SpellType = SpellTargetType.Creatures;
			if (targetCreature is Character player)
			{
				if (PlayerIds == null)
					PlayerIds = new List<int>();
				PlayerIds.Add(player.playerID);
			}
			else
			{
				Creatures.Add(targetCreature);
			}
		}

		public Target(CarriedWeapon weapon)
		{
			Type = AttackTargetType.Weapon;
			SpellType = SpellTargetType.None;
			Weapon = weapon;
		}

		public Target(List<PlayerRoll> playerRolls)
		{
			foreach (PlayerRoll playerRoll in playerRolls)
			{
				if (playerRoll.id > 0)
				{
					Character player = AllPlayers.GetFromId(playerRoll.id);
					if (player != null)
						Creatures.Add(player);
				}
				else
				{
					InGameCreature creature = AllInGameCreatures.GetByIndex(InGameCreature.GetNormalIndexFromUniversal(playerRoll.id));
					if (creature != null)
						Creatures.Add(creature.Creature);
				}
			}
		}
	}
}
