using System;
using System.Linq;
using System.Collections.Generic;
using DndCore;

namespace DndCore
{
	public class PlayerActionShortcut
	{
		static int shortcutIndex;
		public List<WindupDto> Windups { get; }
		public AttackType AttackType { get; set; }
		public int PlusModifier { get; set; }
		public int AbilityModifier { get; set; }
		public int ProficiencyBonus { get; set; }
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
		public bool UsedWithProficiency
		{
			get
			{
				return ProficiencyBonus != 0;
			}
		}
		public int ToHitModifier
		{
			get
			{
				return PlusModifier + AbilityModifier + ProficiencyBonus;
			}
		}

		public int DamageModifier
		{
			get
			{
				return PlusModifier + AbilityModifier;
			}
		}
		public bool ModifiesExistingRoll { get; set; }
		public string Commands { get; set; }
		public Ability AttackAbility { get; set; }

		public PlayerActionShortcut()
		{
			AttackAbility = Ability.none;  // Calculated later.
			ModifiesExistingRoll = false;
			MinDamage = 0;
			UsesMagic = false;
			PlusModifier = 0;
			AbilityModifier = 0;
			ProficiencyBonus = 0;
			AttackType = AttackType.None;
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

		public static TurnPart GetTurnPart(string time)
		{
			string timeLowerCase = time.ToLower();
			switch (timeLowerCase)
			{
				case "1a":
					return TurnPart.Action;
				case "1ba":
					return TurnPart.BonusAction;
				case "1r":
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
			string spellName = DndUtils.GetCleanItemName(Name);
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

		public static List<PlayerActionShortcut> From(PlayerActionShortcutDto shortcutDto)
		{
			// TODO: Delete this check.
			if (string.Compare(shortcutDto.name, "battleaxe", true) == 0)
			{
				
			}
			List<PlayerActionShortcut> results = new List<PlayerActionShortcut>();
			string cleanName = DndUtils.GetCleanItemName(shortcutDto.name);
			Weapon weapon = AllWeapons.Get(cleanName);
			Character player = AllPlayers.GetFromId(PlayerID.FromName(shortcutDto.player));

			if (weapon != null)
			{
				if ((weapon.weaponProperties & WeaponProperties.Versatile) == WeaponProperties.Versatile)
				{
					if ((weapon.weaponProperties & WeaponProperties.Melee) == WeaponProperties.Melee &&
							(weapon.weaponProperties & WeaponProperties.Ranged) == WeaponProperties.Ranged)
					{
						results.Add(FromWeapon(shortcutDto, player, weapon.damageOneHanded, " (1H Stabbed)", weapon.weaponProperties, AttackType.Melee));
						results.Add(FromWeapon(shortcutDto, player, weapon.damageTwoHanded, " (2H Slice)", weapon.weaponProperties, AttackType.Melee));
						results.Add(FromWeapon(shortcutDto, player, weapon.damageOneHanded, " (1H Thrown)", weapon.weaponProperties, AttackType.Range));
					}
					else
					{
						results.Add(FromWeapon(shortcutDto, player, weapon.damageOneHanded, " (1H)", weapon.weaponProperties));
						results.Add(FromWeapon(shortcutDto, player, weapon.damageTwoHanded, " (2H)", weapon.weaponProperties));
					}

				}
				else if ((weapon.weaponProperties & WeaponProperties.TwoHanded) == WeaponProperties.TwoHanded)
					results.Add(FromWeapon(shortcutDto, player, weapon.damageTwoHanded, "", weapon.weaponProperties));
				else
					results.Add(FromWeapon(shortcutDto, player, weapon.damageOneHanded, "", weapon.weaponProperties));
			}
			else
			{
				// TODO: if there's a spell, add entries for each spell slot!!!

				Spell spell = AllSpells.Get(cleanName, player);
				if (spell != null)
				{
					if (spell.MorePowerfulWhenCastAtHigherLevels)
					{
						int[] spellSlotLevels = player.GetSpellSlotLevels();
						int availableSlots = spellSlotLevels[spell.Level];
						if (availableSlots > 0)
							for (int slotLevel = spell.Level; slotLevel <= 9; slotLevel++)
							{
								if (slotLevel == spell.Level)
									results.Add(FromSpell(shortcutDto, player, spell));
								else
									results.Add(FromSpell(shortcutDto, player, spell, slotLevel));
							}
					}
					else
						results.Add(FromSpell(shortcutDto, player, spell));
				}
				else
					results.Add(FromAction(shortcutDto, player));


			}

			return results;
		}

		private static PlayerActionShortcut FromWeapon(PlayerActionShortcutDto shortcutDto, Character player, string damageStr = null, string suffix = "", WeaponProperties weaponProperties = WeaponProperties.None, AttackType attackType = AttackType.None)
		{
			PlayerActionShortcut result = FromAction(shortcutDto, player, damageStr, suffix);

			if (attackType == AttackType.None && weaponProperties != WeaponProperties.None)
			{
				if ((weaponProperties & WeaponProperties.Melee) == WeaponProperties.Melee)
				{
					attackType = AttackType.Melee;
				}
				else if ((weaponProperties & WeaponProperties.Ranged) == WeaponProperties.Ranged)
				{
					attackType = AttackType.Range;
				}
			}
			result.AttackType = attackType;

			if ((attackType & AttackType.Range) == AttackType.Range ||
				(attackType & AttackType.MartialRange) == AttackType.MartialRange ||
				(attackType & AttackType.Melee) == AttackType.Melee ||
				(attackType & AttackType.MartialMelee) == AttackType.MartialMelee)
			{
				if (player.IsProficientWith(shortcutDto.name))
					result.ProficiencyBonus = (int)Math.Round(player.proficiencyBonus);
			}
			result.AbilityModifier = player.GetAbilityModifier(weaponProperties, attackType);
			result.AttackAbility = player.attackingAbility;

			result.ProcessDieStr(shortcutDto, damageStr);

			return result;
		}

		private static PlayerActionShortcut FromSpell(PlayerActionShortcutDto shortcutDto, Character player, Spell spell, int slotLevelOverride = 0, string damageStr = null, string suffix = "")
		{
			PlayerActionShortcut result = FromAction(shortcutDto, player, damageStr, suffix);

			int spellSlotLevel = spell.Level;
			if (slotLevelOverride > 0)
				spellSlotLevel = slotLevelOverride;
			result.AddSpell(spellSlotLevel, player);
			result.AttackAbility = player.spellCastingAbility;
			result.ProcessDieStr(shortcutDto, damageStr);
			return result;
		}

		private static PlayerActionShortcut FromAction(PlayerActionShortcutDto shortcutDto, Character player, string damageStr = "", string suffix = "")
		{
			PlayerActionShortcut result = new PlayerActionShortcut();
			result.Description = shortcutDto.description;
			result.AddDice = shortcutDto.addDice;
			result.AddDiceOnHit = shortcutDto.addDiceOnHit;
			result.AddDiceOnHitMessage = shortcutDto.addDiceOnHitMessage;
			result.AdditionalRollTitle = shortcutDto.addDiceTitle;

			result.LimitCount = MathUtils.GetInt(shortcutDto.limitCount);
			result.LimitSpan = DndTimeSpan.FromString(shortcutDto.limitSpan);
			result.MinDamage = MathUtils.GetInt(shortcutDto.minDamage);
			result.PlusModifier = MathUtils.GetInt(shortcutDto.plusModifier);
			result.Name = shortcutDto.name + suffix;

			result.Part = GetTurnPart(shortcutDto.time);
			result.PlayerId = PlayerID.FromName(shortcutDto.player);
			result.Type = GetDiceRollType(shortcutDto.type);
			result.VantageMod = DndUtils.ToVantage(shortcutDto.vantageMod);
			result.ModifiesExistingRoll = MathUtils.IsChecked(shortcutDto.rollMod);
			result.Commands = shortcutDto.commands;
			result.AddEffect(shortcutDto, player);
			player.ResetPlayerTurnBasedState();
			result.ProcessDieStr(shortcutDto, damageStr);
			return result;
		}

		void ProcessDieStr(PlayerActionShortcutDto shortcutDto, string damageStr)
		{
			string dieStr = shortcutDto.dieStr;

			if (damageStr != null)
				dieStr = damageStr;

			bool isInstantDice = dieStr.StartsWith("!");
			if (isInstantDice)
				dieStr = dieStr.Substring(1);

			DieRollDetails dieRollDetails = DieRollDetails.From(dieStr);
			if (dieRollDetails?.Rolls.Count > 0)
			{
				dieRollDetails.Rolls[0].Offset += DamageModifier;
				dieStr = dieRollDetails.ToString();
			}

			if (isInstantDice)
				InstantDice = dieStr;
			else
				Dice = dieStr;
		}

		public void ExecuteCommands(Character player)
		{
			// Commands
		}
	}
}
