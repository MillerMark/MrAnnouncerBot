//#define profiling
using DndCore;
using ObsControl;
using OBSWebsocketDotNet.Types;
using System;
using System.Linq;
using System.Windows.Threading;

namespace DHDM
{
	public class DndObsManager : IObsManager
	{
		public const string STR_PlayerScene = "Players";
		public string lastScenePlayed;
		DispatcherTimer sceneReturnTimer;
		PlateManager plateManager;
		WeatherManager weatherManager;
		DispatcherTimer playSceneTimer;
		
		// TODO: Consider stacking scene play requests.
		string nextSceneToPlay;
		int nextReturnMs;

		public IDungeonMasterApp DungeonMasterApp { get; set; }

		public void PlayScene(string sceneName, int returnMs = -1)
		{
			lastScenePlayed = sceneName;
			string dmMessage = $"Playing scene: {sceneName}";

			try
			{
				if (ObsManager.IsConnected)
				{
					if (returnMs > 0)
						foundationalScene = GetFoundationalScene();

					ObsManager.SetCurrentScene(sceneName);

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

		public static void SetSourceVisibility(string sourceName, string sceneName, bool visible, double delaySeconds)
		{
			SetSourceVisibility(new SetObsSourceVisibilityEventArgs());
		}

	public static void SetSourceVisibility(SetObsSourceVisibilityEventArgs ea)
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

			ObsManager.SetSourceVisibility(ea.SceneName, ea.SourceName, ea.Visible);
		}

		private static void DelayShowSourceTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			if (sender is SourceVisibilityTimer timer)
			{
				timer.Stop();
				SetSourceVisibility(timer.ea);
			}
		}

		public void PlaySceneAfter(string sceneName, int delayMs, int returnMs)
		{
			playSceneTimer.Stop();
			nextSceneToPlay = sceneName;
			nextReturnMs = returnMs;
			playSceneTimer.Interval = TimeSpan.FromMilliseconds(delayMs);
			playSceneTimer.Start();
		}

		string GetFoundationalScene()
		{
			try
			{
				OBSScene currentScene = ObsManager.GetCurrentScene();
				if (IsFoundationScene(currentScene.Name))
					return currentScene.Name;
				return DndObsManager.STR_PlayerScene;
			}
			catch (Exception ex)
			{
				return DndObsManager.STR_PlayerScene;
			}
		}

		string foundationalScene;
		
		bool IsFoundationScene(string name)
		{
			return name.SameLetters(STR_PlayerScene) || name.SameLetters("DH.Tunnels") || name.SameLetters("DH.Nebula") ||
				name.ToLower().StartsWith("DH.TheVoid.".ToLower());
		}

		public void Initialize(DndGame game, IDungeonMasterApp dungeonMasterApp)
		{
			DungeonMasterApp = dungeonMasterApp;
			plateManager = new PlateManager();
			weatherManager = new WeatherManager(game);
			weatherManager.Load();

			sceneReturnTimer = new DispatcherTimer(DispatcherPriority.Send);
			sceneReturnTimer.Tick += new EventHandler(ReturnToSceneHandler);

			playSceneTimer = new DispatcherTimer(DispatcherPriority.Send);
			playSceneTimer.Tick += new EventHandler(PlaySceneHandler);
		}

		void PlaySceneHandler(object sender, EventArgs e)
		{
			playSceneTimer.Stop();
			if (nextSceneToPlay == null)
				return;
			PlayScene(nextSceneToPlay, nextReturnMs);
			nextSceneToPlay = null;
		}

		// Keep this getPlayerX function in sync with code in DragonGame.ts:
		double playerVideoLeftMargin = 10;
		double playerVideoRightMargin = 1384;

		int numberOfPlayers = 4;
		

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

		VideoFeed[] GetVideoFeeds(string sceneName, string itemName, double videoAnchorHorizontal, double videoAnchorVertical, double videoWidth, double videoHeight)
		{
			VideoFeed[] result = new VideoFeed[4];
			result[0] = new VideoFeed() { sceneName = sceneName, sourceName = itemName,
				videoAnchorHorizontal = videoAnchorHorizontal,
				videoAnchorVertical = videoAnchorVertical,
				videoWidth = videoWidth,
				videoHeight = videoHeight
			};
			return result;
		}
		void StartLiveFeedAnimation(string itemName, string sceneName, double playerX, double videoAnchorHorizontal, double videoAnchorVertical, double videoWidth, double videoHeight, double targetScale, double timeMs)
		{
			SceneItem sceneItem = ObsManager.GetSceneItem(sceneName, itemName);

			SceneItemProperties sceneItemProperties = ObsManager.GetSceneItemProperties(itemName, sceneName);
			double startScale = sceneItemProperties.Bounds.Height / videoHeight;
			VideoFeed[] videoFeeds = GetVideoFeeds(sceneName, itemName, videoAnchorHorizontal, videoAnchorVertical, videoWidth, videoHeight);
			LiveFeedScaler liveFeedAnimation = new LiveFeedScaler(videoFeeds, playerX, startScale, targetScale, timeMs);
			if (!sceneItem.Render)
				ObsManager.SizeAndPositionItem(liveFeedAnimation, (float)liveFeedAnimation.TargetScale);
			else
				liveFeedAnimation.Render += LiveFeedAnimation_Render;
		}

		private void LiveFeedAnimation_Render(object sender, LiveFeedScaler e)
		{
			float scale = (float)e.GetTargetScale();
			ObsManager.SizeAndPositionItem(e, scale);
		}

		public void AnimateLiveFeed(string sourceName, string sceneName, double videoAnchorHorizontal, double videoAnchorVertical, double videoWidth, double videoHeight, double targetScale, double timeMs, int playerIndex)
		{
			string[] parts = sourceName.Split(';');
			foreach (string part in parts)
				StartLiveFeedAnimation(part.Trim(), sceneName, getPlayerX(playerIndex), videoAnchorHorizontal, videoAnchorVertical, videoWidth, videoHeight, targetScale, timeMs);
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
	}
}
