using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;

namespace DndCore
{
	public abstract class DndPropertyAccessor : DndVariable
	{
		private const BindingFlags BindingAttributes = BindingFlags.Public | BindingFlags.Instance;
		protected List<string> propertyNames = null;
		protected List<string> fieldNames = null;

		public DndPropertyAccessor()
		{

		}

		public static List<PropertyCompletionInfo> AddPropertiesAndFields<T>(List<PropertyCompletionInfo> result = null, string typeNameOverride = null, bool usePrefix = true)
		{
			if (result == null)
				result = new List<PropertyCompletionInfo>();

			PropertyInfo[] properties = typeof(T).GetProperties(BindingAttributes);
			
			string typeName;
			if (typeNameOverride != null)
				typeName = typeNameOverride;
			else
				typeName = typeof(T).Name;

			string prefix;
			if (usePrefix)
				prefix = GetPrefix(typeName);
			else
				prefix = string.Empty;

			foreach (PropertyInfo propertyInfo in properties)
			{
				string description = $"{typeName} Property: {propertyInfo.Name}";
				DescriptionAttribute descriptionAttribute = propertyInfo.GetCustomAttribute<DescriptionAttribute>();
				if (descriptionAttribute != null)
					description = descriptionAttribute.Description;
				TypeHelper.GetTypeDetails(propertyInfo.PropertyType, out string enumTypeName, out ExpressionType expressionType);
				result.Add(new PropertyCompletionInfo() { Name = $"{prefix}{propertyInfo.Name}", Description = description, EnumTypeName = enumTypeName, Type = expressionType });
			}

			FieldInfo[] fields = typeof(T).GetFields(BindingAttributes);
			foreach (FieldInfo fieldInfo in fields)
			{
				string description = $"{typeName} Field: {fieldInfo.Name}";
				DescriptionAttribute descriptionAttribute = fieldInfo.GetCustomAttribute<DescriptionAttribute>();
				if (descriptionAttribute != null)
					description = descriptionAttribute.Description;
				TypeHelper.GetTypeDetails(fieldInfo.FieldType, out string enumTypeName, out ExpressionType expressionType);
				result.Add(new PropertyCompletionInfo() { Name = $"{prefix}{fieldInfo.Name}", Description = description, EnumTypeName = enumTypeName, Type = expressionType });
			}
			return result;
		}

		public static string GetPrefix(string typeName)
		{
			if (string.IsNullOrEmpty(typeName))
				return string.Empty;

			if (typeName.Length == 1)
				return char.ToLower(typeName[0]).ToString() + '_';

			return char.ToLower(typeName[0]) + typeName.Substring(1) + '_';
		}

		public static string GetPrefix<T>()
		{
			string typeName = typeof(T).Name;
			return GetPrefix(typeName);
		}

		void CollectPropertyNamesIfNecessary<T>()
		{
			if (propertyNames != null)  // Already collected.
				return;

			propertyNames = new List<string>();
			fieldNames = new List<string>();
			PropertyInfo[] properties = typeof(T).GetProperties(BindingAttributes);
			foreach (PropertyInfo propertyInfo in properties)
				propertyNames.Add(propertyInfo.Name);

			FieldInfo[] fields = typeof(T).GetFields(BindingAttributes);
			foreach (FieldInfo fieldInfo in fields)
				fieldNames.Add(fieldInfo.Name);
		}

		protected object GetValue<T>(string token, object instance, bool usePrefix = true)
		{
			if (instance == null)
				return null;

			CollectPropertyNamesIfNecessary<T>();

			string propertyOrFieldName;
			if (usePrefix)
			{
				string prefix = GetPrefix<T>();

				if (token.IndexOf(prefix) != 0)
					return null;

				propertyOrFieldName = token.EverythingAfter(prefix);
			}
			else
				propertyOrFieldName = token;

			if (fieldNames.IndexOf(propertyOrFieldName) >= 0)
			{
				FieldInfo field = typeof(T).GetField(propertyOrFieldName);
				return field?.GetValue(instance);
			}

			if (propertyNames.IndexOf(propertyOrFieldName) >= 0)
			{
				PropertyInfo property = typeof(T).GetProperty(propertyOrFieldName);
				return property?.GetValue(instance);
			}

			return null;
		}

		protected bool Handles<T>(string tokenName, bool usePrefix = true)
		{
			string propertyName = tokenName;
 
			if (usePrefix)
			{
				string prefix = GetPrefix<T>();
				if (!tokenName.StartsWith(prefix))
					return false;
				propertyName = tokenName.EverythingAfter(prefix);
			}

			CollectPropertyNamesIfNecessary<T>();
			
			if (propertyNames.Contains(propertyName) | fieldNames.Contains(propertyName))
				return true;

			return false;
		}
	}
}

