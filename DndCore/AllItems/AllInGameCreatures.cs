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

		public static InGameCreature GetActiveCreatureByFirstName(string name)
		{
			string lowerName = name.ToLower();
			return Creatures.FirstOrDefault(x => x.Name.ToLower().StartsWith(lowerName) && x.OnScreen);
		}

		public static InGameCreature GetCreatureByName(string name)
		{
			if (name == null)
				return null;
			string lowerName = name.ToLower();
			InGameCreature inGameCreature = Creatures.FirstOrDefault(x => x.Name.ToLower() == lowerName);
			if (inGameCreature != null)
				return inGameCreature;
			inGameCreature = Creatures.FirstOrDefault(x => x.Name.ToLower().StartsWith(lowerName));
			if (inGameCreature != null)
				return inGameCreature;
			inGameCreature = Creatures.FirstOrDefault(x => x.Name.ToLower().EndsWith(lowerName));
			return inGameCreature;
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

		public static List<InGameCreature> ToggleSelected(InGameCreature inGameCreature)
		{
			List<InGameCreature> changedCreatures = new List<InGameCreature>();
			
			foreach (InGameCreature creature in Creatures)
				if (creature == inGameCreature)
				{
					creature.IsSelected = !creature.IsSelected;
					changedCreatures.Add(creature);
				}
				else if (creature.IsSelected)
				{
					changedCreatures.Add(creature);
					creature.IsSelected = false;
				}

			return changedCreatures;
		}

		public static List<InGameCreature> Select(InGameCreature inGameCreature)
		{
			List<InGameCreature> changedCreatures = new List<InGameCreature>();

			foreach (InGameCreature creature in Creatures)
				if (creature == inGameCreature)
				{
					if (!creature.IsSelected)
					{
						creature.IsSelected = true;
						changedCreatures.Add(creature);
					}
				}
				else if (creature.IsSelected)
				{
					changedCreatures.Add(creature);
					creature.IsSelected = false;
				}

			return changedCreatures;
		}


		public static List<InGameCreature> ClearSelection()
		{
			List<InGameCreature> changedCreatures = new List<InGameCreature>();

			foreach (InGameCreature creature in Creatures)
				if (creature.IsSelected)
				{
					creature.IsSelected = false;
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

		public static int GetTargetCount(WhatSide targetStatus)
		{
			int count = 0;
			foreach (InGameCreature inGameCreature in Creatures)
				if (inGameCreature.IsTargeted)
					if (targetStatus.HasFlag(WhatSide.Friendly) && inGameCreature.IsAlly)
						count++;
					else if (targetStatus.HasFlag(WhatSide.Enemy) && inGameCreature.IsEnemy)
						count++;
					else if (targetStatus.HasFlag(WhatSide.All))
						count++;
			return count;
		}

		public static string GetTargetedCreatureDisplayList()
		{
			string result = string.Empty;
			foreach (InGameCreature inGameCreature in Creatures)
				if (inGameCreature.IsTargeted)
					result += inGameCreature.Name + ", ";
			return result.TrimEnd(',', ' ');
		}

		public static void AddD20sForSelected(List<DiceDto> diceDtos, DiceRollType rollType)
		{
			foreach (InGameCreature inGameCreature in Creatures)
			{
				if (inGameCreature.OnScreen)
				{
					DiceDto npcMonsterDice = new DiceDto();
					npcMonsterDice.Sides = 20;
					npcMonsterDice.CreatureId = InGameCreature.GetUniversalIndex(inGameCreature.Index);
					npcMonsterDice.Quantity = 1;
					SetDiceFromCreature(inGameCreature, npcMonsterDice);
					npcMonsterDice.Label = null;  // Backwards compatibility. May be able to change after reworking code in DieRoller.ts
					if (rollType == DiceRollType.Initiative)
					{
						// TODO: Get initiative vantage for NPC/Monster
					}
					diceDtos.Add(npcMonsterDice);
				}
			}
		}

		public static void AddDiceForTargeted(List<DiceDto> diceDtos, string dieStr)
		{
			DieRollDetails dieRollDetails = DieRollDetails.From(dieStr);
			foreach (InGameCreature inGameCreature in Creatures)
			{
				if (inGameCreature.IsTargeted)
				{
					foreach (Roll roll in dieRollDetails.Rolls)
					{
						DiceDto npcMonsterDice = new DiceDto
						{
							Sides = roll.Sides,
							Quantity = (int)Math.Round(roll.Count),
						};
						npcMonsterDice.SetRollDetails(DiceRollType.None, roll.Descriptor);
						SetDiceFromCreature(inGameCreature, npcMonsterDice);
						diceDtos.Add(npcMonsterDice);
					}
				}
			}
		}

		public static void AddDiceForCreature(List<DiceDto> diceDtos, string dieStr, int creatureIndex, DiceRollType type)
		{
			DieRollDetails dieRollDetails = DieRollDetails.From(dieStr);
			foreach (InGameCreature inGameCreature in Creatures)
			{
				if (inGameCreature.Index == creatureIndex)
				{
					foreach (Roll roll in dieRollDetails.Rolls)
					{
						DiceDto npcMonsterDice = new DiceDto();
						npcMonsterDice.Sides = roll.Sides;
						npcMonsterDice.Quantity = (int)Math.Round(roll.Count);
						npcMonsterDice.SetRollDetails(type, roll.Descriptor);
						SetDiceFromCreature(inGameCreature, npcMonsterDice);
						diceDtos.Add(npcMonsterDice);
					}
				}
			}
		}

		private static void SetDiceFromCreature(InGameCreature inGameCreature, DiceDto npcMonsterDice)
		{
			npcMonsterDice.CreatureId = InGameCreature.GetUniversalIndex(inGameCreature.Index);
			npcMonsterDice.Label = inGameCreature.Name;
			npcMonsterDice.PlayerName = inGameCreature.Name;
			npcMonsterDice.BackColor = inGameCreature.BackgroundHex;
			npcMonsterDice.FontColor = inGameCreature.ForegroundHex;
		}

		public static Creature GetByTaleSpireId(string taleSpireId, WhatSide whatSide = WhatSide.All)
		{
			return inGameCreatures.FirstOrDefault(x => x.TaleSpireId == taleSpireId && x.SideMatches(whatSide))?.Creature;
		}

		public static InGameCreature GetSelected()
		{
			return inGameCreatures.FirstOrDefault(x => x.IsSelected);
		}
	}
}

