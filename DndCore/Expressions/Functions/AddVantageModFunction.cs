using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Adds a specified vantage mod to the active magic owner (a CreaturePlusModId from Magic set in the Expression Evaluator's custom data).")]
	[Param(1, typeof(DiceRollType), "rollType", "The type of roll to mod.", ParameterIs.Required)]
	[Param(2, typeof(Skills), "skills", "Detail describing the roll type. For example, if the roll type is \"AbilityCheck\", this detail could be \"Strength\", \"Dexterity\", etc..", ParameterIs.Required)]
	[Param(3, typeof(string), "dieLabel", "Label for the advantage or disadvantage die added to the roll.", ParameterIs.Required)]
	[Param(4, typeof(int), "vantageOffset", "The offset of the vantage. Can be +1 for advantage, or -1 for disadvantage.", ParameterIs.Required)]
	public class AddVantageModFunction : DndFunction
	{
		public override string Name { get; set; } = "AddVantageMod";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, CastedSpell spell, RollResults dice = null)
		{
			CreaturePlusModId recipient = Expressions.GetCustomData<CreaturePlusModId>(evaluator.Variables);
			if (recipient == null)
				throw new Exception($"CreaturePlusModId recipient must be specified before evaluating expressions containing AddPropertyMod.");

			ExpectingArguments(args, 4);
			DiceRollType rollType = Expressions.Get<DiceRollType>(args[0], player, target, spell);
			Skills skills = Expressions.Get<Skills>(args[1].Trim(), player, target, spell);
			string dieLabel = args[2].Trim();
			if (dieLabel.StartsWith("\""))
				dieLabel = Expressions.GetStr(dieLabel, player, target, spell);
			int vantageOffset = Expressions.GetInt(args[3], player, target, spell);

			recipient.Creature.AddVantageMod(recipient.ID, rollType, skills, dieLabel, vantageOffset);
			return null;
		}
	}
}
