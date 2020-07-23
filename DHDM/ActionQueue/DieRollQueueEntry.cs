//#define profiling
using System;
using System.Linq;
using DndCore;

namespace DHDM
{
	public class SpellSavingThrowQueueEntry : DieRollQueueEntry
	{
		public string SpellName { get; set; }

		public SpellSavingThrowQueueEntry()
		{
			
		}

		public SpellSavingThrowQueueEntry(string spellName)
		{
			SpellName = spellName;
		}
		void AddDieStr(string dieStr)
		{
			if (string.IsNullOrWhiteSpace(DieStr))
				DieStr = dieStr;
			else
				DieStr += "; " + dieStr;
		}
		public void AddSavingThrowFor(Ability savingThrowAbility, InGameCreature inGameCreature)
		{
			Monster creature = AllMonsters.GetByKind(inGameCreature.Kind);
			if (creature != null)
			{
				VantageKind vantage = creature.GetVantage(DiceRollType.SavingThrow, savingThrowAbility);
				string vantageStr = string.Empty;
				switch (vantage)
				{
					case VantageKind.Advantage:
						vantageStr = ", advantage";
						break;
					case VantageKind.Disadvantage:
						vantageStr = ", disadvantage";
						break;
				}
				double modifier = creature.GetSavingThrowModifier(savingThrowAbility);
				AddDieStr($"1d20(\"{inGameCreature.Name}\", {modifier}{vantageStr})");
			}
		}
	}
	public class DieRollQueueEntry : ActionQueueEntry
	{
		public string DieStr { get; set; }
		public int SavingThrowThreshold { get; set; } = -1;
		public int AttackThreshold { get; set; } = -1;
		public int SkillCheckThreshold { get; set; } = -1;
		public DieRollQueueEntry()
		{

		}
	}
}
