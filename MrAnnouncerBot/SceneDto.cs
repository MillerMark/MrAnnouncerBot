using SheetsPersist;
using System;

namespace MrAnnouncerBot
{
	[Document("Mr. Announcer Guy")]
	[Sheet("Scenes")]
	public class SceneDto
	{
		double? minMinutesToSame;

		[Column]
		public string ChatShortcut { get; set; }
		
		[Column]
		public string AlternateShortcut { get; set; }
		
		[Column]
		public string Category { get; set; }
		
		[Column]
		public string SceneName { get; set; }
		
		[Column]
		public string LevelStr { get; set; }
		
		[Column]
		public string MinSpanToSameStr { get; set; }
		
		[Column]
		public string LimitToUser { get; set; }

		public double MinMinutesToSame
		{
			get
			{
				if (minMinutesToSame.HasValue)
					return minMinutesToSame.Value;
				double result;
				if (double.TryParse(MinSpanToSameStr, out result))
				{
					minMinutesToSame = result;
					return minMinutesToSame.Value;
				}

				return 0.5;
			}
		}
		int level = -1;
		public int Level
		{
			get
			{
				if (level < 0)
					if (!int.TryParse(LevelStr, out level))
						level = 0;

				return level;
			}
		}

		public bool Matches(string command)
{
	return string.Compare(ChatShortcut, command, StringComparison.OrdinalIgnoreCase) == 0 ||
				string.Compare(AlternateShortcut, command, StringComparison.OrdinalIgnoreCase) == 0;
}
	}
}