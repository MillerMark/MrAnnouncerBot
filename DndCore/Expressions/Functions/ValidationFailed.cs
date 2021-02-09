using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Stops spellcasting if the target count matches the specified value using the specified comparison operation.")]
	[Param(1, typeof(string), "dungeonMasterMessage", "The text to send the DM.", ParameterIs.Required)]
	[Param(2, typeof(string), "floatText", "The text to float above the spellcaster.", ParameterIs.Required)]
	[Param(3, typeof(ValidationAction), "validationAction", "What to do (Error or Warn).", ParameterIs.Required)]
	public class ValidationFailed : DndFunction
	{
		public override string Name { get; set; } = "ValidationFailed";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, CastedSpell spell, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 3);
			string dungeonMasterMessage = Expressions.GetStr(args[0], player, target, spell, dice);
			string floatText = Expressions.GetStr(args[1], player, target, spell, dice);
			ValidationAction validationAction = Expressions.Get<ValidationAction>(args[2].Trim(), player, target, spell, dice);
			Validation.OnValidationFailed(dungeonMasterMessage, floatText, validationAction);
			return null;
		}
	}
}
