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
		public static string InjectParameters(string expression, List<string> parameters, List<string> arguments)
		{
			for (int i = 0; i < parameters.Count; i++)
			{
				string searchStr = parameters[i];
				string replaceStr = arguments[i];
				expression = expression.Replace(searchStr, replaceStr);
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

		public static int GetAvailableSpellSlots(string className, int level, int slotLevel)
		{
			object data = AllTables.GetData(className, $"Slot{slotLevel}Spells", "Level", level);
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

		public static int GetAvailableSpellSlots(CharacterClass characterClass, int slotLevel)
		{
			return GetAvailableSpellSlots(characterClass.Name, characterClass.Level, slotLevel);
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

		static string ToAbilityDisplayString(Ability ability)
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
			switch (className.ToLower())
			{
				case "sorcerer":
				case "bard":
				case "favored soul":
					return Ability.charisma;
				case "wizard":
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
	}
}
