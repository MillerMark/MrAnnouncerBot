using System;
using System.Collections.Generic;
using System.Reflection;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	/// <summary>
	/// Sets the specified property of the player to the specified value.
	/// </summary>
	public class SetFunction : DndFunction
	{
		public override string Name => "Set";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player)
		{
			ExpectingArguments(args, 2);

			string propertyName = args[0];
			object value = evaluator.Evaluate(Expressions.Clean(args[1]));

			FieldInfo field = typeof(Character).GetField(propertyName);
			if (field != null)
			{
				field.SetValue(player, value);
				return null;
			}
			PropertyInfo property = typeof(Character).GetProperty(propertyName);
			if (property != null)
			{
				property.SetValue(player, value);
				return null;
			}

			player.SetState(propertyName, value);
			
			return null;
		}
	}
}
