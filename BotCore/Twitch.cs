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

namespace BotCore
{
	public static class Twitch
	{
		private const string STR_ChannelName = "CodeRushed";
		private const string STR_TwitchUserName = "MrAnnouncerGuy";
		static readonly IConfigurationRoot configuration;

		public static void InitializeConnections()
		{
			var oAuthToken = Configuration["Secrets:TwitchBotOAuthToken"];  // Settings.Default.TwitchBotOAuthToken;
			var connectionCredentials = new ConnectionCredentials(STR_TwitchUserName, oAuthToken);
			Client.Initialize(connectionCredentials, STR_ChannelName);
			Client.Connect();
			//Client.JoinRoom(STR_ChannelName, "#botcontrol");
			HookEvents();
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

		async public static Task<User> GetUser(string userName)
		{
			List<string> userNames = new List<string>();
			userNames.Add(userName);
			var results = await Api.Users.v5.GetUsersByNameAsync(userNames);
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
			Api = new TwitchAPI();
			Api.Settings.ClientId = Configuration["Secrets:TwitchApiClientId"];  // Settings.Default.TwitchApiClientId;
			Api.Settings.AccessToken = Configuration["Secrets:TwitchBotOAuthToken"];  // Settings.Default.TwitchBotOAuthToken;
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
			Client.SendMessage(STR_ChannelName, msg);
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