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
		TwitchClient twitchClient = new TwitchClient();
		readonly IHubContext<CodeRushedHub, IOverlayCommands> hub;
		public BackgroundWorker(IConfiguration configuration, IHubContext<CodeRushedHub, IOverlayCommands> hub)
		{
			this.hub = hub;
			Configuration = configuration;
			// TODO: Talk to browser using SignalR.
		}

		private void ConnectTwitchClient()
		{
			var oAuthToken = Configuration["Secrets:TwitchBotOAuthToken"];
			var connectionCredentials = new ConnectionCredentials(STR_TwitchUserName, oAuthToken);
			twitchClient.Initialize(connectionCredentials, STR_ChannelName);
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			ConnectTwitchClient();
			HookEvents();
			twitchClient.Connect();
			return Task.CompletedTask;
		}

		public void HookEvents()
		{
			twitchClient.OnLog += TwitchClient_OnLog;
			twitchClient.OnConnectionError += TwitchClient_OnConnectionError;
			twitchClient.OnJoinedChannel += TwitchClient_OnJoinedChannel;
			twitchClient.OnChatCommandReceived += TwitchClient_OnChatCommandReceived;
			twitchClient.OnMessageReceived += TwitchClient_OnMessageReceived;
			twitchClient.OnUserJoined += TwitchClient_OnUserJoined;
			twitchClient.OnUserLeft += TwitchClient_OnUserLeft;
		}

		private void TwitchClient_OnUserLeft(object sender, TwitchLib.Client.Events.OnUserLeftArgs e)
		{
			
		}

		private void TwitchClient_OnUserJoined(object sender, TwitchLib.Client.Events.OnUserJoinedArgs e)
		{
			
		}

		private void TwitchClient_OnMessageReceived(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
		{
			
		}

		void Launch()
		{
			hub.Clients.All.ExecuteCommand("Launch", "");
		}
		void Right(string args)
		{
			hub.Clients.All.ExecuteCommand("Right", args);
		}
		void Left(string args)
		{
			hub.Clients.All.ExecuteCommand("Left", args);
		}
		void Up(string args)
		{
			hub.Clients.All.ExecuteCommand("Up", args);
		}
		void Drop()
		{
			hub.Clients.All.ExecuteCommand("Drop", "");
		}
		void Dock()
		{
			hub.Clients.All.ExecuteCommand("Dock", "");
		}
		void Down(string args)
		{
			hub.Clients.All.ExecuteCommand("Down", args);
		}
		void Chutes()
		{
			hub.Clients.All.ExecuteCommand("Chutes", "");
		}
		void Extend()
		{
			hub.Clients.All.ExecuteCommand("Extend", "");
		}
		void Retract()
		{
			hub.Clients.All.ExecuteCommand("Retract", "");
		}

		private void TwitchClient_OnChatCommandReceived(object sender, TwitchLib.Client.Events.OnChatCommandReceivedArgs e)
		{
			string cmdText = e.Command.CommandText;
			string args = e.Command.ArgumentsAsString;
			switch (cmdText)
			{
				case "cmd":
				case "?":
					Chat($"To control the CodeRushed Rocket use: launch, dock, retract, extend, drop, up, down, left, & right.");
					break;
				case "launch": Launch(); break;
				case "up": Up(args); break;
				case "down": Down(args); break;
				case "left": Left(args); break;
				case "right": Right(args); break;
				case "dock": Dock(); break;
				case "drop": Drop(); break;
				case "extend": Extend(); break;
				case "retract": Retract(); break;
				case "chutes": Chutes(); break;
			}
		}

		private void Chat(string message)
		{
			twitchClient.SendMessage(STR_ChannelName, message);
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
			twitchClient.Disconnect();
			return null;
		}
		public IConfiguration Configuration { get; set; }
	}
}
