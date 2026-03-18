using System;
using OBSWebsocketDotNet.Types;

namespace ObsControl {
    public class SceneItemEventArgs : EventArgs {
        public string SceneName { get; set; }
        public SceneItemDetails Item { get; set; }
    }
}
