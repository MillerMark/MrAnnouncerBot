using System;
using System.Linq;
using System.Collections.Generic;
using DndCore;

namespace DndCore
{
	public class PlayerActionShortcut
	{
		public List<WindupDto> Windups { get; }
		static int shortcutIndex;
		public int PlusModifier { get; set; }
		public string Dice { get; set; }
		public string Name { get; set; }
		public bool UsesMagic { get; set; }
		public int PlayerId { get; set; }
		public int SpellSlotLevel { get; set; }
		public TurnPart Part { get; set; }

		public DiceRollType Type { get; set; }
		public int Index { get; set; }
		public int LimitCount { get; set; }
		public DndTimeSpan LimitSpan { get; set; }
		public string Description { get; set; }
		public VantageKind VantageMod { get; set; }
		public string AddDice { get; set; }
		public string InstantDice { get; set; }
		public string AdditionalRollTitle { get; set; }
		public int MinDamage { get; set; }
		public Spell Spell { get; set; }
		public string AddDiceOnHit { get; set; }
		public string AddDiceOnHitMessage { get; set; }
		public PlayerActionShortcut()
		{
			MinDamage = 0;
			UsesMagic = false;
			PlusModifier = 0;
			Type = DiceRollType.None;
			Index = shortcutIndex;
			shortcutIndex++;
			Part = TurnPart.Action;
			LimitCount = 0;
			LimitSpan = DndTimeSpan.Never;
			Description = "";
			AddDice = "";
			AddDiceOnHit = "";
			AddDiceOnHitMessage = "";
			InstantDice = "";
			AdditionalRollTitle = "";
			Dice = "";
			VantageMod = VantageKind.Normal;
			Windups = new List<WindupDto>();
		}

		public static void PrepareForCreation()
		{
			shortcutIndex = 0;
		}
		
		public List<WindupDto> CloneWindups()
		{
			List<WindupDto> result = new List<WindupDto>();
			foreach (WindupDto windup in Windups)
			{
				result.Add(windup.Clone());
			}
			return result;
		}
		static TurnPart GetTurnPart(string time)
		{
			string timeLowerCase = time.ToLower();
			switch (timeLowerCase)
			{
				case "1A":
					return TurnPart.Action;
				case "1BA":
					return TurnPart.BonusAction;
				case "1R":
					return TurnPart.Reaction;
				case "*":
					return TurnPart.Special;
			}
			return TurnPart.Action;
		}
		
		static DiceRollType GetDiceRollType(string type)
		{
			switch (type)
			{
				case "Attack":
					return DiceRollType.Attack;
				case "AddOnDice":
					return DiceRollType.AddOnDice;
				case "ChaosBolt":
					return DiceRollType.ChaosBolt;
				case "LuckRollHigh":
					return DiceRollType.LuckRollHigh;
				case "WildMagic":
					return DiceRollType.WildMagic;
				case "SavingThrow":
					return DiceRollType.SavingThrow;
				case "NonCombatInitiative":
					return DiceRollType.NonCombatInitiative;
				case "Initiative":
					return DiceRollType.Initiative;
				case "DamageOnly":
					return DiceRollType.DamageOnly;
				case "BendLuckAdd":
					return DiceRollType.BendLuckAdd;
				case "FlatD20":
					return DiceRollType.FlatD20;
				case "None":
					return DiceRollType.None;
				case "WildMagicD20Check":
					return DiceRollType.WildMagicD20Check;
				case "HealthOnly":
					return DiceRollType.HealthOnly;
				case "BendLuckSubtract":
					return DiceRollType.BendLuckSubtract;
				case "DeathSavingThrow":
					return DiceRollType.DeathSavingThrow;
				case "SkillCheck":
					return DiceRollType.SkillCheck;
				case "InspirationOnly":
					return DiceRollType.InspirationOnly;
				case "ExtraOnly":
					return DiceRollType.ExtraOnly;
				case "LuckRollLow":
					return DiceRollType.LuckRollLow;
				case "PercentageRoll":
					return DiceRollType.PercentageRoll;
			}
			return DiceRollType.None;
		}
		void AddSpell(int spellSlotLevel, Character player)
		{
			string spellName = Name;
			if (spellName.IndexOf('[') > 0)
				spellName = spellName.EverythingBefore("[").Trim();
			Spell = AllSpells.Get(spellName, player, spellSlotLevel);
			Dice = Spell.DieStr;
			Part = DndUtils.ToTurnPart(Spell.CastingTime);
		}
		public void AddEffect(PlayerActionShortcutDto shortcutDto, Character player)
		{
			WindupDto windup = WindupDto.From(shortcutDto, player);
			if (windup == null)
				return;
			if (Spell?.Duration.HasValue() == true)
				windup.Lifespan = 0;
			Windups.Add(windup);
		}
		static int GetInt(string str)
		{
			if (string.IsNullOrEmpty(str))
				return 0;
			if (int.TryParse(str, out int result))
				return result;
			return 0;
		}
		public static PlayerActionShortcut From(PlayerActionShortcutDto shortcutDto)
		{
			PlayerActionShortcut result = new PlayerActionShortcut();
			result.AddDice = shortcutDto.addDice;
			result.AddDiceOnHit = shortcutDto.addDiceOnHit;
			result.AddDiceOnHitMessage = shortcutDto.addDiceOnHitMessage;
			result.AdditionalRollTitle = shortcutDto.addDiceTitle;
			result.Description = string.Empty;
			if (shortcutDto.dieStr.StartsWith("!"))
				result.InstantDice = shortcutDto.dieStr.Substring(1);
			else
				result.Dice = shortcutDto.dieStr;

			result.LimitCount = GetInt(shortcutDto.limitCount);
			result.LimitSpan = DndTimeSpan.FromString(shortcutDto.limitSpan);
			result.MinDamage = GetInt(shortcutDto.minDamage);
			result.PlusModifier = GetInt(shortcutDto.plusModifier);
			result.Name = shortcutDto.name;
			result.Part = GetTurnPart(shortcutDto.time);
			result.PlayerId = PlayerID.FromName(shortcutDto.player);
			Character player = AllPlayers.GetFromId(result.PlayerId);
			if (!string.IsNullOrEmpty(shortcutDto.spellSlotLevel) && int.TryParse(shortcutDto.spellSlotLevel, out int level))
				result.AddSpell(level, player);

			result.Type = GetDiceRollType(shortcutDto.type);
			result.VantageMod = DndUtils.ToVantage(shortcutDto.vantageMod);
			result.AddEffect(shortcutDto, player);
			// TODO: if there's a spell, bubble up the appropriate settings like dieStr, description, etc.
			return result;
		}
	}
}
