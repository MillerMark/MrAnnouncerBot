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
	public interface IOverlayCommands
	{
		Task ExecuteCommand(string command, string args, string userId, string displayName, string color);
	}

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
	}
}
