using BotCore;
using Microsoft.AspNetCore.SignalR;
using OverlayManager.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using TwitchLib.Client.Models;

namespace OverlayManager
{
	public class ChatCommand
	{
		List<string> commands = new List<string>();
		readonly string translatedCommand;
		bool commandBecomesArgs;
		string chatBackMessage;

		public ChatCommand(string command, string translatedCommand): this(command)
		{
			this.translatedCommand = translatedCommand;
		}

		public ChatCommand(string command)
		{
			commands.Add(command);
		}

		public bool Matches(string cmdText)
		{
			foreach (string command in commands)
				if (string.Compare(command, cmdText, true) == 0)
					return true;
			return false;
		}

		public ChatCommand CommandBecomesArg()
		{
			commandBecomesArgs = true;
			return this;
		}

		public ChatCommand AddAliases(params string[] aliases)
		{
			foreach (string alias in aliases)
			{
				commands.Add(alias);
			}
			return this;
		}

		public ChatCommand ChatBack(string chatMessage)
		{
			chatBackMessage = chatMessage;
			return this;
		}

		public virtual string Translate(string cmdText)
		{
			if (translatedCommand != null)
				return translatedCommand;

			return cmdText;
		}

		public virtual void Execute(IHubContext<CodeRushedHub, IOverlayCommands> hub, ChatMessage chatMessage, string cmdText, string args, int showsWatched)
		{
			if (commandBecomesArgs)
				args = cmdText;
			string targetCommand = Translate(cmdText);
			if (targetCommand != null)
			{
				UserInfo userInfo = UserInfo.FromChatMessage(chatMessage, showsWatched);
				hub.Clients.All.ExecuteCommand(targetCommand, args, userInfo.userId, userInfo.userName, userInfo.displayName, userInfo.color, userInfo.showsWatched);
			}

			if (chatBackMessage != null)
				Twitch.Chat(Twitch.CodeRushedClient, chatBackMessage);
		}
	}
}
