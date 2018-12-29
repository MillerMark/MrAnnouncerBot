using System;
using System.Linq;

namespace OverlayManager
{
	public class CommandExpansion
	{
		private char firstChar;
		private string v;

		public string Command { get; set; }
		public string Arguments { get; set; }
		public CommandExpansion()
		{

		}

		public CommandExpansion(string command, string arguments)
		{
			Command = command;
			Arguments = arguments.Trim();
		}
	}
}
