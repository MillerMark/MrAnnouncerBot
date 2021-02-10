using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class MultiTargetNotificationVariable : DndVariable
	{
		public static int Offset = 0;

		public MultiTargetNotificationVariable()
		{
			Name = "MultiTargetNotificationOffset";
		}

		public override List<PropertyCompletionInfo> GetCompletionInfo()
		{
			return null;
		}

		public override object GetValue(string variableName, ExpressionEvaluator evaluator, Creature player)
		{
			return Offset;
		}
	}
}

