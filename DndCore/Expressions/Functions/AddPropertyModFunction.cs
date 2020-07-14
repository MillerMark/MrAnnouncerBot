using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Adds a specified property mod to the active magic owner (a CreaturePlusModId from Magic set in the Expression Evaluator's custom data).")]
	[Param(1, typeof(string), "propertyName", "The name of the property to mod.", ParameterIs.Required)]
	[Param(2, typeof(double), "offset", "The offset to add to the property.", ParameterIs.Required)]
	[Param(3, typeof(double), "multiplier", "The multiplier to multiply the property by. Defaults to one if not specified.", ParameterIs.Optional)]
	public class AddPropertyModFunction : DndFunction
	{
		public override string Name { get; set; } = "AddPropertyMod";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Target target, CastedSpell spell, DiceStoppedRollingData dice = null)
		{
			CreaturePlusModId recipient = Expressions.GetCustomData<CreaturePlusModId>(evaluator.Variables);
			if (recipient == null)
				throw new Exception($"CreaturePlusModId recipient must be specified before evaluating expressions containing AddPropertyMod.");
			
			ExpectingArguments(args, 2, 3);
			string propertyName = string.Empty;
			if (args[0] is string)
				propertyName = args[0];
			double offset = Expressions.GetDouble(args[1], player, target, spell);
			double multiplier = 1;
			if (args.Count == 3)
				multiplier = Expressions.GetDouble(args[2], player, target, spell);

			recipient.Creature.AddPropertyMod(propertyName, recipient.ID, offset, multiplier);
			return null;
		}
	}
}
