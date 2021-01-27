using System;
using DndCore;
using System.Collections.Generic;
using System.Linq;
using Streamloots;
using System.Timers;
using Imaging;

namespace DHDM
{
	public static class CardCommands
	{
		static bool waitingForDiceToBeDestroyed;
		static List<CardDto> savedCardsForViewerDieRolls = new List<CardDto>();
		static Queue<DiceRoll> viewerRollQueue = new Queue<DiceRoll>();
		private const int TimeToForceRollDice = 22000;
		static Timer timer = new Timer(TimeToForceRollDice);
		static CardCommands()
		{
			HubtasticBaseStation.AllDiceDestroyed += HubtasticBaseStation_AllDiceDestroyed;
			HubtasticBaseStation.DiceStoppedRolling += HubtasticBaseStation_DiceStoppedRolling;
			timer.Elapsed += Timer_Elapsed;
		}

		static void RollIsComplete(CardDto cardInWaiting, DiceStoppedRollingData stopRollingData)
		{
			// TODO: Complete roll with card saved in savedCardsForViewerDieRolls.
		}

		private static void HubtasticBaseStation_DiceStoppedRolling(object sender, DiceEventArgs ea)
		{
			if (ea.StopRollingData == null)
			{
				System.Diagnostics.Debugger.Break();
				return;
			}

			if (ea.StopRollingData.diceGroup == DiceGroup.Viewers)
			{
				CardDto cardInWaiting = savedCardsForViewerDieRolls.FirstOrDefault(x => x.CardID == ea.StopRollingData.rollId);
				if (cardInWaiting != null)
				{
					savedCardsForViewerDieRolls.Remove(cardInWaiting);
					RollIsComplete(cardInWaiting, ea.StopRollingData);
				}
				//if (viewerRollQueue.Count > 0)
				//	DiceRoller.ClearViewerDice();  // Accelerate the destruction of the die in case we have viewers in the queue.
			}
		}

		private static void HubtasticBaseStation_AllDiceDestroyed(object sender, DiceEventArgs ea)
		{
			if (ea.StopRollingData == null)
				return;

			if (ea.StopRollingData.diceGroup == DiceGroup.Viewers)
			{
				DequeueViewerRoll();
			}
		}

		public static void Execute(string command, CardDto cardDto, DndViewer viewer)
		{
			if (command.StartsWith("RollDie"))
				RollViewerDie(command, cardDto, viewer);
		}

		private static void RollViewerDie(string command, CardDto cardDto, DndViewer viewer)
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

			HueSatLight hueSatLight = new HueSatLight(viewer.DieBackColor);
			CorrectHueShifts(diceRoll, hueSatLight);

			DiceDto diceDto = new DiceDto()
			{
				PlayerName = cardDto.Card.UserName,
				CreatureId = int.MinValue,
				Sides = sides,
				Quantity = quantity,
				Label = $"{cardDto.Card.UserName}",
				Scale = viewer.Reputation * 1 + 0.35,
				BackColor = viewer.DieBackColor,
				FontColor = viewer.DieTextColor,
				Data = cardDto.Card.Guid
			};

			diceRoll.DieTotalMessage = $"{cardDto.Card.UserName}'s {dieLabel}";
			diceRoll.TextOutlineColor = viewer.DieTextColor;
			diceRoll.TextFillColor = viewer.DieBackColor;
			savedCardsForViewerDieRolls.Add(cardDto);
			diceRoll.DiceDtos.Add(diceDto);
			diceRoll.DiceGroup = DiceGroup.Viewers;
			diceRoll.RollScope = RollScope.Viewer;
			diceRoll.RollID = cardDto.CardID;

			if (waitingForDiceToBeDestroyed)
			{
				if (!timer.Enabled)
					timer.Start();
				viewerRollQueue.Enqueue(diceRoll);
				return;
			}

			RollViewerDice(diceRoll);
		}

		private static void CorrectHueShifts(DiceRoll diceRoll, HueSatLight hueSatLight)
		{
			foreach (TrailingEffect trailingEffect in diceRoll.TrailingEffects)
			{
				double hueOffset = 0;
				if (trailingEffect.Name == "FrostTrail")
					hueOffset = 163;  // FrostTrail raw color is blue - this shifts it back to red.

				trailingEffect.HueShift = (360 * hueSatLight.Hue + hueOffset).ToString();
			}
		}

		private static void RollViewerDice(DiceRoll diceRoll)
		{
			timer.Start();
			waitingForDiceToBeDestroyed = true;
			DiceRoller.RollTheDice(diceRoll);
		}

		static void DequeueViewerRoll()
		{
			timer.Stop();
			waitingForDiceToBeDestroyed = false;

			if (viewerRollQueue.Count == 0)
				return;

			RollViewerDice(viewerRollQueue.Dequeue());
			// TODO: Update in-game UI
		}

		private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			System.Diagnostics.Debugger.Break();    // We should never reach this point. Expecting die roll to finish and it never did?
			DequeueViewerRoll();
		}

		public static void RegisterDiceRoller(IDiceRoller diceRoller)
		{
			DiceRoller = diceRoller;
		}

		public static IDiceRoller DiceRoller { get; set; }
	}
}
