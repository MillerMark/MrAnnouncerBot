using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using TwitchLib.Client.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TwitchLib.Client;

namespace OverlayManager
{
	public class BackgroundWorker: IHostedService
	{
		private const string STR_ChannelName = "CodeRushed";
		private const string STR_TwitchUserName = "MrAnnouncerGuy";
		TwitchClient twitchClient = new TwitchClient();
		public BackgroundWorker(IConfiguration configuration)
		{
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

		private void TwitchClient_OnChatCommandReceived(object sender, TwitchLib.Client.Events.OnChatCommandReceivedArgs e)
		{
			//if (e.Command.CommandText == "cmd" || e.Command.CommandText == "?")
			//{
			//	twitchClient.SendMessage(STR_ChannelName, "Here's what you can say: up, down, left, right, center");
			//}
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
