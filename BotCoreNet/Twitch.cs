using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TwitchLib.Api;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.EventSub.Websockets;
using TwitchLib.Communication.Interfaces;
using TwitchLib.EventSub.Websockets.Extensions;
using TwitchLib.Api.Helix.Models.Users.GetUsers;
using TwitchLib.EventSub.Websockets.Core.EventArgs;
using Newtonsoft.Json;

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
            DroneCommandsClient = new TwitchClient();

            FredGptClient = new TwitchClient();
            RoryGptClient = new TwitchClient();
            MarksVoiceClient = new TwitchClient();

            var builder = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            configuration = builder.Build();
            InitializeApiClient();
            RefreshBotAccessToken();
            InitializeEventSub();
        }

        static void InitializeEventSub()
        {
            var services = new ServiceCollection();
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });
            services.AddTwitchLibEventSubWebsockets();
            var serviceProvider = services.BuildServiceProvider();
            CodeRushedEventSub = serviceProvider.GetRequiredService<EventSubWebsocketClient>();
            CodeRushedEventSub.WebsocketConnected += CodeRushedEventSub_OnWebsocketConnected;
            // Fire-and-forget: cannot block in a static constructor — the CLR type-initialization
            // lock is held for the entire duration, so any GetResult() here deadlocks.
            // Subscriptions are registered in OnWebsocketConnected once the connection is established.
            var eventSubClient = CodeRushedEventSub;
            _ = Task.Run(() => eventSubClient.ConnectAsync(new Uri("wss://eventsub.wss.twitch.tv/ws")));
        }

        /// <summary>
        /// Re-creates the EventSub client and reconnects to the Twitch production WebSocket URL.
        /// This is required because v0.0.3 of TwitchLib.EventSub.Websockets hardcodes the old
        /// beta URL ("eventsub-beta") in ReconnectAsync(), which is decommissioned and always fails.
        /// </summary>
        /// <returns>true if the new session was established within 10 seconds; false otherwise.</returns>
        public static async Task<bool> ReconnectEventSubAsync()
        {
            // Unregister the subscription handler from the old client before replacing it.
            CodeRushedEventSub.WebsocketConnected -= CodeRushedEventSub_OnWebsocketConnected;

            InitializeEventSub(); // assigns CodeRushedEventSub to a fresh client + starts ConnectAsync

            // Poll for the session_welcome message (which sets SessionId) for up to 10 seconds.
            var deadline = DateTimeOffset.Now.AddSeconds(10);
            while (string.IsNullOrEmpty(CodeRushedEventSub.SessionId) && DateTimeOffset.Now < deadline)
                await Task.Delay(100).ConfigureAwait(false);

            return !string.IsNullOrEmpty(CodeRushedEventSub.SessionId);
        }

        private static async void CodeRushedEventSub_OnWebsocketConnected(object? sender, WebsocketConnectedArgs e)
        {
            if (!e.IsRequestedReconnect)
            {
                try
                {
                    await Api.Helix.EventSub.CreateEventSubSubscriptionAsync(
                        "channel.channel_points_custom_reward_redemption.add",
                        "1",
                        new Dictionary<string, string> { { "broadcaster_user_id", STR_CodeRushedChannelId } },
                        TwitchLib.Api.Core.Enums.EventSubTransportMethod.Websocket,
                        websocketSessionId: CodeRushedEventSub.SessionId
                    ).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"EventSub subscription error: {ex.Message}");
                }
            }
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



        private static void onListenResponse(object sender, EventArgs e)
        {
        }

        private static void CodeRushedPubSub_OnPubSubServiceError(object sender, EventArgs e)
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
        public static EventSubWebsocketClient CodeRushedEventSub { get; private set; }
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

        static void RefreshBotAccessToken()
        {
            var refreshToken = Configuration["Secrets:TwitchBotRefreshToken"];
            var clientSecret = Configuration["Secrets:TwitchClientSecret"];
            var clientId = Configuration["Secrets:TwitchApiClientId"];
            // Capture Api in a local so Task.Run lambdas don't access static Twitch members
            // (which would deadlock against the CLR type-initialization lock held by the static ctor).
            var api = Api;

            if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(clientSecret))
            {
                Console.WriteLine("Token auto-refresh skipped: TwitchBotRefreshToken or TwitchClientSecret not set in config.");
            }
            else
            {
                try
                {
                    Console.WriteLine("Refreshing Twitch bot access token...");
                    var response = Task.Run(() => api.Auth.RefreshAuthTokenAsync(refreshToken, clientSecret, clientId)).GetAwaiter().GetResult();
                    api.Settings.AccessToken = response.AccessToken;
                    Console.WriteLine("Twitch bot access token refreshed successfully.");
                    PersistNewBotTokens(response.AccessToken, response.RefreshToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Twitch token refresh failed: {ex.Message}. Using existing token from config.");
                }
            }

            // Validate the current token and sync Api.Settings.ClientId to the app that
            // issued the token. This fixes "ClientID invalid" errors when the configured
            // TwitchApiClientId doesn't match the app that generated TwitchBotAccessToken.
            try
            {
                var validation = Task.Run(() => api.Auth.ValidateAccessTokenAsync()).GetAwaiter().GetResult();
                if (validation != null && !string.IsNullOrEmpty(validation.ClientId))
                {
                    api.Settings.ClientId = validation.ClientId;
                    Console.WriteLine($"Token validated. ClientId synced: {validation.ClientId}");
                }
                else
                {
                    Console.WriteLine("Token validation returned null — token may be expired or invalid. Regenerate TwitchBotAccessToken with channel:read:redemptions scope.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Token validation failed: {ex.Message}. EventSub subscription may fail if ClientId is mismatched.");
            }
        }

        static string FindCoreAppSettingsPath()
        {
            var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (dir != null)
            {
                var candidate = Path.Combine(dir.FullName, "core_appsettings.json");
                if (File.Exists(candidate))
                    return candidate;
                dir = dir.Parent;
            }
            return null;
        }

        static void PersistNewBotTokens(string newAccessToken, string newRefreshToken)
        {
            // Write to both the runtime file (what ConfigurationBuilder reads) and
            // core_appsettings.json (the master file), so that token rotation is
            // reflected in whichever file each subsequent run picks up.
            var pathsToUpdate = new List<string>();

            var localSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            if (File.Exists(localSettingsPath))
                pathsToUpdate.Add(localSettingsPath);

            var coreSettingsPath = FindCoreAppSettingsPath();
            if (coreSettingsPath != null && !pathsToUpdate.Contains(coreSettingsPath))
                pathsToUpdate.Add(coreSettingsPath);

            if (pathsToUpdate.Count == 0)
            {
                Console.WriteLine("Warning: No appsettings.json or core_appsettings.json found to persist tokens.");
                return;
            }

            foreach (var appSettingsPath in pathsToUpdate)
            {
                try
                {
                    var content = File.ReadAllText(appSettingsPath);

                    // Read old token values from the file itself (not from Configuration),
                    // so the replacement always matches the content being updated.
                    var oldAccessMatch = System.Text.RegularExpressions.Regex.Match(
                        content, @"""TwitchBotAccessToken""\s*:\s*""([^""]+)""");
                    var oldRefreshMatch = System.Text.RegularExpressions.Regex.Match(
                        content, @"""TwitchBotRefreshToken""\s*:\s*""([^""]+)""");

                    var oldAccessToken = oldAccessMatch.Success ? oldAccessMatch.Groups[1].Value : null;
                    var oldRefreshToken = oldRefreshMatch.Success ? oldRefreshMatch.Groups[1].Value : null;

                    if (!string.IsNullOrEmpty(oldAccessToken) && !string.IsNullOrEmpty(newAccessToken))
                        content = content.Replace($"\"{oldAccessToken}\"", $"\"{newAccessToken}\"");

                    if (!string.IsNullOrEmpty(oldRefreshToken) && !string.IsNullOrEmpty(newRefreshToken))
                        content = content.Replace($"\"{oldRefreshToken}\"", $"\"{newRefreshToken}\"");

                    File.WriteAllText(appSettingsPath, content);
                    Console.WriteLine($"Refreshed tokens persisted to {Path.GetFileName(appSettingsPath)}.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to persist refreshed tokens to {Path.GetFileName(appSettingsPath)}: {ex.Message}");
                }
            }
        }

        public static void Disconnect()
        {
            try
            {
                Task.Run(() => CodeRushedEventSub?.DisconnectAsync()).GetAwaiter().GetResult();
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

    }
}