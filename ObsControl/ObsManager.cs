//#define profiling
using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using BotCore;
using DndCore;
using Newtonsoft.Json.Linq;
using CommonCore;
using OBSWebsocketDotNet;
using OBSWebsocketDotNet.Types;

namespace ObsControl {
    public static class ObsManager {
        public static event EventHandler<string> SceneChanged;
        public static event EventHandler<SceneItemDetails> SceneItemEnabled;
        public static event EventHandler<SceneItemDetails> SceneItemDisabled;
        public static event EventHandler<OutputState> StateChanged;
        public static event EventHandler StreamStarted;
        public static event EventHandler StreamEnded;
        public static event EventHandler RecordingStarted;
        public static event EventHandler RecordingEnded;
        private static readonly OBSWebsocket obsWebsocket = new OBSWebsocket();

        static ObsManager() {
            Console.WriteLine($"Connecting from ObsManager...");
            Connect();
        }

        public static void SetFilterVisibility(string sourceName, string filterName, bool filterEnabled) {
            string[] parts = sourceName.Split(';');
            foreach (string part in parts)
                obsWebsocket.SetSourceFilterEnabled(part.Trim(), filterName, filterEnabled);
        }

        //static GetSceneListInfo sceneList;

        private static void ObsWebsocket_StreamStateChanged(object sender, OBSWebsocketDotNet.Types.Events.StreamStateChangedEventArgs e) {
            StateChanged?.Invoke(sender, e.OutputState.State);
            if (e.OutputState.State == OutputState.OBS_WEBSOCKET_OUTPUT_STARTED)
                StreamStarted?.Invoke(sender, EventArgs.Empty);
            else if (e.OutputState.State == OutputState.OBS_WEBSOCKET_OUTPUT_STOPPED)
                StreamEnded?.Invoke(sender, EventArgs.Empty);
        }

        private static void ObsWebsocket_RecordStateChanged(object sender, OBSWebsocketDotNet.Types.Events.RecordStateChangedEventArgs e) {
            if (e.OutputState.State == OutputState.OBS_WEBSOCKET_OUTPUT_STARTED)
                RecordingStarted?.Invoke(sender, EventArgs.Empty);
            else if (e.OutputState.State == OutputState.OBS_WEBSOCKET_OUTPUT_STOPPED)
                RecordingEnded?.Invoke(sender, EventArgs.Empty);
        }

        public static void OnSceneChanged(object sender, string sceneName) {
            SceneChanged?.Invoke(sender, sceneName);
        }

        static void OnSceneItemChanged(object sender, string sceneName, int sceneItemId, bool sceneItemEnabled) {
            try {
                List<SceneItemDetails> sceneItemList = obsWebsocket.GetSceneItemList(sceneName);
                SceneItemDetails sceneItem = sceneItemList.FirstOrDefault(x => x.ItemId == sceneItemId);
                if (sceneItem != null) {
                    if (sceneItemEnabled)
                        SceneItemEnabled?.Invoke(sender, sceneItem);
                    else
                        SceneItemDisabled?.Invoke(sender, sceneItem);
                }
            }
            catch (Exception ex) {
                //var groupList = obsWebsocket.GetGroupSceneItemList(sceneName);
            }
        }

        public static void Connect() {
            if (obsWebsocket.IsConnected)
                return;
            try {
                obsWebsocket.ConnectAsync(ObsHelper.WebSocketPort, Twitch.Configuration["Secrets:ObsPassword"]);  // Settings.Default.ObsPassword);
                obsWebsocket.CurrentProgramSceneChanged += ObsWebsocket_CurrentProgramSceneChanged;
                obsWebsocket.StreamStateChanged += ObsWebsocket_StreamStateChanged;
                obsWebsocket.RecordStateChanged += ObsWebsocket_RecordStateChanged;
                obsWebsocket.Disconnected += ObsWebsocket_Disconnected;
                obsWebsocket.SceneItemEnableStateChanged += ObsWebsocket_SceneItemEnableStateChanged;
            }
            catch (AuthFailureException) {
                Console.WriteLine("Authentication failed.");
            }
            catch (ErrorResponseException ex) {
                Console.WriteLine($"Connect failed. {ex.Message}");
            }
        }

        private static void ObsWebsocket_SceneItemEnableStateChanged(object sender, OBSWebsocketDotNet.Types.Events.SceneItemEnableStateChangedEventArgs e) {
            OnSceneItemChanged(sender, e.SceneName, e.SceneItemId, e.SceneItemEnabled);
        }

        private static void ObsWebsocket_Disconnected(object sender, OBSWebsocketDotNet.Communication.ObsDisconnectionInfo e) {
            //System.Diagnostics.Debugger.Break();
        }

        private static void ObsWebsocket_CurrentProgramSceneChanged(object sender, OBSWebsocketDotNet.Types.Events.ProgramSceneChangedEventArgs e) {
            OnSceneChanged(sender, e.SceneName);
        }

        public static void SizeAndPositionItem(BaseLiveFeedAnimator e, double scale, double opacity = 1, double rotation = 0, bool flipped = false) {
            if (e.HasLastCamera()) {
                SetSceneItemEnabled(e.LastSceneName, e.LastItemName, false);
                e.ClearLastCamera();
            }

            double screenAnchorLeft = e.ScreenAnchorLeft;
            double screenAnchorTop = e.ScreenAnchorTop;

            double newLeft = screenAnchorLeft - e.VideoAnchorHorizontal * Math.Abs(e.VideoWidth) * scale;
            double newTop = screenAnchorTop - e.VideoAnchorVertical * e.VideoHeight * scale;

            try {
                SceneItemDetails sceneItemDetails = GetSceneItemDetails(e.SceneName, e.ItemName);
                SceneItemTransformInfo sceneItemTransformInfo = GetSceneItemProperties(e.SceneName, e.ItemName);
                int sceneItemId = GetSceneItemId(e.SceneName, e.ItemName);

                obsWebsocket.SetSceneItemEnabled(e.SceneName, sceneItemId, opacity > 0);

                if (opacity > 0) {
                    // TODO: Consider optimizing this to only change when the new value is different from the last one set. Confidence should be high (e.g., time between sets should be short, and everything else should match).
                    const string ImageMaskFilter = "Image Mask/Blend";
                    FilterSettings sourceFilterInfo = obsWebsocket.GetSourceFilter(e.ItemName, ImageMaskFilter);
                    JObject settings = sourceFilterInfo.Settings;
                    int newOpacity = (int)(opacity * 100);
                    ImageMask imageMask = settings.ToObject<ImageMask>();
                    if (imageMask.opacity != newOpacity) {
                        imageMask.opacity = newOpacity;
                        obsWebsocket.SetSourceFilterSettings(e.ItemName, ImageMaskFilter, JObject.FromObject(imageMask));
                    }
                }
                //if (rotation != sceneItemProperties.Rotation)
                //{
                sceneItemTransformInfo.Rotation = rotation;

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

                sceneItemTransformInfo.BoundsHeight = e.VideoHeight * scale;
                sceneItemTransformInfo.BoundsWidth = Math.Abs(e.VideoWidth) * scale;

                sceneItemTransformInfo.X = newLeft;
                sceneItemTransformInfo.Y = newTop;

                obsWebsocket.SetSceneItemTransform(e.SceneName, sceneItemId, sceneItemTransformInfo);

                float flipMultiplier = 1;
                if (e.VideoWidth < 0)
                    flipMultiplier = -1;

                if (flipped) {
                    flipMultiplier *= -1;
                }

                // TODO: Verify that flipping the video still works!!!

                // Old
                //obsWebsocket.SetSceneItemTransform(e.ItemName, (float)rotation, flipMultiplier * (float)scale, (float)scale, e.SceneName);
            }
            catch //(Exception ex)
            {

            }
        }

        public static void SetCurrentScene(string sceneName) {
            obsWebsocket.SetCurrentProgramScene(sceneName);
        }

        public static SceneBasicInfo GetCurrentScene() {
            string currentProgramSceneName = GetCurrentSceneName();
            GetSceneListInfo sceneListInfo = obsWebsocket.GetSceneList();
            return sceneListInfo.Scenes.FirstOrDefault(x => x.Name == currentProgramSceneName);
        }

        public static string GetCurrentSceneName() {
            return obsWebsocket.GetCurrentProgramScene();
        }

        static SceneItemDetails GetSceneItemDetails(string sceneName, string sourceName) {
            return obsWebsocket.GetSceneItemList(sceneName).FirstOrDefault(x => x.SourceName == sourceName);
        }

        public static bool GetSceneItemEnabled(string sceneName, string sourceName) {
            return obsWebsocket.GetSceneItemEnabled(sceneName, GetSceneItemId(sceneName, sourceName));
        }

        public static void SetSceneItemEnabled(string sceneName, string sourceName, bool sceneItemEnabled) {
            obsWebsocket.SetSceneItemEnabled(sceneName, GetSceneItemId(sceneName, sourceName), sceneItemEnabled);
        }

        public static SceneItemTransformInfo GetSceneItemProperties(string sceneName, string itemName) {
            return obsWebsocket.GetSceneItemTransform(sceneName, GetSceneItemId(sceneName, itemName));
        }

        private static int GetSceneItemId(string sceneName, string itemName) {
            return obsWebsocket.GetSceneItemId(sceneName, itemName, 0);
        }

        public static bool IsConnected => obsWebsocket.IsConnected;
    }
}

