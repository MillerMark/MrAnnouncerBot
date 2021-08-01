using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Gives the specified condition to the active player or magic recipient.")]
	[Param(1, typeof(Conditions), "condition", "The condition to give.", ParameterIs.Required)]
	public class AddConditionFunction : DndFunction
	{
		public override string Name { get; set; } = "AddCondition";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature creature, Target target = null, CastedSpell spell = null, RollResults dice = null)
		{
			ExpectingArguments(args, 1);
			string spellId = evaluator.GetSpellId(spell);

			if (creature != null)
			{
				Conditions condition = Expressions.Get<Conditions>(args[0]);

				creature.AddSpellCondition(spellId, condition);
			}

			return null;
		}
	}
}
