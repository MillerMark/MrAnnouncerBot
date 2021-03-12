//#define profiling
using System;
using System.Linq;
using System.Windows.Threading;
using DndCore;
using BotCore;
using OBSWebsocketDotNet;
using OBSWebsocketDotNet.Types;

namespace DHDM
{
	public class ObsManager : IObsManager
	{
		public static event EventHandler<string> SceneChanged;
		public static event EventHandler<OutputState> StateChanged;
		private readonly OBSWebsocket obsWebsocket = new OBSWebsocket();
		DispatcherTimer sceneReturnTimer;
		PlateManager plateManager;
		WeatherManager weatherManager;
		DispatcherTimer playSceneTimer;
		public string lastScenePlayed;
		// TODO: Consider stacking scene play requests.
		string nextSceneToPlay;
		int nextReturnMs;

		public const string STR_PlayerScene = "Players";
		public ObsManager()
		{

		}

		public void SetFilterVisibility(string sourceName, string filterName, bool filterEnabled)
		{
			string[] parts = sourceName.Split(';');
			foreach (string part in parts)
				obsWebsocket.SetSourceFilterVisibility(part.Trim(), filterName, filterEnabled);
		}

		// TODO: Use GetSceneItem to smoothly scale video feeds up and down in 
		public SceneItem GetSceneItem(string sceneName, string itemName)
		{
			GetSceneListInfo sceneList = obsWebsocket.GetSceneList();
			OBSScene scene = sceneList?.Scenes?.FirstOrDefault(x => x.Name == sceneName);
			return scene?.Items?.FirstOrDefault(x => x.SourceName == itemName);
		}

		public static void OnStateChanged(object sender, OutputState state)
		{
			StateChanged?.Invoke(sender, state);
		}

		public static void OnSceneChanged(object sender, string sceneName)
		{
			SceneChanged?.Invoke(sender, sceneName);
		}

		public void Initialize(DndGame game, IDungeonMasterApp dungeonMasterApp)
		{
			DungeonMasterApp = dungeonMasterApp;
			plateManager = new PlateManager(obsWebsocket);
			weatherManager = new WeatherManager(obsWebsocket, game);
			weatherManager.Load();

			sceneReturnTimer = new DispatcherTimer(DispatcherPriority.Send);
			sceneReturnTimer.Tick += new EventHandler(ReturnToSceneHandler);

			playSceneTimer = new DispatcherTimer(DispatcherPriority.Send);
			playSceneTimer.Tick += new EventHandler(PlaySceneHandler);
		}

		public void PlayScene(string sceneName, int returnMs = -1)
		{
			lastScenePlayed = sceneName;
			string dmMessage = $"Playing scene: {sceneName}";

			try
			{
				if (obsWebsocket.IsConnected)
				{
					if (returnMs > 0)
						foundationalScene = GetFoundationalScene();

					obsWebsocket.SetCurrentScene(sceneName);

					if (returnMs > 0)
					{
						sceneReturnTimer.Interval = TimeSpan.FromMilliseconds(returnMs);
						sceneReturnTimer.Start();
					}
				}
			}
			catch (Exception ex)
			{
				dmMessage = $"{Icons.WarningSign} Unable to play {sceneName}: {ex.Message}";
			}
			DungeonMasterApp.TellDungeonMaster(dmMessage);
		}

		bool IsFoundationScene(string name)
		{
			return name.SameLetters(STR_PlayerScene) || name.SameLetters("DH.Tunnels") || name.SameLetters("DH.Nebula") ||
				name.ToLower().StartsWith("DH.TheVoid.".ToLower());
		}

		string GetFoundationalScene()
		{
			try
			{
				OBSScene currentScene = obsWebsocket.GetCurrentScene();
				if (IsFoundationScene(currentScene.Name))
					return currentScene.Name;
				return ObsManager.STR_PlayerScene;
			}
			catch (Exception ex)
			{
				return ObsManager.STR_PlayerScene;
			}
		}

		string foundationalScene;

		void PlaySceneHandler(object sender, EventArgs e)
		{
			playSceneTimer.Stop();
			if (nextSceneToPlay == null)
				return;
			PlayScene(nextSceneToPlay, nextReturnMs);
			nextSceneToPlay = null;
		}

		void ReturnToSceneHandler(object sender, EventArgs e)
		{
			sceneReturnTimer.Stop();
			if (foundationalScene == null)
				return;
			PlayScene(foundationalScene);
			foundationalScene = null;
		}
		public void ShowPlateBackground(string sourceName)
		{
			plateManager.ShowBackground(sourceName);
		}
		public void ShowPlateForeground(string sourceName)
		{
			plateManager.ShowForeground(sourceName);
		}
		public void ShowWeather(string weatherKeyword)
		{
			weatherManager.ShowWeather(weatherKeyword);
		}
		public void SetSourceVisibility(SetObsSourceVisibilityEventArgs ea)
		{
			if (ea.DelaySeconds > 0)
			{
				DispatcherTimer delayFloatTextTimer = new DispatcherTimer(DispatcherPriority.Send);
				delayFloatTextTimer.Interval = TimeSpan.FromSeconds(ea.DelaySeconds);
				ea.DelaySeconds = 0;
				delayFloatTextTimer.Tick += new EventHandler(SetObsSourceVisibiltyNow);
				delayFloatTextTimer.Tag = ea;
				delayFloatTextTimer.Start();
				return;
			}

			try
			{
				obsWebsocket.SetSourceRender(ea.SourceName, ea.Visible, ea.SceneName);
			}
			catch (Exception ex)
			{

			}
		}

		void SetObsSourceVisibiltyNow(object sender, EventArgs e)
		{
			if (!(sender is DispatcherTimer dispatcherTimer))
				return;

			dispatcherTimer.Stop();

			if (!(dispatcherTimer.Tag is SetObsSourceVisibilityEventArgs ea))
				return;
			SetSourceVisibility(ea);
		}
		public void Connect()
		{
			if (obsWebsocket.IsConnected)
				return;
			try
			{
				obsWebsocket.Connect(ObsHelper.WebSocketPort, Twitch.Configuration["Secrets:ObsPassword"]);  // Settings.Default.ObsPassword);
				obsWebsocket.SceneChanged += ObsWebsocket_SceneChanged;
				obsWebsocket.StreamingStateChanged += ObsWebsocket_StreamingStateChanged;
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

		private void ObsWebsocket_StreamingStateChanged(OBSWebsocket sender, OutputState type)
		{
			OnStateChanged(sender, type);
		}

		private void ObsWebsocket_SceneChanged(OBSWebsocket sender, string newSceneName)
		{
			OnSceneChanged(sender, newSceneName);
		}

		public void PlaySceneAfter(string sceneName, int delayMs, int returnMs)
		{
			playSceneTimer.Stop();
			nextSceneToPlay = sceneName;
			nextReturnMs = returnMs;
			playSceneTimer.Interval = TimeSpan.FromMilliseconds(delayMs);
			playSceneTimer.Start();
		}
		public IDungeonMasterApp DungeonMasterApp { get; set; }
	}
}
