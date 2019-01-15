using Microsoft.AspNetCore.SignalR;
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

		public void PlayerPageChanged(int playerID, int pageID, string playerData)
		{
			coderushedHub.Clients.All.PlayerPageChanged(playerID, pageID, playerData);
		}

		public void FocusItem(int playerID, int pageID, string itemID)
		{
			coderushedHub.Clients.All.FocusItem(playerID, pageID, itemID);
		}

		readonly IHubContext<CodeRushedHub, IOverlayCommands> coderushedHub;
		public MrAnnouncerBotHub(IHubContext<CodeRushedHub, IOverlayCommands> hub)
		{
			coderushedHub = hub;
		}
	}
}
