using CodingSeb.ExpressionEvaluator;
using System;
using System.Linq;

namespace DndCore
{
	public abstract class DndVariable: DndToken
	{
		public abstract object GetValue(string variableName, ExpressionEvaluator evaluator, Character player);
	}
}

