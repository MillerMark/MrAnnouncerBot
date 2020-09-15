using System;
using System.Linq;
using System.Collections.Generic;
using GoogleHelper;

namespace DndCore
{
	public static class AllInGameCreatures
	{
		static AllInGameCreatures()
		{
		}
		public static void Invalidate()
		{
			inGameCreatures = null;
		}
		public static InGameCreature Get(string name)
		{
			return Creatures.FirstOrDefault(x => x.Name == name);
		}

		public static InGameCreature GetByIndex(int index)
		{
			return Creatures.FirstOrDefault(x => x.Index == index);
		}

		public static InGameCreature GetByCreature(Creature creature)
		{
			return Creatures.FirstOrDefault(x => x.Creature == creature);
		}

		public static void SaveHp()
		{
			// TODO: Optimize this code to be more efficient (takes about 6 seconds).
			GoogleSheets.SaveChanges(Creatures.ToArray(), "TempHitPointsStr,HitPointsStr");
		}
		public static void AddD20sForSelected(List<DiceDto> diceDtos, DiceRollType rollType)
		{
			foreach (InGameCreature inGameCreature in AllInGameCreatures.Creatures)
			{
				if (inGameCreature.OnScreen)
				{
					DiceDto npcMonsterDice = new DiceDto();
					npcMonsterDice.Sides = 20;
					npcMonsterDice.CreatureId = InGameCreature.GetUniversalIndex(inGameCreature.Index);
					npcMonsterDice.Quantity = 1;
					//npcMonsterDice.Label = inGameCreature.Name;
					npcMonsterDice.PlayerName = inGameCreature.Name;
					npcMonsterDice.BackColor = inGameCreature.BackgroundHex;
					npcMonsterDice.FontColor = inGameCreature.ForegroundHex;
					if (rollType == DiceRollType.Initiative)
					{
						// TODO: Get initiative vantage for NPC/Monster
					}
					diceDtos.Add(npcMonsterDice);
				}
			}
		}

		public static void SetActiveTurn(InGameCreature inGameCreature)
		{
			foreach (InGameCreature creature in Creatures)
				creature.TurnIsActive = creature == inGameCreature;
		}
		public static void ClearAllActiveTurns()
		{
			foreach (InGameCreature creature in Creatures)
				creature.TurnIsActive = false;
		}

		public static List<InGameCreature> ToggleTalking(InGameCreature inGameCreature)
		{
			List<InGameCreature> changedCreatures = new List<InGameCreature>();
			
			foreach (InGameCreature creature in Creatures)
				if (creature == inGameCreature)
				{
					creature.IsTalking = !creature.IsTalking;
					changedCreatures.Add(creature);
				}
				else if (creature.IsTalking)
				{
					changedCreatures.Add(creature);
					creature.IsTalking = false;
				}

			return changedCreatures;
		}

		public static List<InGameCreature> ClearTalking()
		{
			List<InGameCreature> changedCreatures = new List<InGameCreature>();

			foreach (InGameCreature creature in Creatures)
				if (creature.IsTalking)
				{
					creature.IsTalking = false;
					changedCreatures.Add(creature);
				}

			return changedCreatures;
		}

		static List<InGameCreature> inGameCreatures;
		public static List<InGameCreature> Creatures
		{
			get
			{
				if (inGameCreatures == null)
					inGameCreatures = GoogleSheets.Get<InGameCreature>().Where(x => !string.IsNullOrEmpty(x.Kind)).ToList();
				return inGameCreatures;
			}
		}

		public static List<InGameCreature> GetOnScreen()
		{
			return Creatures.Where(x => x.OnScreen).ToList();
		}

		public static void ClearAllTargets()
		{
			foreach (InGameCreature inGameCreature in Creatures)
				inGameCreature.IsTargeted = false;
		}
	}
}

