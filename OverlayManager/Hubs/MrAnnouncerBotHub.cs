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

		public void ChangePlayerHealth(string playerData)
		{
			coderushedHub.Clients.All.ChangePlayerHealth(playerData);
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
