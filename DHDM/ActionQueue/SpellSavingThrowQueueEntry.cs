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
			RollType = DiceRollType.SavingThrow;
		}

		public SpellSavingThrowQueueEntry(string spellName): this()
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

		void AddDice(int quantity, int sides, string label, double modifier, VantageKind vantage, string backColor = null, string fontColor = null)
		{
			if (vantage != VantageKind.Normal && quantity == 1)
				quantity = 2;

			DiceDto diceDto = new DiceDto();
			if (backColor != null)
				diceDto.BackColor = backColor;
			if (fontColor != null)
				diceDto.FontColor = fontColor;
			diceDto.Label = label;
			diceDto.Quantity = quantity;
			diceDto.Sides = sides;
			diceDto.Modifier = modifier;
			diceDto.Vantage = vantage;
			DiceDtos.Add(diceDto);
		}

		public void AddSavingThrowFor(Ability savingThrowAbility, InGameCreature inGameCreature)
		{
			Monster creature = AllMonsters.GetByKind(inGameCreature.Kind);
			if (creature != null)
			{
				VantageKind vantage = creature.GetVantage(DiceRollType.SavingThrow, savingThrowAbility);
				double modifier = creature.GetSavingThrowModifier(savingThrowAbility);
				AddDice(1, 20, inGameCreature.Name, modifier, vantage);
			}
		}
	}
}
