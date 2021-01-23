using System;
using DndCore;
using System.Collections.Generic;
using Streamloots;

namespace DHDM
{
	public static class CardCommands
	{
		static List<StreamlootsCard> savedCardsForViewerDieRolls = new List<StreamlootsCard>();
		public static void Execute(string command, CardDto cardDto, IDiceRoller iDiceRoller, DndViewer viewer)
		{
			if (command.StartsWith("RollDie"))
			{
				string parameterStr = command.EverythingAfter("(").EverythingBeforeLast(")");
				string[] parameters = parameterStr.Split(',');
				if (parameters.Length == 2)
				{
					string dieLabel = parameters[1];
					string dieStr = parameters[0];
					string[] dieParts = dieStr.Split('d');
					if (dieParts.Length == 2)
					{
						int quantity;
						int sides;
						if (int.TryParse(dieParts[0], out quantity))
							if (int.TryParse(dieParts[1], out sides))
							{
								DiceRoll diceRoll = new DiceRoll(DiceRollType.ExtraOnly);
								DiceDto diceDto = new DiceDto()
								{
									PlayerName = cardDto.Card.UserName,
									CreatureId = int.MinValue,
									Sides = sides,
									Quantity = quantity,
									Label = $"{cardDto.Card.UserName}'s {dieLabel}",
									Scale = viewer.Reputation + 0.5,
									BackColor = viewer.DieBackColor,
									FontColor = viewer.DieTextColor,
									Data = cardDto.Card.Guid
								};
								savedCardsForViewerDieRolls.Add(cardDto.Card);
								diceRoll.DiceDtos.Add(diceDto);
								diceRoll.DiceGroup = DiceGroup.Viewers;
								diceRoll.RollScope = RollScope.Viewer;
								iDiceRoller.RollTheDice(diceRoll);
							}
					}
				}
			}
		}
	}
}
