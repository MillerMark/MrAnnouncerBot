using System;
using System.Linq;

namespace DndCore
{
	public static class TypeHelper
	{
		public static void GetTypeDetails(Type type, out string enumTypeName, out ExpressionType expressionType)
		{
			enumTypeName = null;
			expressionType = ExpressionType.unknown;
			if (type.IsEnum)
			{
				expressionType = ExpressionType.@enum;
				enumTypeName = type.Name;
				return;
			}

			switch (type.Name)
			{
				case "Int32":
				case "Double":
					expressionType = ExpressionType.number;
					break;
				case "Boolean":
					expressionType = ExpressionType.boolean;
					break;
				case "String":
					expressionType = ExpressionType.text;
					break;
				default:
					break;
			}
		}

		public static bool Matches(ExpressionType expressionType, Type type)
		{
			if (type == typeof(int) || type == typeof(double) || type == typeof(decimal))
				return expressionType.HasFlag(ExpressionType.number);
			if (type == typeof(string))
				return expressionType.HasFlag(ExpressionType.text);
			if (type == typeof(bool))
				return expressionType.HasFlag(ExpressionType.boolean);
			if (type.IsEnum)
				return expressionType.HasFlag(ExpressionType.@enum);
			return false;
		}
	}
}

