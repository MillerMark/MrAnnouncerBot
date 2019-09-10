using System;
using System.Collections.Generic;
using System.Reflection;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class OffsetFunction : DndFunction
	{
		public override string Name => "Offset";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player)
		{
			if (player == null)
				return null;

			ExpectingArguments(args, 2);

			string variableName = args[0];
			double valueDouble = MathUtils.GetDouble(evaluator.Evaluate<object>(args[1]).ToString());
			int valueInt = (int)Math.Round(valueDouble);

			// TODO: Wil says Convert.ConvertTo() can simplify this.

			FieldInfo field = typeof(Character).GetField(variableName);
			if (field != null)
			{
				if (field.FieldType.FullName == "System.Int32")
					field.SetValue(player, MathUtils.GetInt(field.GetValue(player).ToString()) + valueInt);
				else
					field.SetValue(player, MathUtils.GetDouble(field.GetValue(player).ToString()) + valueDouble);
				return null;
			}

			PropertyInfo property = typeof(Character).GetProperty(variableName);
			if (property != null)
			{
				if (property.PropertyType.FullName == "System.Int32")
					property.SetValue(player, MathUtils.GetInt(property.GetValue(player).ToString()) + valueInt);
				else
					property.SetValue(player, MathUtils.GetDouble(property.GetValue(player).ToString()) + valueDouble);
				return null;
			}

			object existingValueRaw = player.GetState(variableName);
			if (existingValueRaw != null)
			{
				if (existingValueRaw is int)
					player.SetState(variableName, MathUtils.GetInt(existingValueRaw.ToString()) + valueInt);
				else
					player.SetState(variableName, MathUtils.GetDouble(existingValueRaw.ToString()) + valueDouble);
			}

			return null;
		}
	}
}
