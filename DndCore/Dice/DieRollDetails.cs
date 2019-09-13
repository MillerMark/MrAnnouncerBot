using System;
using System.Linq;
using System.Collections.Generic;

namespace DndCore
{
	public class DieRollDetails
	{
		List<Roll> rolls = new List<Roll>();

		public DieRollDetails()
		{

		}
		public override string ToString()
		{
			string result = string.Empty;
			
			foreach (Roll roll in rolls)
			{
				if (!string.IsNullOrWhiteSpace(result))
					result += ",";
				result += roll.ToString();
			}
			return result;
		}
		
		public Roll GetRoll(int sides)
		{
			return rolls.FirstOrDefault(x => x.Sides == sides);
		}
		
		public List<Roll> Rolls { get => rolls; set => rolls = value; }

		public void AddRoll(string dieStr, int spellcastingAbilityModifier = int.MinValue)
		{
			if (string.IsNullOrEmpty(dieStr))
				return;
			Roll roll = Roll.From(dieStr, spellcastingAbilityModifier);
			if (roll == null)
				return;
			rolls.Add(roll);
		}
		
		public static DieRollDetails From(string str, int spellcastingAbilityModifier = int.MinValue)
		{
			DieRollDetails dieRollDetails = new DieRollDetails();
			string[] split = str.Split(',');
			foreach (string dieStr in split)
				dieRollDetails.AddRoll(dieStr, spellcastingAbilityModifier);
			return dieRollDetails;
		}
		
	}
}
