using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Removes a specified property mod from the active magic owner (a Creature set in the Expression Evaluator's custom data).")]
	[Param(1, typeof(string), "propertyName", "The name of the property to mod.", ParameterIs.Required)]
	[Param(2, typeof(string), "id", "The unique identity of the mod to remove.", ParameterIs.Required)]
	public class RemovePropertyModFunction : DndFunction
	{
		public override string Name { get; set; } = "RemovePropertyMod";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, CastedSpell spell, DiceStoppedRollingData dice = null)
		{
			Creature modOwner = Expressions.GetCustomData<Creature>(evaluator.Variables);
			if (modOwner == null)
				throw new Exception($"Creature mod owner must be specified before evaluating expressions containing RemovePropertyMod.");

			ExpectingArguments(args, 2);
			string propertyName = Expressions.GetStr(args[0], player, target, spell);
			string id = Expressions.GetStr(args[1], player, target, spell);
			modOwner.RemovePropertyMod(propertyName, id);
			return null;
		}
	}
}
