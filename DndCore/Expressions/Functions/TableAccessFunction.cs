using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[ReturnType(ExpressionType.number | ExpressionType.text)]
	[Tooltip("Gets the data in the specified table from a specified column from the row that has a matching value in a different specified column.")]
	[Param(1, typeof(string), "tableName", "The name of the table to look up.", ParameterIs.Required, CompletionProviderNames.DndTableName)]
	[Param(2, typeof(string), "fieldLookup", "The column to retrieve the value from.", ParameterIs.Required, CompletionProviderNames.DndTableColumn)]
	[Param(3, typeof(string), "matchColumn", "The column to find the match in.", ParameterIs.Required, CompletionProviderNames.DndTableColumn)]
	[Param(4, typeof(string), "matchValue", "The value to match in the specified matchColumn (determines the row for the fieldLookup).", ParameterIs.Required)]
	public class TableAccessFunction : DndFunction
	{
		public override string Name => "Table";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Target target, CastedSpell spell, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 4);
			
			string tableName = evaluator.Evaluate<string>(args[0]);
			string fieldLookup = evaluator.Evaluate<string>(args[1]);
			string matchColumn = evaluator.Evaluate<string>(args[2]);
			object matchValue = evaluator.Evaluate(Expressions.Clean(args[3]));

			return AllTables.GetData(tableName, fieldLookup, matchColumn, matchValue);
		}
	}
}
