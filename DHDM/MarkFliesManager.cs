using ObsControl;
using System;
using System.Linq;

namespace DHDM
{
	public static class MarkFliesManager
	{
		static BaseLiveFeedAnimator liveFeedAnimator;
		static MarkFliesManager()
		{
			
		}

		public static void Initialize()
		{
			HubtasticBaseStation.UpdateVideoFeed += HubtasticBaseStation_OnUpdateVideoFeed;
		}

		public static BaseLiveFeedAnimator LiveFeedAnimator 
		{
			get
			{
				if (liveFeedAnimator == null)
				{
					VideoFeed[] videoFeeds = new VideoFeed[] { AllVideoFeeds.Get("MarkCam") };
					liveFeedAnimator = new BaseLiveFeedAnimator(videoFeeds);
				}

				return liveFeedAnimator;
			}
		}

		private static void HubtasticBaseStation_OnUpdateVideoFeed(object sender, VideoFeedDto videoFeedDto)
		{
			LiveFeedAnimator.ScreenAnchorLeft = videoFeedDto.X;
			LiveFeedAnimator.ScreenAnchorTop = videoFeedDto.Y;
			if (videoFeedDto.Scale == 0)
				videoFeedDto.Scale = 1;
			ObsManager.SizeAndPositionItem(LiveFeedAnimator, (float)videoFeedDto.Scale, 1, videoFeedDto.Rotation);
		}
	}
}
