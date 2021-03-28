using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	// TODO: support code completion with Tooltip 
	public class DndProcEvaluator : DndFunction
	{
		ProcDto proc;
		public override bool Handles(string tokenName, Creature player, CastedSpell castedSpell)
		{
			proc = AllProcs.Get(tokenName);
			return proc != null;
		}

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, CastedSpell spell, RollResults dice = null)
		{
			if (proc == null)
				return null;

			Expressions.Do(DndUtils.InjectParameters(proc.Script, proc.Parameters, args), player, target, spell);
			return null;
		}
	}
}

