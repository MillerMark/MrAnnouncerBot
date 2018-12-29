using System;
using System.Collections.Generic;
using System.Linq;

namespace MrAnnouncerBot
{
	public class Viewer
	{
		public Viewer()
		{
			NumberOfShowsWatched = 1;
			LastShowWatched = DateTime.Now;
		}
		public string UserId { get; set; }
		public int NumberOfShowsWatched { get; set; }
		public int CoinsCollected { get; set; }
		public DateTime LastShowWatched { get; set; }
		public int NumberOfChatMessagesSent { get; set; }
		public int ModeratorOffset { get; set; }
		public string DisplayName { get; set; }
		public string UserName { get; set; }
		public int GetLevel()
		{
			return ModeratorOffset + NumberOfShowsWatched / 12 + NumberOfChatMessagesSent / 100 + CoinsCollected / 100;
		}
	}
}