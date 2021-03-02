using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class ApplyCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		public static DateTime lastDigitSetTime
		{
			get
			{
				if (activeKeyword == null)
					return DateTime.MinValue;
				if (!_lastDigitSetTimes.ContainsKey(activeKeyword))
					_lastDigitSetTimes.Add(activeKeyword, DateTime.MinValue);
				return _lastDigitSetTimes[activeKeyword];
			}
			set
			{
				if (activeKeyword == null)
					return;
				if (!_lastDigitSetTimes.ContainsKey(activeKeyword))
					_lastDigitSetTimes.Add(activeKeyword, value);
				else
					_lastDigitSetTimes[activeKeyword] = value;
			}
		}

		static Dictionary<string, decimal> _digitsEntered = new Dictionary<string, decimal>();
		static Dictionary<string, decimal> _decimalMultipliers = new Dictionary<string, decimal>();
		static Dictionary<string, DateTime> _lastDigitSetTimes = new Dictionary<string, DateTime>();
		static string activeKeyword;
		
		public static decimal digitsEntered
		{
			get
			{
				if (activeKeyword == null)
					return decimal.MinValue;
				if (!_digitsEntered.ContainsKey(activeKeyword))
					_digitsEntered.Add(activeKeyword, decimal.MinValue);
				return _digitsEntered[activeKeyword];
			}
			set
			{
				if (activeKeyword == null)
					return;
				if (!_digitsEntered.ContainsKey(activeKeyword))
					_digitsEntered.Add(activeKeyword, value);
				else
					_digitsEntered[activeKeyword] = value;
			}
		}

		public static decimal decimalMultiplier
		{
			get
			{
				if (activeKeyword == null)
					return 1m;
				if (!_decimalMultipliers.ContainsKey(activeKeyword))
					_decimalMultipliers.Add(activeKeyword, 1m);
				return _decimalMultipliers[activeKeyword];
			}
			set
			{
				if (activeKeyword == null)
					return;
				if (!_decimalMultipliers.ContainsKey(activeKeyword))
					_decimalMultipliers.Add(activeKeyword, value);
				else
					_decimalMultipliers[activeKeyword] = value;
			}
		}

		public static void AddDigit(string keyword, string value)
		{
			activeKeyword = keyword;
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

		public static decimal GetValue(string keyword)
		{
			activeKeyword = keyword;
			return digitsEntered;
		}

		public static void ResetValue(string keyword)
		{
			activeKeyword = keyword;
			digitsEntered = decimal.MinValue;
			decimalMultiplier = 1m;
		}

		void SetActiveKeyword()
		{
			switch (applyCommand)
			{
				case "DamageToTargetedCreatures":
				case "Damage":
				case "Health":
				case "HealthToTargetedCreatures":
				case "TempHpToTargetedCreatures":
				case "TempHp":
					activeKeyword = "health";
					break;

				case "RemoveCoins":
				case "AddCoins":
					activeKeyword = "wealth";
					break;
				case "SkillThreshold":
				case "SaveThreshold":
				case "AttackThreshold":
					activeKeyword = "threshold";
					break;
			}
		}

		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			if (applyToTargetedCreatures)
			{
				dungeonMasterApp.ApplyToTargetedCreatures(applyCommand);
				return;
			}
			SetActiveKeyword();
			decimal value = GetValue(activeKeyword);
			if (value == decimal.MinValue)
			{
				dungeonMasterApp.TellDungeonMaster($"Set numeric value first before applying {applyCommand}.");
				return;
			}
			dungeonMasterApp.Apply(applyCommand, value, GetPlayerIds(dungeonMasterApp, applyToAllPlayers));
			ResetValue(activeKeyword);
		}

		string applyCommand;
		bool applyToAllPlayers;
		bool applyToTargetedCreatures;
		
		public bool Matches(string message)
		{
			applyToTargetedCreatures = false;
			applyToAllPlayers = false;
			if (message == "Apply LastHealth [TargetedCreatures]")
			{
				applyToTargetedCreatures = true;
				applyCommand = "LastHealth";
				activeKeyword = "health";
				return true;
			}
			if (message == "Apply LastDamage [TargetedCreatures]")
			{
				applyToTargetedCreatures = true;
				applyCommand = "LastDamage";
				activeKeyword = "health";
				return true;
			}
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
