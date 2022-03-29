using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using TwitchLib.PubSub;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Users.GetUsers;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Interfaces;

namespace BotCore
{
	public static class Twitch
	{
		private const string STR_CodeRushedChannelName = "CodeRushed";
		private const string STR_CodeRushedChannelId = "237584851";
		private const string STR_DroneCommandsChannelName = "DroneCommands";
		private const string STR_CodeRushedChannelUserName = "MrAnnouncerGuy";
		private const string STR_DroneCommandsChannelUserName = "DroneCommands";
		static readonly IConfigurationRoot configuration;

		public static void InitializeConnections()
		{
			//`! !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
			//`! !!!                                                                                      !!!
			//`! !!!  Turn off Debug Visualizer before stepping through this method live on the stream!!! !!!
			//`! !!!                                                                                      !!!
			//`! !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

			UnhookEvents(CodeRushedClient);
			var codeRushedOAuthToken = Configuration["Secrets:TwitchBotOAuthToken"];
			var codeRushedConnectionCredentials = new ConnectionCredentials(STR_CodeRushedChannelUserName, codeRushedOAuthToken);
			CodeRushedClient.Initialize(codeRushedConnectionCredentials, STR_CodeRushedChannelName);
			try
			{
				CodeRushedClient.Connect();
				//Client.JoinRoom(STR_ChannelName, "#botcontrol");
				HookEvents(CodeRushedClient);
			}
			catch //(Exception ex)
			{
				System.Diagnostics.Debugger.Break();
			}

			var droneCommandsOAuthToken = Configuration["Secrets:DroneCommandsOAuthToken"];
			var droneCommandsConnectionCredentials = new ConnectionCredentials(STR_DroneCommandsChannelUserName, droneCommandsOAuthToken);
			DroneCommandsClient.Initialize(droneCommandsConnectionCredentials, STR_DroneCommandsChannelName);
			try
			{
				DroneCommandsClient.Connect();
				//Client.JoinRoom(STR_ChannelName, "#botcontrol");
				HookEvents(DroneCommandsClient);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine(ex.StackTrace);
				Console.WriteLine();
			}

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
			catch //(Exception ex)
			{
				return null;
			}
		}

		static Twitch()
		{
			//Logging = true;
			CodeRushedClient = new TwitchClient();
			CodeRushedPubSub = new TwitchPubSub();
			CodeRushedPubSub.OnPubSubServiceConnected += CodeRushedPubSub_OnPubSubServiceConnected;
			CodeRushedPubSub.ListenToChannelPoints(STR_CodeRushedChannelId);
			CodeRushedPubSub.Connect();
			DroneCommandsClient = new TwitchClient();
			var builder = new ConfigurationBuilder()
				 .SetBasePath(Directory.GetCurrentDirectory())
				 .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

			configuration = builder.Build();
			InitializeApiClient();
		}

		public static IConfigurationRoot Configuration { get => configuration; }
		public static TwitchAPI Api { get; private set; }
		public static TwitchClient CodeRushedClient { get; private set; }
		public static TwitchPubSub CodeRushedPubSub { get; private set; }
		public static TwitchClient DroneCommandsClient { get; private set; }
		public static bool Logging { get; set; } = true;
		public static string CodeRushedBotApiClientId { get; set; }

		async public static Task<User> GetUser(string userName)
		{
			GetUsersResponse results = await Api.Helix.Users.GetUsersAsync();
			return results.Users.FirstOrDefault(x => x.Login == userName);
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
				CodeRushedPubSub.Disconnect();
				CodeRushedClient.Disconnect();
				DroneCommandsClient.Disconnect();
			}
			catch (Exception ex)
			{
				Log(ex);
			}
		}

		static void CodeRushedPubSub_OnPubSubServiceConnected(object sender, EventArgs e)
		{
			Authenticate(CodeRushedPubSub);
		}

		static void Log(Exception ex)
		{
			if (Logging)
				Console.WriteLine($"Exception: {ex.Message}");
		}

		public static void Chat(TwitchClient twitchClient, string msg)
		{
			try
			{
				twitchClient.SendMessage(STR_CodeRushedChannelName, msg);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		public static void Whisper(string userName, string msg)
		{
			CodeRushedClient.SendWhisper(userName, msg);
		}

		static void HookEvents(TwitchClient client)
		{
			client.OnLog += TwitchClientLog;
			client.OnConnectionError += TwitchClient_OnConnectionError;
		}

		static void UnhookEvents(TwitchClient client)
		{
			client.OnLog -= TwitchClientLog;
			client.OnConnectionError -= TwitchClient_OnConnectionError;
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

		private static async Task<LiveStreamData<LiveShowData>> GetLiveStreamData(MySecureString clientId, MySecureString accessToken, string userId)
		{
			string responseBody = await GetLiveShowDataStr(clientId, accessToken, userId);
			LiveStreamData<LiveShowData> liveShowData = JsonConvert.DeserializeObject<LiveStreamData<LiveShowData>>(responseBody);
			return liveShowData;
		}

		private static async Task<string> GetLiveShowDataStr(MySecureString clientId, MySecureString accessToken, string userId)
		{
			var client = new HttpClient();
			client.DefaultRequestHeaders.Add("Client-ID", clientId.GetStr());
			client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken.GetStr()}");
			string requestUri = $"https://api.twitch.tv/helix/videos?user_id={userId}";
			HttpResponseMessage response = await client.GetAsync(requestUri);
			string responseBody = await response.Content.ReadAsStringAsync();
			return responseBody;
		}

		private static TimeSpan GetTimeMarker(LiveShowData showData, TimeSpan rewindTimeSpan)
		{
			TimeSpan timeMarker = TimeSpan.MinValue;

			try
			{
				timeMarker = GetTimeSpanFromString(showData.duration).Subtract(rewindTimeSpan);
			}
			catch (Exception ex)
			{
				if (ex != null)
				{

				}
				Debugger.Break();
			}

			return timeMarker;
		}

		private static TimeSpan GetRewindTimeSpan(string backTrackStr)
		{
			TimeSpan rewindTimeSpan = new TimeSpan();

			if (string.IsNullOrWhiteSpace(backTrackStr))
			{
				rewindTimeSpan = new TimeSpan(hours: 0, minutes: 0, seconds: 10);
			}
			else
			{
				int dollarIndex = backTrackStr.IndexOf("$");
				if (dollarIndex >= 0)
				{
					string backTrackValue = backTrackStr.Substring(dollarIndex + 1);

					rewindTimeSpan = GetTimeSpanFromString(backTrackValue);
				}
			}

			return rewindTimeSpan;
		}

		private static string GetTimeParseFormatExpressionFromWilBennett(string timeString)
		{
			void subst(char ch)
			{
				var search = $@"(\d+)(?={ch})"; // 1 or more digits followed by ch. e.g. "1h", "22m"
				var suffix = $@"\"; // \ch. e.g. "\h", "\m"
														// Replace the match with ch instead of the digits and \ at the end
														// e.g. "1h" => "h\h", "22m" => "mm\m"
				timeString = System.Text.RegularExpressions.Regex.Replace(timeString, search, m => new String(ch, m.Captures[0].Length) + suffix);
			}

			subst('h');
			subst('m');
			subst('s');

			return timeString;
		}

		private static TimeSpan GetTimeSpanFromString(string timeString)
		{
			TimeSpan timeSpan;
			try
			{
				// TODO: Maybe give up on ParseExact...
				timeSpan = TimeSpan.ParseExact(timeString, GetTimeParseFormatExpressionFromWilBennett(timeString), System.Globalization.CultureInfo.CurrentCulture);
			}
			catch //(Exception ex)
			{
				Debugger.Break();
				timeSpan = TimeSpan.FromSeconds(1);
			}
			return timeSpan;
		}

		public static async Task<string> GetActiveShowPointURL(MySecureString clientId, MySecureString accessToken, string userId, string backTrackStr = "")
		{
			try
			{
				LiveStreamData<LiveShowData> liveShowData = await GetLiveStreamData(clientId, accessToken, userId);
				if (liveShowData?.data?.Count > 0)  // Thanks to Wil Bennett!
				{
					LiveShowData showData = liveShowData.data[0];

					TimeSpan rewindTimeSpan = GetRewindTimeSpan(backTrackStr);
					TimeSpan timeMarker = GetTimeMarker(showData, rewindTimeSpan);

					return showData.url + "?t=" + $"{timeMarker.Hours}h{timeMarker.Minutes}m{timeMarker.Seconds}s";
				}
			}
			catch (Exception ex)
			{
				if (ex != null)
				{

				}
				Debugger.Break();
			}

			return null;
		}

		public static void Authenticate(TwitchPubSub codeRushedPubSub)
		{
			codeRushedPubSub.SendTopics(Configuration["Secrets:PubSubAccessToken"]);
		}
	}
}