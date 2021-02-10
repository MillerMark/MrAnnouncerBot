using System;
using System.Linq;
using System.Collections.Generic;

namespace DndCore
{
	public class DiceDto
	{
		public int Quantity { get; set; }
		public int Sides { get; set; }
		public int ScoreMultiplier { get; set; }
		public int CreatureId { get; set; }
		public bool IsMagic { get; set; }
		public double Modifier { get; set; }
		public double Scale { get; set; } = 1;
		public string Label { get; set; }
		public string Data { get; set; }  // Any string sent down through this property will return when the roll is complete in multiplayerSummary's PlayerRoll.data property.
		public string PlayerName { get; set; }
		public VantageKind Vantage { get; set; } = VantageKind.Normal;
		public DamageType DamageType { get; set; } = DamageType.None;
		public DieCountsAs DieCountsAs { get; set; } = DieCountsAs.totalScore;
		public string BackColor { get; set; } = "#ebebeb";
		public string FontColor { get; set; } = "#000000";
		//public List<string> Effects { get; set; }
		// Trailing effects
		public DiceDto()
		{
			Quantity = 1;
			ScoreMultiplier = 1;
		}
		public void SetRollDetails(DiceRollType type, string descriptor)
		{
			descriptor = descriptor.ToLower();
			DamageType = DndUtils.ToDamage(descriptor);
			if (DamageType != DamageType.None)
				DieCountsAs = DieCountsAs.damage;
			else if (descriptor.Contains("health") || type == DiceRollType.HealthOnly)
				DieCountsAs = DieCountsAs.health;
			else if (descriptor.Contains("inspiration") || type == DiceRollType.InspirationOnly)
				DieCountsAs = DieCountsAs.inspiration;
			else if (descriptor.Contains("extra") || type == DiceRollType.ExtraOnly)
				DieCountsAs = DieCountsAs.extra;
			else if (descriptor.Contains("bent luck") || type == DiceRollType.BendLuckAdd || type == DiceRollType.BendLuckSubtract)
				DieCountsAs = DieCountsAs.bentLuck;
			else if (descriptor.Contains("bonus"))
				DieCountsAs = DieCountsAs.bonus;
			else 
				DieCountsAs = DieCountsAs.totalScore;
		}

		public static DiceDto D20FromInGameCreature(InGameCreature inGameCreature, DiceRollType diceRollType)
		{
			DieCountsAs dieCountsAs = DieCountsAs.totalScore;
			double modifier = 0;
			string label;
			if (IsSavingThrow(diceRollType))
			{
				dieCountsAs = DieCountsAs.savingThrow;
				label = $"{inGameCreature.Name}'s Save";
			}
			else
				label = inGameCreature.Name;

			return AddD20ForCreature(inGameCreature, label, modifier, dieCountsAs);
		}

		public static bool IsSavingThrow(DiceRollType diceRollType)
		{
			return diceRollType == DiceRollType.SavingThrow || diceRollType == DiceRollType.DamagePlusSavingThrow || diceRollType == DiceRollType.OnlyTargetsSavingThrow;
		}

		private static DiceDto AddD20ForCreature(InGameCreature inGameCreature, string label, double modifier, DieCountsAs dieCountsAs)
		{
			return new DiceDto()
			{
				PlayerName = inGameCreature.Name,
				CreatureId = -inGameCreature.Index,
				Sides = 20,
				Quantity = 1,
				Label = label,
				Modifier = modifier,
				DamageType = DamageType.None,
				BackColor = inGameCreature.BackgroundHex,
				FontColor = inGameCreature.ForegroundHex,
				DieCountsAs = dieCountsAs
			};
		}

		public static bool IsDamage(DiceRollType type)
		{
			return type == DiceRollType.DamageOnly || type == DiceRollType.DamagePlusSavingThrow;
		}

		static DiceDto FromRoll(Roll roll, string dieBackColor, string dieFontColor, int creatureId, string playerName)
		{
			// TODO: Set color for creature, set label, set player name.
			DiceDto result = new DiceDto();
			result.DamageType = DndUtils.ToDamage(roll.Descriptor);
			result.Quantity = (int)roll.Count;
			result.BackColor = dieBackColor;
			result.FontColor = dieFontColor;
			result.Label = roll.Label;
			result.PlayerName = playerName;
			result.CreatureId = creatureId;
			result.Sides = roll.Sides;
			result.ScoreMultiplier = roll.ScoreMultiplier;


			return result;
		}

		public static void AddDtosFromDieStr(List<DiceDto> diceDtos, string diceStr, string dieBackColor, string dieFontColor, int creatureId, string playerName)
		{
			DieRollDetails dieRollDetails = DieRollDetails.From(diceStr);
			foreach (Roll roll in dieRollDetails.Rolls)
				diceDtos.Add(FromRoll(roll, dieBackColor, dieFontColor, creatureId, playerName));
		}
	}
}
