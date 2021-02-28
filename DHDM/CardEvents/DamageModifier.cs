//#define profiling
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DndCore;

namespace DHDM
{
	public class DamageModifier
	{
		public double offset {
			get
			{
				if (@operator == "x")
					return 0;
				if (@operator == "-")
					return -modifier;
				return modifier;
			}
		}

		public double multiplier {
			get
			{
				if (@operator == "x")
					return modifier;
				return 1;
			}
		}

		public double modifier { get; set; }
		public string damageType { get; set; }
		public string @operator { get; set; }
		public string modifierPart { get; set; }
		public CardModType CardModType
		{
			get
			{
				if (modifierPart == "attack" || modifierPart == "total")
					return CardModType.TotalScore;
				if (modifierPart == "damage")
					return CardModType.TotalDamage;
				return CardModType.TotalScorePlusDamage;
			}
		}

		/// <summary>
		/// Creates a new DamageModifier based on the specified input text.
		/// </summary>
		/// <param name="input">The input text to get a match for. For example, "+3(cold:both)".</param>
		/// <returns>Returns the new DamageModifier, or null if a no matches were found for the specified input.</returns>
		public static List<DamageModifier> Create(string input)
		{
			const string pattern = @"(?<operator>[+-x]?)(?<modifier>\d+)\((?<damageType>\w+):(?<modifierPart>(total)|(attack)|(damage)|(both))";

			Regex regex = new Regex(pattern);
			MatchCollection matches = regex.Matches(input);
			if (matches.Count == 0)
				return null;

			List<DamageModifier> damageModifiers = new List<DamageModifier>();

			foreach (Match match in matches)
			{
				DamageModifier damageModifier = new DamageModifier();
				damageModifier.@operator = RegexHelper.GetValue<string>(match, "operator");
				damageModifier.modifier = RegexHelper.GetValue<double>(match, "modifier");
				damageModifier.damageType = RegexHelper.GetValue<string>(match, "damageType");
				damageModifier.modifierPart = RegexHelper.GetValue<string>(match, "modifierPart");
				damageModifiers.Add(damageModifier);
			}
			return damageModifiers;
		}
	}
}
