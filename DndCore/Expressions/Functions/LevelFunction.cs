using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class LevelFunction : DndFunction
	{
		public override string Name => "Level";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player)
		{
			ExpectingArguments(args, 1);

			string characterClassName = evaluator.Evaluate<string>(args[0]);

			return player.GetLevel(characterClassName);
		}
	}
	public class GiveAdvantageFunction : DndFunction
	{
		public override string Name => "GiveAdvantage";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player)
		{
			ExpectingArguments(args, 0);

			player.GiveAdvantageThisRoll();
			return null;
		}
	}

	public class DisadvantageFunction : DndFunction
	{
		public override string Name => "GiveDisadvantage";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player)
		{
			ExpectingArguments(args, 0);

			player.GiveDisadvantageThisRoll();
			return null;
		}
	}
}
