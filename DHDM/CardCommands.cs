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
				RollDie(command, cardDto, iDiceRoller, viewer);
		}

		private static void RollDie(string command, CardDto cardDto, IDiceRoller iDiceRoller, DndViewer viewer)
		{
			string parameterStr = command.EverythingAfter("(").EverythingBeforeLast(")");
			string[] parameters = parameterStr.Split(',');
			if (parameters.Length != 2)
				return;

			string dieLabel = parameters[1];
			string dieStr = parameters[0];
			string[] dieParts = dieStr.Split('d');
			if (dieParts.Length != 2)
				return;

			int quantity;
			int sides;
			if (!int.TryParse(dieParts[0], out quantity))
				return;

			if (!int.TryParse(dieParts[1], out sides))
				return;
			
			DiceRoll diceRoll = new DiceRoll(DiceRollType.ExtraOnly);
			diceRoll.AddTrailingEffects(viewer.TrailingEffects);

			Imaging.HueSatLight hueSatLight = new Imaging.HueSatLight(viewer.DieBackColor);
			foreach (TrailingEffect trailingEffect in diceRoll.TrailingEffects)
			{
				trailingEffect.HueShift = (360 * hueSatLight.Hue).ToString();
			}

			DiceDto diceDto = new DiceDto()
			{
				PlayerName = cardDto.Card.UserName,
				CreatureId = int.MinValue,
				Sides = sides,
				Quantity = quantity,
				Label = $"{cardDto.Card.UserName}",
				Scale = viewer.Reputation * 1.2 + 0.35,
				BackColor = viewer.DieBackColor,
				FontColor = viewer.DieTextColor,
				Data = cardDto.Card.Guid
			};
			diceRoll.DieTotalMessage = $"{cardDto.Card.UserName}'s {dieLabel}";
			diceRoll.TextOutlineColor = viewer.DieTextColor;
			diceRoll.TextFillColor = viewer.DieBackColor;
			savedCardsForViewerDieRolls.Add(cardDto.Card);
			diceRoll.DiceDtos.Add(diceDto);
			diceRoll.DiceGroup = DiceGroup.Viewers;
			diceRoll.RollScope = RollScope.Viewer;
			iDiceRoller.RollTheDice(diceRoll);
		}
	}
}
