using System;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class SetPropertyCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		string valueAsStr;
		object valueStr;
		string propertyName;

		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			const bool verbose = false;
			int playerId = dungeonMasterApp.GetPlayerIdFromNameStart(TargetPlayer);
			string valueUppercase = valueAsStr.ToUpper().Trim();
			if (valueUppercase == "TRUE" || valueUppercase == "FALSE")
			{
				string firstName = dungeonMasterApp.GetPlayFirstNameFromId(playerId);
				bool newValue = valueUppercase == "TRUE";
				bool previousValue = dungeonMasterApp.GetBoolProperty(playerId, propertyName);
				if (previousValue == newValue)
				{
					if (verbose)
						dungeonMasterApp.TellDungeonMaster($"{firstName}'s {propertyName} is already {valueAsStr.Trim()}.");
					return;
				}

				dungeonMasterApp.SetBoolProperty(playerId, propertyName, newValue);
				bool newlySetValue = dungeonMasterApp.GetBoolProperty(playerId, propertyName);
				if (newlySetValue == previousValue)
					dungeonMasterApp.TellDungeonMaster($"Error. Unable to set {firstName}'s {propertyName} to {valueAsStr.Trim()}.");
				else if (verbose)
					dungeonMasterApp.TellDungeonMaster($"{firstName}'s {propertyName} = {valueAsStr.Trim()}.");
			}
			//bool GetBoolProperty(int playerId, string propertyName);
		}

		public bool Matches(string message)
		{
			// TODO: support quoted strings in the second parameter/group
			Match match = Regex.Match(message, @"^SetProperty\(\s*(\w+),\s*(\w+)," + PlayerSpecifier + @"\)");
			if (match.Success)
			{
				SetTargetPlayer(match.Groups);
				propertyName = match.Groups[1].Value;
				valueAsStr = match.Groups[2].Value;
				return true;
			}
			return false;
		}
	}
}
