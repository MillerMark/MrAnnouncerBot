//#define profiling
using System;
using System.Linq;
using DndCore;

namespace DHDM
{
	public class SpellSavingThrowQueueEntry : DieRollQueueEntry
	{
		public Ability SavingThrowAbility { get; set; }
		public string SpellName { get; set; }

		public SpellSavingThrowQueueEntry()
		{
			RollType = DiceRollType.SavingThrow;
			RollScope = RollScope.TargetedInGameCreatures;
		}

		public override void PrepareRoll(DiceRoll diceRoll)
		{
			base.PrepareRoll(diceRoll);
			diceRoll.SavingThrow = SavingThrowAbility;
			diceRoll.SpellName = SpellName;
		}

		public SpellSavingThrowQueueEntry(string spellName, Ability savingThrowAbility) : this()
		{
			SpellName = spellName;
			SavingThrowAbility = savingThrowAbility;
		}
		
		void AddDice(int quantity, int sides, string label, double modifier, VantageKind vantage, int creatureId, string backColor = null, string fontColor = null, string playerName = null)
		{
			if (vantage != VantageKind.Normal && quantity == 1)
				quantity = 2;

			DiceDto diceDto = new DiceDto();
			if (backColor != null)
				diceDto.BackColor = backColor;
			if (fontColor != null)
				diceDto.FontColor = fontColor;
			diceDto.Label = label;
			if (playerName == null)
				playerName = label;
			diceDto.PlayerName = playerName;
			diceDto.CreatureId = creatureId;
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
				AddDice(1, 20, inGameCreature.Name, modifier, vantage, InGameCreature.GetUniversalIndex(inGameCreature.Index));
			}
		}
	}
}
