using System;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class ApplyCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		static int oneValue = int.MinValue;
		static int tenValue = int.MinValue;
		static DateTime lastTenSetTime = DateTime.MinValue;
		public static void SetOnes(string value)
		{
			if (int.TryParse(value, out int asInt))
				oneValue = asInt;
		}

		public static void SetTens(string value)
		{
			if (int.TryParse(value, out int asInt))
			{
				tenValue = asInt;
				lastTenSetTime = DateTime.Now;
			}
		}

		public static int GetValue()
		{
			int tens;
			int ones;

			if (lastTenSetTime == DateTime.MinValue || (DateTime.Now - lastTenSetTime).TotalSeconds > 15)
				tens = 0;
			else
			{
				if (tenValue == int.MinValue)
					tens = 0;
				else
					tens = tenValue;
			}

			if (oneValue == int.MinValue)
				ones = 0;
			else
				ones = oneValue;
			return tens + ones;
		}

		void ResetValue()
		{
			tenValue = 0;
			oneValue = 0;
		}

		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			dungeonMasterApp.Apply(applyCommand, GetValue(), GetPlayerIds(dungeonMasterApp, applyToAllPlayers));
			ResetValue();
		}

		string applyCommand;
		bool applyToAllPlayers;
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
				return true;
			}
			
			return false;
		}
	}
}
