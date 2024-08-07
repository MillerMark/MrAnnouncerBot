﻿using Microsoft.Extensions.Configuration;
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
using TwitchLib.PubSub.Events;
using TwitchLib.PubSub.Models.Responses;

namespace BotCore
{
	public static class Twitch
	{
		private const string STR_CodeRushedChannelName = "CodeRushed";
		private const string STR_CodeRushedChannelId = "237584851";
		private const string STR_DroneCommandsChannelName = "DroneCommands";
        private const string STR_DroneCommandsChannelUserName = "DroneCommands";
        private const string STR_CodeRushedChannelUserName = "MrAnnouncerGuy";
        private const string STR_FredGptChannelName = "FredGpt";
        private const string STR_FredGptChannelUserName = "FredGpt";
        private const string STR_RoryGptChannelUserName = "RoryGpt";
        private const string STR_MarksVoiceChannelUserName = "MarksVoice";
        static readonly IConfigurationRoot configuration;

		public static void InitializeConnections()
		{
            InitializeCodeRushedConnection();
			var droneCommandsOAuthToken = Configuration["Secrets:DroneCommandsOAuthToken"];
			var droneCommandsConnectionCredentials = new ConnectionCredentials(STR_DroneCommandsChannelUserName, droneCommandsOAuthToken);
			DroneCommandsClient.Initialize(droneCommandsConnectionCredentials, STR_DroneCommandsChannelName);
			try
			{
				DroneCommandsClient.Connect();
				HookBasicEvents(DroneCommandsClient);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine(ex.StackTrace);
				Console.WriteLine();
            }

            InitializeFredGptClient();
            InitializeRoryGptClient();
            InitializeMarksVoiceClient();
        }

        public static void InitializeCodeRushedConnection()
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
                HookBasicEvents(CodeRushedClient);
            }
            catch //(Exception ex)
            {
                System.Diagnostics.Debugger.Break();
            }
        }

        public static void InitializeMarksVoiceClient()
        {
            //`! !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //`! !!!                                                                                      !!!
            //`! !!!  Turn off Debug Visualizer before stepping through this method live on the stream!!! !!!
            //`! !!!                                                                                      !!!
            //`! !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            var marksVoiceOAuthToken = Configuration["Secrets:MarksVoiceOAuthToken"];
            var marksVoiceConnectionCredentials = new ConnectionCredentials(STR_MarksVoiceChannelUserName, marksVoiceOAuthToken);
            MarksVoiceClient.Initialize(marksVoiceConnectionCredentials /* , STR_MarksVoiceChannelName */);
            try
            {
                MarksVoiceClient.Connect();
                HookBasicEvents(MarksVoiceClient);
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

			CodeRushedPubSub.OnListenResponse += onListenResponse;
			CodeRushedPubSub.OnPubSubServiceConnected += CodeRushedPubSub_OnPubSubServiceConnected;
			CodeRushedPubSub.OnPubSubServiceClosed += CodeRushedPubSub_OnPubSubServiceClosed;
			CodeRushedPubSub.OnPubSubServiceError += CodeRushedPubSub_OnPubSubServiceError;
			CodeRushedPubSub.ListenToChannelPoints(STR_CodeRushedChannelId);
			CodeRushedPubSub.Connect();
			DroneCommandsClient = new TwitchClient();
            
            FredGptClient = new TwitchClient();
            RoryGptClient = new TwitchClient();
            MarksVoiceClient = new TwitchClient();

            //DroneCommandsClient.SendMessage(, message)
            var builder = new ConfigurationBuilder()
				 .SetBasePath(Directory.GetCurrentDirectory())
				 .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

			configuration = builder.Build();
			InitializeApiClient();
		}

        public static void ClientChat(TwitchClient client, string msg)
        {
            if (client.JoinedChannels.Count == 0)
                client.JoinChannel(STR_CodeRushedChannelName);

            Chat(client, TruncateIfNeeded(msg));
        }

        public static void FredChat(string msg)
        {
            ClientChat(FredGptClient, msg);
        }

        public static void InitializeFredGptClient()
        {
            //`! !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //`! !!!                                                                                      !!!
            //`! !!!  Turn off Debug Visualizer before stepping through this method live on the stream!!! !!!
            //`! !!!                                                                                      !!!
            //`! !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            var fredGptOAuthToken = Configuration["Secrets:FredGptOAuthToken"];
            var fredGptConnectionCredentials = new ConnectionCredentials(STR_FredGptChannelUserName, fredGptOAuthToken);
            FredGptClient.Initialize(fredGptConnectionCredentials /* , STR_FredGptChannelName */);
            try
            {
                FredGptClient.Connect();
                HookBasicEvents(FredGptClient);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine();
            }
        }

        public static void InitializeRoryGptClient()
        {
            //`! !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //`! !!!                                                                                      !!!
            //`! !!!  Turn off Debug Visualizer before stepping through this method live on the stream!!! !!!
            //`! !!!                                                                                      !!!
            //`! !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            var roryGptOAuthToken = Configuration["Secrets:RoryGptOAuthToken"];
            var roryGptConnectionCredentials = new ConnectionCredentials(STR_RoryGptChannelUserName, roryGptOAuthToken);
            RoryGptClient.Initialize(roryGptConnectionCredentials /* , STR_RoryGptChannelName */);
            try
            {
                RoryGptClient.Connect();
                HookBasicEvents(RoryGptClient);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine();
            }
        }

        public static void RoryGptChat(string msg)
        {
            ClientChat(RoryGptClient, msg);
        }

        public static void MarksVoiceChat(string msg)
        {
            ClientChat(MarksVoiceClient, msg);
        }



        private static void onListenResponse(object sender, OnListenResponseArgs e)
		{
			if (!e.Successful)
			{
				string error = e.Response.Error;
			}
		}

		private static void CodeRushedPubSub_OnPubSubServiceError(object sender, TwitchLib.PubSub.Events.OnPubSubServiceErrorArgs e)
		{
			
		}

		private static void CodeRushedPubSub_OnPubSubServiceClosed(object sender, EventArgs e)
		{
			
		}

		public static IConfigurationRoot Configuration { get => configuration; }
		public static TwitchAPI Api { get; private set; }
		public static TwitchClient CodeRushedClient { get; private set; }
        public static TwitchClient FredGptClient { get; private set; }
        public static TwitchClient RoryGptClient { get; private set; }
        public static TwitchClient MarksVoiceClient { get; private set; }
        public static TwitchPubSub CodeRushedPubSub { get; private set; }
		public static TwitchClient DroneCommandsClient { get; private set; }
		public static bool Logging { get; set; } = true;
		public static string CodeRushedBotApiClientId { get; set; }

		async public static Task<User> GetUser(string userName)
		{
			try
			{
				GetUsersResponse results = await Api.Helix.Users.GetUsersAsync();
				return results?.Users?.FirstOrDefault(x => x.Login == userName);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Exception: \"{ex.Message}\" - await Api.Helix.Users.GetUsersAsync();");
				return null;
			}
			
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
			//Api.Settings.AccessToken = Configuration["Secrets:TwitchBotOAuthToken"];
			Api.Settings.AccessToken = Configuration["Secrets:TwitchBotAccessToken"];
			CodeRushedBotApiClientId = Configuration["Secrets:CodeRushedBotTwitchApiClientId"];
		}

		public static void Disconnect()
		{
			try
			{
				CodeRushedPubSub.Disconnect();
				CodeRushedClient.Disconnect();
                FredGptClient.Disconnect();
                RoryGptClient.Disconnect();
                MarksVoiceClient.Disconnect();
                DroneCommandsClient.Disconnect();
			}
			catch (Exception ex)
			{
				Log(ex);
			}
		}

        public static string TruncateIfNeeded(string msg)
        {
            const string STR_Ellipsis = "...";
            const int maxLength = 410;
            if (msg.Length > maxLength)
                msg = msg.Substring(0, maxLength - STR_Ellipsis.Length) + STR_Ellipsis;
            return msg;
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

		public static void DroneCommandsChat(string msg)
		{
			try
			{
				if (!DroneCommandsClient.IsInitialized || !DroneCommandsClient.IsConnected)
				{
					var droneCommandsOAuthToken = Configuration["Secrets:DroneCommandsOAuthToken"];
					var droneCommandsConnectionCredentials = new ConnectionCredentials(STR_DroneCommandsChannelUserName, droneCommandsOAuthToken);
					DroneCommandsClient.Initialize(droneCommandsConnectionCredentials, STR_DroneCommandsChannelName);
					DroneCommandsClient.Connect();
				}
				DroneCommandsClient.SendMessage(STR_DroneCommandsChannelName, msg);
			}
			catch (Exception ex)
			{
				if (!DroneCommandsClient.IsConnected)
				{
					DroneCommandsClient.Disconnect();
					DroneCommandsClient.Connect();
				}

				Console.WriteLine(ex.Message);
			}
		}

		public static void Whisper(string userName, string msg)
		{
			CodeRushedClient.SendWhisper(userName, msg);
		}

		static void HookBasicEvents(TwitchClient client)
		{
			client.OnLog += TwitchClientLog;
			client.OnConnectionError += TwitchClient_OnConnectionError;
		}

		static void UnhookEvents(TwitchClient client)
		{
			client.OnLog -= TwitchClientLog;
			client.OnConnectionError -= TwitchClient_OnConnectionError;
		}

		static void TwitchClientLog(object sender, TwitchLib.Client.Events.OnLogArgs e)
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
			codeRushedPubSub.SendTopics(Configuration["Secrets:TwitchBotAccessToken"]);
		}
	}
}