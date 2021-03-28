using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Gets the portion of the die string that applies the specified damage.")]
	[Param(1, typeof(string), "damageFilter", "The type of damage to filter (e.g., \"cold\", \"fire\", etc.).", ParameterIs.Required)]
	public class GetSpellRawDieStr : DndFunction
	{
		public override string Name => "GetSpellRawDieStr";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, CastedSpell spell, RollResults dice = null)
		{
			ExpectingArguments(args, 1);

			if (spell == null)
				return null;
			return spell.GetSpellRawDieStr(args[0]);
		}
	}
}
