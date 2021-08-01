using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Removes any condition added in association with the active spell (or magic) from the active player (or magic recipient).")]
	public class RemoveConditionFunction : DndFunction
	{
		public override string Name { get; set; } = "RemoveCondition";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature creature, Target target = null, CastedSpell spell = null, RollResults dice = null)
		{
			ExpectingArguments(args, 0);
			string spellId = evaluator.GetSpellId(spell);

			if (creature != null)
			{
				//Conditions condition = Expressions.Get<Conditions>(args[1]);

				creature.RemoveSpellCondition(spellId);
			}

			return null;
		}
	}
}
