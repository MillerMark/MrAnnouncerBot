using System;
using System.Linq;
using System.Reflection;

namespace DndCore
{
	public class BaseRow
	{

		public BaseRow()
		{

		}

		public bool GetBool(string fieldName, bool defaultValue = false)
		{
			object value = GetValue(fieldName);

			if (value == null)
				return defaultValue;

			if (value is string)
				return MathUtils.IsChecked((string)value);

			return defaultValue;
		}

		public double GetDouble(string fieldName, double defaultValue = 0)
		{
			object value = GetValue(fieldName);

			if (value == null)
				return defaultValue;

			if (value is string)
			{
				string valueStr = (string)value;
				if (double.TryParse(valueStr, out double result))
					return result;
			}

			if (value is int)
				return (double)value;

			if (value is double)
				return (double)value;

			return defaultValue;
		}

		public int GetInt(string fieldName, int defaultValue = 0)
		{
			object value = GetValue(fieldName);

			if (value == null)
				return defaultValue;

			if (value is string)
			{
				string valueStr = (string)value;
				if (int.TryParse(valueStr, out int result))
					return result;
			}

			if (value is int)
				return (int)value;

			if (value is double)
				return (int)Math.Round((double)value);

			return defaultValue;
		}

		public object GetValue(string fieldName)
		{
			PropertyInfo propertyInfo = GetType().GetProperty(fieldName);
			if (propertyInfo != null)
				return propertyInfo.GetValue(this, null);
			return null;
		}
	}
}
