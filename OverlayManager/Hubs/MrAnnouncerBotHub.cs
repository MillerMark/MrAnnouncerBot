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

		readonly IHubContext<CodeRushedHub, IOverlayCommands> coderushedHub;
		public MrAnnouncerBotHub(IHubContext<CodeRushedHub, IOverlayCommands> hub)
		{
			coderushedHub = hub;
		}
	}
}
