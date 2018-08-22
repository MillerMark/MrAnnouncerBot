using System;
using System.Linq;

namespace MrAnnouncerBot
{
	public class Chatters
	{
		public string[] moderators { get; set; }
		public object[] staff { get; set; }
		public object[] admins { get; set; }
		public object[] global_mods { get; set; }
		public string[] viewers { get; set; }
	}
}