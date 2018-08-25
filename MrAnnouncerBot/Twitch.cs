using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.Models.v5.Channels;
using TwitchLib.Api.Models.v5.Users;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace MrAnnouncerBot
{
	public static class Twitch
	{
		private const string STR_ChannelName = "CodeRushed";
		private const string STR_TwitchUserName = "MrAnnouncerGuy";
		static TwitchClient twitchClient;
		static TwitchAPI twitchApi;
		static IConfigurationRoot configuration;

		public static void InitializeConnections()
		{
			var oAuthToken = Twitch.Configuration["Secrets:TwitchBotOAuthToken"];  // Settings.Default.TwitchBotOAuthToken;
			var connectionCredentials = new ConnectionCredentials(STR_TwitchUserName, oAuthToken);
			twitchClient.Initialize(connectionCredentials, STR_ChannelName);
			twitchClient.Connect();
			HookEvents();
		}

		static Twitch()
		{
			//Logging = true;
			twitchClient = new TwitchClient();
			var builder = new ConfigurationBuilder()
				 .SetBasePath(Directory.GetCurrentDirectory())
				 .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

			configuration = builder.Build();
			InitializeApiClient();
		}

		public static IConfigurationRoot Configuration { get => configuration; }
		public static TwitchAPI Api { get => twitchApi; }
		public static TwitchClient Client { get => twitchClient; }
		public static bool Logging { get; set; }

		async public static Task<User> GetUser(string userName)
		{
			List<string> userNames = new List<string>();
			userNames.Add(userName);
			var results = await twitchApi.Users.v5.GetUsersByNameAsync(userNames);
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
			twitchApi = new TwitchAPI();
			twitchApi.Settings.ClientId = Configuration["Secrets:TwitchApiClientId"];  // Settings.Default.TwitchApiClientId;
			twitchApi.Settings.AccessToken = Configuration["Secrets:TwitchBotOAuthToken"];  // Settings.Default.TwitchBotOAuthToken;
		}

		public static void Disconnect()
		{
			try
			{
				twitchClient.Disconnect();
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
			twitchClient.SendMessage(STR_ChannelName, msg);
		}

		static void HookEvents()
		{
			twitchClient.OnLog += TwitchClientLog;
			twitchClient.OnConnectionError += TwitchClient_OnConnectionError;
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