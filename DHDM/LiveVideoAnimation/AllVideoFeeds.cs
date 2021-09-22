using System;
using System.Linq;
using System.Collections.Generic;
using GoogleHelper;

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
