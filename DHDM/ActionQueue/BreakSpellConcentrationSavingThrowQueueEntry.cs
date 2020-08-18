//#define profiling
using DndCore;
using System;
using System.Linq;

namespace DHDM
{
	public class BreakSpellConcentrationSavingThrowQueueEntry : DieRollQueueEntry
	{
		public const string STR_ConcentrationSave = "Concentration Save";
		public BreakSpellConcentrationSavingThrowQueueEntry()
		{
			RollType = DiceRollType.SavingThrow;
			RollScope = RollScope.ActivePlayer;
		}

		public override void PrepareRoll(DiceRoll diceRoll)
		{
			base.PrepareRoll(diceRoll);
			diceRoll.SavingThrow = Ability.constitution;
			diceRoll.SuppressLegacyRoll = true;
			string backgroundColor, textColor;
			Character player = AllPlayers.GetFromId(PlayerId);
			GetPlayerDieColor(player, out backgroundColor, out textColor);
			string name = "";
			if (player != null)
				name = player.name;
			DiceDto diceDto = CreateDie(backgroundColor, textColor, name, 20, STR_ConcentrationSave);
			diceDto.Label = STR_ConcentrationSave;
			diceRoll.DiceDtos.Add(diceDto);
		}

		private DiceDto CreateDie(string backgroundColor, string textColor, string playerName, int sides, string type = "")
		{
			return new DiceDto() { PlayerName = playerName, CreatureId = PlayerId, Sides = sides, BackColor = backgroundColor, FontColor = textColor, Data = type };
		}
	}
}
