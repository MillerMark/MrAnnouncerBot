using System;
using System.Linq;

namespace MrAnnouncerBot
{
	public class SceneDto
	{
		public string ChatShortcut { get; set; }
		public string AlternateShortcut { get; set; }
		public string Category { get; set; }
		public string SceneName { get; set; }
		public string LevelStr { get; set; }
		public string MinSpanToSameStr { get; set; }
		public string LimitToUser { get; set; }
		public SceneDto()
		{

		}
	}
}
