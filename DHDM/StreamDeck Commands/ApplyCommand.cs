using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class ApplyCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		string GetActiveKeyword()
		{
			switch (applyCommand)
			{
				case "DamageToTargetedCreatures":
				case "DamageToAllCreatures":
				case "DamageToSelected":
				case "Damage":
				case "Health":
				case "HealthToTargetedCreatures":
				case "HealthToAllCreatures":
				case "HealthToSelected":
				case "TempHpToTargetedCreatures":
				case "TempHpToAllCreatures":
				case "TempHpToSelected":
				case "TempHp":
					return "health";

				case "RemoveCoins":
				case "AddCoins":
					return "wealth";

				case "SkillThreshold":
				case "SaveThreshold":
				case "AttackThreshold":
					return "threshold";
			}
			return null;
		}

		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			if (applyToTargetedCreatures)
			{
				dungeonMasterApp.ApplyToTargetedCreatures(applyCommand);
				return;
			}

			string keyword = GetActiveKeyword();
			decimal value = DigitManager.GetValue(keyword);
			if (value > 10000)
			{
				System.Diagnostics.Debugger.Break();
				dungeonMasterApp.TellDungeonMaster($"{keyword} value unreasonably high - {value}. Resetting it to zero. Please try again.");
				DigitManager.SetValue(keyword, 0);
				return;
			}

			if (applyToOnScreenCreatures && value != decimal.MinValue)
			{
				dungeonMasterApp.ApplyToOnScreenCreatures(applyCommand, (int)value);
				return;
			}

			if (applyToSelectedCreature && value != decimal.MinValue)
			{
				dungeonMasterApp.ApplyToSelectedCreature(applyCommand, (int)value);
				return;
			}

			if (value == decimal.MinValue)
			{
				dungeonMasterApp.TellDungeonMaster($"Set numeric value first before applying {applyCommand}.");
				return;
			}
			dungeonMasterApp.Apply(applyCommand, value, GetPlayerIds(dungeonMasterApp, applyToAllPlayers));
			DigitManager.ResetValue(keyword);
		}

		string applyCommand;
		bool applyToAllPlayers;
		bool applyToTargetedCreatures;
		bool applyToOnScreenCreatures;
		bool applyToSelectedCreature;

		public bool Matches(string message)
		{
			applyToTargetedCreatures = false;
			applyToOnScreenCreatures = false;
			applyToSelectedCreature = false;
			applyToAllPlayers = false;
			if (message == "Apply LastHealth [TargetedCreatures]")
			{
				applyToTargetedCreatures = true;
				applyCommand = "LastHealth";
				return true;
			}
			if (message == "Apply LastDamage [TargetedCreatures]")
			{
				applyToTargetedCreatures = true;
				applyCommand = "LastDamage";
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
				if (applyCommand == "DamageToAllCreatures" || applyCommand == "HealthToAllCreatures" || applyCommand == "TempHpToAllCreatures")
				{
					applyToAllPlayers = false;
					applyToOnScreenCreatures = true;
				}
				if (applyCommand == "DamageToSelected" || applyCommand == "HealthToSelected" || applyCommand == "TempHpToSelected")
				{
					applyToAllPlayers = false;
					applyToSelectedCreature = true;
				}
				if (applyCommand == "LastDamage" || applyCommand == "LastHealth")
					DigitManager.SetValue("health", -1);
				return true;
			}

			return false;
		}
	}
}
