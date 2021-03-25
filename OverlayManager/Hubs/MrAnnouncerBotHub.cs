using BotCore;
using Microsoft.AspNetCore.SignalR;
using OBSWebsocketDotNet;
using System;
using System.Linq;

namespace OverlayManager.Hubs
{
	public class MrAnnouncerBotHub : Hub<IAnnouncerBotCommands>
	{
		public void UserHasCoins(string userId, int amount)
		{
			coderushedHub.Clients.All.UserHasCoins(userId, amount);
		}

		public void SuppressVolume(int seconds)
		{
			coderushedHub.Clients.All.SuppressVolume(seconds);
		}

		public void PlayerDataChanged(int playerID, int pageID, string playerData)
		{
			const int numPlayers = 4;
			string sceneName = $"Player {playerID + 1}/{numPlayers}";
			Clients.All.ChangeScene(sceneName);
			coderushedHub.Clients.All.PlayerDataChanged(playerID, pageID, playerData);
		}

		public void DmDataChanged(string dmData)
		{
			coderushedHub.Clients.All.DmDataChanged(dmData);
		}

		public void CalibrateLeapMotion(string calibrationData)
		{
			coderushedHub.Clients.All.CalibrateLeapMotion(calibrationData);
		}

		public void UpdateSkeletalData(string skeletalData)
		{
			coderushedHub.Clients.All.UpdateSkeletalData(skeletalData);
		}

		public void SpeechBubble(string speechStr)
		{
			coderushedHub.Clients.All.SpeechBubble(speechStr);
		}
		
		public void CardCommand(string cardStr)
		{
			coderushedHub.Clients.All.CardCommand(cardStr);
		}

		public void ContestCommand(string contestStr)
		{
			coderushedHub.Clients.All.ContestCommand(contestStr);
		}

		public void ChangePlayerHealth(string playerData)
		{
			coderushedHub.Clients.All.ChangePlayerHealth(playerData);
		}

		public void ChangePlayerStats(string playerStatsData)
		{
			coderushedHub.Clients.All.ChangePlayerStats(playerStatsData);
		}
		
		public void ChangePlayerWealth(string playerData)
		{
			coderushedHub.Clients.All.ChangePlayerWealth(playerData);
		}

		public void ChangeFrameRate(string frameRateData)
		{
			coderushedHub.Clients.All.ChangeFrameRate(frameRateData);
		}

		public void InGameUICommand(string commandData)
		{
			coderushedHub.Clients.All.InGameUICommand(commandData);
		}

		public void AddWindup(string windupData)
		{
			coderushedHub.Clients.All.AddWindup(windupData);
		}

		public void CastSpell(string spellData)
		{
			coderushedHub.Clients.All.CastSpell(spellData);
		}

		public void ClearWindup(string windupName)
		{
			coderushedHub.Clients.All.ClearWindup(windupName);
		}

		public void MoveFred(string movement)
		{
			coderushedHub.Clients.All.MoveFred(movement);
		}

		public void FocusItem(int playerID, int pageID, string itemID)
		{
			coderushedHub.Clients.All.FocusItem(playerID, pageID, itemID);
		}

		public void UnfocusItem(int playerID, int pageID, string itemID)
		{
			coderushedHub.Clients.All.UnfocusItem(playerID, pageID, itemID);
		}

		public void TriggerEffect(string effectData)
		{
			coderushedHub.Clients.All.TriggerEffect(effectData);
		}

		public void PlaySound(string soundFileName)
		{
			coderushedHub.Clients.All.PlaySound(soundFileName);
		}

		public void AnimateSprinkles(string commandData)
		{
			coderushedHub.Clients.All.AnimateSprinkles(commandData);
		}

		public void UpdateInGameCreatures(string commandData)
		{
			coderushedHub.Clients.All.UpdateInGameCreatures(commandData);
		}
		
		public void UpdateClock(string clockData)
		{
			coderushedHub.Clients.All.UpdateClock(clockData);
		}

		public void FloatPlayerText(int playerID, string message, string fillColor, string outlineColor)
		{
			coderushedHub.Clients.All.FloatPlayerText(playerID, message, fillColor, outlineColor);
		}

		public void RollDice(string diceRollData)
		{
			coderushedHub.Clients.All.RollDice(diceRollData);
		}
		public void ClearDice(string diceGroup)
		{
			coderushedHub.Clients.All.ClearDice(diceGroup);
		}
		public void SetPlayerData(string playerData)
		{
			coderushedHub.Clients.All.SetPlayerData(playerData);
		}
		public void SendScrollLayerCommand(string commandData)
		{
			coderushedHub.Clients.All.SendScrollLayerCommand(commandData);
		}
		public void ExecuteSoundCommand(string commandData)
		{
			coderushedHub.Clients.All.ExecuteSoundCommand(commandData);
		}
		public void ShowValidationIssue(string commandData)
		{
			coderushedHub.Clients.All.ShowValidationIssue(commandData);
		}

		readonly IHubContext<CodeRushedHub, IOverlayCommands> coderushedHub;
		public MrAnnouncerBotHub(IHubContext<CodeRushedHub, IOverlayCommands> hub)
		{
			coderushedHub = hub;
		}
	}
}
