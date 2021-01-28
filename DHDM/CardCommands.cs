using System;
using System.Linq;
using System.Timers;
using System.Collections.Generic;
using DndCore;
using Imaging;
using Newtonsoft.Json;
using Streamloots;

namespace DHDM
{
	public static class CardCommands
	{
		static bool waitingForDiceToBeDestroyed;
		static List<CardDto> savedCardsForViewerDieRolls = new List<CardDto>();
		static Queue<DiceRoll> viewerRollQueue = new Queue<DiceRoll>();
		/// <summary>
		/// The time in ms between the start of the die roll and the time we force clear it.
		/// </summary>
		private const int TimeToForceRollDice = 32000;

		/// <summary>
		/// The time after the die have stopped moving to the time we force clear the dice.
		/// </summary>
		private const int TimeToClearNowDice = 1700;

		static Timer forceDieRollTimer = new Timer(TimeToForceRollDice);
		static Timer clearDiceNowTimer = new Timer(TimeToClearNowDice);
		static CardCommands()
		{
			HubtasticBaseStation.AllDiceDestroyed += HubtasticBaseStation_AllDiceDestroyed;
			HubtasticBaseStation.DiceStoppedRolling += HubtasticBaseStation_DiceStoppedRolling;
			forceDieRollTimer.Elapsed += ForceDieRollTimer_Elapsed;
			clearDiceNowTimer.Elapsed += ClearDiceNowTimer_Elapsed;
		}

		static void RollIsComplete(CardDto cardInWaiting, DiceStoppedRollingData stopRollingData)
		{
			// TODO: Complete roll with card saved in savedCardsForViewerDieRolls.
		}

		private static void HubtasticBaseStation_DiceStoppedRolling(object sender, DiceEventArgs ea)
		{
			if (ea.StopRollingData.diceGroup == DiceGroup.Viewers)
			{
				History.Log($"HubtasticBaseStation_DiceStoppedRolling - {ea.StopRollingData.rollId.Substring(ea.StopRollingData.rollId.Length - 4)}");
				CardDto cardInWaiting = savedCardsForViewerDieRolls.FirstOrDefault(x => x.CardID == ea.StopRollingData.rollId);
				if (cardInWaiting != null)
				{
					savedCardsForViewerDieRolls.Remove(cardInWaiting);
					RollIsComplete(cardInWaiting, ea.StopRollingData);
				}
				clearDiceNowTimer.Start();
			}
		}

		private static void HubtasticBaseStation_AllDiceDestroyed(object sender, DiceEventArgs ea)
		{
			clearDiceNowTimer.Stop();
			if (ea.StopRollingData == null)
				return;

			if (ea.StopRollingData.diceGroup == DiceGroup.Viewers)
			{
				History.Log($"HubtasticBaseStation_AllDiceDestroyed - {ea.StopRollingData.rollId.Substring(ea.StopRollingData.rollId.Length - 4)}");

				DequeueViewerRoll();
			}
		}

		public static void Execute(string command, CardDto cardDto, DndViewer viewer)
		{
			if (command.StartsWith("RollDie"))
				RollViewerDie(command, cardDto, viewer);
		}

		static DiceRoll CreateRoll(string command, CardDto cardDto, DndViewer viewer)
		{
			string parameterStr = command.EverythingAfter("(").EverythingBeforeLast(")");
			string[] parameters = parameterStr.Split(',');
			if (parameters.Length != 2)
				return null;

			string dieLabel = parameters[1];
			string dieStr = parameters[0];
			string[] dieParts = dieStr.Split('d');
			if (dieParts.Length != 2)
				return null;

			int quantity;
			int sides;
			if (!int.TryParse(dieParts[0], out quantity))
				return null;

			if (!int.TryParse(dieParts[1], out sides))
				return null;

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
				Scale = viewer.Reputation + 0.25,
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
			return diceRoll;
		}

		private static void RollViewerDie(string command, CardDto cardDto, DndViewer viewer)
		{
			DiceRoll diceRoll = CreateRoll(command, cardDto, viewer);
			if (diceRoll == null)
				return;

			if (waitingForDiceToBeDestroyed)
			{
				if (!forceDieRollTimer.Enabled)
					forceDieRollTimer.Start();
				WaitForRoll(diceRoll);
				return;
			}

			RollViewerDice(diceRoll);
		}

		static void UpdateInGameViewerRollQueue()
		{
			ViewerQueueDto viewerQueueDto = new ViewerQueueDto(viewerRollQueue);
			HubtasticBaseStation.CardCommand(JsonConvert.SerializeObject(viewerQueueDto));
		}

		private static void WaitForRoll(DiceRoll diceRoll)
		{
			viewerRollQueue.Enqueue(diceRoll);
			UpdateInGameViewerRollQueue();
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
			History.Log($"RollViewerDice - {diceRoll.RollID.Substring(diceRoll.RollID.Length - 4)}");
			clearDiceNowTimer.Stop();
			forceDieRollTimer.Start();
			waitingForDiceToBeDestroyed = true;
			DiceRoller.RollTheDice(diceRoll);
		}

		static void DequeueViewerRoll()
		{
			clearDiceNowTimer.Stop();
			forceDieRollTimer.Stop();
			waitingForDiceToBeDestroyed = false;

			if (viewerRollQueue.Count == 0)
				return;

			RollViewerDice(GetNewViewerRoll());
		}

		private static DiceRoll GetNewViewerRoll()
		{
			DiceRoll diceRoll = viewerRollQueue.Dequeue();
			UpdateInGameViewerRollQueue();
			return diceRoll;
		}

		private static void ForceDieRollTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			System.Diagnostics.Debugger.Break();    // We should never reach this point. Expecting die roll to finish and it never did?
			DequeueViewerRoll();
		}

		private static void ClearDiceNowTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			DiceRoller.ClearViewerDice();
		}

		public static void RegisterDiceRoller(IDiceRoller diceRoller)
		{
			DiceRoller = diceRoller;
		}

		public static IDiceRoller DiceRoller { get; set; }
	}
}
