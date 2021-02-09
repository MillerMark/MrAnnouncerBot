using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Returns \"a\" (or \"an\" if the nextWord starts with a vowel).")]
	[Param(1, typeof(string), "nextWord", "The word to check.", ParameterIs.Required)]
	public class GetArticle : DndFunction
	{
		public override string Name { get; set; } = "GetArticle";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target = null, CastedSpell spell = null, DiceStoppedRollingData dice = null)
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
