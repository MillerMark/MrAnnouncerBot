using System;
using System.Linq;
using System.Collections.Generic;
using DndCore;

namespace DndCore
{
	public class PlayerActionShortcut
	{
		public const string SpellWindupPrefix = "Spell.";
		public const string WeaponWindupPrefix = "Weapon.";
		public const string STR_OtherPrefix = "Other.";
		static int shortcutIndex;
		public List<WindupDto> Windups { get; }
		public List<WindupDto> WindupsReversed {
			get
			{
				List<WindupDto> result = new List<WindupDto>();
				for (int i = Windups.Count - 1; i >= 0; i--)
					result.Add(Windups[i]);
				return result;
			}
		}
		public AttackType AttackingType { get; set; }
		public int PlusModifier { get; set; }
		public int AttackingAbilityModifier { get; set; }
		public int ProficiencyBonus { get; set; }
		public string Dice { get; set; }
		public string Name { get; set; }
		public bool UsesMagic { get; set; }
		public int PlayerId { get; set; }
		public int SpellSlotLevel { get; set; }
		public TurnPart Part { get; set; }

		public DiceRollType Type { get; set; }
		public int Index { get; set; }
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
				return PlusModifier + AttackingAbilityModifier + ProficiencyBonus;
			}
		}

		public int DamageModifier
		{
			get
			{
				return PlusModifier + AttackingAbilityModifier;
			}
		}
		public bool ModifiesExistingRoll { get; set; }
		public string Commands { get; set; }
		public Ability AttackingAbility { get; set; }
		public WeaponProperties WeaponProperties { get; set; }
		public string TrailingEffects { get; set; }
		public string DieRollEffects { get; set; }
		public string lastPrefix;

		public PlayerActionShortcut()
		{
			AttackingAbility = Ability.none;  // Calculated later.
			WeaponProperties = WeaponProperties.None;
			ModifiesExistingRoll = false;
			MinDamage = 0;
			UsesMagic = false;
			PlusModifier = 0;
			AttackingAbilityModifier = 0;
			ProficiencyBonus = 0;
			AttackingType = AttackType.None;
			Type = DiceRollType.None;
			Index = shortcutIndex;
			shortcutIndex++;
			Part = TurnPart.Action;
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
			if (time == null)
				return TurnPart.Action;

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
		
		void AddSpell(int spellSlotLevel, Character player, Spell spell)
		{
			Spell = spell.Clone(player, spellSlotLevel);
			Dice = Spell.DieStr;
			Part = DndUtils.ToTurnPart(Spell.CastingTime);
		}
		public void AddEffect(PlayerActionShortcutDto shortcutDto, string windupPrefix, Character player, int slotLevel = 0, bool isWindup = false)
		{
			if (isWindup)
			{
				if (!windupPrefix.StartsWith("Windup."))
					windupPrefix = "Windup." + windupPrefix;
			}
			
			lastPrefix = windupPrefix;
			int minSlotLevel = MathUtils.GetInt(shortcutDto.minSlotLevel);
			if (slotLevel < minSlotLevel)
				return;

			WindupDto windup = WindupDto.From(shortcutDto, player);

			if (windup == null)
				return;

			string shortcutName = shortcutDto.name;
			if (string.IsNullOrWhiteSpace(shortcutName))
				shortcutName = this.Name;

			windup.Name = windupPrefix + shortcutName;
			if (Spell?.Duration.HasValue() == true)
				windup.Lifespan = 0;
			Windups.Add(windup);
		}

		public static List<PlayerActionShortcut> From(PlayerActionShortcutDto shortcutDto)
		{
			List<PlayerActionShortcut> results = new List<PlayerActionShortcut>();
			string cleanName = DndUtils.GetCleanItemName(shortcutDto.name);
			Weapon weapon = AllWeapons.Get(cleanName);
			if (weapon == null && cleanName.IndexOf(" of ") > 0)
			{
				// Try again with the weapon...
				cleanName = cleanName.EverythingBefore(" of ");
				weapon = AllWeapons.Get(cleanName);
			}
			Character player = AllPlayers.GetFromId(PlayerID.FromName(shortcutDto.player));

			if (weapon != null)
			{
				AddWeaponShortcuts(shortcutDto, results, weapon, player);
			}
			else
			{
				List<Spell> spells = AllSpells.GetAll(cleanName, player);
				if (spells != null)
				{
					AddSpellShortcuts(shortcutDto, results, player, spells);
				}
				else // Not a weapon or a spell.
				{
					PlayerActionShortcut shortcut = FromAction(shortcutDto);
					results.Add(shortcut);
					shortcut.AddEffect(shortcutDto, "", player);

				}
			}

			return results;
		}

		private static void AddSpellShortcuts(PlayerActionShortcutDto shortcutDto, List<PlayerActionShortcut> results, Character player, List<Spell> spells)
		{
			foreach (Spell spell in spells)
			{
				if (spell.MorePowerfulWhenCastAtHigherLevels)
				{
					int[] spellSlotLevels = player.GetSpellSlotLevels();
					int availableSlots = spellSlotLevels[spell.Level];
					if (availableSlots > 0)
					{
						bool needToDisambiguateMultipleSpells = false;
						if (spell.Level < 9 && spellSlotLevels[spell.Level + 1] > 0)
							needToDisambiguateMultipleSpells = true;

						for (int slotLevel = spell.Level; slotLevel <= 9; slotLevel++)
						{
							if (spellSlotLevels[slotLevel] == 0)
								break;

							string suffix = string.Empty;
							if (needToDisambiguateMultipleSpells)
								suffix = $" [{slotLevel}]";

							results.Add(FromSpell(shortcutDto, player, spell, slotLevel, null, suffix));
						}
					}
					else
						results.Add(FromSpell(shortcutDto, player, spell));
				}
				else
					results.Add(FromSpell(shortcutDto, player, spell));
			}
		}

		private static void AddWeaponShortcuts(PlayerActionShortcutDto shortcutDto, List<PlayerActionShortcut> results, Weapon weapon, Character player)
		{
			if ((weapon.weaponProperties & WeaponProperties.Versatile) == WeaponProperties.Versatile)
			{
				if ((weapon.weaponProperties & WeaponProperties.Melee) == WeaponProperties.Melee &&
						(weapon.weaponProperties & WeaponProperties.Ranged) == WeaponProperties.Ranged)
				{
					results.Add(FromWeapon(weapon.Name, shortcutDto, player, weapon.damageOneHanded, " (1H Stabbed)", weapon.weaponProperties, AttackType.Melee));
					results.Add(FromWeapon(weapon.Name, shortcutDto, player, weapon.damageTwoHanded, " (2H Slice)", weapon.weaponProperties, AttackType.Melee));
					results.Add(FromWeapon(weapon.Name, shortcutDto, player, weapon.damageOneHanded, " (1H Thrown)", weapon.weaponProperties, AttackType.Range));
				}
				else
				{
					results.Add(FromWeapon(weapon.Name, shortcutDto, player, weapon.damageOneHanded, " (1H)", weapon.weaponProperties));
					results.Add(FromWeapon(weapon.Name, shortcutDto, player, weapon.damageTwoHanded, " (2H)", weapon.weaponProperties));
				}

			}
			else if ((weapon.weaponProperties & WeaponProperties.TwoHanded) == WeaponProperties.TwoHanded)
				results.Add(FromWeapon(weapon.Name, shortcutDto, player, weapon.damageTwoHanded, "", weapon.weaponProperties));
			else
				results.Add(FromWeapon(weapon.Name, shortcutDto, player, weapon.damageOneHanded, "", weapon.weaponProperties));
		}

		private static PlayerActionShortcut FromWeapon(string weaponName, PlayerActionShortcutDto shortcutDto, Character player, string damageStr = null, string suffix = "", WeaponProperties weaponProperties = WeaponProperties.None, AttackType attackType = AttackType.None)
		{
			PlayerActionShortcut result = FromAction(shortcutDto, damageStr, suffix);
			result.WeaponProperties = weaponProperties;

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
			result.AttackingType = attackType;

			if ((attackType & AttackType.Range) == AttackType.Range ||
				(attackType & AttackType.MartialRange) == AttackType.MartialRange ||
				(attackType & AttackType.Melee) == AttackType.Melee ||
				(attackType & AttackType.MartialMelee) == AttackType.MartialMelee)
			{
				if (player.IsProficientWith(weaponName))
					result.ProficiencyBonus = (int)Math.Round(player.proficiencyBonus);
			}
			result.UpdatePlayerAttackingAbility(player);

			result.ProcessDieStr(shortcutDto, damageStr);

			result.AddEffect(shortcutDto, WeaponWindupPrefix, player);

			return result;
		}

		public void UpdatePlayerAttackingAbility(Character player)
		{
			AttackingAbilityModifier = player.GetAttackingAbilityModifier(WeaponProperties, AttackingType);
			AttackingAbility = player.attackingAbility;
		}

		public static List<PlayerActionShortcut> FromItemSpellEffect(ItemEffect spellEffect, Character player)
		{
			List<PlayerActionShortcut> results = new List<PlayerActionShortcut>();
			
			List<Spell> spells = AllSpells.GetAll(spellEffect.name);

			foreach (Spell spell in spells)
			{
				PlayerActionShortcutDto dto = new PlayerActionShortcutDto();
				dto.name = spell.Name;
				dto.player = player.name;

				//dto.effectAvailableWhen = weaponEffect.effectAvailableWhen;

				SetDtoFromEffect(dto, spellEffect);
				if (spell.SpellType == SpellType.RangedSpell || spell.SpellType == SpellType.MeleeSpell)
					if (spell.Name == "Chaos Bolt")
						dto.type = DndUtils.DiceRollTypeToStr(DiceRollType.ChaosBolt);
					else
						dto.type = DndUtils.DiceRollTypeToStr(DiceRollType.Attack);
				else if (spell.SpellType == SpellType.SavingThrowSpell)
					dto.type = DndUtils.DiceRollTypeToStr(DiceRollType.DamageOnly);
				else if (spell.SpellType == SpellType.OtherSpell)
					dto.type = DndUtils.DiceRollTypeToStr(DiceRollType.None);
				else if (spell.SpellType == SpellType.HpCapacitySpell)
					dto.type = DndUtils.DiceRollTypeToStr(DiceRollType.ExtraOnly);
				else
				{
					dto.type = DndUtils.DiceRollTypeToStr(DiceRollType.None);
				}

				SetSpellCastingTime(dto, spell);
				List<Spell> oneSpell = new List<Spell>();
				oneSpell.Add(spell);
				AddSpellShortcuts(dto, results, player, oneSpell);
			}
			
			return results;
		}

		public static List<PlayerActionShortcut> FromItemWeaponEffect(ItemEffect weaponEffect, Character player, string weaponName)
		{
			List<PlayerActionShortcut> results = new List<PlayerActionShortcut>();
			Weapon weapon = AllWeapons.Get(weaponEffect.name);
			PlayerActionShortcutDto dto = new PlayerActionShortcutDto();
			if (!string.IsNullOrWhiteSpace(weaponName))
				dto.name = weaponName;
			else
				dto.name = weapon.Name;

			//dto.effectAvailableWhen = weaponEffect.effectAvailableWhen;

			dto.player = player.name;
			SetDtoFromEffect(dto, weaponEffect);
			dto.type = DndUtils.DiceRollTypeToStr(DiceRollType.Attack);

			SetWeaponHitTime(dto, weapon);
			AddWeaponShortcuts(dto, results, weapon, player);
			return results;
		}

		private static void SetDtoFromEffect(PlayerActionShortcutDto dto, ItemEffect itemEffect)
		{
			dto.brightness = itemEffect.brightness.ToString();
			dto.degreesOffset = itemEffect.degreesOffset.ToString();
			dto.dieRollEffects = itemEffect.dieRollEffects;
			dto.effect = itemEffect.effect;
			dto.endSound = itemEffect.endSound;
			dto.flipHorizontal = MathUtils.BoolToStr(itemEffect.flipHorizontal);
			dto.endSound = itemEffect.endSound;
			dto.hue = itemEffect.hue;
			dto.moveLeftRight = itemEffect.moveLeftRight.ToString();
			dto.moveUpDown = itemEffect.moveUpDown.ToString();
			dto.opacity = itemEffect.opacity.ToString();
			dto.playToEndOnExpire = MathUtils.BoolToStr(itemEffect.playToEndOnExpire);
			dto.rotation = itemEffect.rotation.ToString();
			dto.saturation = itemEffect.saturation.ToString();
			dto.scale = itemEffect.scale.ToString();
			dto.startSound = itemEffect.startSound;
			dto.trailingEffects = itemEffect.trailingEffects;
			dto.vantageMod = DndUtils.VantageToStr(VantageKind.Normal);
		}

		static void SetWeaponHitTime(PlayerActionShortcutDto dto, Weapon weapon)
		{
			
		}
		private static void SetSpellCastingTime(PlayerActionShortcutDto dto, Spell spell)
		{
			if (spell.CastingTime == DndTimeSpan.OneAction)
				dto.time = "1a";
			else if (spell.CastingTime == DndTimeSpan.OneBonusAction)
				dto.time = "1ba";
			else if (spell.CastingTime == DndTimeSpan.OneReaction)
				dto.time = "1r";
			else
				dto.time = "*";
		}

		private static PlayerActionShortcut FromSpell(PlayerActionShortcutDto shortcutDto, Character player, Spell spell, int slotLevelOverride = 0, string damageStr = null, string suffix = "")
		{
			PlayerActionShortcut result = FromAction(shortcutDto, damageStr, suffix, slotLevelOverride);
			result.UsesMagic = true;
			int spellSlotLevel = spell.Level;
			if (slotLevelOverride > 0)
				spellSlotLevel = slotLevelOverride;
			result.AddSpell(spellSlotLevel, player, spell);
			result.Description = spell.Description;
			result.AttackingAbility = player == null ? Ability.none : player.spellCastingAbility;
			result.ProcessDieStr(shortcutDto, damageStr);
			bool isWindup = result.Spell != null && !result.Spell.Duration.HasValue() && result.Spell.MustRollDiceToCast();
			result.AddEffect(shortcutDto, SpellWindupPrefix, player, spellSlotLevel, isWindup);
			return result;
		}

		private static PlayerActionShortcut FromAction(PlayerActionShortcutDto shortcutDto, string damageStr = "", string suffix = "", int slotLevel = 0)
		{
			PlayerActionShortcut result = new PlayerActionShortcut();
			result.Description = shortcutDto.description;
			result.AddDice = shortcutDto.addDice;
			result.AddDiceOnHit = shortcutDto.addDiceOnHit;
			result.AddDiceOnHitMessage = shortcutDto.addDiceOnHitMessage;
			result.AdditionalRollTitle = shortcutDto.addDiceTitle;

			result.MinDamage = MathUtils.GetInt(shortcutDto.minDamage);
			result.PlusModifier = MathUtils.GetInt(shortcutDto.plusModifier);
			result.Name = shortcutDto.name + suffix;

			result.Part = GetTurnPart(shortcutDto.time);
			result.PlayerId = PlayerID.FromName(shortcutDto.player);
			result.Type = GetDiceRollType(shortcutDto.type);
			result.VantageMod = DndUtils.ToVantage(shortcutDto.vantageMod);
			result.ModifiesExistingRoll = MathUtils.IsChecked(shortcutDto.rollMod);
			result.UsesMagic = MathUtils.IsChecked(shortcutDto.magic);
			result.Commands = shortcutDto.commands;
			result.SpellSlotLevel = slotLevel;
			//player.ResetPlayerTurnBasedState();
			result.ProcessDieStr(shortcutDto, damageStr);
			result.TrailingEffects = shortcutDto.trailingEffects;
			result.DieRollEffects = shortcutDto.dieRollEffects;

			return result;
		}

		void ProcessDieStr(PlayerActionShortcutDto shortcutDto, string damageStr)
		{
			string dieStr = shortcutDto.dieStr;

			if (!string.IsNullOrWhiteSpace(damageStr))
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

		void ExecuteCommand(string command, Character player)
		{
			Expressions.Do(command, player);
		}
		public void ExecuteCommands(Character player)
		{
			string[] commands = Commands.Split(';');
			foreach (string command in commands)
			{
				ExecuteCommand(command, player);
			}
		}

	}
}
