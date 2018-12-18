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
	public interface IAnnouncerBotCommands
	{
		Task AddCoins(string userID, int amount);
	}

	public interface IOverlayCommands
	{
		Task ExecuteCommand(string command, string args, string userId, string userName, string displayName, string color);
	}
}
