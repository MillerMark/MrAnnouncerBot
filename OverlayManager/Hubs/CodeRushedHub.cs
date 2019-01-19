using Microsoft.AspNetCore.SignalR;
using System;
using BotCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitchLib.Client;
using TwitchLib.Client.Models;

namespace OverlayManager.Hubs
{
	public class CodeRushedHub: Hub<IOverlayCommands>
	{
		public void Chat(string message)
		{
			Twitch.Chat(message);
		}

		public void Whisper(string userName, string message)
		{
			Twitch.Whisper(userName, message);
		}

		public void AddCoins(string userId, int count) {
			mrAnnouncerBotHub.Clients.All.AddCoins(userId, count);
		}

		public void NeedToGetCoins(string userId)
		{
			mrAnnouncerBotHub.Clients.All.NeedToGetCoins(userId);
		}
		public void ChangeScene(string sceneName)
		{
			mrAnnouncerBotHub.Clients.All.ChangeScene(sceneName);
		}


		readonly IHubContext<MrAnnouncerBotHub, IAnnouncerBotCommands> mrAnnouncerBotHub;
		public CodeRushedHub(IHubContext<MrAnnouncerBotHub, IAnnouncerBotCommands> hub)
		{
			mrAnnouncerBotHub = hub;
		}
	}
}
