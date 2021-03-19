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
	public class SourceVisibilityTimer: System.Timers.Timer
	{
		
		public SourceVisibilityTimer()
		{
			
		}
		public SetObsSourceVisibilityEventArgs ea { get; set; }
	}
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

		public SceneItem GetSceneItem(string sceneName, string itemName)
		{
			if (sceneList == null)
				sceneList = obsWebsocket.GetSceneList();
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

		public void SetSourceVisibility(string sourceName, string sceneName, bool visible, double delaySeconds = 0)
		{
			SetSourceVisibility(new SetObsSourceVisibilityEventArgs(sourceName, sceneName, visible, delaySeconds));
		}

		public void SetSourceVisibility(SetObsSourceVisibilityEventArgs ea)
		{
			if (ea.DelaySeconds > 0)
			{
				SourceVisibilityTimer delayShowSourceTimer = new SourceVisibilityTimer();
				delayShowSourceTimer.Interval = ea.DelaySeconds * 1000;
				ea.DelaySeconds = 0;  // Prevents us from setting multiple timers for a single source switch.
				delayShowSourceTimer.Elapsed += DelayShowSourceTimer_Elapsed;
				delayShowSourceTimer.ea = ea;
				delayShowSourceTimer.Start();
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

		private void DelayShowSourceTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			if (sender is SourceVisibilityTimer timer)
			{
				timer.Stop();
				SetSourceVisibility(timer.ea);
			}
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

		// Keep this getPlayerX function in sync with code in DragonGame.ts:
		double playerVideoLeftMargin = 10;
		double playerVideoRightMargin = 1384;

		int numberOfPlayers = 4;
		GetSceneListInfo sceneList;

		public double getPlayerX(int playerIndex)
		{
			double distanceForPlayerVideos = playerVideoRightMargin - playerVideoLeftMargin;
			double distanceBetweenPlayers = distanceForPlayerVideos / numberOfPlayers;
			double halfDistanceBetweenPlayers = distanceBetweenPlayers / 2;
			double horizontalNudge = 0;
			if (playerIndex == 0)  // Fred.
				horizontalNudge = 25;
			return playerIndex * distanceBetweenPlayers + halfDistanceBetweenPlayers + horizontalNudge;
		}

		void StartLiveFeedAnimation(string itemName, string sceneName, double playerX, double videoAnchorHorizontal, double videoAnchorVertical, double videoWidth, double videoHeight, double targetScale, double timeMs)
		{
			SceneItem sceneItem = GetSceneItem(sceneName, itemName);

			SceneItemProperties sceneItemProperties = obsWebsocket.GetSceneItemProperties(itemName, sceneName);
			double startScale;
			//if (sceneItem.Height == 0)  // Height for video feeds seems to be zero.
				startScale = sceneItemProperties.Bounds.Height / videoHeight;
			//else
			//	startScale = sceneItemProperties.Bounds.Height / sceneItem.Height;

			LiveFeedAnimation liveFeedAnimation = new LiveFeedAnimation(itemName, sceneName, playerX, videoAnchorHorizontal, videoAnchorVertical, videoWidth, videoHeight, startScale, targetScale, timeMs);
			if (!sceneItem.Render)
				SizeItem(liveFeedAnimation, (float)liveFeedAnimation.TargetScale);
			else
				liveFeedAnimation.Render += LiveFeedAnimation_Render;
		}

		private void LiveFeedAnimation_Render(object sender, LiveFeedAnimation e)
		{
			float scale = (float)e.GetTargetScale();
			SizeItem(e, scale);
		}

		private void SizeItem(LiveFeedAnimation e, float scale)
		{
			double anchorLeft = e.PlayerX;
			double anchorTop = 1080;

			double newLeft = anchorLeft - e.VideoAnchorHorizontal * Math.Abs(e.VideoWidth) * scale;
			double newTop = anchorTop - e.VideoAnchorVertical * e.VideoHeight * scale;
			//Console.WriteLine($"scale = {scale}");

			SceneItemProperties sceneItemProperties = obsWebsocket.GetSceneItemProperties(e.ItemName, e.SceneName);
			sceneItemProperties.Bounds = new SceneItemBoundsInfo()
			{
				Height = e.VideoHeight * scale,
				Width = Math.Abs(e.VideoWidth) * scale,
				Alignnment = sceneItemProperties.Bounds.Alignnment,
				Type = sceneItemProperties.Bounds.Type
			};

			sceneItemProperties.Position = new SceneItemPositionInfo() { X = newLeft, Y = newTop, 
				Alignment = sceneItemProperties.Position.Alignment };

			obsWebsocket.SetSceneItemProperties(sceneItemProperties, e.SceneName);
		}

		public void AnimateLiveFeed(string sourceName, string sceneName, double videoAnchorHorizontal, double videoAnchorVertical, double videoWidth, double videoHeight, double targetScale, double timeMs, int playerIndex)
		{
			string[] parts = sourceName.Split(';');
			foreach (string part in parts)
				StartLiveFeedAnimation(part.Trim(), sceneName, getPlayerX(playerIndex), videoAnchorHorizontal, videoAnchorVertical, videoWidth, videoHeight, targetScale, timeMs);
		}

		public IDungeonMasterApp DungeonMasterApp { get; set; }
	}
}
