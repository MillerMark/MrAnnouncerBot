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
		public static event ViewerDieRollEndsEventHandler ViewerDieRollComplete;
		public static event ViewerDieRollStartsEventHandler ViewerDieRollStarts;
		static bool logDieRollIds = false;
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
		const int INT_ViewerSpellCasterDC = 12;

		static Timer forceDieRollTimer = new Timer(TimeToForceRollDice);
		static Timer clearDiceNowTimer = new Timer(TimeToClearNowDice);
		static CardCommands()
		{
			HubtasticBaseStation.AllDiceDestroyed += HubtasticBaseStation_AllDiceDestroyed;
			HubtasticBaseStation.DiceStoppedRolling += HubtasticBaseStation_DiceStoppedRolling;
			forceDieRollTimer.Elapsed += ForceDieRollTimer_Elapsed;
			clearDiceNowTimer.Elapsed += ClearDiceNowTimer_Elapsed;
		}

		public static void OnViewerDieRollComplete(object sender, ViewerDieRollStoppedEventArgs ea)
		{
			ViewerDieRollComplete?.Invoke(sender, ea);
		}

		public static void OnViewerDieRollStarts(object sender, ViewerDieRollStartedEventArgs ea)
		{
			ViewerDieRollStarts?.Invoke(sender, ea);
		}

		static void RollIsComplete(CardDto card, DiceStoppedRollingData stopRollingData)
		{
			OnViewerDieRollComplete(null, new ViewerDieRollStoppedEventArgs(card, stopRollingData));
		}

		static void RollStarts(CardDto card, DiceRoll startRollingData)
		{
			OnViewerDieRollStarts(null, new ViewerDieRollStartedEventArgs(card, startRollingData));
		}

		private static void HubtasticBaseStation_DiceStoppedRolling(object sender, DiceEventArgs ea)
		{
			if (ea.StopRollingData.diceGroup == DiceGroup.Viewers)
			{
				if (logDieRollIds)
					History.Log($"HubtasticBaseStation_DiceStoppedRolling - {ea.StopRollingData.rollId.Substring(ea.StopRollingData.rollId.Length - 4)}");
				CardDto cardInWaiting = savedCardsForViewerDieRolls.FirstOrDefault(x => x.InstanceID == ea.StopRollingData.rollId);
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
				if (logDieRollIds)
					History.Log($"HubtasticBaseStation_AllDiceDestroyed - {ea.StopRollingData.rollId.Substring(ea.StopRollingData.rollId.Length - 4)}");

				DequeueViewerRoll();
			}
		}

		public static void Execute(string command, CardDto cardDto, DndViewer viewer)
		{
			if (command.StartsWith(CardDto.CMD_RollDie))
				RollViewerDie(command, cardDto, viewer);
			else
				Expressions.Do(command);
		}

		static DiceRoll CreateRoll(string command, CardDto cardDto, DndViewer viewer)
		{
			string parameterStr = command.EverythingAfter("(").EverythingBeforeLast(")");
			string[] parameters = parameterStr.Split(',');
			if (parameters.Length < 2)
				return null;

			DiceRoll diceRoll = new DiceRoll(DiceRollType.ViewerRoll);
			diceRoll.AddTrailingEffects(viewer.TrailingEffects);

			HueSatLight hueSatLight = new HueSatLight(viewer.DieBackColor);
			CorrectHueShifts(diceRoll, hueSatLight);
			diceRoll.DieTotalMessage = $"{cardDto.Card.UserName}'s {cardDto.Card.CardName}";
			diceRoll.TextOutlineColor = viewer.DieTextColor;
			diceRoll.TextFillColor = viewer.DieBackColor;
			diceRoll.DiceGroup = DiceGroup.Viewers;
			diceRoll.RollScope = RollScope.Viewer;
			diceRoll.RollID = cardDto.InstanceID;
			diceRoll.Viewer = cardDto.Card.UserName;

			for (int i = 0; i < parameters.Length; i += 2)
			{
				string dieLabel = parameters[i + 1].Trim();
				string dieStr = parameters[i].Trim();
				if (dieStr.Contains("save:"))
					foreach (int targetCharacterId in cardDto.TargetCharacterIds)
					{
						DiceDto diceDto = AddDieStr(diceRoll, cardDto, viewer, dieStr, dieLabel, targetCharacterId);
						diceDto.DieCountsAs = DieCountsAs.savingThrow;
					}
				else
					AddDieStr(diceRoll, cardDto, viewer, dieStr, dieLabel);
			}

			savedCardsForViewerDieRolls.Add(cardDto);

			return diceRoll;
		}

		static string GetDieBackColor(Creature targetCreature)
		{
			if (targetCreature is Character player)
				return player.dieBackColor;
			InGameCreature inGameCreature = AllInGameCreatures.GetByCreature(targetCreature);
			if (inGameCreature != null)
				return inGameCreature.BackgroundHex;
			return "#ffffff";
		}

		static string GetDieTextColor(Creature targetCreature, string dieBackColor)
		{
			if (targetCreature is Character player)
				return player.dieFontColor;
			InGameCreature inGameCreature = AllInGameCreatures.GetByCreature(targetCreature);
			if (inGameCreature != null)
				return inGameCreature.ForegroundHex;
			return DndViewer.GetDieTextColor(dieBackColor);
		}

		private static DiceDto AddDieStr(DiceRoll diceRoll, CardDto cardDto, DndViewer viewer, string dieStr, string dieLabelOverride = null, int targetCharacterId = int.MinValue)
		{
			string dieBackColorOverride = viewer.DieBackColor;
			string dieTextColorOverride = viewer.DieTextColor;
			int parenIndex = dieStr.IndexOf("(");
			DamageType damageType = DamageType.None;
			DieCountsAs dieCountsAs = DieCountsAs.totalScore;
			string diePlayerName = cardDto.Card.UserName;
			double modifier = 0;
			double scaleOverride = viewer.Reputation + 0.30;
			int dieOwnerOverride = int.MinValue;
			if (parenIndex >= 0)
				ProcessDieDetails(diceRoll, targetCharacterId, ref dieStr, ref dieBackColorOverride, ref dieTextColorOverride, parenIndex, ref damageType, ref dieCountsAs, ref modifier, ref diePlayerName, ref scaleOverride, ref dieOwnerOverride);

			string[] dieParts = dieStr.Split('d');
			if (dieParts.Length != 2)
				return null;

			string dieLabel;
			if (string.IsNullOrWhiteSpace(dieLabelOverride) || dieLabelOverride.Trim() == "\"\"")
				dieLabel = $"{cardDto.Card.UserName}";
			else
				dieLabel = dieLabelOverride.Trim().TrimStart('"').TrimEnd('"');

			int quantity;
			int sides;
			string sidesStr = dieParts[1];
			int offset;
			string offsetStr = "0";
			if (sidesStr.Contains("+"))
			{
				offsetStr = sidesStr.EverythingAfter("+").Trim();
				sidesStr = sidesStr.EverythingBefore("+").Trim();
			}
			if (!int.TryParse(dieParts[0], out quantity) || !int.TryParse(sidesStr, out sides) || !int.TryParse(offsetStr, out offset))
				return null;

			diceRoll.Modifier = offset;
			DiceDto diceDto = new DiceDto()
			{
				PlayerName = diePlayerName,
				CreatureId = dieOwnerOverride,
				Sides = sides,
				Quantity = quantity,
				Label = dieLabel.Replace("target_name", DndUtils.GetFirstName(diePlayerName)),
				Scale = scaleOverride,
				Modifier = modifier,
				DamageType = damageType,
				BackColor = dieBackColorOverride,
				FontColor = dieTextColorOverride,
				DieCountsAs = dieCountsAs,
				Data = cardDto.Card.Guid
			};
			diceRoll.DiceDtos.Add(diceDto);
			return diceDto;
		}

		private static void ProcessDieDetails(DiceRoll diceRoll, int dieOwnerId, ref string dieStr, ref string dieBackColorOverride, 
																					ref string dieTextColorOverride, int parenIndex, ref DamageType damageType, ref DieCountsAs dieCountsAs, 
																					ref double modifier, ref string diePlayerName, ref double scaleOverride, ref int dieOwnerOverride)
		{
			string dieTypeStr = dieStr.Substring(parenIndex).Trim();
			dieStr = dieStr.Substring(0, parenIndex);

			int colonPos = dieTypeStr.IndexOf(':');
			if (colonPos > 0)
			{
				// Die format specifier - "type:detail", e.g., "save:Dexterity" for a Dexterity saving throw.
				string[] dieTypeParts = dieTypeStr.TrimStart('(').TrimEnd(')').Split(':');
				if (dieTypeParts.Length == 2)
				{
					string rollKindStr = dieTypeParts[0];
					string rollData = dieTypeParts[1];
					if (rollKindStr == "save")
					{
						PrepareForSavingThrow(diceRoll, dieOwnerId, rollData, ref dieBackColorOverride, ref dieTextColorOverride, ref modifier, ref diePlayerName, ref scaleOverride, ref dieOwnerOverride);
						dieCountsAs = DieCountsAs.savingThrow;
					}
					else
						dieCountsAs = DieCountsAs.totalScore;
				}
			}
			else
			{
				// Probably just straight damage...
				damageType = DndUtils.ToDamage(dieTypeStr);
				if (damageType != DamageType.None)
					dieCountsAs = DieCountsAs.damage;
			}
		}

		private static void PrepareForSavingThrow(DiceRoll diceRoll, int dieOwnerId, string rollData, ref string dieBackColorOverride, 
																							ref string dieTextColorOverride, ref double modifier, ref string diePlayerName, 
																							ref double scaleOverride, ref int dieOwnerOverride)
		{
			Ability savingThrowAbility = DndUtils.ToAbility(rollData);
			if (savingThrowAbility == Ability.none)
				return;

			// We have a saving throw!
			Creature targetCreature = DndUtils.GetCreatureById(dieOwnerId);
			if (targetCreature == null)
				return;
			dieOwnerOverride = dieOwnerId;

			diePlayerName = targetCreature.Name;
			if (scaleOverride > 1)
				scaleOverride = 1; // Player dice are always thrown at no more than 100% scale.
			modifier = targetCreature.GetSavingThrowModifier(savingThrowAbility);
			dieBackColorOverride = GetDieBackColor(targetCreature);
			dieTextColorOverride = GetDieTextColor(targetCreature, dieBackColorOverride);
			diceRoll.SavingThrow = savingThrowAbility;
			diceRoll.Type = DiceRollType.DamagePlusSavingThrow;
			diceRoll.HiddenThreshold = INT_ViewerSpellCasterDC;
			diceRoll.TrailingEffects.Clear();  // No viewer trailing effects on saving throws.
			diceRoll.DieTotalMessage = "";
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
			if (logDieRollIds)
				History.Log($"RollViewerDice - {diceRoll.RollID.Substring(diceRoll.RollID.Length - 4)}");
			clearDiceNowTimer.Stop();
			forceDieRollTimer.Start();
			waitingForDiceToBeDestroyed = true;
			CardDto cardInWaiting = savedCardsForViewerDieRolls.FirstOrDefault(x => x.InstanceID == diceRoll.RollID);
			if (cardInWaiting != null)
				RollStarts(cardInWaiting, diceRoll);
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
