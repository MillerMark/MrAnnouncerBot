using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Drawing;
using System.Net.Http;
using System.Net.Mail;
using System.Threading;
using System.Diagnostics;
using System.Configuration;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Speech.Recognition;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BotCore;
using DndCore;
using CsvHelper;
using TwitchLib.Client;
using TwitchLib.PubSub;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.EventSub.Websockets;
using TwitchLib.PubSub.Models.Responses.Messages;
using TwitchLib.EventSub.Websockets.Core.EventArgs;
using TwitchLib.EventSub.Websockets.Core.EventArgs.Channel;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using SheetsPersist;
using MrAnnouncerBot.Games.Zork;
using StudioPanelSdk;
using OBSWebsocketDotNet;
using OBSWebsocketDotNet.Types;
using static MrAnnouncerBot.MrAnnouncerBot;

namespace MrAnnouncerBot
{
    // TODO: Move these classes to a new file.
    public static class IEnumerableExtensionMethods
    {
        public static T PickOne<T>(this IEnumerable<T> source)
        {
            if (source.Count() == 1)
                return source.ElementAt(0);

            var index = new Random((int)DateTime.Now.Ticks).Next(source.Count());
            return source.ElementAt(index);
        }
    }


    [Document("Mr. Announcer Guy")]
    [Sheet("Special Fanfares")]
    public class SpecialFanfare
    {
        [Column]
        public string UserId { get; set; }
        [Column]
        public string DisplayName { get; set; }
        [Column]
        public string KeyPhrase { get; set; }
        [Column]
        public string SceneName { get; set; }
        [Column]
        public double Duration { get; set; }
    }

    public partial class MrAnnouncerBot
    {
        MySecureString mrAnnouncerGuyClientId;
        MySecureString mrAnnouncerGuyAccessToken;

        TwitchClient kidzCodeClient;
        List<Entry> log = new List<Entry>();
        public static readonly HttpClient httpClient = new HttpClient();

        Dictionary<string, DateTime> lastScenePlayTime = new Dictionary<string, DateTime>();
        Dictionary<string, DateTime> lastCategoryPlayTime = new Dictionary<string, DateTime>();
        AllViewers allViewers = new AllViewers();
        private const string STR_ChannelName = "CodeRushed";
        //private const string STR_TwitchUserName = "MrAnnouncerGuy";
        const string STR_GetChattersApi = "https://tmi.twitch.tv/group/user/coderushed/chatters";
        const string STR_CodeRushedUserId = "237584851";

        private static List<SceneDto> scenes;
        private static List<RestrictedSceneDto> restrictedScenes;
        private static List<ChannelPointAction> channelPointActions;
        private static List<EventActionMap> eventActionMaps;
        private static List<SpecialFanfare> specialFanfares;
        private string activeSceneName;
        private Timer checkChatRoomTimer;
        private Timer autoSaveTimer;
        private OBSWebsocket obsWebsocket = new OBSWebsocket();
        private StudioPanel _studioPanel;
        private ZorkGame zork;
        private Random random = new Random((int)DateTime.Now.Ticks);

        Timer reconnectObsClientTimer;

        private bool useObs = true;
        HubConnection hubConnection;

        public MrAnnouncerBot()
        {
            FredGpt.SetApiKey(new MySecureString(Twitch.Configuration["Secrets:openaiApiKey"]));
            RegisterSpreadsheets();
            CheckDocs();
            InitChatRoomTimer();
            LoadPersistentData();
            InitZork();
            new BotCommand("?", HandleQuestionCommand);
            new BotCommand("reload", ReloadCommand);
            new BotCommand("help", HandleQuestionCommand);
            new BotCommand("commands", HandleQuestionCommand);
            new BotCommand("+", HandleLevelUp);
            new BotCommand("github", HandleGitHubCommand);
            new BotCommand("vscode", HandleVsCodeCommand);
            new BotCommand("suppressFanfare", HandleSuppressFanfareCommand);
            new BotCommand("crIssue", MarkCodeRushIssue);
            new BotCommand("crIssueStart", MarkCodeRushIssueStart);
            new BotCommand("discord", HandleDiscordCommand);
            new BotCommand("dh", HandleDragonHCommand);
            new BotCommand("dhn", HandleDragonHNewTimeCommand);
            new BotCommand("book*", HandleBookCommand);
            hubConnection = new HubConnectionBuilder().WithUrl("http://localhost:64303/MrAnnouncerBotHub").Build();
            if (hubConnection != null)
            {
                //hubConnection.Closed += HubConnection_Closed;
                hubConnection.On<string, int>("AddCoins", AddCoins);
                hubConnection.On<string>("NeedToGetCoins", NeedToGetCoins);
                hubConnection.On<string>("ChangeScene", ChangeScene);
                // TODO: Check out benefits of stopping gracefully with a cancellation token.
                hubConnection.StartAsync();
            }
            lastFanfareDuration = 15;

            InitializeKidzCodeBot();
            mrAnnouncerGuyClientId = new MySecureString(Twitch.Configuration["Secrets:MrAnnouncerGuyTwitchClientId"]);
            mrAnnouncerGuyAccessToken = new MySecureString(Twitch.Configuration["Secrets:MrAnnouncerGuyTwitchAccessToken"]);
        }

        void ChangeScene(string sceneName)
        {
            try
            {
                obsWebsocket.SetCurrentProgramScene(sceneName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //Debugger.Break();
            }
        }

        void NeedToGetCoins(string userId)
        {
            Viewer viewerById = allViewers.GetViewerById(userId);
            if (viewerById != null)
                hubConnection.InvokeAsync("UserHasCoins", userId, viewerById.CoinsCollected);
        }

        void AddCoins(string userID, int amount)
        {
            Viewer viewerById = allViewers.GetViewerById(userID);
            if (viewerById != null)
                viewerById.CoinsCollected += amount;
        }

        //private System.Threading.Tasks.Task HubConnection_Closed(Exception arg)
        //{
        //	
        //}

        public void Disconnect()
        {
            Chat(GetExitMessage());
            Twitch.Disconnect();
            if (checkChatRoomTimer != null)
                checkChatRoomTimer.Dispose();
            if (autoSaveTimer != null)
                autoSaveTimer.Dispose();
            allViewers.Save();
            obsWebsocket.Disconnect();
        }

        void InitChatRoomTimer()
        {
            int oneMinute = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
            int fiveMinutes = (int)TimeSpan.FromMinutes(5).TotalMilliseconds;
            int thirtySeconds = (int)TimeSpan.FromMinutes(0.5).TotalMilliseconds;

            checkChatRoomTimer = new Timer(CheckViewers, null, oneMinute, oneMinute);
            autoSaveTimer = new Timer(AutoSaveViewers, null, fiveMinutes, fiveMinutes);
        }

        private void InitZork()
        {
            zork = new ZorkGame(Twitch.CodeRushedClient, STR_ChannelName);
            new BotCommand("zork", zork.HandleCommand);
        }

        private void LoadPersistentData()
        {
            fanfares = CsvData.Get<FanfareDto>(FileName.FanfareData);
            try
            {
                allViewers.Load();
                if (allViewers.Viewers.Count < 270)  // We have had at least 270 viewers tracked as of the writing of this bug check code.
                {
                    Console.Beep();
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine(string.Empty);
                    Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                    Console.WriteLine("!!                                                             !!");
                    Console.WriteLine("!!  Possible corruption detected in the AllViewers.json file!  !!");
                    Console.WriteLine("!!                                                             !!");
                    Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                    Console.WriteLine(string.Empty);
                    Console.WriteLine($"allViewers.Viewers.Count = {allViewers.Viewers.Count}");
                    Console.WriteLine(string.Empty);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception loading allViewers data: " + ex.Message);
                Debugger.Break();
            }
        }

        private void InitializeConnections()
        {
            if (useObs)
                InitializeObsWebSocket();
            _ = InitializeStudioPanelAsync();
            HookupCoreEvents(Twitch.FredGptClient);
            HookupCoreEvents(Twitch.RoryGptClient);
            HookupCoreEvents(Twitch.MarksVoiceClient);
            HookupTwitchEvents(Twitch.CodeRushedClient);
            HookupPubSubEvents(Twitch.CodeRushedEventSub);
            HookupTwitchEvents(Twitch.DroneCommandsClient);
            HookupObsEvents();
        }

        void HookupObsEvents()
        {
            ObsControl.ObsManager.StreamStarted += ObsManager_StreamStarted;
            ObsControl.ObsManager.RecordingStarted += ObsManager_RecordingStarted;
            ObsControl.ObsManager.StreamEnded += ObsManager_StreamEnded;
            ObsControl.ObsManager.RecordingEnded += ObsManager_RecordingEnded;
            ObsControl.ObsManager.SceneChanged += ObsManager_SceneChanged;
            ObsControl.ObsManager.SceneItemEnabled += ObsManager_SceneItemEnabled;
            ObsControl.ObsManager.SceneItemDisabled += ObsManager_SceneItemDisabled;
        }

        void HookupPubSubEvents(EventSubWebsocketClient eventSubClient)
        {
            eventSubClient.ErrorOccurred += CodeRushedEventSub_OnErrorOccurred;
            eventSubClient.WebsocketDisconnected += CodeRushedEventSub_OnWebsocketDisconnected;
            eventSubClient.WebsocketConnected += CodeRushedEventSub_OnWebsocketConnected;
            eventSubClient.ChannelPointsCustomRewardRedemptionAdd += CodeRushedEventSub_OnChannelPointsRewardRedeemed;
        }

        void UnhookPubSubEvents(EventSubWebsocketClient eventSubClient)
        {
            eventSubClient.ErrorOccurred -= CodeRushedEventSub_OnErrorOccurred;
            eventSubClient.WebsocketDisconnected -= CodeRushedEventSub_OnWebsocketDisconnected;
            eventSubClient.WebsocketConnected -= CodeRushedEventSub_OnWebsocketConnected;
            eventSubClient.ChannelPointsCustomRewardRedemptionAdd -= CodeRushedEventSub_OnChannelPointsRewardRedeemed;
        }

        private void CodeRushedEventSub_OnWebsocketConnected(object sender, WebsocketConnectedArgs e)
        {

        }

        private async void CodeRushedEventSub_OnWebsocketDisconnected(object sender, EventArgs e)
        {
            Console.WriteLine("EventSub WebSocket disconnected. Attempting to reconnect...");
            // NOTE: v0.0.3 ReconnectAsync() hardcodes the decommissioned beta URL
            // ("wss://eventsub-beta.wss.twitch.tv/ws"), so it always fails.
            // ReconnectEventSubAsync() re-creates the client using the production URL instead.
            UnhookPubSubEvents(Twitch.CodeRushedEventSub);
            while (!await Twitch.ReconnectEventSubAsync())
            {
                Console.WriteLine("EventSub WebSocket reconnect timed out, retrying in 1 second...");
                await Task.Delay(1000);
            }
            HookupPubSubEvents(Twitch.CodeRushedEventSub);
            Console.WriteLine("EventSub WebSocket reconnected successfully.");
        }

        private void CodeRushedEventSub_OnErrorOccurred(object sender, ErrorOccuredArgs e)
        {
            var msg = e.Exception?.Message ?? e.Message ?? "(unknown error)";
            Console.WriteLine($"EventSub error: {msg}");
            if (e.Exception != null)
                Console.WriteLine($"  → {e.Exception}");
            log.Add(new ErrorEntry() { Exception = e.Exception });
        }

        void QueueSceneToPlay(string scenesToPlay)
        {
            string sceneToPlay;
            if (scenesToPlay.Contains(";"))
                sceneToPlay = scenesToPlay.Split(";", StringSplitOptions.RemoveEmptyEntries).PickOne();
            else
                sceneToPlay = scenesToPlay;

            obsWebsocket.SetCurrentProgramScene(sceneToPlay);
        }

        private Task _treadmillPollTask = null;
        private CancellationTokenSource _treadmillPollCts = null;

        private void EnsureTreadmillPolling()
        {
            if (_treadmillPollTask == null || _treadmillPollTask.IsCompleted)
            {
                _treadmillPollCts = new CancellationTokenSource();
                _treadmillPollTask = PollTreadmillAsync(_treadmillPollCts.Token);
            }
        }

        private void StopTreadmillPolling()
        {
            _treadmillPollCts?.Cancel();
            _treadmillPollCts = null;
        }

        async Task ExecuteEventActionsAsync(string eventActions)
        {
            foreach (var (key, value) in ActionParser.ParseLines(eventActions))
            {
                switch (key)
                {
                    case "scene":
                        QueueSceneToPlay(value);
                        break;
                    case "labjack":
                        await ExecuteLabJackCommandAsync(value).ConfigureAwait(false);
                        break;
                    case "delay":
                        if (int.TryParse(value, out int ms) && ms > 0)
                            await Task.Delay(ms).ConfigureAwait(false);
                        break;
                    case "treadmill":
                        await ExecuteTreadmillActionAsync(value).ConfigureAwait(false);
                        break;
                    case "obs":
                        ExecuteObsSourceVisibilityCommand(value);
                        break;
                }
            }
        }

        private async Task ExecuteTreadmillActionAsync(string action)
        {
            if (_studioPanel == null) return;
            switch (action.ToLower())
            {
                case "start":
                    await _studioPanel.PressSwitchAsync(SwitchChannel.SW1).ConfigureAwait(false);
                    await Task.Delay(500).ConfigureAwait(false);
                    await _studioPanel.PressSwitchAsync(SwitchChannel.SW1).ConfigureAwait(false);
                    EnsureTreadmillPolling();
                    break;
                case "stop":
                    await _studioPanel.PressSwitchAsync(SwitchChannel.SW1).ConfigureAwait(false);
                    StopTreadmillPolling();
                    break;
                case "+":
                    await _studioPanel.PressSwitchAsync(SwitchChannel.SW2).ConfigureAwait(false);
                    break;
                case "-":
                    await _studioPanel.PressSwitchAsync(SwitchChannel.SW3).ConfigureAwait(false);
                    break;
            }
        }

        private async Task InitializeStudioPanelAsync()
        {
            Console.WriteLine("[LabJackBridge] Attempting to connect to LabJackBridge named pipe...");
            try
            {
                _studioPanel = StudioPanel.CreateDefault();
                Console.WriteLine($"[LabJackBridge] Loaded panel mapping (pipe: \"{_studioPanel.Map.PipeName}\"). Connecting...");
                await _studioPanel.ConnectAsync().ConfigureAwait(false);
                Console.WriteLine("[LabJackBridge] Connected. Initializing...");
                await _studioPanel.InitializeAsync().ConfigureAwait(false);
                Console.WriteLine("[LabJackBridge] Connected and initialized successfully.");
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"[LabJackBridge] FAILED — panel mapping file not found: {ex.FileName}");
                _studioPanel = null;
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine($"[LabJackBridge] FAILED — timed out waiting for LabJackBridge pipe (is the bridge running?): {ex.Message}");
                _studioPanel = null;
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"[LabJackBridge] FAILED — access denied connecting to pipe \"{_studioPanel?.Map.PipeName}\": {ex.Message}");
                Console.WriteLine("[LabJackBridge] Try running MrAnnouncerBot as Administrator, or ensure the LabJackBridge pipe grants access to the current user.");
                _studioPanel = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LabJackBridge] FAILED — {ex.GetType().Name}: {ex.Message}");
                _studioPanel = null;
            }
        }

        private async Task PollTreadmillAsync(CancellationToken cancellationToken = default)
        {
            const int intervalMs = 500;
            while (_studioPanel != null && !cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var metrics = _studioPanel.GetTreadmillMetrics();
                    double speedKph = metrics.SpeedMetersPerSecond * 3.6;
                    double distanceKm = metrics.TotalMeters / 1000.0;
                    await hubConnection.InvokeAsync("TreadmillStatus", speedKph, distanceKm).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Treadmill] Poll error: {ex.Message}");
                }
                await System.Threading.Tasks.Task.Delay(intervalMs, cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task ExecuteLabJackCommandAsync(string command)
        {
            if (_studioPanel == null) return;
            var parts = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2) return;
            string channel = parts[0];
            string verb = parts[1];
            try
            {
                await RunLabJackCommandCoreAsync(channel, verb).ConfigureAwait(false);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("not connected"))
            {
                Console.WriteLine("[StudioPanel] Not connected — attempting reconnect...");
                try
                {
                    await _studioPanel.ConnectAsync().ConfigureAwait(false);
                    await RunLabJackCommandCoreAsync(channel, verb).ConfigureAwait(false);
                }
                catch (Exception retryEx)
                {
                    Console.WriteLine($"[StudioPanel] Command failed after reconnect ({command}): {retryEx.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[StudioPanel] Command failed ({command}): {ex.Message}");
            }
        }

        private void ExecuteObsSourceVisibilityCommand(string value)
        {
            // Expected format: "SceneName, SourceName, show|hide"
            var parts = value.Split(',');
            if (parts.Length < 3) return;
            string sceneName = parts[0].Trim();
            string sourceName = parts[1].Trim();
            string verb = parts[2].Trim();
            bool visible = verb.Equals("show", StringComparison.OrdinalIgnoreCase);
            ObsControl.ObsManager.SetSceneItemEnabled(sceneName, sourceName, visible);
        }

        private async Task RunLabJackCommandCoreAsync(string channel, string verb)
        {
            if (Enum.TryParse<PowerChannel>(channel, true, out var pwr))
            {
                if (verb.Equals("On", StringComparison.OrdinalIgnoreCase))
                    await _studioPanel!.SetPowerAsync(pwr, true).ConfigureAwait(false);
                else if (verb.Equals("Off", StringComparison.OrdinalIgnoreCase))
                    await _studioPanel!.SetPowerAsync(pwr, false).ConfigureAwait(false);
            }
            else if (Enum.TryParse<SwitchChannel>(channel, true, out var sw))
            {
                if (verb.Equals("Pulse", StringComparison.OrdinalIgnoreCase))
                    await _studioPanel!.PressSwitchAsync(sw).ConfigureAwait(false);
            }
        }

        void ExecuteObsEvent(string eventName, string parameters = null)
        {
            string action = EventActionMaps
                .FirstOrDefault(x =>
                    string.Equals(x.EventName, eventName, StringComparison.OrdinalIgnoreCase) &&
                    (string.IsNullOrEmpty(x.Parameters) || string.Equals(x.Parameters, parameters, StringComparison.OrdinalIgnoreCase)))
                ?.Action;
            if (action != null)
                _ = ExecuteEventActionsAsync(action);
        }

        void ObsManager_StreamStarted(object sender, EventArgs e) => ExecuteObsEvent("StreamStarted");
        void ObsManager_RecordingStarted(object sender, EventArgs e) => ExecuteObsEvent("RecordingStarted");
        void ObsManager_StreamEnded(object sender, EventArgs e) => ExecuteObsEvent("StreamEnded");
        void ObsManager_RecordingEnded(object sender, EventArgs e) => ExecuteObsEvent("RecordingEnded");
        void ObsManager_SceneChanged(object sender, string sceneName) => ExecuteObsEvent("SceneActivated", sceneName);
        void ObsManager_SceneItemEnabled(object sender, ObsControl.SceneItemEventArgs e) => ExecuteObsEvent("SourceVisible", $"{e.SceneName}, {e.Item.SourceName}");
        void ObsManager_SceneItemDisabled(object sender, ObsControl.SceneItemEventArgs e) => ExecuteObsEvent("SourceHidden", $"{e.SceneName}, {e.Item.SourceName}");

        void ExecuteChannelPointAction(ChannelPointAction channelPointAction, User user)
        {
            if (channelPointAction == null)
                return;
            if (!string.IsNullOrWhiteSpace(channelPointAction.SceneToPlay))
                QueueSceneToPlay(channelPointAction.SceneToPlay);
            if (!string.IsNullOrWhiteSpace(channelPointAction.Action))
                _ = ExecuteEventActionsAsync(channelPointAction.Action);
        }

        private void CodeRushedEventSub_OnChannelPointsRewardRedeemed(object sender, ChannelPointsCustomRewardRedemptionArgs e)
        {
            string id = e.Notification.Payload.Event.Reward.Id;
            string title = e.Notification.Payload.Event.Reward.Title;
            ExecuteChannelPointAction(GetChannelPointAction(id, title), null);
        }

        ChannelPointAction GetChannelPointAction(string id, string title)
        {
            return ChannelPointActionLookup.Find(ChannelPointActions, id, title);
        }

        void HookupCoreEvents(TwitchClient client)
        {
            client.OnError += Client_OnError;
            client.OnDisconnected += Client_OnDisconnected;
        }

        void UnhookCoreEvents(TwitchClient client)
        {
            client.OnError -= Client_OnError;
            client.OnDisconnected -= Client_OnDisconnected;
        }

        void HookupTwitchEvents(TwitchClient client)
        {
            HookupCoreEvents(client);

            client.OnJoinedChannel += TwitchClient_OnJoinedChannel;
            client.OnChatCommandReceived += TwitchClient_OnChatCommandReceived;
            client.OnMessageReceived += TwitchClient_OnMessageReceived;
            client.OnUserJoined += TwitchClient_OnUserJoined;
            client.OnUserLeft += TwitchClient_OnUserLeft;
            client.OnChannelStateChanged += Client_OnChannelStateChanged;
            client.OnLog += Client_OnLog;
        }

        void UnHookTwitchEvents(TwitchClient client)
        {
            client.OnJoinedChannel -= TwitchClient_OnJoinedChannel;
            client.OnChatCommandReceived -= TwitchClient_OnChatCommandReceived;
            client.OnMessageReceived -= TwitchClient_OnMessageReceived;
            client.OnUserJoined -= TwitchClient_OnUserJoined;
            client.OnUserLeft -= TwitchClient_OnUserLeft;
            client.OnChannelStateChanged -= Client_OnChannelStateChanged;
            client.OnLog -= Client_OnLog;
            UnhookCoreEvents(client);
        }

        private void Client_OnLog(object sender, OnLogArgs e)
        {
            log.Add(new LogEntry() { BotUsername = e.BotUsername, Data = e.Data, Time = e.DateTime });
        }

        private void Client_OnError(object sender, TwitchLib.Communication.Events.OnErrorEventArgs e)
        {
            log.Add(new ErrorEntry() { Exception = e.Exception, Time = DateTime.Now });
        }

        private void Client_OnDisconnected(object sender, TwitchLib.Communication.Events.OnDisconnectedEventArgs e)
        {
            log.Add(new MessageEntry() { Message = "ClientDisconnected", Time = DateTime.Now });
        }

        private void Client_OnChannelStateChanged(object sender, OnChannelStateChangedArgs e)
        {
            log.Add(new MessageEntry() { Message = "ChannelStateChanged: " + e.Channel, Time = DateTime.Now });
        }

        void AutoSaveViewers(object obj)
        {
            Console.WriteLine($"Saving allViewers data for {allViewers.Viewers.Count} viewers... {DateTime.Now:T}");
            allViewers.Save();
        }

        async void CheckViewers(object obj)
        {
            try
            {
                HttpResponseMessage response = await httpClient.PostAsync(STR_GetChattersApi, null);

                string responseString = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    if (responseString == null)
                        return;

                    LiveViewers liveViewers = JsonConvert.DeserializeObject<LiveViewers>(responseString);
                    if (liveViewers != null)
                        allViewers.UpdateLiveViewers(liveViewers.chatters.viewers);
                }
                else
                {
                    // TODO: Respond to errors in responseString
                    //System.Diagnostics.Debugger.Break();
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in CheckViewers: " + ex.Message);
                //Debugger.Break();
            }
        }

        private void TwitchClient_OnUserLeft(object sender, OnUserLeftArgs e)
        {
            allViewers.UserLeft(e.Username);
        }

        private void TwitchClient_OnUserJoined(object sender, OnUserJoinedArgs e)
        {
            allViewers.UserJoined(e.Username);
        }

        Dictionary<string, DateTime> playedFanfares = new Dictionary<string, DateTime>();
        Dictionary<string, DateTime> playedGreetingFromFred = new Dictionary<string, DateTime>();
        Dictionary<string, DateTime> playedGreetingFromRory = new Dictionary<string, DateTime>();

        Queue<string> fanfareQueue = new Queue<string>();
        List<FanfareDto> fanfares = new List<FanfareDto>();
        DateTime lastFanfareActivated = DateTime.Now;
        double lastFanfareDuration;
        bool suppressingFanfare;
        string startTimeURL;
        DateTime issueStartTime;

        void HandleUserFanfare(ChatMessage chatMessage)
        {
            if (suppressingFanfare)
                return;

            int userFanfareCount = GetFanfareCount(chatMessage.DisplayName);

            if (userFanfareCount > 0)
            {
                PlayFanfare(chatMessage.DisplayName, chatMessage.Message, chatMessage.UserId);
            }
            else
                PlayBackloggedFanfare();
        }

        private int GetFanfareCount(string displayName)
        {
            return fanfares.Where(x => string.Compare(x.DisplayName, displayName, StringComparison.InvariantCultureIgnoreCase) == 0).Count();
        }

        void PlayBackloggedFanfare()
        {

            if (fanfareQueue.Count == 0)
                return;

            string displayName = fanfareQueue.Peek();

            if (PlayFanfare(displayName))
                fanfareQueue.Dequeue();

        }

        private const string emptyString = "";
        const string STR_MarkSaysOrThinks = "!mark";
        const string STR_FredSaysOrThinks = "!fred";
        const string STR_CampbellSaysOrThinks = "!campbell";
        const string STR_RorySaysOrThinks = "!rory";
        const string STR_RichardSaysOrThinks = "!richard";
        private const int minUserLevelForSpeechBubbles = 4;

        bool TriggersSpecialFanfare(string displayName, string message)
        {
            if (specialFanfares == null)
            {
                specialFanfares = GoogleSheets.Get<SpecialFanfare>();
            }
            SpecialFanfare specialFanfare = specialFanfares.FirstOrDefault(x => x.UserId == displayName);
            if (specialFanfare != null && message.Contains(specialFanfare.KeyPhrase, StringComparison.InvariantCultureIgnoreCase))
            {
                string sceneName = specialFanfare.SceneName;
                ActivatingSceneByName(sceneName, "SpecialFanfare");
                try
                {
                    hubConnection.InvokeAsync("SuppressVolume", specialFanfare.Duration);
                    obsWebsocket.SetCurrentProgramScene(sceneName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unable to play special fanfare: {sceneName}. Exception: {ex.Message}");
                }
                return true;
            }

            return false;
        }

        private bool PlayFanfare(string displayName, string message = emptyString, string id = emptyString)
        {
            if (TriggersSpecialFanfare(id, message))
                return false;
            string fanfareKey = displayName.ToLower();
            if (playedFanfares.ContainsKey(fanfareKey) && playedFanfares[fanfareKey].DayOfYear == DateTime.Now.DayOfYear)
                return true;

            bool stillPlaying = DateTime.Now - lastFanfareActivated < TimeSpan.FromSeconds(lastFanfareDuration);
            bool suppressFanfareToday = MessageSuppressesFanfare(message);

            if (suppressFanfareToday)
            {
                MarkFanfareAsPlayed(displayName);
                return true;
            }

            if (stillPlaying || RestrictedSceneIsActive())
            {
                if (!fanfareQueue.Contains(displayName))
                    fanfareQueue.Enqueue(displayName);
                return false;
            }

            lastFanfareActivated = DateTime.Now;


            // Determine the Fanfare to be played
            FanfareDto fanfare = DetermineFanfareToPlay(displayName);

            if (fanfare != null && (DateTime.Now - fanfare.LastPlayed).TotalHours > 5)
            {
                string sceneName = fanfare.DisplayName;
                if (GetFanfareCount(fanfare.DisplayName) > 1)
                    sceneName += fanfare.Index;

                lastFanfareDuration = fanfare.SecondsLong + 3;

                ActivatingSceneByName(sceneName, "Fanfare");
                try
                {
                    hubConnection.InvokeAsync("SuppressVolume", fanfare.SecondsLong);
                    obsWebsocket.SetCurrentProgramScene(sceneName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unable to play fanfare: {sceneName}. Exception: {ex.Message}");
                }

                MarkFanfareAsPlayed(fanfare);

                Chat(new VIPGreeting(displayName).Greeting);
            }

            return true;
        }

        FanfareDto DetermineFanfareToPlay(string displayName)
        {
            List<FanfareDto> userFanfares = fanfares.Where(x => string.Compare(x.DisplayName, displayName, StringComparison.InvariantCultureIgnoreCase) == 0).ToList();
            return SelectFanfareFromList(userFanfares);
        }

        /// <summary>
        /// Core fanfare-selection logic extracted for unit testability.
        /// Prefers full-length fanfares not played in the last 5 hours;
        /// falls back to any clipped fanfare when none are available.
        /// Returns <c>null</c> when every candidate was played within the last 5 hours.
        /// </summary>
        internal static FanfareDto SelectFanfareFromList(IEnumerable<FanfareDto> userFanfares)
        {
            var list = userFanfares.ToList();

            // Make sure at least one fanfare hasn't been played in the last 5 hours
            // (handles restart-mid-stream scenario)
            if (!list.Any(f => (DateTime.Now - f.LastPlayed).TotalHours > 5))
                return null;

            // Prefer full-length fanfares not played in the last 5 hours
            IEnumerable<FanfareDto> candidates = list
                .Where(f => f.Duration == FanfareDuration.fullLength)
                .Where(f => (DateTime.Now - f.LastPlayed).TotalHours > 5);

            // Fall back to clipped fanfares (no recency filter — they are the last resort)
            if (!candidates.Any())
                candidates = list.Where(f => f.Duration == FanfareDuration.clipped);

            if (!candidates.Any())
                return null;

            if (candidates.Count() == 1)
                return candidates.First();

            return candidates.ElementAt(new Random().Next(candidates.Count()));
        }

        /// <summary>
        /// Returns <c>true</c> when the chat message signals that the fanfare
        /// should be silently marked as played without actually triggering it.
        /// A message beginning with '[' (e.g. "[lurking]") suppresses the fanfare.
        /// </summary>
        internal static bool MessageSuppressesFanfare(string message)
            => message.StartsWith('[');

        static void WriteFanfareData(string dataFileName, List<FanfareDto> records)
        {
            using (var writer = new StreamWriter(dataFileName))
            using (var csv = new CsvWriter(writer, System.Globalization.CultureInfo.CurrentCulture))
            {
                csv.WriteRecords(records);
            }
        }

        void MarkFanfareAsPlayed(FanfareDto fanfare)
        {

            FanfareDto updatedFanfare = fanfares.Where(_ => _.DisplayName == fanfare.DisplayName && _.Index == fanfare.Index && _.Duration == fanfare.Duration).First();

            updatedFanfare.LastPlayed = DateTime.Now;

            WriteFanfareData(FileName.FanfareData_Source, fanfares);

            MarkFanfareAsPlayed(fanfare.DisplayName);
        }

        void MarkFanfareAsPlayed(string DisplayName)
        {
            string fanfareKey = DisplayName.ToLower();
            if (playedFanfares.ContainsKey(fanfareKey))
                playedFanfares[fanfareKey] = DateTime.Now;
            else
                playedFanfares.Add(fanfareKey, DateTime.Now);
        }

        void MarkCodeRushIssue(OnChatCommandReceivedArgs obj)
        {
            if (obj.Command.ChatMessage.UserId != STR_CodeRushedUserId)
                return;

            bool attachLogFiles = false;
            bool attachSettingsFiles = false;
            bool sendPrz = false;
            bool sendAlex = false;
            bool sendPerf = false;
            bool sendAllDevs = false;
            string message = string.Empty;
            string backTrackStr = string.Empty;
            foreach (string arg in obj.Command.ArgumentsAsList)
            {
                if (arg == "-log")
                    attachLogFiles = true;
                else if (arg == "-settings")
                    attachSettingsFiles = true;
                else if (arg == "-prz")
                    sendPrz = true;
                else if (arg == "-alex")
                    sendAlex = true;
                else if (arg == "-perf")
                    sendPerf = true;
                else if (arg == "-allDevs")
                    sendAllDevs = true;
                else if (arg.StartsWith("-$"))
                    backTrackStr = arg.Substring(1);
                else
                    message = arg;
            }

            MarkCodeRushIssue(message, attachLogFiles, attachSettingsFiles, sendPrz, sendAlex, sendPerf, sendAllDevs, backTrackStr);
        }

        public enum Greeter
        {
            Fred,
            Rory
        }

        string GetGreeting(Greeter greeter, string userName, string userId)
        {
            string settingName;
            if (greeter == Greeter.Rory)
                settingName = "rory";
            else
                settingName = "fred";

            DataRow viewerSetting = AllViewerListSettings.Instance.GetViewerSetting(userId, userName, settingName);
            if (viewerSetting == null)
                return null;

            return viewerSetting.SelectRandom();
        }

        public enum PlayGreetingResult
        {
            NothingPlayed,
            RoryPlayed,
            FredPlayed
        }

        PlayGreetingResult PlayGreetingIfNeeded(Dictionary<string, DateTime> greetingCache, string userName, string userId, Greeter greeter)
        {
            if (greetingCache.ContainsKey(userId) && greetingCache[userId].DayOfYear == DateTime.Now.DayOfYear)
                return PlayGreetingResult.NothingPlayed;  // Already played the greeting today.

            string greeting = GetGreeting(greeter, userName, userId);
            if (greeting == null)
                return PlayGreetingResult.NothingPlayed;

            greetingCache[userId] = DateTime.Now;

            if (greeter == Greeter.Fred)
            {
                SayItOrThinkItAsync("fred", greeting);
                return PlayGreetingResult.FredPlayed;
            }
            else
            {
                SayItOrThinkItAsync("rory", greeting);
                return PlayGreetingResult.RoryPlayed;
            }
        }

        Random randominator = new Random();

        void PlayGreetingsFromAvatars(ChatMessage chatMessage)
        {
            PlayGreetingResult playGreetingResult;
            if (randominator.Next(100) < 50)
            {
                playGreetingResult = PlayGreetingIfNeeded(playedGreetingFromFred, chatMessage.Username, chatMessage.UserId, Greeter.Fred);
                if (playGreetingResult == PlayGreetingResult.NothingPlayed)
                    playGreetingResult = PlayGreetingIfNeeded(playedGreetingFromRory, chatMessage.Username, chatMessage.UserId, Greeter.Rory);
            }
            else
            {
                playGreetingResult = PlayGreetingIfNeeded(playedGreetingFromRory, chatMessage.Username, chatMessage.UserId, Greeter.Rory);
                if (playGreetingResult == PlayGreetingResult.NothingPlayed)
                    playGreetingResult = PlayGreetingIfNeeded(playedGreetingFromFred, chatMessage.Username, chatMessage.UserId, Greeter.Fred);
            }
        }

        void MarkThatWeAlreadyGreetedFromFred(string userId)
        {
            playedGreetingFromFred[userId] = DateTime.Now;
        }

        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else
                return Color.FromArgb(255, v, p, q);
        }

        Color GetHighContrastTextColorAgainstWhiteBackground(Color color)
        {
            float hue = color.GetHue();
            float saturation = color.GetSaturation();
            float brightness = color.GetBrightness();
            if (brightness > 0.4)
            {
                brightness = 0.4f;
                return ColorFromHSV(hue, saturation, brightness);
            }
            return color;
        }

        bool IsNotFred(string userId)
        {
            return userId != "904388657";
        }

        bool IsNotMarksVoice(string userId)
        {
            return userId != "907014337";
        }

        private async void TwitchClient_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            if (IsNotFred(e.ChatMessage.UserId) && IsNotMarksVoice(e.ChatMessage.UserId) && FredGpt.IsTalkingToFred(e.ChatMessage.Message))
            {
                MarkThatWeAlreadyGreetedFromFred(e.ChatMessage.UserId);
                string response = await FredGpt.GetResponse(e.ChatMessage.UserId, e.ChatMessage.Username, e.ChatMessage.Message);
                if (string.IsNullOrWhiteSpace(response))
                    PlayGreetingsFromAvatars(e.ChatMessage);
                else
                {
                    string textColor = ColorTranslator.ToHtml(GetHighContrastTextColorAgainstWhiteBackground(e.ChatMessage.Color)) ?? "#000";
                    SayItOrThinkItAsync("fred", response, textColor);
                    string trimmedResponse = response.TrimStart('"').TrimEnd('"');
                    Twitch.FredChat(trimmedResponse);
                }
            }
            else
                PlayGreetingsFromAvatars(e.ChatMessage);

            HandleUserFanfare(e.ChatMessage);
            allViewers.OnMessageReceived(e.ChatMessage);
        }

        async void MarkCodeRushIssue(string title, bool attachLogFiles, bool attachSettingsFiles, bool sendPrz, bool sendAlex, bool sendPerf, bool sendAllDevs, string backTrackStr)
        {
            string showStartURL;

            string durationStr = string.Empty;
            string errors = string.Empty;

            if (startTimeURL == null)
            {
                try
                {
                    showStartURL = await Twitch.GetActiveShowPointURL(mrAnnouncerGuyClientId, mrAnnouncerGuyAccessToken, STR_CodeRushedUserId, backTrackStr);
                }
                catch
                {
                    showStartURL = startTimeURL;
                    Debugger.Break();
                }
            }
            else
            {  // We already marked a start time for this issue.
                TimeSpan timeSpan = DateTime.Now - issueStartTime;
                durationStr = $" (duration: {timeSpan.TotalMinutes:F} minutes)";
                showStartURL = startTimeURL;
                startTimeURL = null;
            }

            List<string> attachedFiles = new List<string>();

            if (attachLogFiles)
            {
                try
                {
                    const string path = @"C:\Users\Mark Miller\AppData\Local\CodeRush\Logs\";
                    string baseZipFileName = Path.GetFileNameWithoutExtension(showStartURL);
                    baseZipFileName = "CodeRushLogFiles_" + baseZipFileName.Replace("?t=", "_");
                    string fullPathToZipFile = Path.Combine(path, baseZipFileName + ".zip");
                    using (var zip = ZipFile.Open(fullPathToZipFile, ZipArchiveMode.Create))
                    {
                        IEnumerable<string> logFiles = Directory.EnumerateFiles(@"C:\Users\Mark Miller\AppData\Local\CodeRush\Logs", "*.log");
                        foreach (string file in logFiles)
                        {
                            try
                            {
                                // new FileStream("c:\test.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                                // What is going on here?
                                zip.CreateEntryFromFile(file, Path.GetFileName(file), CompressionLevel.Optimal);
                            }
                            catch
                            {
                                try
                                {
                                    string destFileName = Path.Combine(Path.GetDirectoryName(file), "MostRecent_" + Path.GetFileName(file));
                                    File.Copy(file, destFileName);
                                    zip.CreateEntryFromFile(destFileName, Path.GetFileName(destFileName), CompressionLevel.Optimal);
                                    File.Delete(destFileName);
                                }
                                catch (Exception ex2)
                                {
                                    errors += $"\n\n Exception attached log file {file}: " + ex2.Message;
                                }

                            }
                        }
                    }
                    attachedFiles.Add(fullPathToZipFile);
                }

#pragma warning disable CS0168 // Used for diagnostics and debugging.
                catch (Exception ex)
                {
                    Debugger.Break();
                }
                // 
            }

            string htmlBody = $"{title}:\n{showStartURL} {durationStr}{errors}\nEmail sent at {DateTime.Now.ToLongTimeString()}, local time.";
            Email($"CodeRush Issue - {title}", htmlBody, attachedFiles);
        }

        public static void Email(string subject, string htmlBody, List<string> attachedFiles)
        {
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                //`! !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //`! !!!                                                                                      !!!
                //`! !!!  Turn off Debug Visualizer before stepping through this method live on the stream!!! !!!
                //`! !!!                                                                                      !!!
                //`! !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                message.From = new MailAddress(Twitch.Configuration["Secrets:EmailFromAddress"]);
                message.To.Add(new MailAddress(Twitch.Configuration["Secrets:EmailMark"]));
                message.To.Add(new MailAddress(Twitch.Configuration["Secrets:EmailRory"]));
                //if (sendPrz)
                //	message.To.Add(new MailAddress(Twitch.Configuration["Secrets:EmailPrz"]));
                //if (sendAlex)
                //message.To.Add(new MailAddress(Twitch.Configuration["Secrets:EmailAlex"]));
                //if (sendPerf)
                //	message.To.Add(new MailAddress(Twitch.Configuration["Secrets:EmailPerf"]));
                //if (sendAllDevs)
                //	message.To.Add(new MailAddress(Twitch.Configuration["Secrets:EmailAllDevs"]));
                message.Subject = subject;
                message.IsBodyHtml = true; //to make message body as html  
                message.Body = htmlBody;

                foreach (string attachedFile in attachedFiles)
                {
                    message.Attachments.Add(new Attachment(attachedFile));
                }

                smtp.Port = 587;
                smtp.Host = Twitch.Configuration["Secrets:EmailHost"];
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(Twitch.Configuration["Secrets:EmailUserName"], Twitch.Configuration["Secrets:EmailPassword"]);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    //`! !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    //`! !!!                                                                                      !!!
                    //`! !!!  Turn off Debug Visualizer before stepping through this method live on the stream!!! !!!
                    //`! !!!                                                                                      !!!
                    //`! !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                    Debugger.Break();
                }
            }
        }

        private void ConnectToObs()
        {
            if (obsWebsocket.IsConnected) return;
            try
            {
                //`! !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //`! !!!                                                                                      !!!
                //`! !!!  Turn off Debug Visualizer before stepping through this method live on the stream!!! !!!
                //`! !!!                                                                                      !!!
                //`! !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                obsWebsocket.ConnectAsync(ObsHelper.WebSocketPort, Twitch.Configuration["Secrets:ObsPassword"]);  // Settings.Default.ObsPassword);

            }
            catch (AuthFailureException)
            {
                Console.WriteLine("Authentication failed.");
                Debugger.Break();
            }
            catch (ErrorResponseException ex)
            {
                Console.WriteLine($"Connect failed. {ex.Message}");
                Debugger.Break();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected exception. {ex.Message}");
                Debugger.Break();
            }
        }

        private void InitializeObsWebSocket()
        {
            HookObsEvents();

            ConnectToObs();
        }

        private void HookObsEvents()
        {
            obsWebsocket.Connected += ObsWebsocket_Connected;
            obsWebsocket.Disconnected += ObsWebsocket_Disconnected;
            obsWebsocket.CurrentProgramSceneChanged += ObsWebsocket_CurrentProgramSceneChanged;
            obsWebsocket.ProfileListChanged += ObsWebsocket_ProfileListChanged;
            obsWebsocket.StreamStateChanged += ObsWebsocket_StreamStateChanged;
        }

        private void UnhookObsEvents()
        {
            obsWebsocket.Connected -= ObsWebsocket_Connected;
            obsWebsocket.Disconnected -= ObsWebsocket_Disconnected;
            obsWebsocket.CurrentProgramSceneChanged -= ObsWebsocket_CurrentProgramSceneChanged;
            obsWebsocket.ProfileListChanged -= ObsWebsocket_ProfileListChanged;
            obsWebsocket.StreamStateChanged -= ObsWebsocket_StreamStateChanged;
        }

        private void ObsWebsocket_StreamStateChanged(object sender, OBSWebsocketDotNet.Types.Events.StreamStateChangedEventArgs e)
        {
            Console.WriteLine($"ObsWebsocket_ProfileChanged: {e.OutputState}");
        }

        private void ObsWebsocket_ProfileListChanged(object sender, OBSWebsocketDotNet.Types.Events.ProfileListChangedEventArgs e)
        {
            Console.WriteLine("ObsWebsocket_ProfileChanged");
        }

        private void ObsWebsocket_CurrentProgramSceneChanged(object sender, OBSWebsocketDotNet.Types.Events.ProgramSceneChangedEventArgs e)
        {
            activeSceneName = e.SceneName;
            if (activeSceneName == "EventReset")
            {
                Debugger.Break();

                UnhookPubSubEvents(Twitch.CodeRushedEventSub);
                UnHookTwitchEvents(Twitch.CodeRushedClient);
                UnHookTwitchEvents(Twitch.DroneCommandsClient);
                UnhookCoreEvents(Twitch.FredGptClient);
                UnhookCoreEvents(Twitch.RoryGptClient);
                UnhookCoreEvents(Twitch.MarksVoiceClient);
                Twitch.InitializeConnections();
                HookupTwitchEvents(Twitch.DroneCommandsClient);
                HookupTwitchEvents(Twitch.CodeRushedClient);
                HookupPubSubEvents(Twitch.CodeRushedEventSub);
                HookupCoreEvents(Twitch.FredGptClient);
                HookupCoreEvents(Twitch.RoryGptClient);
                HookupCoreEvents(Twitch.MarksVoiceClient);
            }
            Console.WriteLine($"Active Scene: {activeSceneName}");
        }

        private void ObsWebsocket_Disconnected(object sender, OBSWebsocketDotNet.Communication.ObsDisconnectionInfo e)
        {
            UnhookObsEvents();
            obsWebsocket = null;
            reconnectObsClientTimer = new Timer(ReconnectObsClient, null, 250, Timeout.Infinite);
        }

        void ReconnectObsClient(object obj)
        {
            reconnectObsClientTimer = null;
            obsWebsocket = new OBSWebsocket();
            InitializeObsWebSocket();
        }

        private void ObsWebsocket_Connected(object sender, EventArgs e)
        {
            Console.WriteLine("ObsWebsocket_Connected");
        }

        private void TwitchClient_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            try
            {
                Chat(GetEntranceMessage());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception thrown in TwitchClient_OnJoinedChannel: " + ex);
                Debugger.Break();
            }
        }

        private void Chat(string msg)
        {
            Twitch.Chat(Twitch.CodeRushedClient, Twitch.TruncateIfNeeded(msg));
        }

        public void Run()
        {
            Twitch.InitializeConnections();
            InitializeConnections();
        }

        private SceneDto GetScene(string command)
        {
            return useObs ? Scenes.FirstOrDefault(m => m.Matches(command)) : null;
        }

        string SelectRandomScene(string sceneName)
        {
            var filter = sceneName;
            if (filter.EndsWith("*"))
                filter = filter.TrimEnd('*');
            var currentSceneCollection = obsWebsocket.ListScenes();
            List<string> foundNames = new List<string>();

            foundNames = currentSceneCollection.Where(x => x.Name.StartsWith(filter)).Select(x => x.Name).ToList();

            if (foundNames.Count == 0)
                return null;

            int index = RandomInt(foundNames.Count);
            return foundNames[index];
        }

        private int RandomInt(int maxValue)
        {
            return random.Next(maxValue);
        }

        string GetBreakMessage()
        {
            switch (RandomInt(6))
            {
                case 0:
                    return "I'm on a break right now.";
                case 1:
                    return "On a break. Ask me later.";
                case 2:
                    return "Consuming coffee. Back in a bit.";
                case 3:
                    return "I'm sorry. What?";
                case 4:
                    return "I say we let Mark talk.";
                case 5:
                    return "Maybe later.";
                default:
                    return "Gimme a sec...";
            }
        }

        string GetEntranceMessage()
        {
            switch (RandomInt(6))
            {
                case 0:
                    return "Mr. Announcer Bot is in da House!";
                case 1:
                    return "Mr. Announcer Bot has arrived!";
                case 2:
                    return "You called? Mr. Announcer Bot at your service!";
                case 3:
                    return "Mr. Announcer Bot is here to take care of all your chatting needs!";
                case 4:
                    return "LET'S DO THIS!!! (in the house).";
                case 5:
                    return "Mr. Announcer Bot greets you: Good day!";
                default:
                    return "Mr. Announcer Bot is ready to ROCK!!!";
            }
        }

        object GetLevelName(int userLevel)
        {
            if (userLevel == 0)
                return "padawan";
            if (userLevel == 1)
                return "wizardling";
            if (userLevel == 2)
                return "apprentice";
            if (userLevel == 3)
                return "student";
            if (userLevel == 4)
                return "magician";
            return "wizard";
        }

        string GetNeedToLevelUpMessage(SceneDto scene, string displayName, int userLevel)
        {
            string learnMore = "You can learn about botcasting levels here: https://github.com/MillerMark/MrAnnouncerBot";
            switch (RandomInt(4))
            {
                case 0:
                    return $"{displayName}, that's a level {scene.Level} spell, but alas, you are a level {userLevel} {GetLevelName(userLevel)}. " + learnMore;
                case 1:
                    return $"Unfortunately {displayName}, there's no way a level {userLevel} {GetLevelName(userLevel)} can botcast level {scene.Level} spell! " + learnMore;
                case 2:
                    return $"{displayName}, you'll need to level-up to {scene.Level} before you botcast that spell! " + learnMore;
                default:
                    return $"{displayName} that's a level {scene.Level} spell! You need to level-up first! " + learnMore;
            }
        }
        string GetExitMessage()
        {
            switch (RandomInt(6))
            {
                case 0:
                    return "MrAnnouncerBot has left the building!";
                case 1:
                    return "Mr. Announcer Bot has departed! (the chat room)";
                case 2:
                    return "Mr. Announcer Bot is off to another PARTY!";
                case 3:
                    return "Mr. Announcer Bot is gone! You're on your own!";
                case 4:
                    return "I'm outta here!";
                case 5:
                    return "Good day! Goodbye! And good luck!";
                default:
                    return "Like Schrödinger's cat, am I in the box? Or am I out? Don't look!";
            }
        }

        TimeSpan GetTimeSinceLastSceneActivation(SceneDto scene)
        {
            if (lastScenePlayTime.ContainsKey(scene.SceneName))
                return DateTime.Now - lastScenePlayTime[scene.SceneName];
            return TimeSpan.MaxValue;
        }

        TimeSpan GetTimeSinceLastCategoryActivation(SceneDto scene)
        {
            if (lastCategoryPlayTime.ContainsKey(scene.Category))
                return DateTime.Now - lastCategoryPlayTime[scene.Category];
            return TimeSpan.MaxValue;
        }

        void ActivatingSceneByName(string name, string category)
        {
            DateTime now = DateTime.Now;

            if (!lastScenePlayTime.ContainsKey(name))
                lastScenePlayTime.Add(name, now);
            else
                lastScenePlayTime[name] = now;

            if (!lastCategoryPlayTime.ContainsKey(category))
                lastCategoryPlayTime.Add(category, now);
            else
                lastCategoryPlayTime[category] = now;
        }

        void ActivatingScene(SceneDto scene)
        {
            ActivatingSceneByName(scene.SceneName, scene.Category);
        }

        double GetSpanWaitAdjust(int userLevel)
        {
            if (userLevel < 0)
                return 2;

            if (userLevel < 5)
                return 1;

            if (userLevel < 10)
                return 0.75;

            if (userLevel < 15)
                return 0.5;

            return 0.25;
        }

        void ActivateScene(SceneDto scene, string displayName, int userLevel)
        {
            if (scene.Level > userLevel)
            {
                Chat(GetNeedToLevelUpMessage(scene, displayName, userLevel));
                return;
            }
            string sceneName = GetSceneName(scene);
            if (sceneName == null)
                return;

            double minutesSinceLastSceneActivation = GetTimeSinceLastSceneActivation(scene).TotalMinutes;
            double minutesSinceLastCategoryActivation = GetTimeSinceLastCategoryActivation(scene).TotalMinutes;

            var adjustedMinutesToSame = GetSpanWaitAdjust(userLevel) * scene.MinMinutesToSame;
            if (adjustedMinutesToSame > minutesSinceLastSceneActivation && userLevel < 99)
            {
                double minutesToWait = scene.MinMinutesToSame - minutesSinceLastSceneActivation;
                Chat($"I already said that @{displayName}. You'll have to wait another {minutesToWait:0.#} minutes until I can say that again.");
                return;
            }
            ActivatingScene(scene);
            try
            {
                obsWebsocket.SetCurrentProgramScene(sceneName);
            }
            catch (Exception e)
            {
                Chat($"Sorry, I can't find that scene: {sceneName}");
            }
        }

        private void ActivateSceneIfPermitted(SceneDto scene, string displayName, int userLevel)
        {
            if (RestrictedSceneIsActive() && userLevel < AllViewers.ModeratorLevel)
                Chat(GetBreakMessage());
            else
                ActivateScene(scene, displayName, userLevel);
        }

        private string GetSceneName(SceneDto scene)
        {
            string sceneName = scene.SceneName;
            if (sceneName.EndsWith("*"))
                sceneName = SelectRandomScene(sceneName);
            return sceneName;
        }

        private bool RestrictedSceneIsActive()
        {
            return RestrictedScenes.Any(x => x.SceneName == activeSceneName);
        }

        ProfanityFilter.ProfanityFilter profanityFilter;

        public static List<SceneDto> Scenes
        {
            get
            {
                if (scenes == null)
                    scenes = GoogleSheets.Get<SceneDto>();
                return scenes;
            }
        }

        public static List<RestrictedSceneDto> RestrictedScenes
        {
            get
            {
                if (restrictedScenes == null)
                {
                    restrictedScenes = GoogleSheets.Get<RestrictedSceneDto>();
                }
                return restrictedScenes;
            }
        }

        public static List<ChannelPointAction> ChannelPointActions
        {
            get
            {
                if (channelPointActions == null)
                    channelPointActions = GoogleSheets.Get<ChannelPointAction>();
                return channelPointActions;
            }
        }

        static List<EventActionMap> EventActionMaps =>
            eventActionMaps ?? (eventActionMaps = GoogleSheets.Get<EventActionMap>());

        async void SayIt(int playerId, string phrase)
        {
            string colorStr = ExtractColorStr(ref phrase);
            string offsetStr = ExtractOffsetStr(ref phrase);
            string quotedPhrase = phrase.Trim('"').Trim() + colorStr + offsetStr;
            await SafeHubInvokeAsync("SpeechBubble", $"{playerId} says: {quotedPhrase}").ConfigureAwait(false);
        }

        private async Task SafeHubInvokeAsync(string methodName, string parameters)
        {
            Console.WriteLine($"[DIAG] SafeHubInvokeAsync method={methodName} state={hubConnection.State}");
            if (hubConnection.State == HubConnectionState.Disconnected)
                await hubConnection.StartAsync().ConfigureAwait(false);

            try
            {
                await hubConnection.InvokeAsync(methodName, parameters).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                var msg = ex.Message ?? "(no message)";
                Console.Error.WriteLine($"[ERR] SafeHubInvokeAsync failed method={methodName} state={hubConnection.State} params=\"{parameters}\" err=\"{msg}\"");
                Console.Error.WriteLine(ex.ToString());
                log.Add(new ErrorEntry() { Exception = ex, Time = DateTime.Now });
            }
        }

        async Task ThinkItAsync(int playerId, string phrase)
        {
            string colorStr = ExtractColorStr(ref phrase);
            string offsetStr = ExtractOffsetStr(ref phrase);

            string quotedPhrase = phrase.Trim().TrimStart('(').TrimEnd(')') + colorStr + offsetStr;
            await SafeHubInvokeAsync("SpeechBubble", $"{playerId} thinks: {quotedPhrase}").ConfigureAwait(false);
        }

        private static string ExtractColorStr(ref string phrase)
        {
            string colorStr = string.Empty;
            int colorStrStart = phrase.IndexOf("(#");
            if (colorStrStart >= 0)
            {
                int colorStrStop = phrase.IndexOf(")", colorStrStart);
                if (colorStrStop > 0)
                {
                    colorStr = phrase.Substring(colorStrStart, colorStrStop - colorStrStart + 1);
                    string firstPart = phrase.Substring(0, colorStrStart);
                    string secondPart = string.Empty;
                    if (colorStrStop < phrase.Length - 1)
                        secondPart = phrase.Substring(colorStrStop + 1);
                    phrase = firstPart + secondPart;
                }
            }

            return colorStr;
        }

        private static string ExtractOffsetStr(ref string phrase)
        {
            string offsetStr = string.Empty;
            int offsetStrStart = phrase.IndexOf("(+");
            if (offsetStrStart < 0)
                offsetStrStart = phrase.IndexOf("(-");
            if (offsetStrStart >= 0)
            {
                int offsetStrStop = phrase.IndexOf(")", offsetStrStart);
                if (offsetStrStop > 0)
                {
                    offsetStr = phrase.Substring(offsetStrStart, offsetStrStop - offsetStrStart + 1);
                    string firstPart = phrase.Substring(0, offsetStrStart);
                    string secondPart = string.Empty;
                    if (offsetStrStop < phrase.Length - 1)
                        secondPart = phrase.Substring(offsetStrStop + 1);
                    phrase = firstPart + secondPart;
                }
            }

            return offsetStr;
        }

        async Task SayItOrThinkItAsync(ChatMessage chatMessage)
        {
            int userLevel = allViewers.GetUserLevel(chatMessage);
            Console.WriteLine($"[DIAG] SayOrThinkIt user={chatMessage.Username} level={userLevel} min={minUserLevelForSpeechBubbles}");
            if (userLevel < minUserLevelForSpeechBubbles)
            {
                Chat($"{chatMessage.Username}, this command is only available for level {minUserLevelForSpeechBubbles} users and up.");
                return;
            }

            string msg = chatMessage.Message.Trim();
            GetNameAndPhrase(msg, out string name, out string phrase);
            Console.WriteLine($"[DIAG] GetNameAndPhrase name=\"{name}\" phrase=\"{phrase}\"");

            await SayItOrThinkItAsync(name, phrase).ConfigureAwait(false);
        }

        private async Task SayItOrThinkItAsync(string name, string phrase, string colorOverride = null)
        {
            string colorStr = string.Empty;
            int playerId;
            if (name == "mark")
            {
                playerId = 2;
                colorStr = "(#3600d1)";
            }
            else if (name == "fred" || name == "richard")
            {
                playerId = 4;
                colorStr = "(#284974)";
            }
            else if (name == "campbell")
                playerId = 5;
            else if (name == "rory")
            {
                playerId = 5;
                colorStr = "(#880000)";
            }
            else
            {
                Console.WriteLine($"[DIAG] SayOrThinkIt(name,phrase) name=\"{name}\" not recognized, returning");
                return;
            }

            if (!string.IsNullOrWhiteSpace(colorOverride))
            {
                colorStr = $"({colorOverride})";
            }

            var censoredPhrase = CensorText(phrase);

            if (censoredPhrase.Contains("(#"))  // Already specifies a color?
                colorStr = string.Empty;

            if (phrase.StartsWith("("))
                await ThinkItAsync(playerId, censoredPhrase + colorStr).ConfigureAwait(false);
            else
                SayIt(playerId, censoredPhrase + colorStr);
        }

        internal static void GetNameAndPhrase(string msg, out string name, out string phrase)
        {
            name = null;
            phrase = null;

            // Use the FIRST separator (space or colon), matching what Vocalizes() accepts.
            // This ensures "!mark says: hello" (space after name) parses correctly instead
            // of finding the colon inside "says:" and extracting "mark says" as the name.
            int colonPos = msg.IndexOf(':');
            int spacePos = msg.IndexOf(' ');

            bool colonValid = colonPos >= 0 && colonPos < msg.Length - 1;
            bool spaceValid = spacePos >= 0 && spacePos < msg.Length - 1;

            if (!colonValid && !spaceValid)
                return;

            int breakPos;
            if (!colonValid)
                breakPos = spacePos;
            else if (!spaceValid)
                breakPos = colonPos;
            else
                breakPos = Math.Min(colonPos, spacePos);

            name = msg.Substring(1, breakPos - 1).Trim().ToLower();
            phrase = msg.Substring(breakPos + 1).Trim();

            // Strip optional "says: " / "thinks: " verb keywords so that
            // "!mark says: hello" and "!mark thinks: hello" work naturally.
            if (phrase.StartsWith("says: ", StringComparison.OrdinalIgnoreCase))
                phrase = phrase.Substring("says: ".Length).Trim();
            else if (phrase.StartsWith("thinks: ", StringComparison.OrdinalIgnoreCase))
                phrase = "(" + phrase.Substring("thinks: ".Length).Trim() + ")";
        }

        internal static bool Vocalizes(string lowerMessage, string prefix)
        {
            return lowerMessage.StartsWith(prefix) &&
                lowerMessage.Length > prefix.Length &&
                (lowerMessage[prefix.Length] == ':' || lowerMessage[prefix.Length] == ' ');
        }

        private async void TwitchClient_OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
        {
            var command = e.Command.CommandText;
            var lowerMessage = e.Command.ChatMessage.Message.ToLower();

            bool vocalizes = Vocalizes(lowerMessage, STR_MarkSaysOrThinks) || Vocalizes(lowerMessage, STR_FredSaysOrThinks) ||
                Vocalizes(lowerMessage, STR_CampbellSaysOrThinks) || Vocalizes(lowerMessage, STR_RorySaysOrThinks) ||
                Vocalizes(lowerMessage, STR_RichardSaysOrThinks);
            Console.WriteLine($"[DIAG] OnChatCommandReceived channel={e.Command.ChatMessage.Channel} user={e.Command.ChatMessage.Username} msg=\"{e.Command.ChatMessage.Message}\" vocalizes={vocalizes}");

            if (vocalizes)
            {
                await SayItOrThinkItAsync(e.Command.ChatMessage).ConfigureAwait(false);
                return;
            }

            if (BotCommands.Execute(e.Command.CommandText, e) > 0)
                return;

            if (e.Command.ChatMessage.DisplayName == "CodeRushed")
            {
                if (e.Command.CommandText == "Reset" && e.Command.ArgumentsAsString == "Fanfare")
                    ResetFanfares();

                if (e.Command.CommandText == "Fanfare")
                {
                    string fanfare = e.Command.ArgumentsAsString;
                    PlayFanfare(fanfare);
                }
            }

            var scene = GetScene(command);
            if (scene != null)
                ActivateSceneIfPermitted(scene, e.Command.ChatMessage.DisplayName, allViewers.GetUserLevel(e.Command.ChatMessage));
            //else
            //	Whisper(e.Command.ChatMessage.Username, GetWhatMessage() + " Command not recognized: " + e.Command.CommandText);
        }

        void ResetFanfares()
        {
            playedFanfares.Clear();
            foreach (FanfareDto fanfareDto in fanfares)
            {
                fanfareDto.LastPlayed = DateTime.MinValue;
            }
            WriteFanfareData(FileName.FanfareData_Source, fanfares);
        }

        string QuotedIfSpace(string chatShortcut)
        {
            if (chatShortcut.IndexOf(' ') >= 0)
                return $"\"{chatShortcut}\"";
            else
                return chatShortcut;
        }

        void ReloadCommand(OnChatCommandReceivedArgs obj)
        {
            scenes = null;
            restrictedScenes = null;
            channelPointActions = null;
            eventActionMaps = null;
            AllViewerListSettings.Instance.Invalidate();
            playedGreetingFromFred.Clear();
            playedGreetingFromRory.Clear();
        }

        void HandleQuestionCommand(OnChatCommandReceivedArgs obj)
        {
            int userLevel = allViewers.GetUserLevel(obj.Command.ChatMessage);

            List<string> accessibleScenes = Scenes.Where(m => m.Level <= userLevel).Select(x => x.SceneName).ToList();

            string sceneList = string.Join(", ", accessibleScenes);

            //Whisper(obj.Command.ChatMessage.Username, $"{obj.Command.ChatMessage.DisplayName}, your user level is: {userLevel}. You can say any of these: {sceneList}." );
            Chat($"{obj.Command.ChatMessage.DisplayName}, your user level is: {userLevel}. You can say any of these: {sceneList}.");
            Chat($"See https://github.com/MillerMark/MrAnnouncerBot/blob/master/README.md for more info.");
        }

        void HandleGitHubCommand(OnChatCommandReceivedArgs obj)
        {
            Chat($"Active Projects: ");
            Chat($"https://github.com/MillerMark/MrAnnouncerBot");
            Chat($"https://github.com/MillerMark/TimeLine");
        }

        void HandleDiscordCommand(OnChatCommandReceivedArgs obj)
        {
            Chat($"Join the CodeRush community on Discord: ");
            Chat($"https://discord.gg/B7WSz6Q");
        }

        void HandleDragonHCommand(OnChatCommandReceivedArgs obj)
        {
            Chat($"Live comedy Dungeons and Dragons with over-the-top special effects (built right here), every Wednesday and Sunday: ");
            Chat($"https://twitch.tv/DragonHumpers");
            Chat($"9p ET / 6p PT / 1a GMT / 11a AEST");
        }

        void HandleDragonHNewTimeCommand(OnChatCommandReceivedArgs obj)
        {
            Chat($"Special time for Dungeons and Dragons today/tonight: ");
            Chat($"https://twitch.tv/DragonHumpers");
            Chat($"11p ET / 8p PT / 3a GMT / 13a AEST");
        }

        void HandleVsCodeCommand(OnChatCommandReceivedArgs obj)
        {
            Chat($"Please vote here: ");
            Chat($"https://github.com/microsoft/vscode/issues/63791");
        }

        async void HandleBookCommand(OnChatCommandReceivedArgs obj)
        {
            ChatCommand chatCommand = obj.Command;
            string bookTitle = chatCommand.ArgumentsAsString;

            await SafeHubInvokeAsync("ShowBook", CensorText(bookTitle));
        }

        private string CensorText(string text)
        {
            CreateProfanityFilterIfNecessary();
            return profanityFilter.CensorString(text);
        }

        private void CreateProfanityFilterIfNecessary()
        {
            if (profanityFilter == null)
                profanityFilter = new ProfanityFilter.ProfanityFilter();
        }

        void HandleLevelUp(OnChatCommandReceivedArgs obj)
        {
            int userLevel = allViewers.GetUserLevel(obj.Command.ChatMessage);
            if (userLevel < AllViewers.ModeratorLevel)
                return;

            if (obj.Command.ArgumentsAsString != null)
            {
                string userName = obj.Command.ArgumentsAsString.TrimStart('@');
                if (allViewers.LevelChange(userName, 1) is Viewer viewer)
                {
                    int newUserLevel = viewer.GetLevel();
                    Chat($"{userName} is now at level {newUserLevel}.");
                }
                else
                    Chat($"{userName} not found.");
                var scene = GetScene("levelup");
                if (scene != null)
                    ActivateSceneIfPermitted(scene, "CodeRushed", AllViewers.ModeratorLevel);
            }
        }

        void CheckDocs()
        {
            if (ReadmeManager.NeedToGenerateNewReadme())
            {
                Console.WriteLine("Generating updated readme...");
                ReadmeManager.GenerateNewReadme();
            }
        }

        async void MarkCodeRushIssueStart(OnChatCommandReceivedArgs obj)
        {
            startTimeURL = await Twitch.GetActiveShowPointURL(mrAnnouncerGuyClientId, mrAnnouncerGuyAccessToken, STR_CodeRushedUserId);
            issueStartTime = DateTime.Now;
        }

        void HandleSuppressFanfareCommand(OnChatCommandReceivedArgs obj)
        {
            suppressingFanfare = true;
            Chat("Fanfare is suppressed.");
        }

        void InitializeKidzCodeBot()
        {
            kidzCodeClient = Twitch.CreateNewClient("cheese_minor", "cheese_minor", "DragonHumpersDmOAuthToken");
            HookTwitchClientEvents();
        }

        private void HookTwitchClientEvents()
        {
            if (kidzCodeClient == null)
                return;
            kidzCodeClient.OnDisconnected += KidzCodeClient_OnDisconnected;
            kidzCodeClient.OnChatCommandReceived += TwitchClient_OnChatCommandReceived;
        }

        private void KidzCodeClient_OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
        {
            throw new NotImplementedException();
        }

        private void KidzCodeClient_OnDisconnected(object sender, TwitchLib.Communication.Events.OnDisconnectedEventArgs e)
        {
            UnhookTwitchClientEvents();
            kidzCodeClient = null;
        }

        private void UnhookTwitchClientEvents()
        {
            if (kidzCodeClient == null)
                return;
            kidzCodeClient.OnDisconnected -= KidzCodeClient_OnDisconnected;
            kidzCodeClient.OnChatCommandReceived -= TwitchClient_OnChatCommandReceived;
        }

        void RegisterSpreadsheets()
        {
            GoogleSheets.RegisterDocumentID("Mr. Announcer Guy", "1s-j-4EF3KbI8ZH0nSj4G4a1ApNFPz_W5DK9A9JTyb3g");
        }
    }
}
