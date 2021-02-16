using CodingSeb.ExpressionEvaluator;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;

namespace DndCore
{
	public abstract class DndVariable: DndToken
	{
		public abstract object GetValue(string variableName, ExpressionEvaluator evaluator, Creature player);
		public abstract List<PropertyCompletionInfo> GetCompletionInfo();
	}
}

