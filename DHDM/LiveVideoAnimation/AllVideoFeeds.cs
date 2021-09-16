﻿using System;
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
