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
			Twitch.Chat(Twitch.CodeRushedClient, message);
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

		public void DiceHaveStoppedRolling(string diceData)
		{
			mrAnnouncerBotHub.Clients.All.DiceHaveStoppedRolling(diceData);
		}

		public void UpdateVideoFeed(string videoFeedData)
		{
			mrAnnouncerBotHub.Clients.All.UpdateVideoFeed(videoFeedData);
		}

		public void TellDM(string message)
		{
			mrAnnouncerBotHub.Clients.All.TellDM(message);
		}

		public void AllDiceHaveBeenDestroyed(string diceData)
		{
			mrAnnouncerBotHub.Clients.All.AllDiceHaveBeenDestroyed(diceData);
		}

		public void InGameUIResponse(string response)
		{
			mrAnnouncerBotHub.Clients.All.InGameUIResponse(response);
		}

		public void Arm(string userId)
		{
			mrAnnouncerBotHub.Clients.All.Arm(userId);
		}

		public void Disarm(string userId)
		{
			mrAnnouncerBotHub.Clients.All.Disarm(userId);
		}

		public void Fire(string userId)
		{
			mrAnnouncerBotHub.Clients.All.Fire(userId);
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
