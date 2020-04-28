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
			return GetElement<DamageType>(damageStr);
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
				return default(T);
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
	}
}
