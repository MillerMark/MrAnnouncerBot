using System;
using System.Linq;
using System.Collections.Generic;
using SheetsPersist;
using ObsControl;

namespace DHDM
{
	public static class AllVideoFeeds
	{
		static List<VideoFeed> allVideoFeeds;

		static void LoadAllVideoFeeds()
		{
			allVideoFeeds = GoogleSheets.Get<VideoFeed>();
		}

    public static VideoFeed Get(string sourceName)
    {
      return AllFeeds.FirstOrDefault(x => x.sourceName == sourceName);
    }

		public static void Invalidate()
		{
			allVideoFeeds = null;
		}

		public static VideoFeed[] GetAll(VideoAnimationBinding videoAnimationBinding)
		{
			VideoFeed[] result = new VideoFeed[4];
			result[0] = Get(videoAnimationBinding.Camera1);
			result[1] = Get(videoAnimationBinding.Camera2);
			result[2] = Get(videoAnimationBinding.Camera3);
			result[3] = Get(videoAnimationBinding.Camera4);

			return result;
		}

		public static List<VideoFeed> AllFeeds
		{
			get
			{
				if (allVideoFeeds == null)
					LoadAllVideoFeeds();
				return allVideoFeeds;
			}
			set => allVideoFeeds = value;
		}
	}
}
