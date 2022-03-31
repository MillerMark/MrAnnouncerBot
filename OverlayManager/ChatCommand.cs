using BotCore;
using Microsoft.AspNetCore.SignalR;
using OverlayManager.Hubs;
using System;
using SheetsPersist;
using System.Collections.Generic;
using System.Linq;
using TwitchLib.Client.Models;

namespace OverlayManager
{
	[Document("Mr. Announcer Guy")]
	[Sheet("Commands")]
	public class ChatCommand
	{
		public string CommandStr { get; set; }

		List<string> commands;
		bool commandBecomesArgs;
		string chatBackMessage;

		[Column]
		public string Command { get; set; }

		[Column]
		public string TranslatedCommand { get; set; }

		[Column]
		public string Aliases { get; set; }

		[Column]
		public bool CommandBecomesArg { get => commandBecomesArgs; set => commandBecomesArgs = value; }

		[Column("ChatBack")]
		// TODO: Rename ChatBack2 to ChatBack
		public string ChatBack2 { get => chatBackMessage; set => chatBackMessage = value; }

		public ChatCommand(string command, string translatedCommand) : this(command)
		{
			TranslatedCommand = translatedCommand;
		}

		[Column]
		public string MarkFliesCommand { get; set; }

		[Column]
		public string MarkFliesData { get; set; }

		public ChatCommand(string command)
		{
			commands = new List<string>();
			commands.Add(command);
		}

		public ChatCommand()
		{
		}

		void InitializeCommands()
		{
			commands = new List<string>();
			commands.Add(Command);
			if (string.IsNullOrWhiteSpace(Aliases))
				return;
			string[] aliases = Aliases.Split(',');
			foreach (string alias in aliases)
				commands.Add(alias.Trim(' ').Trim('"'));
		}

		public bool Matches(string cmdText)
		{
			if (commands == null)
				InitializeCommands();
			foreach (string command in commands)
				if (string.Compare(command, cmdText, true) == 0)
					return true;
			return false;
		}

		public ChatCommand SetCommandBecomesArg()
		{
			commandBecomesArgs = true;
			return this;
		}

		public ChatCommand AddAliases(params string[] aliases)
		{
			foreach (string alias in aliases)
				commands.Add(alias);
			return this;
		}

		public ChatCommand SetChatBack(string chatMessage)
		{
			chatBackMessage = chatMessage;
			return this;
		}

		public virtual string Translate(string cmdText)
		{
			if (!string.IsNullOrWhiteSpace(TranslatedCommand))
				return TranslatedCommand;

			return cmdText;
		}

		public virtual void Execute(IHubContext<CodeRushedHub, IOverlayCommands> hub, ChatMessage chatMessage, string cmdText, string args, int showsWatched)
		{
			if (commandBecomesArgs)
				args = cmdText;
			string targetCommand = Translate(cmdText);
			if (targetCommand != null)
			{
				ExecuteChatCommand(hub, chatMessage, args, showsWatched, targetCommand);
			}

			if (chatBackMessage != null)
				Twitch.Chat(Twitch.CodeRushedClient, chatBackMessage);
		}

		private void ExecuteChatCommand(IHubContext<CodeRushedHub, IOverlayCommands> hub, ChatMessage chatMessage, string args, int showsWatched, string targetCommand)
		{
			UserInfo userInfo = UserInfo.FromChatMessage(chatMessage, showsWatched);

			if (string.IsNullOrWhiteSpace(MarkFliesCommand))
				hub.Clients.All.ExecuteCommand(targetCommand, args, userInfo.userId, userInfo.userName, userInfo.displayName, userInfo.color, userInfo.showsWatched);
			else
			{
				if (!string.IsNullOrWhiteSpace(args))
					MarkFliesData = args;
				hub.Clients.All.ControlSpaceship(MarkFliesCommand, MarkFliesData, userInfo.userId, userInfo.userName, userInfo.displayName, userInfo.color, userInfo.showsWatched);
			}
		}

		public static void RegisterSheet()
		{
			GoogleSheets.RegisterDocumentID("Mr. Announcer Guy", "1s-j-4EF3KbI8ZH0nSj4G4a1ApNFPz_W5DK9A9JTyb3g");
		}
	}
}
