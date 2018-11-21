using BotCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using TwitchLib.Client.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TwitchLib.Client;
using Microsoft.AspNetCore.SignalR;
using OverlayManager.Hubs;

namespace OverlayManager
{
	public class BackgroundWorker: IHostedService
	{
		private const string STR_ChannelName = "CodeRushed";
		private const string STR_TwitchUserName = "MrAnnouncerGuy";
		//TwitchClient twitchClient = new TwitchClient();
		readonly IHubContext<CodeRushedHub, IOverlayCommands> hub;
		public BackgroundWorker(IConfiguration configuration, IHubContext<CodeRushedHub, IOverlayCommands> hub)
		{
			this.hub = hub;
			Configuration = configuration;
		}

		//private void ConnectTwitchClient()
		//{
		//	var oAuthToken = Configuration["Secrets:TwitchBotOAuthToken"];
		//	var connectionCredentials = new ConnectionCredentials(STR_TwitchUserName, oAuthToken);
		//	twitchClient.Initialize(connectionCredentials, STR_ChannelName);
		//}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			//ConnectTwitchClient();
			HookEvents();
			//twitchClient.Connect();
			Twitch.InitializeConnections();
			return Task.CompletedTask;
		}

		public void HookEvents()
		{
			Twitch.Client.OnLog += TwitchClient_OnLog;
			Twitch.Client.OnConnectionError += TwitchClient_OnConnectionError;
			Twitch.Client.OnJoinedChannel += TwitchClient_OnJoinedChannel;
			Twitch.Client.OnChatCommandReceived += TwitchClient_OnChatCommandReceived;
			Twitch.Client.OnMessageReceived += TwitchClient_OnMessageReceived;
			Twitch.Client.OnWhisperReceived += TwitchClient_OnWhisperReceived;
			Twitch.Client.OnUserJoined += TwitchClient_OnUserJoined;
			Twitch.Client.OnUserLeft += TwitchClient_OnUserLeft;
		}

		private void TwitchClient_OnUserLeft(object sender, TwitchLib.Client.Events.OnUserLeftArgs e)
		{
			
		}

		private void TwitchClient_OnUserJoined(object sender, TwitchLib.Client.Events.OnUserJoinedArgs e)
		{
			
		}

		CommandExpansion GetCommandExpansion(string msg)
		{
			string message = msg.ToLower();
			if (message.StartsWith('!'))
				message = message.Substring(1);
			if (message.Length == 0)
				return null;

			bool secondCharIsNumberOrSpaceOrNull = (message.Length == 1 || message[1] == ' ' || int.TryParse(message[1].ToString(), out int number));

			if (secondCharIsNumberOrSpaceOrNull)
			{
				char firstChar = message[0];

				switch (firstChar)
				{
					case 'd':
					case 'u':
					case 'l':
					case 'r':
					case 't':
						return new CommandExpansion(firstChar.ToString(), message.Substring(1));
				}
			}

			return null;
		}

		private void TwitchClient_OnMessageReceived(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
		{
			string msg = e.ChatMessage.Message;
			switch (msg)
			{
				case "1":
				case "2":
				case "3":
				case "4":
				case "5":
				case "6":
				case "7":
				case "8":
				case "9":
				case "y":
				case "Y":
				case "n":
				case "N":
					AnswerQuiz(msg, e.ChatMessage); break;
			}
			CommandExpansion commandExpansion = GetCommandExpansion(msg);
			if (commandExpansion != null)
				HandleCommand(commandExpansion.Command, commandExpansion.Arguments, e.ChatMessage);
		}

		private void TwitchClient_OnWhisperReceived(object sender, TwitchLib.Client.Events.OnWhisperReceivedArgs e)
		{
			WhisperMessage whisperMessage = e.WhisperMessage;
			string msg = whisperMessage.Message;
			switch (msg)
			{
				case "1":
				case "2":
				case "3":
				case "4":
				case "5":
				case "6":
				case "7":
				case "8":
				case "9":
				case "y":
				case "Y":
				case "n":
				case "N":
					SilentAnswerQuiz(msg, whisperMessage); break;
			}
		}

		void Launch(ChatMessage chatMessage)
		{
			hub.Clients.All.ExecuteCommand("Launch", "", chatMessage.UserId, chatMessage.Username, chatMessage.DisplayName, chatMessage.ColorHex);
		}
		void Right(string args, ChatMessage chatMessage)
		{
			hub.Clients.All.ExecuteCommand("Right", args, chatMessage.UserId, chatMessage.Username, chatMessage.DisplayName, chatMessage.ColorHex);
		}
		void Drone(string args, ChatMessage chatMessage)
		{
			hub.Clients.All.ExecuteCommand("Drone", args, chatMessage.UserId, chatMessage.Username, chatMessage.DisplayName, chatMessage.ColorHex);
		}
		void Bee(string args, ChatMessage chatMessage)
		{
			hub.Clients.All.ExecuteCommand("Bee", args, chatMessage.UserId, chatMessage.Username, chatMessage.DisplayName, chatMessage.ColorHex);
		}
		void MoveAbsolute(string args, ChatMessage chatMessage)
		{
			hub.Clients.All.ExecuteCommand("MoveAbsolute", args, chatMessage.UserId, chatMessage.Username, chatMessage.DisplayName, chatMessage.ColorHex);
		}
		void MoveRelative(string args, ChatMessage chatMessage)
		{
			hub.Clients.All.ExecuteCommand("MoveRelative", args, chatMessage.UserId, chatMessage.Username, chatMessage.DisplayName, chatMessage.ColorHex);
		}
		void Toss(string args, ChatMessage chatMessage)
		{
			hub.Clients.All.ExecuteCommand("Toss", args, chatMessage.UserId, chatMessage.Username, chatMessage.DisplayName, chatMessage.ColorHex);
		}
		void TestCommand(string args, ChatMessage chatMessage)
		{
			hub.Clients.All.ExecuteCommand("TestCommand", args, chatMessage.UserId, chatMessage.Username, chatMessage.DisplayName, chatMessage.ColorHex);
		}
		void Left(string args, ChatMessage chatMessage)
		{
			hub.Clients.All.ExecuteCommand("Left", args, chatMessage.UserId, chatMessage.Username, chatMessage.DisplayName, chatMessage.ColorHex);
		}
		void Up(string args, ChatMessage chatMessage)
		{
			hub.Clients.All.ExecuteCommand("Up", args, chatMessage.UserId, chatMessage.Username, chatMessage.DisplayName, chatMessage.ColorHex);
		}
		void ChangePlanet(string args, ChatMessage chatMessage)
		{
			hub.Clients.All.ExecuteCommand("ChangePlanet", args, chatMessage.UserId, chatMessage.Username, chatMessage.DisplayName, chatMessage.ColorHex);
		}
		void ClearQuiz(string args, ChatMessage chatMessage)
		{
			hub.Clients.All.ExecuteCommand("ClearQuiz", args, chatMessage.UserId, chatMessage.Username, chatMessage.DisplayName, chatMessage.ColorHex);
		}
		void StartQuiz(string args, ChatMessage chatMessage)
		{
			hub.Clients.All.ExecuteCommand("StartQuiz", args, chatMessage.UserId, chatMessage.Username, chatMessage.DisplayName, chatMessage.ColorHex);
		}
		void ShowLastQuizResults(string args, ChatMessage chatMessage)
		{
			hub.Clients.All.ExecuteCommand("ShowLastQuizResults", args, chatMessage.UserId, chatMessage.Username, chatMessage.DisplayName, chatMessage.ColorHex);
		}
		void AnswerQuiz(string args, ChatMessage chatMessage)
		{
			hub.Clients.All.ExecuteCommand("AnswerQuiz", args, chatMessage.UserId, chatMessage.Username, chatMessage.DisplayName, chatMessage.ColorHex);
		}
		void SilentAnswerQuiz(string args, WhisperMessage chatMessage)
		{
			hub.Clients.All.ExecuteCommand("SilentAnswerQuiz", args, chatMessage.UserId, chatMessage.Username, chatMessage.DisplayName, chatMessage.ColorHex);
		}
		void Drop(string args, ChatMessage chatMessage)
		{
			hub.Clients.All.ExecuteCommand("Drop", args, chatMessage.UserId, chatMessage.Username, chatMessage.DisplayName, chatMessage.ColorHex);
		}
		void Dock(ChatMessage chatMessage)
		{
			hub.Clients.All.ExecuteCommand("Dock", "", chatMessage.UserId, chatMessage.Username, chatMessage.DisplayName, chatMessage.ColorHex);
		}
		void Down(string args, ChatMessage chatMessage)
		{
			hub.Clients.All.ExecuteCommand("Down", args, chatMessage.UserId, chatMessage.Username, chatMessage.DisplayName, chatMessage.ColorHex);
		}
		void Chutes(ChatMessage chatMessage)
		{
			hub.Clients.All.ExecuteCommand("Chutes", "", chatMessage.UserId, chatMessage.Username, chatMessage.DisplayName, chatMessage.ColorHex);
		}

		void ChangeDroneVelocity(string args, ChatMessage chatMessage)
		{
			hub.Clients.All.ExecuteCommand("ChangeDroneVelocity", args, chatMessage.UserId, chatMessage.Username, chatMessage.DisplayName, chatMessage.ColorHex);
		}

		void Extend(ChatMessage chatMessage)
		{
			hub.Clients.All.ExecuteCommand("Extend", "", chatMessage.UserId, chatMessage.Username, chatMessage.DisplayName, chatMessage.ColorHex);
		}
		void Retract(ChatMessage chatMessage)
		{
			hub.Clients.All.ExecuteCommand("Retract", "", chatMessage.UserId, chatMessage.Username, chatMessage.DisplayName, chatMessage.ColorHex);
		}

		void PlantSeed(string args, ChatMessage chatMessage)
		{
			hub.Clients.All.ExecuteCommand("Seed", args, chatMessage.UserId, chatMessage.Username, chatMessage.DisplayName, chatMessage.ColorHex);
		}

		void Paint(string cmdText, string args, ChatMessage chatMessage)
		{
			hub.Clients.All.ExecuteCommand(cmdText, args, chatMessage.UserId, chatMessage.Username, chatMessage.DisplayName, chatMessage.ColorHex);
		}

		void DroneRight(string args, ChatMessage chatMessage)
		{
			hub.Clients.All.ExecuteCommand("DroneRight", args, chatMessage.UserId, chatMessage.Username, chatMessage.DisplayName, chatMessage.ColorHex);
		}
		void DroneUp(string args, ChatMessage chatMessage)
		{
			hub.Clients.All.ExecuteCommand("DroneUp", args, chatMessage.UserId, chatMessage.Username, chatMessage.DisplayName, chatMessage.ColorHex);
		}
		void DroneDown(string args, ChatMessage chatMessage)
		{
			hub.Clients.All.ExecuteCommand("DroneDown", args, chatMessage.UserId, chatMessage.Username, chatMessage.DisplayName, chatMessage.ColorHex);
		}
		void DroneLeft(string args, ChatMessage chatMessage)
		{
			hub.Clients.All.ExecuteCommand("DroneLeft", args, chatMessage.UserId, chatMessage.Username, chatMessage.DisplayName, chatMessage.ColorHex);
		}
		private void TwitchClient_OnChatCommandReceived(object sender, TwitchLib.Client.Events.OnChatCommandReceivedArgs e)
		{
			string cmdText = e.Command.CommandText.ToLower();
			string args = e.Command.ArgumentsAsString;
			ChatMessage chatMessage = e.Command.ChatMessage;
			HandleCommand(cmdText, args, chatMessage);
		}

		private void HandleCommand(string cmdText, string args, ChatMessage chatMessage)
		{
			if (cmdText == "test")
			{
				TestCommand(args, chatMessage);
				return;
			}

			// Main commands...
			switch (cmdText)
			{
				case "cmd":
				case "?":
					Twitch.Chat($"CodeRushed Rocket controls: launch, dock, retract, extend, drop, up, down, left, right, & drone. Drone controls: u, d, l, r, t {{x,y}}. Paint splat: {{color}}.");
					break;
			}

			// Rocket commands...
			switch (cmdText)
			{
				case "launch": Launch(chatMessage); break;
				case "up": Up(args, chatMessage); break;
				case "down": Down(args, chatMessage); break;
				case "left": Left(args, chatMessage); break;
				case "right": Right(args, chatMessage); break;
				case "dock": Dock(chatMessage); break;
				case "drop": Drop(args, chatMessage); break;
				case "extend": Extend(chatMessage); break;
				case "retract": Retract(chatMessage); break;
				case "chutes": Chutes(chatMessage); break;
				case "seed": PlantSeed(args, chatMessage); break;
				case "drone": Drone(args, chatMessage); break;
			}

			// Drone commands...
			switch (cmdText)
			{
				case "t":
				case "toss":
					Toss(args, chatMessage); break;
				case "u": DroneUp(args, chatMessage); break;
				case "d": DroneDown(args, chatMessage); break;
				case "l": DroneLeft(args, chatMessage); break;
				case "r": DroneRight(args, chatMessage); break;
				//case "bee": Bee(args, chatMessage); break;
				//case "moverel":
				//case "mr":
				//	MoveRelative(args, chatMessage); break;
				//case "changevel":
				case "cv": ChangeDroneVelocity(args, chatMessage); break;
				//case "moveabs":
				//case "ma":
				//	MoveAbsolute(args, chatMessage); break;
			}

			// Paint splat commands...
			switch (cmdText)
			{
				case "red":
				case "black":
				case "white":
				case "orange":
				case "amber":
				case "yellow":
				case "green":
				case "cyan":
				case "blue":
				case "indigo":
				case "violet":
				case "magenta":
					Paint(cmdText, args, chatMessage); break;
				case "purple":
					Paint("violet", args, chatMessage); break;
			}
			switch (cmdText)
			{
				case "planet": ChangePlanet(args, chatMessage); break;
				case "earth":
				case "jupiter":
				case "mars":
				case "mercury":
				case "moon":
				case "neptune":
				case "pluto":
				case "saturn":
				case "sun":
				case "uranus":
				case "venus":
					ChangePlanet(cmdText, chatMessage); break;
			}
			switch (cmdText)
			{
				case "poll":
				case "quiz": StartQuiz(args, chatMessage); break; // Posed in the form of "!quiz What would you rather be? 1. Bee, 2. Drone"
				case "clearquiz": ClearQuiz(args, chatMessage); break; // Posed in the form of "!quiz What would you rather be? 1. Bee, 2. Drone"
				case "1":
				case "2":
				case "3":
				case "4":
				case "5":
				case "6":
				case "7":
				case "8":
				case "9":
				case "y":
				case "Y":
				case "n":
				case "N":
					AnswerQuiz(cmdText, chatMessage); break;
				case "vote":
					AnswerQuiz(args, chatMessage); break;
			}
		}

		private void TwitchClient_OnJoinedChannel(object sender, TwitchLib.Client.Events.OnJoinedChannelArgs e)
		{
			
		}

		private void TwitchClient_OnConnectionError(object sender, TwitchLib.Client.Events.OnConnectionErrorArgs e)
		{
			
		}

		private void TwitchClient_OnLog(object sender, TwitchLib.Client.Events.OnLogArgs e)
		{
			
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			//twitchClient.Disconnect();
			return null;
		}
		public IConfiguration Configuration { get; set; }
	}
}
