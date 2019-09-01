using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class TableAccessFunction : DndFunction
	{
		public override string Name => "Table";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player)
		{
			ExpectingArguments(args, 4);
			
			string tableName = evaluator.Evaluate<string>(args[0]);
			string fieldLookup = evaluator.Evaluate<string>(args[1]);
			string matchColumn = evaluator.Evaluate<string>(args[2]);
			object matchValue = evaluator.Evaluate(args[3]);

			return AllTables.GetData(tableName, fieldLookup, matchColumn, matchValue);
		}
	}
}
