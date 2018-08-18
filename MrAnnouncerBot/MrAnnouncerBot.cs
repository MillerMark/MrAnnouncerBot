using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using MrAnnouncerBot.Properties;
using OBSWebsocketDotNet;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace MrAnnouncerBot
{
    public class MrAnnouncerBot
    {
        private const string STR_ChannelName = "CodeRushed";
        private const string STR_WebSocketPort = "ws://127.0.0.1:4444";
        private const string STR_SceneDataFile = "Scene Data\\Mr. Announcer Guy - Scenes.csv";
        private const string STR_TwitchUserName = "MrAnnouncerGuy";

        private static List<SceneDto> scenes = new List<SceneDto>();
        private string activeSceneName;
        private bool logging;
        private readonly OBSWebsocket obsWebsocket = new OBSWebsocket();

        public MrAnnouncerBot()
        {
            GetSceneData();
        }

        public TwitchClient TwitchClient { get; set; }

        private static void GetSceneData()
        {
            if (!File.Exists(STR_SceneDataFile)) return;
            var textReader = File.OpenText(STR_SceneDataFile);
            var csvReader = new CsvReader(textReader);
            scenes = csvReader.GetRecords<SceneDto>().ToList();
        }

        private void TwitchClientLog(object sender, OnLogArgs e)
        {
            if (logging)
                Console.WriteLine(e.Data);
        }

        private void GetCredentials()
        {
            InitializeObsWebSocket();

            var oAuthToken = Settings.Default.TwitchBotOAuthToken;
            var connectionCredentials = new ConnectionCredentials(STR_TwitchUserName, oAuthToken);
            TwitchClient = new TwitchClient();
            TwitchClient.Initialize(connectionCredentials, STR_ChannelName);

            TwitchClient.OnLog += TwitchClientLog;
            TwitchClient.OnConnectionError += TwitchClient_OnConnectionError;
            TwitchClient.OnJoinedChannel += TwitchClient_OnJoinedChannel;
            TwitchClient.OnChatCommandReceived += TwitchClient_OnChatCommandReceived;
        }

        private void TwitchClient_OnConnectionError(object sender, OnConnectionErrorArgs e)
        {
            Console.WriteLine(e.Error.Message);
        }

        private void ConnectToObs()
        {
            if (obsWebsocket.IsConnected) return;
            try
            {
                obsWebsocket.Connect(STR_WebSocketPort, Settings.Default.ObsPassword);
            }
            catch (AuthFailureException)
            {
                Console.WriteLine("Authentication failed.");
            }
            catch (ErrorResponseException ex)
            {
                Console.WriteLine($"Connect failed. {ex.Message}");
            }
        }

        private void InitializeObsWebSocket()
        {
            obsWebsocket.Connected += ObsWebsocket_Connected;
            obsWebsocket.Disconnected += ObsWebsocket_Disconnected;

            obsWebsocket.SceneChanged += ObsWebsocket_SceneChanged;
            obsWebsocket.ProfileChanged += ObsWebsocket_ProfileChanged;

            //obsWebsocket.SceneCollectionChanged += ObsWebsocket_SceneCollectionChanged;
            //obsWebsocket.TransitionChanged += ObsWebsocket_TransitionChanged;
            //obsWebsocket.TransitionDurationChanged += ObsWebsocket_TransitionDurationChanged;
            //obsWebsocket.StreamingStateChanged += ObsWebsocket_StreamingStateChanged;
            //obsWebsocket.RecordingStateChanged += ObsWebsocket_RecordingStateChanged;

            obsWebsocket.StreamStatus += ObsWebsocket_StreamStatus;

            ConnectToObs();
        }

        private void ObsWebsocket_StreamStatus(OBSWebsocket sender, StreamStatus status)
        {
            Console.WriteLine("ObsWebsocket_StreamStatus");
        }

        private void ObsWebsocket_ProfileChanged(object sender, EventArgs e)
        {
            Console.WriteLine("ObsWebsocket_ProfileChanged");
        }

        private void ObsWebsocket_SceneChanged(OBSWebsocket sender, string newSceneName)
        {
            activeSceneName = newSceneName;
            Console.WriteLine($"Active Scene: {activeSceneName}");
        }

        private void ObsWebsocket_Disconnected(object sender, EventArgs e)
        {
            Console.WriteLine("ObsWebsocket_Disconnected");
        }

        private void ObsWebsocket_Connected(object sender, EventArgs e)
        {
            Console.WriteLine("ObsWebsocket_Connected");
        }

        private void TwitchClient_OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
        {
            var command = e.Command.CommandText;
            if (command == "zork")
            {
                Chat($"{e.Command.ChatMessage.DisplayName} is standing in an open field west of a white house, with a boarded front door.");
                return;
            }

            var scene = GetScene(command);
            if (scene != null)
                InvokeScene(scene);
        }

        private void TwitchClient_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            Chat("Mr. Announcer Bot is in da House!");
        }

        private void Chat(string msg)
        {
            TwitchClient.SendMessage(STR_ChannelName, msg);
        }

        public void Run()
        {
            GetCredentials();
            TwitchClient.Connect();
        }

        public void Disconnect()
        {
            TwitchClient.Disconnect();
            obsWebsocket.Disconnect();
        }

        private SceneDto GetScene(string command)
        {
            var foundScene = scenes.FirstOrDefault(m =>
                string.Compare(m.ChatShortcut, "!" + command, StringComparison.OrdinalIgnoreCase) == 0);
            if (foundScene != null)
                return foundScene;
            return scenes.FirstOrDefault(m =>
                string.Compare(m.AlternateShortcut, "!" + command, StringComparison.OrdinalIgnoreCase) == 0);
        }

        private void InvokeScene(SceneDto scene)
        {
            obsWebsocket.SetCurrentScene(scene.SceneName);
        }
    }
}