using System;

namespace ObsControl
{
	public class BaseLiveFeedAnimator
	{
		public double VideoAnchorHorizontal => videoFeeds[camera].videoAnchorHorizontal;
		public double VideoAnchorVertical => videoFeeds[camera].videoAnchorVertical;
		public double VideoWidth => videoFeeds[camera].videoWidth;
		public double VideoHeight => videoFeeds[camera].videoHeight;
		public string ItemName => videoFeeds[camera].sourceName;
		public string LastItemName => videoFeeds[lastCamera].sourceName;
		public string LastSceneName => videoFeeds[lastCamera].sceneName;
		public string SceneName => videoFeeds[camera].sceneName;

		/// <summary>
		/// The left point on the screen around which the video will rotate, scale, etc.
		/// </summary>
		public double ScreenAnchorLeft { get; set; }

		/// <summary>
		/// The top point on the screen around which the video will rotate, scale, etc.
		/// </summary>
		public double ScreenAnchorTop { get; set; }



		public BaseLiveFeedAnimator(VideoFeed[] videoFeeds)
		{
			this.videoFeeds = videoFeeds;
		}

		int camera;
		int lastCamera = -1;
		readonly VideoFeed[] videoFeeds;

		public int Camera
		{
			get
			{
				return camera;
			}
			private set
			{
				if (camera == value)
					return;
				lastCamera = camera;
				camera = value;
			}
		}

		public void SetCamera(int camera)
		{
			Camera = camera;
		}

		public bool HasLastCamera()
		{
			return lastCamera >= 0;
		}

		public void ClearLastCamera()
		{
			lastCamera = -1;
		}
	}
}
