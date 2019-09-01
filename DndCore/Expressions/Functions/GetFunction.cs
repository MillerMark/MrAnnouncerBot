using System;
using System.Reflection;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	/// <summary>
	/// Gets the specified property of the player.
	/// </summary>
	public class GetFunction : DndFunction
	{
		public override string Name => "Get";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player)
		{
			ExpectingArguments(args, 1);

			string propertyName = args[0];

			FieldInfo field = typeof(Character).GetField(propertyName);
			if (field != null)
			{
				return field.GetValue(player);
			}
			PropertyInfo property = typeof(Character).GetProperty(propertyName);
			if (property != null)
			{
				return property.GetValue(player);
			}

			return player.GetState(propertyName);
		}
	}
}
