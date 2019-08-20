using System;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client;
using TwitchLib.Client.Models;

namespace DHDM
{

	public interface IDungeonMasterCommand
	{
		bool Matches(string message);
		void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage);
	}
}
