//#define profiling
using DndCore;
using System;
using System.Linq;
using System.Collections.Generic;

namespace DHDM
{
	public class BreakSpellConcentrationSavingThrowQueueEntry : DieRollQueueEntry
	{
		public const string STR_ConcentrationSave = "Concentration Save";
		List<int> additionalPlayerIds = new List<int>();
		public BreakSpellConcentrationSavingThrowQueueEntry()
		{
			RollType = DiceRollType.SavingThrow;
			RollScope = RollScope.ActivePlayer;
		}

		public override bool CombineWith(ActionQueueEntry dieRoll)
		{
			if (dieRoll is BreakSpellConcentrationSavingThrowQueueEntry breakSpellConcentrationSavingThrowQueueEntry)
			{
				int playerId = breakSpellConcentrationSavingThrowQueueEntry.PlayerId;

				if (additionalPlayerIds.Any(x => x == playerId))
					return false;  // Cannot combine rolls.
				additionalPlayerIds.Add(playerId);
				return true;
			}

			return false;
		}

		public override void PrepareRoll(DiceRoll diceRoll)
		{
			base.PrepareRoll(diceRoll);
			diceRoll.SavingThrow = Ability.constitution;
			diceRoll.SuppressLegacyRoll = true;

			AddConcentrationSaveForPlayer(diceRoll, PlayerId);

			foreach (int additionalPlayerId in additionalPlayerIds)
			{
				AddConcentrationSaveForPlayer(diceRoll, additionalPlayerId);
			}
		}

		private void AddConcentrationSaveForPlayer(DiceRoll diceRoll, int playerId)
		{
			string backgroundColor, textColor;
			Character player = AllPlayers.GetFromId(playerId);
			GetPlayerDieColor(player, out backgroundColor, out textColor);
			string name = "";
			if (player != null)
				name = player.name;
			DiceDto diceDto = CreateDie(playerId, backgroundColor, textColor, name, 20, STR_ConcentrationSave);
			diceDto.Label = STR_ConcentrationSave;
			diceRoll.DiceDtos.Add(diceDto);
		}

		private DiceDto CreateDie(int playerId, string backgroundColor, string textColor, string playerName, int sides, string type = "")
		{
			return new DiceDto() { PlayerName = playerName, CreatureId = playerId, Sides = sides, BackColor = backgroundColor, FontColor = textColor, Data = type };
		}
	}
}
