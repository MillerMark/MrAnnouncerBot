using System;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class ApplyCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		static decimal digitsEntered = decimal.MinValue;
		static DateTime lastDigitSetTime = DateTime.MinValue;
		public static void AddDigit(string value)
		{
			if (lastDigitSetTime == DateTime.MinValue || (DateTime.Now - lastDigitSetTime).TotalSeconds > 12)
			{
				digitsEntered = decimal.MinValue;
			}

			lastDigitSetTime = DateTime.Now;
			if (value == ".")
			{
				decimalMultiplier = 0.1m;
				if (digitsEntered == decimal.MinValue)
					digitsEntered = 0;
			}
			else
			{
				if (int.TryParse(value, out int asInt))
					if (digitsEntered == decimal.MinValue)
						digitsEntered = asInt;
					else
					{
						if (decimalMultiplier == 1m)
						{
							digitsEntered *= 10m;
							digitsEntered += asInt;
						}
						else
						{
							digitsEntered += asInt * decimalMultiplier;
							decimalMultiplier *= 0.1m;
						}
					}
			}
		}

		public static decimal GetValue()
		{
			return digitsEntered;
		}

		public static void ResetValue()
		{
			digitsEntered = decimal.MinValue;
			decimalMultiplier = 1m;
		}

		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			decimal value = GetValue();
			if (value == decimal.MinValue)
			{
				dungeonMasterApp.TellDungeonMaster($"Set numeric value first before applying {applyCommand}.");
				return;
			}
			dungeonMasterApp.Apply(applyCommand, value, GetPlayerIds(dungeonMasterApp, applyToAllPlayers));
			ResetValue();
		}

		string applyCommand;
		bool applyToAllPlayers;
		static decimal decimalMultiplier = 1m;
		public bool Matches(string message)
		{
			applyToAllPlayers = false;
			Match match = Regex.Match(message, @"^Apply\s+(\w+)" + PlayerSpecifier);

			if (!match.Success)
			{
				applyToAllPlayers = true;
				match = Regex.Match(message, @"^Apply\s+(\w+)");
			}
			if (match.Success)
			{
				if (!applyToAllPlayers)
				{
					SetTargetPlayer(match.Groups);
					if (TargetPlayer == null)
						applyToAllPlayers = true;
				}

				applyCommand = match.Groups[1].Value;
				if (applyCommand == "LastDamage" || applyCommand == "LastHealth")
					digitsEntered = -1;
				return true;
			}
			
			return false;
		}
	}
}
