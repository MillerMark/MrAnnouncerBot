using System;
using System.Linq;
using System.Collections.Generic;

namespace DndCore
{
	/// <summary>
	/// Currency in the D&D game world. Enum values are number of those pieces per platinum piece.
	/// </summary>
	public enum Currency
	{
		None = 0,
		CopperPieces = 1000,
		SilverPieces = 100,
		ElectrumPieces = 20,
		GoldPieces = 10,
		PlatinumPieces = 1
	}

	public static class DndUtils
	{
		/// <summary>
		/// Returns the player or the in game creature based on the specified creatureId.
		/// </summary>
		/// <param name="creatureId">The id of the player (non-negative) or in game creature (negative).</param>
		public static Creature GetCreatureById(int creatureId)
		{
			if (creatureId >= 0)
				return AllPlayers.GetFromId(creatureId);
			else
			{
				InGameCreature inGameCreature = AllInGameCreatures.GetByIndex(-creatureId);
				if (inGameCreature != null)
					return inGameCreature.Creature;
			}
			return null;
		}

		public static int GetHueShift(SchoolOfMagic schoolOfMagic)
		{
			switch (schoolOfMagic)
			{
				case SchoolOfMagic.None:
					return 0;
				case SchoolOfMagic.Abjuration:
					return 191;
				case SchoolOfMagic.Illusion:
					return 238;
				case SchoolOfMagic.Conjuration:
					return 277;
				case SchoolOfMagic.Enchantment:
					return 331;
				case SchoolOfMagic.Necromancy:
					return 7;
				case SchoolOfMagic.Evocation:
					return 137;
				case SchoolOfMagic.Transmutation:
					return 101;
				case SchoolOfMagic.Divination:
					return 52;
				case SchoolOfMagic.Heavenly:
					return 44;
				case SchoolOfMagic.Satanic:
					return 352;
			}
			return 0;
		}

		public static string GetFirstName(string name)
		{
			if (name == null)
				return "No name";

			// TODO: this should be data driven.
			if (name.StartsWith("L'il "))
				return "Cutie";

			int spaceIndex = name.IndexOf(' ');
			if (spaceIndex < 0)
				return name;
			return name.Substring(0, spaceIndex);
		}

		public static string InjectParameters(string expression, List<string> parameters, List<string> arguments)
		{
			for (int i = 0; i < parameters.Count; i++)
			{
				string searchStr = parameters[i];
				if (i < arguments.Count)
				{
					string replaceStr = arguments[i];
					if (!string.IsNullOrEmpty(searchStr) && replaceStr != null)
						expression = expression.Replace(searchStr, replaceStr);
				}
			}
			return expression;
		}

		public static string InjectParameters(string str, List<string> parameters, string arguments)
		{
			string[] argumentList = arguments.Split(',');
			return InjectParameters(str, parameters, argumentList.ToList());
		}

		public static string GetName(string name)
		{
			if (name.IndexOf("(") >= 0)
				return name.EverythingBefore("(");
			return name;
		}

		public static List<string> GetParameters(string name)
		{
			List<string> result = new List<string>();
			if (name.IndexOf("(") < 0)
				return result;

			char[] trimChars = { ')', ' ', '\t' };
			string parameters = name.EverythingAfter("(").Trim(trimChars);
			string[] allParameters = parameters.Split(',');
			foreach (string parameter in allParameters)
			{
				result.Add(parameter.Trim());
			}
			return result;

		}

		static string SpellSlotLevelToFieldName(int slotLevel)
		{
			if (slotLevel == 0)
				return "CantripsKnown";
			return $"Slot{slotLevel}Spells";
		}

		public static int GetAvailableSpellSlots(string className, int level, int slotLevel)
		{
			object data = AllTables.GetData(className, SpellSlotLevelToFieldName(slotLevel), "Level", level);
			if (data == null)
				return 0;
			if (data is string dataStr)
			{
				if (int.TryParse(dataStr, out int result))
					return result;
			}

			if (data is int)
				return (int)data;

			return 0;
		}

		public static double PlatinumPiecesToGoldPieces(double platinumPieces)
		{
			return platinumPieces * 10;
		}
		public static double GetGoldPieces(string cost)
		{
			string costStr = cost.ToLower().Trim();
			Currency currency = Currency.None;
			if (costStr.EndsWith("gp"))
				currency = Currency.GoldPieces;
			else if (costStr.EndsWith("ep"))
				currency = Currency.ElectrumPieces;
			else if (costStr.EndsWith("sp"))
				currency = Currency.SilverPieces;
			else if (costStr.EndsWith("cp"))
				currency = Currency.CopperPieces;
			else if (costStr.EndsWith("pp"))
				currency = Currency.PlatinumPieces;

			if (currency == Currency.None)
				currency = Currency.GoldPieces;

			double firstDouble = costStr.GetFirstDouble();
			double platinumPieces = firstDouble / (double)currency;

			return PlatinumPiecesToGoldPieces(platinumPieces);
		}

		public static Skills ToSkill(string skillStr)
		{
			return GetElement<Skills>(skillStr);
		}

		public static Ability ToAbility(string abilityStr)
		{
			return GetElement<Ability>(abilityStr);
		}

		public static DamageType ToDamage(string damageStr)
		{
			if (damageStr.StartsWith("(") && damageStr.EndsWith(")"))
				damageStr = damageStr.Substring(1, damageStr.Length - 2);
			return GetElement<DamageType>(damageStr);
		}

		public static CardModType ToCardModType(string cardModType)
		{
			return GetElement<CardModType>(cardModType);
		}

		public static Conditions ToCondition(string condition)
		{
			return GetElement<Conditions>(condition);
		}


		public static string ToSkillDisplayString(Skills skill)
		{
			switch (skill)
			{
				case Skills.none:
					return "none";
				case Skills.acrobatics:
					return "Acrobatics";
				case Skills.animalHandling:
					return "Animal Handling";
				case Skills.arcana:
					return "Arcana";
				case Skills.athletics:
					return "Athletics";
				case Skills.deception:
					return "Deception";
				case Skills.history:
					return "History";
				case Skills.insight:
					return "Insight";
				case Skills.intimidation:
					return "Intimidation";
				case Skills.investigation:
					return "Investigation";
				case Skills.medicine:
					return "Medicine";
				case Skills.nature:
					return "Nature";
				case Skills.perception:
					return "Perception";
				case Skills.performance:
					return "Performance";
				case Skills.persuasion:
					return "Persuasion";
				case Skills.religion:
					return "Religion";
				case Skills.sleightOfHand:
					return "Sleight of Hand";
				case Skills.stealth:
					return "Stealth";
				case Skills.survival:
					return "Survival";
				case Skills.strength:
					return "Strength";
				case Skills.dexterity:
					return "Dexterity";
				case Skills.constitution:
					return "Constitution";
				case Skills.intelligence:
					return "Intelligence";
				case Skills.wisdom:
					return "Wisdom";
				case Skills.charisma:
					return "Charisma";
			}
			return "?";
		}
		public static string ToArticlePlusSkillDisplayString(Skills skill)
		{
			switch (skill)
			{
				case Skills.acrobatics:
				case Skills.animalHandling:
				case Skills.arcana:
				case Skills.athletics:
				case Skills.insight:
				case Skills.intimidation:
				case Skills.investigation:
				case Skills.intelligence:
					return "an " + ToSkillDisplayString(skill);
			}
			return "a " + ToSkillDisplayString(skill);
		}

		public static string ToAbilityDisplayString(Ability ability)
		{
			switch (ability)
			{
				case Ability.none:
					return "None";
				case Ability.strength:
					return "Strength";
				case Ability.dexterity:
					return "Dexterity";
				case Ability.constitution:
					return "Constitution";
				case Ability.intelligence:
					return "Intelligence";
				case Ability.wisdom:
					return "Wisdom";
				case Ability.charisma:
					return "Charisma";
			}
			return "?";
		}

		public static string GetModStr(int modifier)
		{
			if (modifier <= 0)
				return modifier.ToString();
			return $"+{modifier}";
		}

		public static string ToSpellcastingAbilityDisplayString(Ability ability, int modifier)
		{
			string modifierStr = GetModStr(modifier);
			switch (ability)
			{
				case Ability.strength:
					return $"Str ({modifierStr})";
				case Ability.dexterity:
					return $"Dex ({modifierStr})";
				case Ability.constitution:
					return $"Con ({modifierStr})";
				case Ability.intelligence:
					return $"Int ({modifierStr})";
				case Ability.wisdom:
					return $"Wis ({modifierStr})";
				case Ability.charisma:
					return $"Chr ({modifierStr})";
			}
			return modifierStr;
		}

		public static string ToArticlePlusAbilityDisplayString(Ability ability)
		{
			if (ability == Ability.intelligence)
				return "an " + ToAbilityDisplayString(ability);
			return "a " + ToAbilityDisplayString(ability);
		}

		public static VantageKind ToVantage(string vantageStr)
		{
			return GetElement<VantageKind>(vantageStr);
		}

		public static TurnPart ToTurnPart(DndTimeSpan time)
		{
			if (time == DndTimeSpan.OneAction)
				return TurnPart.Action;
			if (time == DndTimeSpan.OneBonusAction)
				return TurnPart.BonusAction;
			if (time == DndTimeSpan.OneReaction)
				return TurnPart.Reaction;
			return TurnPart.Special;
		}

		private static T GetElement<T>(string elementName) where T : struct
		{
			if (Enum.TryParse(elementName.Trim().Replace(" ", ""), true, out T result))
				return result;
			else
				return default;
		}

		public static Weapons ToWeapon(string weaponStr)
		{
			return GetElement<Weapons>(weaponStr);
		}

		public static string GetCleanItemName(string name)
		{
			if (name.IndexOf(',') > 0)
				name = name.EverythingBefore(",");
			if (name.IndexOf('(') > 0)
				name = name.EverythingBefore("(");
			if (name.IndexOf('[') > 0)
				name = name.EverythingBefore("[");
			return name.Trim();
		}

		public static bool CanCastSpells(string className)
		{
			return GetSpellCastingAbility(className) != Ability.none;
		}

		public static Ability GetSpellCastingAbility(string className)
		{
			// TODO: Put this in a table, Mark. Come on!!!
			switch (className.ToLower())
			{
				case "sorcerer":
				case "bard":
				case "favored soul":
					return Ability.charisma;
				case "wizard":
				case "arcanetrickster":
					return Ability.intelligence;
				case "cleric":
				case "druid":
				case "paladin":
				case "ranger":
				case "spirit shaman":
					return Ability.wisdom;
			}
			return Ability.none;
		}
		public static bool IsAttack(DiceRollType type)
		{
			return type == DiceRollType.Attack || type == DiceRollType.ChaosBolt;
		}
		public static string VantageToStr(VantageKind vantageKind)
		{
			return Enum.GetName(typeof(VantageKind), vantageKind); ;
		}

		public static string DiceRollTypeToStr(DiceRollType diceRollType)
		{
			return Enum.GetName(typeof(DiceRollType), diceRollType);
		}
		public static string ToVarName(string itemName)
		{
			string[] words = itemName.Split(' ');
			for (int i = 0; i < words.Length; i++)
			{
				words[i] = words[i].InitialCap();
			}
			return string.Join("", words);
		}

		public static string GetSpellSlotLevelKey(int spellSlotLevel)
		{
			return $"SpellSlots{spellSlotLevel}";
		}

		public static string GetOrdinal(int i)
		{
			switch (i)
			{
				case 1: return "1st";
				case 2: return "2nd";
				case 3: return "3rd";
			}

			return $"{i}th";
		}

		public static Class ToClass(string name)
		{
			string matchingName = name.ToLower();
			switch (matchingName)
			{
				case "artificer": return Class.Artificer;
				case "barbarian": return Class.Barbarian;
				case "bard": return Class.Bard;
				case "bloodHunter": return Class.BloodHunter;
				case "cleric": return Class.Cleric;
				case "druid": return Class.Druid;
				case "fighter": return Class.Fighter;
				case "monk": return Class.Monk;
				case "paladin": return Class.Paladin;
				case "ranger": return Class.Ranger;
				case "rogue": return Class.Rogue;
				case "sorcerer": return Class.Sorcerer;
				case "warlock": return Class.Warlock;
				case "wizard": return Class.Wizard;
			}
			return Class.None;
		}

		public static SubClass ToSubClass(string name)
		{
			string matchingName = name.Replace(" ", "").Replace("/", "_").ToLower();
			Array values = Enum.GetValues(typeof(SubClass));
			foreach (SubClass value in values)
			{
				string compareValue = value.ToString().ToLower();
				if (matchingName == compareValue)
					return value;
			}

			return SubClass.None;
		}

		public static SchoolOfMagic ToSchoolOfMagic(string school)
		{
			if (school == null)
				return SchoolOfMagic.None;
			switch (school.Trim().ToLower())
			{
				case "abjuration": return SchoolOfMagic.Abjuration;
				case "illusion": return SchoolOfMagic.Illusion;
				case "conjuration": return SchoolOfMagic.Conjuration;
				case "enchantment": return SchoolOfMagic.Enchantment;
				case "necromancy": return SchoolOfMagic.Necromancy;
				case "evocation": return SchoolOfMagic.Evocation;
				case "transmutation": return SchoolOfMagic.Transmutation;
				case "divination": return SchoolOfMagic.Divination;
				case "heavenly": return SchoolOfMagic.Heavenly;
				case "satanic": return SchoolOfMagic.Satanic;
			}
			return SchoolOfMagic.None;
		}

		public static string ToWordStr(int count)
		{
			switch (count)
			{
				case 0: return "zero";
				case 1: return "one";
				case 2: return "two";
				case 3: return "three";
				case 4: return "four";
				case 5: return "five";
				case 6: return "six";
				case 7: return "seven";
				case 8: return "eight";
				case 9: return "nine";
				case 10: return "ten";
				case 11: return "eleven";
				case 12: return "twelve";
				case 13: return "thirteen";
				case 14: return "fourteen";
				case 15: return "fifteen";
				case 16: return "sixteen";
				case 17: return "seventeen";
				case 18: return "eighteen";
				case 19: return "nineteen";
				case 20: return "twenty";
			}
			return count.ToString();
		}
		public static int DamageTypeToIndex(DamageType damageType)
		{
			switch (damageType)
			{
				case DamageType.Acid: return 1;
				case DamageType.Bludgeoning: return 2;
				case DamageType.Cold: return 3;
				case DamageType.Fire: return 4;
				case DamageType.Force: return 5;
				case DamageType.Lightning: return 6;
				case DamageType.Necrotic: return 7;
				case DamageType.Piercing: return 8;
				case DamageType.Poison: return 9;
				case DamageType.Psychic: return 10;
				case DamageType.Radiant: return 11;
				case DamageType.Slashing: return 12;
				case DamageType.Thunder: return 13;
				default:
					return 0;
			}
		}

		public static int SavingThrowToIndex(Ability savingThrow)
		{
			switch (savingThrow)
			{
				case Ability.strength: return 1;
				case Ability.dexterity: return 2;
				case Ability.constitution: return 3;
				case Ability.intelligence: return 4;
				case Ability.wisdom: return 5;
				case Ability.charisma: return 6;
				default:
					return 0;
			}
		}

		public static string Plural(int count, string suffix)
		{
			if (count == 1)
				return $"{count} {suffix}";
			return $"{count} {suffix}s";
		}

		public static string GetTimeSpanStr(TimeSpan time)
		{
			string result;
			if (time.TotalDays >= 1)
				result = $"{Plural(time.Days, "day")}, {Plural(time.Hours, "hour")}, {Plural(time.Minutes, "minute")}, {Plural(time.Seconds, "second")}";
			else if (time.TotalHours >= 1)
				result = $"{Plural(time.Hours, "hour")}, {Plural(time.Minutes, "minute")}, {Plural(time.Seconds, "second")}";
			else if (time.TotalMinutes >= 1)
				result = $"{Plural(time.Minutes, "minute")}, {Plural(time.Seconds, "second")}";
			else
				result = Plural(time.Seconds, "second");
			return result;
		}

		public static DiceRollType ToDiceRollType(string value)
		{
			return GetElement<DiceRollType>(value);
		}

		public static Alignment ToAlignment(string alignmentStr)
		{
			string matchingName = alignmentStr.Replace(" ", "").ToLower();
			if (matchingName == "neutral")
				matchingName = "trueneutral";
			Array values = Enum.GetValues(typeof(Alignment));
			foreach (Alignment value in values)
			{
				string compareValue = value.ToString().ToLower();
				if (matchingName == compareValue)
					return value;
			}
			return Alignment.Unaligned;
		}

		public static CreatureSize ToCreatureSize(string sizeStr)
		{
			string matchingName = sizeStr.ToLower();
			Array values = Enum.GetValues(typeof(CreatureSize));
			foreach (CreatureSize value in values)
			{
				string compareValue = value.ToString().ToLower();
				if (matchingName == compareValue)
					return value;
			}
			return CreatureSize.None;
		}

		public static CreatureKinds ToCreatureKind(string kindStr)
		{
			kindStr = kindStr.Trim('"');
			string matchingName = kindStr.ToLower();
			Array values = Enum.GetValues(typeof(CreatureKinds));
			foreach (CreatureKinds value in values)
			{
				string compareValue = value.ToString().ToLower();
				if (matchingName == compareValue)
					return value;
			}
			return CreatureKinds.None;
		}

		public static TurnSpecifier GetTurnSpecifier(string duration)
		{
			if (duration.Contains("start of turn"))
				return TurnSpecifier.StartOfTurn;
			if (duration.Contains("end of turn"))
				return TurnSpecifier.EndOfTurn;
			return TurnSpecifier.None;
		}

		public static bool IsSkillAGenericAbility(Skills matchSkill)
		{
			switch (matchSkill)
			{
				case Skills.strength:
				case Skills.dexterity:
				case Skills.constitution:
				case Skills.intelligence:
				case Skills.wisdom:
				case Skills.charisma:
					return true;
			}
			return false;
		}

		public static Skills FromSkillToAbility(Skills skill)
		{
			switch (skill)
			{
				case Skills.none:
				case Skills.strength:
				case Skills.dexterity:
				case Skills.constitution:
				case Skills.intelligence:
				case Skills.wisdom:
				case Skills.charisma:
					return skill;

				case Skills.acrobatics:
				case Skills.sleightOfHand:
				case Skills.stealth:
					return Skills.dexterity;

				case Skills.animalHandling:
				case Skills.insight:
				case Skills.medicine:
				case Skills.perception:
				case Skills.survival:
					return Skills.wisdom;

				case Skills.arcana:
				case Skills.history:
				case Skills.investigation:
				case Skills.nature:
				case Skills.religion:
					return Skills.intelligence;

				case Skills.athletics:
					return Skills.strength;

				case Skills.deception:
				case Skills.intimidation:
				case Skills.performance:
				case Skills.persuasion:
					return Skills.charisma;
			}
			return Skills.none;
		}

		public static int HalveValue(int value)
		{
			return (int)Math.Floor(value / 2.0);
		}

		public static int HalveValue(double value)
		{
			return (int)Math.Floor(value / 2.0);
		}
		public static double GetSpellPercentComplete(CastedSpell castedSpell, DndGame dndGame)
		{
			DndTimeSpan duration = castedSpell.Spell.Duration;
			TimeSpan spellDuration;
			switch (duration.TimeMeasure)
			{
				case TimeMeasure.seconds:
				case TimeMeasure.minutes:
				case TimeMeasure.hours:
				case TimeMeasure.days:
					spellDuration = duration.GetTimeSpan();
					return GetPercentCompleteBasedOnSpellDuration(castedSpell, dndGame, spellDuration);
				case TimeMeasure.forever:
					return 50;
				case TimeMeasure.never:
					return 0;
				case TimeMeasure.round:
					// Special case.
					if (castedSpell.CastingRound < 0)
					{
						spellDuration = TimeSpan.FromSeconds(6 * duration.Count);
						return GetPercentCompleteBasedOnSpellDuration(castedSpell, dndGame, spellDuration);
					}
					else
					{
						double roundsSinceCast = dndGame.RoundNumber - castedSpell.CastingRound;
						double turnsSinceCast = dndGame.InitiativeIndex - castedSpell.CastingTurnIndex;
						if (turnsSinceCast < 0)
						{
							turnsSinceCast += dndGame.PlayerCount;
							roundsSinceCast--;
						}

						turnsSinceCast += roundsSinceCast * dndGame.PlayerCount;
						double durationTurns = duration.Count * dndGame.PlayerCount;
						return Math.Max(0.0, Math.Min(100.0, 100.0 * turnsSinceCast / durationTurns));
					}
				case TimeMeasure.actions:
				case TimeMeasure.bonusActions:
				case TimeMeasure.reaction:

					break;
			}

			throw new NotImplementedException();
		}

		private static double GetPercentCompleteBasedOnSpellDuration(CastedSpell castedSpell, DndGame dndGame, TimeSpan spellDuration)
		{
			TimeSpan timeActive = dndGame.Clock.Time - castedSpell.CastingTime;
			return 100.0 * timeActive.TotalSeconds / spellDuration.TotalSeconds;
		}

		public static DamageType GetChaosBoltDamage(int value)
		{
			switch (value)
			{
				case 1:
					return DamageType.Acid;
				case 2:
					return DamageType.Cold;
				case 3:
					return DamageType.Fire;
				case 4:
					return DamageType.Force;
				case 5:
					return DamageType.Lightning;
				case 6:
					return DamageType.Poison;
				case 7:
					return DamageType.Psychic;
				case 8:
					return DamageType.Thunder;
			}
			return DamageType.None;
		}

		public static WhatSide GetSide(string value)
		{
			string lowerTargetType = value.ToLower();
			if (lowerTargetType.Contains("foe"))
				return WhatSide.Enemy;
			return GetElement<WhatSide>(value);
		}

		/// <summary>
		/// Converts a scene name with a one-of-many index count at the end (e.g., "DanceParty[3]"), into one of
		/// "DanceParty1", "DanceParty2", or "DanceParty3". Escape characters ("\") placed before the brackets will 
		/// be converted into actual brackets.
		/// </summary>
		public static string GetRandomSceneIfNecessary(string sceneName)
		{
			int bracketStart = sceneName.IndexOf("[");
			if (sceneName.EndsWith("]") && bracketStart > 0)
			{
				if (sceneName.Contains("\\["))
				{
					sceneName = sceneName.Replace("\\[", "[").Replace("\\]", "]");
				}
				else
				{
					string numSceneStr = sceneName.Substring(bracketStart + 1);
					sceneName = sceneName.Substring(0, bracketStart);
					numSceneStr = numSceneStr.Substring(0, numSceneStr.Length - 1);
					if (int.TryParse(numSceneStr, out int numScenes))
					{
						int sceneNumber = new Random().Next(numScenes) + 1;
						sceneName += sceneNumber;
					}
				}
			}
			return sceneName;
		}

		public static string ToDamageStr(DamageType damageType)
		{
			switch (damageType)
			{
				case DamageType.None:
					return string.Empty;
				case DamageType.Acid:
					return "acid";
				case DamageType.Bludgeoning:
					return "bludgeoning";
				case DamageType.Cold:
					return "cold";
				case DamageType.Fire:
					return "fire";
				case DamageType.Force:
					return "force";
				case DamageType.Lightning:
					return "lightning";
				case DamageType.Necrotic:
					return "necrotic";
				case DamageType.Piercing:
					return "piercing";
				case DamageType.Poison:
					return "poison";
				case DamageType.Psychic:
					return "psychic";
				case DamageType.Radiant:
					return "radiant";
				case DamageType.Slashing:
					return "slashing";
				case DamageType.Thunder:
					return "thunder";
				case DamageType.Superiority:
					return "superiority";
				case DamageType.Condition:
					return "condition";
				case DamageType.Bane:
					return "bane";
				case DamageType.Bless:
					return "bless";
			}
			return string.Empty;
		}

		public static string ToSkillItemName(string skillCheck)
		{
			Skills skill = ToSkill(skillCheck);
			switch (skill)
			{
				case Skills.strength:
				case Skills.dexterity:
				case Skills.constitution:
				case Skills.intelligence:
				case Skills.wisdom:
				case Skills.charisma:
					return skill.ToString().InitialCap();
				case Skills.acrobatics:
				case Skills.animalHandling:
				case Skills.arcana:
				case Skills.athletics:
				case Skills.deception:
				case Skills.history:
				case Skills.insight:
				case Skills.intimidation:
				case Skills.investigation:
				case Skills.medicine:
				case Skills.nature:
				case Skills.perception:
				case Skills.performance:
				case Skills.persuasion:
				case Skills.religion:
				case Skills.sleightOfHand:
				case Skills.stealth:
				case Skills.survival:
					return "Skills" + skill.ToString().InitialCap();
			}
			return string.Empty;
		}
		public static ScrollPage ToScrollPage(string scrollPage)
		{
			return GetElement<ScrollPage>(scrollPage);
		}

		public static Ability ToAbility(Skills skill)
		{
			switch (skill)
			{
				case Skills.strength:
				case Skills.athletics:
					return Ability.strength;

				case Skills.dexterity:
				case Skills.acrobatics:
				case Skills.sleightOfHand:
				case Skills.stealth:
					return Ability.dexterity;

				case Skills.constitution:
					return Ability.constitution;

				case Skills.intelligence:
				case Skills.arcana:
				case Skills.history:
				case Skills.investigation:
				case Skills.nature:
				case Skills.religion:
					return Ability.intelligence;

				case Skills.wisdom:
				case Skills.animalHandling:
				case Skills.insight:
				case Skills.medicine:
				case Skills.perception:
				case Skills.survival:
					return Ability.wisdom;

				case Skills.charisma:
				case Skills.deception:
				case Skills.intimidation:
				case Skills.performance:
				case Skills.persuasion:
					return Ability.charisma;
			}
			return Ability.none;
		}

		public static TargetDetails ToTargetDetails(string targetData)
		{
			return new TargetDetails(targetData);
		}

		public static SpellTargetShape ToShape(string shapeName)
		{
			if (shapeName == null)
				return SpellTargetShape.None;
			string lowerShapeName = shapeName.Trim().ToLower();
			switch (lowerShapeName)
			{
				case "cube":
					return SpellTargetShape.Cube;
				case "sphere":
					return SpellTargetShape.Sphere;
				case "cone":
					return SpellTargetShape.Cone;
				case "line":
				case "ray":
					return SpellTargetShape.Line;
				case "wall":
					return SpellTargetShape.Wall;
				case "point":
					return SpellTargetShape.Point;
				case "square":
					return SpellTargetShape.Square;
				case "circle":
					return SpellTargetShape.Circle;
				case "cylinder":
					return SpellTargetShape.Cylinder;
			}
			return SpellTargetShape.None;
		}
		public static double TilesToFeet(double distanceTiles)
		{
			return distanceTiles * 5;
		}
	}
}
