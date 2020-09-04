using System;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class AdvanceClockCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		const string STR_Resting = ", resting";
		int days;
		int months;
		int years;
		int hours;
		int minutes;
		int seconds;
		bool resting;

		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			dungeonMasterApp.AdvanceDate(days, months, years, resting);
			dungeonMasterApp.AdvanceClock(hours, minutes, seconds, resting);
		}

		public bool Matches(string message)
		{
			Match match = Regex.Match(message, @"^ac\s+(\-*\w+)(.+)?$");
			if (match.Success)
			{
				days = 0;
				months = 0;
				years = 0;
				hours = 0;
				minutes = 0;
				seconds = 0;
				string distance = match.Groups[1].Value;
				if (match.Groups.Count > 2)
				{
					string restingStr = match.Groups[2].Value;
					if (restingStr == STR_Resting)
					{
						resting = true;
					}
				}

				string numStr = distance.Substring(0, distance.Length - 1);
				int numValue = DndCore.MathUtils.GetInt(numStr);
				if (distance.EndsWith("d"))
					days = numValue;
				else if (distance.EndsWith("M"))
					months = numValue;
				else if (distance.EndsWith("h"))
					hours = numValue;
				else if (distance.EndsWith("m"))
					minutes = numValue;
				else if (distance.EndsWith("s"))
					seconds = numValue;
				else if (distance.EndsWith("y"))
					years = numValue;
				else
					return false;
				return numValue != 0;
			}
			return false;
		}
	}
}
