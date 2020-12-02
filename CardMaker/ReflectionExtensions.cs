using System;
using System.Linq;
using System.Reflection;

namespace CardMaker
{
	public static class ReflectionExtensions
	{
		public static void SetValueFromString(this PropertyInfo propInfo, object instance, string valueStr)
		{
			propInfo.SetValue(instance, propInfo.ConvertStrValueToNative(valueStr));
		}
		public static object ConvertStrValueToNative(this PropertyInfo propInfo, string valueStr)
		{
			string fullName = propInfo.PropertyType.FullName;
			switch (fullName)
			{
				case "System.Int32":
					if (!int.TryParse(valueStr, out int intValue))
						intValue = 0;
					return intValue;
				case "System.Decimal":
					if (decimal.TryParse(valueStr, out decimal decimalValue))
						return decimalValue;
					else
					{
						System.Diagnostics.Debugger.Break();
						break;
					}
				case "System.Double":
					if (double.TryParse(valueStr, out double doubleValue))
						return doubleValue;
					else
					{
						System.Diagnostics.Debugger.Break();
						break;
					}
				case "System.String":
					return valueStr;

				case "System.Boolean":
					string compareValue = valueStr.ToLower().Trim();
					return compareValue == "true" || compareValue == "x";
			}
			return valueStr;
		}
	}
}
