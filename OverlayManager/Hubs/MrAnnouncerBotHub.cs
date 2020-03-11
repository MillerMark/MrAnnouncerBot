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
		public void MapDataChanged(string mapData)
		{
			coderushedHub.Clients.All.MapDataChanged(mapData);
		}

		public void ChangePlayerHealth(string playerData)
		{
			coderushedHub.Clients.All.ChangePlayerHealth(playerData);
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

		public void AnimateSprinkles(string commandData)
		{
			coderushedHub.Clients.All.AnimateSprinkles(commandData);
		}
		
		public void UpdateClock(string clockData)
		{
			coderushedHub.Clients.All.UpdateClock(clockData);
		}

		public void RollDice(string diceRollData)
		{
			coderushedHub.Clients.All.RollDice(diceRollData);
		}
		public void ClearDice()
		{
			coderushedHub.Clients.All.ClearDice();
		}
		public void SetPlayerData(string playerData)
		{
			coderushedHub.Clients.All.SetPlayerData(playerData);
		}
		public void SendScrollLayerCommand(string commandData)
		{
			coderushedHub.Clients.All.SendScrollLayerCommand(commandData);
		}

		readonly IHubContext<CodeRushedHub, IOverlayCommands> coderushedHub;
		public MrAnnouncerBotHub(IHubContext<CodeRushedHub, IOverlayCommands> hub)
		{
			coderushedHub = hub;
		}
	}
}
