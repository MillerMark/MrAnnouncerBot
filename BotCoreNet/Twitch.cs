using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.V5.Models.Channels;
using TwitchLib.Api.V5.Models.Users;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace BotCore
{
	public static class Twitch
	{
		private const string STR_ChannelName = "CodeRushed";
		private const string STR_TwitchUserName = "MrAnnouncerGuy";
		static readonly IConfigurationRoot configuration;

		public static void InitializeConnections()
		{
			//`! !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
			//`! !!!                                                                                      !!!
			//`! !!!  Turn off Debug Visualizer before stepping through this method live on the stream!!! !!!
			//`! !!!                                                                                      !!!
			//`! !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

			UnhookEvents();
			var oAuthToken = Configuration["Secrets:TwitchBotOAuthToken"];
			var connectionCredentials = new ConnectionCredentials(STR_TwitchUserName, oAuthToken);
			if (Client.IsConnected)
			{
				System.Diagnostics.Debugger.Break();
				Client.Disconnect();
			}
			Client.Initialize(connectionCredentials, STR_ChannelName);
			Client.Connect();
			//Client.JoinRoom(STR_ChannelName, "#botcontrol");
			HookEvents();
		}

		public static TwitchClient CreateNewClient(string channelName, string userName, string oauthPasswordName)
		{
			//`! !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
			//`! !!!                                                                                      !!!
			//`! !!!  Turn off Debug Visualizer before stepping through this method live on the stream!!! !!!
			//`! !!!                                                                                      !!!
			//`! !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

			TwitchClient client = new TwitchClient();
			var oAuthToken = Configuration[$"Secrets:{oauthPasswordName}"];
			if (oAuthToken == null)
				return null;
			var connectionCredentials = new ConnectionCredentials(userName, oAuthToken);
			client.Initialize(connectionCredentials, channelName);
			try
			{
				client.Connect();
				return client;
			}
			catch (Exception ex)
			{
				return null;
			}
		}

		static Twitch()
		{
			//Logging = true;
			Client = new TwitchClient();
			var builder = new ConfigurationBuilder()
				 .SetBasePath(Directory.GetCurrentDirectory())
				 .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

			configuration = builder.Build();
			InitializeApiClient();
		}

		public static IConfigurationRoot Configuration { get => configuration; }
		public static TwitchAPI Api { get; private set; }
		public static TwitchClient Client { get; private set; }
		public static bool Logging { get; set; }
		public static string CodeRushedBotApiClientId { get; set; }

		async public static Task<User> GetUser(string userName)
		{
			List<string> userNames = new List<string>
			{
				userName
			};
			var results = await Api.V5.Users.GetUsersByNameAsync(userNames);
			var userList = results.Matches;
			if (userList.Length > 0)
				return userList[0];

			return null;
		}

		async public static Task<string> GetUserId(string userName)
		{
			User user = await GetUser(userName);
			if (user != null)
				return user.Id;
			
			return null;
		}

		static void InitializeApiClient()
		{
			//`! !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
			//`! !!!                                                                                      !!!
			//`! !!!  Turn off Debug Visualizer before stepping through this method live on the stream!!! !!!
			//`! !!!                                                                                      !!!
			//`! !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

			Api = new TwitchAPI();
			Api.Settings.ClientId = Configuration["Secrets:TwitchApiClientId"];
			CodeRushedBotApiClientId = Configuration["Secrets:CodeRushedBotTwitchApiClientId"];
			Api.Settings.AccessToken = Configuration["Secrets:TwitchBotOAuthToken"];
		}

		public static void Disconnect()
		{
			try
			{
				Client.Disconnect();
			}
			catch (Exception ex)
			{
				Log(ex);
			}
		}

		static void Log(Exception ex)
		{
			if (Logging)
				Console.WriteLine($"Exception: {ex.Message}");
		}

		public static void Chat(string msg)
		{
			try
			{
				Client.SendMessage(STR_ChannelName, msg);

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		public static void Whisper(string userName, string msg)
		{
			Client.SendWhisper(userName, msg);
		}

		static void HookEvents()
		{
			Client.OnLog += TwitchClientLog;
			Client.OnConnectionError += TwitchClient_OnConnectionError;
		}
		static void UnhookEvents()
		{
			Client.OnLog -= TwitchClientLog;
			Client.OnConnectionError -= TwitchClient_OnConnectionError;
		}

		static void TwitchClientLog(object sender, OnLogArgs e)
		{
			if (Logging)
				Console.WriteLine(e.Data);
		}

		static void TwitchClient_OnConnectionError(object sender, OnConnectionErrorArgs e)
		{
			Console.WriteLine(e.Error.Message);
		}
	}
}