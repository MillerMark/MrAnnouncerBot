using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class GetArticle : DndFunction
	{
		public override string Name { get; set; } = "GetArticle";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Target target = null, CastedSpell spell = null, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 1);
			string nextWord = Expressions.GetStr(args[0], player, target, spell).ToLower();
			if (nextWord.StartsWith("a") || nextWord.StartsWith("e") || nextWord.StartsWith("i") || nextWord.StartsWith("o") || nextWord.StartsWith("u"))
				return "an";
			else
				return "a";
		}
	}
}
