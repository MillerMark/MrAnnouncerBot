using System;
using System.Linq;
using System.Collections.Generic;
using DndCore;

namespace DndCore
{
	public class PlayerActionShortcut
	{
		public HandsOnWeapon HandsOnWeapon { get; set; } = HandsOnWeapon.Zero;
		public const string SpellWindupPrefix = "Spell.";
		public const string WeaponWindupPrefix = "Weapon.";
		public const string STR_OtherPrefix = "Other.";
		static int shortcutIndex;
		public List<WindupDto> Windups { get; }
		public List<WindupDto> WindupsReversed
		{
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
		public string DamageDieBonus { get; set; }
		public int AttackingAbilityModifier { get; set; }
		public int ProficiencyBonus { get; set; }
		public string Dice { get; set; }
		public string DisplayText { get; set; }
		public bool UsesMagic { get; set; }
		public int PlayerId { get; set; }
		public int SpellSlotLevel { get; set; }
		public TurnPart Part { get; set; }

		public DiceRollType Type { get; set; }
		public int Index { get; set; }
		public string Description { get; set; }
		public VantageKind VantageMod { get; set; }
		public string AddDice { get; set; }
		public string WeaponDamage { get; set; }
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
		public CarriedWeapon CarriedWeapon { get; set; }
		public string AvailableWhen { get; set; }
		public Feature SourceFeature { get; set; }
		public bool Available { get; set; }
		public string lastPrefix;

		public PlayerActionShortcut()
		{
			AvailableWhen = string.Empty;
			AttackingAbility = Ability.none;  // Calculated later.
			WeaponProperties = WeaponProperties.None;
			ModifiesExistingRoll = false;
			MinDamage = 0;
			UsesMagic = false;
			PlusModifier = 0;
			DamageDieBonus = "";
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
			CarriedWeapon = null;
		}

		public static void PrepareForCreation()
		{
			shortcutIndex = 0;
		}

		void SetHueFromStr(WindupDto item, Character player, string hueStr)
		{
			if (item.Effect.EndsWith(".Magic") && player.OverrideWeaponMagicHue != int.MinValue)
			{
				item.Hue = player.OverrideWeaponMagicHue;
				return;
			}
			if (hueStr == null)
				hueStr = item.HueStr;
			if (hueStr == null)
				return;
			string hueTrim = hueStr.Trim();
			if (hueTrim.ToLower() == "player")
				item.Hue = player.hueShift;

			if (int.TryParse(hueTrim, out int result))
				item.Hue = result;
		}

		public List<WindupDto> GetAvailableWindups(Character player)
		{
			List<WindupDto> result = new List<WindupDto>();
			int index = 0;
			foreach (WindupDto windup in Windups)
			{
				if (windup != null)
					if (string.IsNullOrWhiteSpace(windup.EffectAvailableWhen) || Expressions.GetBool(windup.EffectAvailableWhen, player))
					{
						WindupDto item = windup.Clone();
						if (CarriedWeapon != null)
						{
							switch (index)
							{
								case 0:
									SetHueFromStr(item, player, CarriedWeapon.WeaponHue);
									break;
								case 1:
									SetHueFromStr(item, player, CarriedWeapon.Hue1);
									break;
								case 2:
									SetHueFromStr(item, player, CarriedWeapon.Hue2);
									break;
								case 3:
									SetHueFromStr(item, player, CarriedWeapon.Hue3);
									break;
							}
						}
						result.Add(item);
					}
				index++;
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

		public static DiceRollType GetDiceRollType(string type)
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

		// This seems to be the problem - we are adding effects to the PlayerActionShortcutDto instead of passing in an ItemEffect.
		public void AddEffect(PlayerActionShortcutDto shortcutDto, string windupPrefix, Character player, int slotLevel = 0, bool isWindup = false)
		{
			return;
			//if (isWindup)
			//{
			//	if (!windupPrefix.StartsWith("Windup."))
			//		windupPrefix = "Windup." + windupPrefix;
			//}
			//
			//lastPrefix = windupPrefix;
			//int minSlotLevel = MathUtils.GetInt(shortcutDto.minSlotLevel);
			//if (slotLevel < minSlotLevel)
			//	return;

			//WindupDto windup = WindupDto.From(shortcutDto, player);

			//if (windup == null)
			//	return;

			//string shortcutName = shortcutDto.name;
			//if (string.IsNullOrWhiteSpace(shortcutName))
			//	shortcutName = this.Name;

			//windup.Name = windupPrefix + shortcutName;
			//if (Spell?.Duration.HasValue() == true)
			//	windup.Lifespan = 0;
			//Windups.Add(windup);
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
			Character player = AllPlayers.GetFromName(shortcutDto.player);

			if (weapon != null)
			{
				AddWeaponShortcuts(shortcutDto, results, weapon, player);
			}
			else
			{
				List<Spell> spells = AllSpells.GetAll(cleanName, player);
				if (spells != null && spells.Count > 0)
				{
					AddSpellShortcuts(shortcutDto, results, player, spells);
				}
				else // Not a weapon or a spell.
				{
					PlayerActionShortcut shortcut = FromAction(shortcutDto);
					shortcut.ProcessDieStr(shortcutDto);
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
					int availableSlots = 0;
					if (spell.Level >= 0)
						availableSlots = spellSlotLevels[spell.Level];

					if (availableSlots > 0)
					{
						bool needToDisambiguateMultipleSpells = spell.Level < 9 && spellSlotLevels[spell.Level + 1] > 0;

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
						results.Add(FromSpell(shortcutDto, player, spell, spell.Level));
				}
				else
					results.Add(FromSpell(shortcutDto, player, spell, spell.Level));
			}
		}

		private static void AddWeaponShortcuts(PlayerActionShortcutDto shortcutDto, List<PlayerActionShortcut> results, Weapon weapon, Character player)
		{
			if ((weapon.weaponProperties & WeaponProperties.Versatile) == WeaponProperties.Versatile)
			{
				if ((weapon.weaponProperties & WeaponProperties.Melee) == WeaponProperties.Melee &&
						(weapon.weaponProperties & WeaponProperties.Ranged) == WeaponProperties.Ranged)
				{
					results.Add(FromWeapon(weapon.Name, shortcutDto, HandsOnWeapon.One, player, weapon.damageOneHanded, " (1H Stabbed)", weapon.weaponProperties, AttackType.Melee));
					results.Add(FromWeapon(weapon.Name, shortcutDto, HandsOnWeapon.Two, player, weapon.damageTwoHanded, " (2H Slice)", weapon.weaponProperties, AttackType.Melee));
					results.Add(FromWeapon(weapon.Name, shortcutDto, HandsOnWeapon.One, player, weapon.damageOneHanded, " (1H Thrown)", weapon.weaponProperties, AttackType.Range));
				}
				else
				{
					results.Add(FromWeapon(weapon.Name, shortcutDto, HandsOnWeapon.One, player, weapon.damageOneHanded, " (1H)", weapon.weaponProperties));
					results.Add(FromWeapon(weapon.Name, shortcutDto, HandsOnWeapon.Two, player, weapon.damageTwoHanded, " (2H)", weapon.weaponProperties));
				}

			}
			else if ((weapon.weaponProperties & WeaponProperties.TwoHanded) == WeaponProperties.TwoHanded)
				results.Add(FromWeapon(weapon.Name, shortcutDto, HandsOnWeapon.Two, player, weapon.damageTwoHanded, "", weapon.weaponProperties));
			else
				results.Add(FromWeapon(weapon.Name, shortcutDto, HandsOnWeapon.One, player, weapon.damageOneHanded, "", weapon.weaponProperties));
		}

		private static PlayerActionShortcut FromWeapon(string weaponName, PlayerActionShortcutDto shortcutDto, HandsOnWeapon handsOnWeapon, Character player, string weaponDamage = null, string suffix = "", WeaponProperties weaponProperties = WeaponProperties.None, AttackType attackType = AttackType.None)
		{
			PlayerActionShortcut result = FromAction(shortcutDto, weaponDamage, suffix, 0, player);
			result.HandsOnWeapon = handsOnWeapon;
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
			result.UpdatePlayerAttackingAbility(player, false);

			result.AddEffect(shortcutDto, WeaponWindupPrefix, player);

			return result;
		}

		public void UpdatePlayerAttackingAbility(Character player, bool isSpell)
		{
			if (isSpell)
			{
				AttackingAbilityModifier = player.SpellcastingAbilityModifier;
				AttackingAbility = player.spellCastingAbility;
			}
			else
			{
				AttackingAbilityModifier = player.GetAttackingAbilityModifier(WeaponProperties, AttackingType);
				AttackingAbility = player.attackingAbility;
			}
		}

		static void AddItemEffect(List<PlayerActionShortcut> shortcuts, ItemEffect itemEffect)
		{
			foreach (PlayerActionShortcut playerActionShortcut in shortcuts)
				playerActionShortcut.Windups.Add(WindupDto.FromItemEffect(itemEffect, playerActionShortcut.DisplayText));
		}

		public static PlayerActionShortcut FromSpell(string spellName, Character player, int spellSlotLevel = -1)
		{
			List<PlayerActionShortcut> allShortcuts = FromItemSpellEffect(spellName, null, player);
			if (allShortcuts == null || allShortcuts.Count == 0)
				return null;

			if (spellSlotLevel == -1)
				return allShortcuts[0];

			return allShortcuts.FirstOrDefault(x => x.SpellSlotLevel == -1 || x.SpellSlotLevel == spellSlotLevel);
		}

		public static List<PlayerActionShortcut> FromItemSpellEffect(string spellName, ItemEffect spellEffect, Character player)
		{
			List<PlayerActionShortcut> results = new List<PlayerActionShortcut>();

			List<Spell> spells = AllSpells.GetAll(spellName);

			foreach (Spell spell in spells)
			{
				PlayerActionShortcutDto dto = new PlayerActionShortcutDto();
				dto.name = spell.Name;
				dto.player = player.name;
				SetSpellCastingTime(dto, spell);

				//dto.effectAvailableWhen = weaponEffect.effectAvailableWhen;

				SetDtoFromEffect(dto, spellEffect);
				dto.type = GetDiceRollTypeStr(spell);
				List<Spell> oneSpell = new List<Spell>();
				oneSpell.Add(spell);
				AddSpellShortcuts(dto, results, player, oneSpell);
			}

			AddItemEffect(results, spellEffect);

			return results;
		}

		public static string GetDiceRollTypeStr(Spell spell)
		{
			if (spell.SpellType == SpellType.RangedSpell || spell.SpellType == SpellType.MeleeSpell)
				if (spell.Name == "Chaos Bolt")
					return DndUtils.DiceRollTypeToStr(DiceRollType.ChaosBolt);
				else
					return DndUtils.DiceRollTypeToStr(DiceRollType.Attack);
			else if (spell.SpellType == SpellType.SavingThrowSpell || spell.SpellType == SpellType.DamageSpell)
				return DndUtils.DiceRollTypeToStr(DiceRollType.DamageOnly);
			else if (spell.SpellType == SpellType.OtherSpell)
				return DndUtils.DiceRollTypeToStr(DiceRollType.None);
			else if (spell.SpellType == SpellType.HealingSpell)
				return DndUtils.DiceRollTypeToStr(DiceRollType.HealthOnly);
			else if (spell.SpellType == SpellType.HpCapacitySpell)
				return DndUtils.DiceRollTypeToStr(DiceRollType.ExtraOnly);
			else
			{
				return DndUtils.DiceRollTypeToStr(DiceRollType.None);
			}
		}

		public static List<PlayerActionShortcut> FromWeapon(CarriedWeapon carriedWeapon, ItemEffect weaponEffect, Character player)
		{
			List<PlayerActionShortcut> results = new List<PlayerActionShortcut>();
			//Weapon weapon = AllWeapons.Get(weaponEffect.name);
			Weapon weapon = AllWeapons.Get(carriedWeapon.Weapon.Name);
			PlayerActionShortcutDto dto = new PlayerActionShortcutDto();
			if (!string.IsNullOrWhiteSpace(carriedWeapon.Name))
				dto.name = carriedWeapon.Name;
			else
				dto.name = weapon.Name;

			//dto.effectAvailableWhen = weaponEffect.effectAvailableWhen;

			dto.player = player.name;
			SetDtoFromEffect(dto, weaponEffect);
			dto.type = DndUtils.DiceRollTypeToStr(DiceRollType.Attack);

			SetWeaponHitTime(dto, weapon);
			dto.plusModifier = carriedWeapon.HitPlusModifier.ToString();
			AddWeaponShortcuts(dto, results, weapon, player);
			foreach (PlayerActionShortcut playerActionShortcut in results)
			{
				playerActionShortcut.CarriedWeapon = carriedWeapon;
				playerActionShortcut.PlusModifier = carriedWeapon.HitPlusModifier;
				playerActionShortcut.DamageDieBonus = carriedWeapon.DamageDieBonus;
				playerActionShortcut.UsesMagic = carriedWeapon.HitPlusModifier > 0;
				playerActionShortcut.ProcessDieStr(dto, playerActionShortcut.WeaponDamage);
			}

			

			AddItemEffect(results, weaponEffect);
			return results;
		}

		// We may be able to safely remove this.
		private static void SetDtoFromEffect(PlayerActionShortcutDto dto, ItemEffect itemEffect)
		{
			//if (itemEffect == null)
			//	return;
			//dto.brightness = itemEffect.brightness.ToString();
			//dto.degreesOffset = itemEffect.degreesOffset.ToString();
			//dto.dieRollEffects = itemEffect.dieRollEffects;
			//dto.effect = itemEffect.effect;
			//dto.endSound = itemEffect.endSound;
			//dto.flipHorizontal = MathUtils.BoolToStr(itemEffect.flipHorizontal);
			//dto.endSound = itemEffect.endSound;
			//dto.hue = itemEffect.hue;
			//dto.moveLeftRight = itemEffect.moveLeftRight.ToString();
			//dto.moveUpDown = itemEffect.moveUpDown.ToString();
			//dto.opacity = itemEffect.opacity.ToString();
			//dto.playToEndOnExpire = MathUtils.BoolToStr(itemEffect.playToEndOnExpire);
			//dto.rotation = itemEffect.rotation.ToString();
			//dto.saturation = itemEffect.saturation.ToString();
			//dto.scale = itemEffect.scale.ToString();
			//dto.startSound = itemEffect.startSound;
			//dto.trailingEffects = itemEffect.trailingEffects;
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

		public static PlayerActionShortcut FromSpell(PlayerActionShortcutDto shortcutDto, Character player, Spell spell, int slotLevelOverride = 0, string damageStr = null, string suffix = "")
		{
			PlayerActionShortcut result = FromAction(shortcutDto, damageStr, suffix, slotLevelOverride, player);
			result.ProficiencyBonus = (int)Math.Round(player.proficiencyBonus);
			result.ProcessDieStr(shortcutDto, damageStr);
			result.Type = GetDiceRollType(GetDiceRollTypeStr(spell));
			result.UsesMagic = true;
			int spellSlotLevel = spell.Level;
			if (slotLevelOverride > 0)
				spellSlotLevel = slotLevelOverride;
			result.AddSpell(spellSlotLevel, player, spell);
			result.Description = spell.Description;
			result.AttackingAbility = player == null ? Ability.none : player.spellCastingAbility;
			result.ProcessDieStr(shortcutDto, damageStr);
			bool mustRollDiceToCast = result.Spell.MustRollDiceToCast();
			bool isWindup = result.Spell != null && !result.Spell.Duration.HasValue() && mustRollDiceToCast;
			result.AddEffect(shortcutDto, SpellWindupPrefix, player, spellSlotLevel, isWindup);
			if (!mustRollDiceToCast)
				result.Type = DiceRollType.CastSimpleSpell;

			if (player != null)
				result.AttackingAbilityModifier = player.GetSpellcastingAbilityModifier();

			return result;
		}

		public static PlayerActionShortcut FromAction(PlayerActionShortcutDto shortcutDto, string weaponDamage = "", string suffix = "", int slotLevel = 0, Character player = null)
		{
			PlayerActionShortcut result = new PlayerActionShortcut();
			result.WeaponDamage = weaponDamage;
			result.Description = shortcutDto.description;
			result.AddDice = shortcutDto.addDice;
			result.AddDiceOnHit = shortcutDto.addDiceOnHit;
			result.AddDiceOnHitMessage = shortcutDto.addDiceOnHitMessage;
			result.AdditionalRollTitle = shortcutDto.addDiceTitle;

			result.MinDamage = MathUtils.GetInt(shortcutDto.minDamage);
			result.PlusModifier = MathUtils.GetInt(shortcutDto.plusModifier);
			result.DisplayText = shortcutDto.name + suffix;

			result.Part = GetTurnPart(shortcutDto.time);
			if (player != null)
				result.PlayerId = player.playerID;
			else
				result.PlayerId = AllPlayers.GetPlayerIdFromName(shortcutDto.player);

			result.Type = GetDiceRollType(shortcutDto.type);
			result.VantageMod = DndUtils.ToVantage(shortcutDto.vantageMod);
			result.ModifiesExistingRoll = MathUtils.IsChecked(shortcutDto.rollMod);
			result.UsesMagic = MathUtils.IsChecked(shortcutDto.magic);
			result.Commands = shortcutDto.commands;
			result.SpellSlotLevel = slotLevel;
			result.TrailingEffects = shortcutDto.trailingEffects;
			result.DieRollEffects = shortcutDto.dieRollEffects;

			return result;
		}

		void ProcessDieStr(PlayerActionShortcutDto shortcutDto, string damageStr = null)
		{
			string dieStr = shortcutDto.dieStr;

			if (!string.IsNullOrWhiteSpace(damageStr))
				dieStr = damageStr;

			bool isInstantDice = dieStr.StartsWith("!");
			if (isInstantDice)
				dieStr = dieStr.Substring(1);

			DieRollDetails dieRollDetails = DieRollDetails.From(dieStr);
			if (!string.IsNullOrWhiteSpace(DamageDieBonus))
				dieRollDetails.AddRoll(DamageDieBonus);
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
			if (string.IsNullOrWhiteSpace(command))
				return;
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

		public static PlayerActionShortcut FromFeature(Feature feature, Character player, ActivationShortcutKind activationShortcutKind)
		{
			if (activationShortcutKind == ActivationShortcutKind.Deactivate)
				if (string.IsNullOrWhiteSpace(feature.DeactivateShortcutDisplayText))
					return null;

			string availableWhen;
			string displayText;
			TurnPart executionTime;
			string commands;
			if (activationShortcutKind == ActivationShortcutKind.Deactivate)
			{
				availableWhen = feature.DeactivateShortcutAvailableWhen;
				displayText = feature.DeactivateShortcutDisplayText;
				executionTime = feature.DeactivationTime;
				commands = $"DeactivateFeature({feature.Name})";
			}
			else
			{
				availableWhen = feature.ActivateShortcutAvailableWhen;
				displayText = feature.ActivateShortcutDisplayText;
				executionTime = feature.ActivationTime;
				commands = $"ActivateFeature({feature.Name})";
			}

			PlayerActionShortcut result = new PlayerActionShortcut();
			result.Description = feature.Description;

			if (string.IsNullOrEmpty(displayText))
				result.DisplayText = feature.Name;
			else
				result.DisplayText = displayText;
			result.AvailableWhen = availableWhen;
			result.UsesMagic = feature.Magic;
			result.ModifiesExistingRoll = feature.ModifiesExistingRoll;
			result.Part = executionTime;
			result.PlayerId = player.playerID;
			result.Commands = commands;
			result.SourceFeature = feature;
			return result;
		}
		public bool HasInstantDice()
		{
			return !string.IsNullOrWhiteSpace(InstantDice);
		}

	}
}
