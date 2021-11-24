using System;
using System.Collections.Generic;
using Imaging;
using System.Timers;
using OBSWebsocketDotNet.Types;
using ObsControl;

namespace DHDM
{
	public class LiveFeedAnimator : BaseLiveFeedAnimator
	{
		public FrameAnimator FrameAnimator { get; set; }
		public List<ObsTransform> LiveFeedSequences { get; set; }

		public LiveFeedAnimator(VideoFeed[] videoFeeds, List<ObsTransform> liveFeedFrames) : base(videoFeeds)
		{
			LiveFeedSequences = liveFeedFrames;
			FrameAnimator = new FrameAnimator(liveFeedFrames.Count);
			FrameAnimator.RenderFrame += FrameAnimator_RenderFrame;
		}

		private void FrameAnimator_RenderFrame(object sender, RenderFrameEventArgs ea)
		{
			RenderFrame(out double duration, out bool shouldStop);
			ea.Duration = duration;
			ea.ShouldStop = shouldStop;
		}

		private void RenderFrame(out double duration, out bool shouldStop)
		{
			shouldStop = false;
			ObsTransform liveFeedFrame = LiveFeedSequences[FrameAnimator.FrameIndex];
			if (liveFeedFrame == null)
			{
				FrameAnimator.Stop();
				shouldStop = true;
			}
			RenderActiveFrame(liveFeedFrame);
			duration = liveFeedFrame.Duration;
		}

		void RenderActiveFrame(ObsTransform liveFeedFrame)
		{
			ScreenAnchorLeft = liveFeedFrame.Origin.X;
			ScreenAnchorTop = liveFeedFrame.Origin.Y;
			SetCamera(liveFeedFrame.Camera);
			ObsManager.SizeAndPositionItem(this, (float)liveFeedFrame.Scale, liveFeedFrame.Opacity, liveFeedFrame.Rotation, liveFeedFrame.Flipped);
		}
	}
}