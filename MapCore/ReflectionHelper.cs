using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using MapCore;

namespace MapCore
{
	public static class ReflectionHelper
	{
		static List<Type> knownDesignTimeAttributes = new List<Type>();
		static ReflectionHelper()
		{
			List<Type> designTimeAttributes = typeof(ReflectionHelper).Assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(DesignTimeAttribute))).ToList();

			foreach (Type type in designTimeAttributes)
				knownDesignTimeAttributes.Add(type);
		}

		public static bool HasAttribute(this PropertyInfo propertyInfo, Type type)
		{
			return propertyInfo.GetCustomAttribute(type) != null;
		}

		public static bool HasAttribute<T>(PropertyInfo propertyInfo) where T : Attribute
		{
			return propertyInfo.GetCustomAttribute<T>() != null;
		}

		public static bool IsEditable(PropertyInfo propertyInfo)
		{
			if (!propertyInfo.CanWrite || !propertyInfo.CanRead)
				return false;

			foreach (Type type in knownDesignTimeAttributes)
				if (propertyInfo.HasAttribute(type))
					return true;

			return false;
		}

		public static void GetPropertiesFrom<T>(this T target, T source)
		{
			PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (!IsEditable(propertyInfo))
					continue;

				propertyInfo.SetValue(target, propertyInfo.GetValue(source));
			}
		}
	}
}
