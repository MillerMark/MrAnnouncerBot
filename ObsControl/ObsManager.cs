//#define profiling
using System;
using System.Linq;
using BotCore;
using Newtonsoft.Json.Linq;
using OBSWebsocketDotNet;
using OBSWebsocketDotNet.Types;
using System.Drawing;
using DndCore;
using CommonCore;

namespace ObsControl
{
	public static class ObsManager
	{
		public static event EventHandler<string> SceneChanged;
		public static event EventHandler<OutputState> StateChanged;
		private static readonly OBSWebsocket obsWebsocket = new OBSWebsocket();

		static ObsManager()
		{
			Connect();
		}

		public static void SetFilterVisibility(string sourceName, string filterName, bool filterEnabled)
		{
			string[] parts = sourceName.Split(';');
			foreach (string part in parts)
				obsWebsocket.SetSourceFilterVisibility(part.Trim(), filterName, filterEnabled);
		}

		static GetSceneListInfo sceneList;

		public static SceneItem GetSceneItem(string sceneName, string itemName)
		{
			if (sceneList == null)
			{
				if (!obsWebsocket.IsConnected)
					return null;
				sceneList = obsWebsocket.GetSceneList();
			}

			OBSScene scene = sceneList?.Scenes?.FirstOrDefault(x => x.Name == sceneName);
			return scene?.Items?.FirstOrDefault(x => x.SourceName == itemName);
		}

		public static void OnStateChanged(object sender, OutputState state)
		{
			StateChanged?.Invoke(sender, state);
		}

		public static	void OnSceneChanged(object sender, string sceneName)
		{
			SceneChanged?.Invoke(sender, sceneName);
		}

		public static void SetSourceVisibility(string sceneName, string sourceName, bool visible)
		{
			try
			{
				obsWebsocket.SetSourceRender(sourceName, visible, sceneName);
			}
			catch //(Exception ex)
			{

			}
		}

		public static void Connect()
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

		//
		private static void ObsWebsocket_StreamingStateChanged(OBSWebsocket sender, OutputState type)
		{
			OnStateChanged(sender, type);
		}

		private static void ObsWebsocket_SceneChanged(OBSWebsocket sender, string newSceneName)
		{
			OnSceneChanged(sender, newSceneName);
		}

		public static void SizeAndPositionItem(BaseLiveFeedAnimator e, double scale, double opacity = 1, double rotation = 0, bool flipped = false)
		{
			if (e.HasLastCamera())
			{
				// TODO: Hide that sourceItem...
				SetSourceVisibility(e.LastSceneName, e.LastItemName, false);
				e.ClearLastCamera();
			}

			double screenAnchorLeft = e.ScreenAnchorLeft;
			double screenAnchorTop = e.ScreenAnchorTop;

			double newLeft = screenAnchorLeft - e.VideoAnchorHorizontal * Math.Abs(e.VideoWidth) * scale;
			double newTop = screenAnchorTop - e.VideoAnchorVertical * e.VideoHeight * scale;

			try
			{
				SceneItemProperties sceneItemProperties = obsWebsocket.GetSceneItemProperties(e.ItemName, e.SceneName);
				sceneItemProperties.Visible = opacity > 0;

				if (sceneItemProperties.Visible)
				{
					// TODO: Consider optimizing this to only change when the new value is different from the last one set. Confidence should be high (e.g., time between sets should be short, and everything else should match).
					const string ImageMaskFilter = "Image Mask/Blend";
					FilterSettings sourceFilterInfo = obsWebsocket.GetSourceFilterInfo(e.ItemName, ImageMaskFilter);
					JObject settings = sourceFilterInfo.Settings;
					int newOpacity = (int)(opacity * 100);
					ImageMask imageMask = settings.ToObject<ImageMask>();
					if (imageMask.opacity != newOpacity)
					{
						imageMask.opacity = newOpacity;
						obsWebsocket.SetSourceFilterSettings(e.ItemName, ImageMaskFilter, JObject.FromObject(imageMask));
					}
				}
				//if (rotation != sceneItemProperties.Rotation)
				//{
				sceneItemProperties.Rotation = rotation;

				// This will rotate around the top left.
				// ![](342CE4E3D8508B3F1D4B944701C25E97.png)

				// But we also need to rotate the upper left anchor by that amount..
				// ![](50D76BD4D4A4D1EF91B34011CF9EA1D7.png)

				// screenAnchorLeft and screenAnchorTop are the red point.
				// newLeft and newTop are the original yellow anchor point in the upper left.

				Point2d upperLeft = new Point2d(newLeft, newTop);
				Point2d centerPoint = new Point2d(screenAnchorLeft, screenAnchorTop);

				Point2d newUpperLeft = Math2D.RotatePoint(upperLeft, centerPoint, rotation);
				newLeft = newUpperLeft.X;
				newTop = newUpperLeft.Y;
				
				sceneItemProperties.Bounds = new SceneItemBoundsInfo()
				{
					Height = e.VideoHeight * scale,
					Width = Math.Abs(e.VideoWidth) * scale,
					Alignnment = sceneItemProperties.Bounds.Alignnment,
					Type = sceneItemProperties.Bounds.Type
				};

				sceneItemProperties.Position = new SceneItemPositionInfo()
				{
					X = newLeft,
					Y = newTop,
					Alignment = sceneItemProperties.Position.Alignment
				};

				obsWebsocket.SetSceneItemProperties(sceneItemProperties, e.SceneName);

				float flipMultiplier = 1;
				if (e.VideoWidth < 0)
					flipMultiplier = -1;

				if (flipped)
				{
					flipMultiplier *= -1;
				}

				// TODO: See if commenting this out still animates the video sources correctly. And it looks like it still works!!!
				//obsWebsocket.SetSceneItemTransform(e.ItemName, (float)rotation, flipMultiplier * (float)scale, (float)scale, e.SceneName);
				//sceneItemProperties;
				//SceneItemProperties  = obsWebsocket.GetSceneItemProperties(e.ItemName, e.SceneName);
				//WTF???
				//obsWebsocket.SetSceneItemProperties(props)

			}
			catch //(Exception ex)
			{
				
			}
		}

		public static void SetCurrentScene(string sceneName)
		{
			obsWebsocket.SetCurrentScene(sceneName);
		}

		public static OBSScene GetCurrentScene()
		{
			return obsWebsocket.GetCurrentScene();
		}

		// TODO: Reorder parameters..
		public static void SetSourceRender(string sourceName, bool visible, string sceneName)
		{
			obsWebsocket.SetSourceRender(sourceName, visible, sceneName);
		}

		public static SceneItemProperties GetSceneItemProperties(string itemName, string sceneName)
		{
			return obsWebsocket.GetSceneItemProperties(itemName, sceneName);
		}

		public static bool IsConnected => obsWebsocket.IsConnected;
	}
}
