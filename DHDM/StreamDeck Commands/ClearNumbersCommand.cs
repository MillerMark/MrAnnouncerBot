using System;
using System.Linq;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class ClearNumbersCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		const string STR_ClearNumbersPrefix = "ClearNumbers (";
		string keyword;
		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			ApplyCommand.ResetValue(keyword);
		}

		public bool Matches(string message)
		{
			if (message.StartsWith(STR_ClearNumbersPrefix))
			{
				keyword = message.Substring(STR_ClearNumbersPrefix.Length).TrimEnd(')');
				return true;
			}
			return false;
		}
	}
}
