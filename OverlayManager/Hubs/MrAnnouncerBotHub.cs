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
			string[] sceneNames = { "Kent's Turn", "Karen's Turn", "Mark's Turn", "Kayla's Turn" };
			if (playerID >= 0 && playerID < sceneNames.Length)
				Clients.All.ChangeScene(sceneNames[playerID]);
			coderushedHub.Clients.All.PlayerDataChanged(playerID, pageID, playerData);
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

		readonly IHubContext<CodeRushedHub, IOverlayCommands> coderushedHub;
		public MrAnnouncerBotHub(IHubContext<CodeRushedHub, IOverlayCommands> hub)
		{
			coderushedHub = hub;
		}
	}
}
