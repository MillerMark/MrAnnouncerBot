using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using UnityEngine;

namespace TaleSpireCore
{
	public static class ReflectionHelper
	{
		public static string GetAllProperties(object instance)
		{
			StringBuilder stringBuilder;
			stringBuilder = new StringBuilder();
			GetAllProperties(stringBuilder, instance);
			return stringBuilder.ToString();
		}

		public static void GetAllProperties(StringBuilder stringBuilder, object instance, string indent = "", int levelsDeep = 0, int maxLevels = 5)
		{
			if (instance == null)
				return;
			levelsDeep++;
			if (levelsDeep >= maxLevels)
				return;

			PropertyInfo[] properties = instance.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (PropertyInfo propertyInfo in properties)
			{
				object value = null;
				string valueStr;
				try
				{
					value = propertyInfo.GetValue(instance);
					if (value == null)
						valueStr = "null";
					else
					{
						valueStr = value.ToString();

					}
				}
				catch (Exception ex)
				{
					valueStr = "!" + ex.Message;
				}

				if (value is string[] strArray)
				{
					valueStr = $"string[{strArray.Length}]";
				}

				stringBuilder.AppendLine($"{indent}{propertyInfo.Name}: {valueStr}");

				if (value is string[] strArray2)
				{
					const int maxEntriesToShow = 14;
					for (int i = 0; i < Math.Min(maxEntriesToShow, strArray2.Length); i++)
						stringBuilder.AppendLine($"{indent}[{i}]: \"{strArray2[i]}\"");

					if (strArray2.Length > maxEntriesToShow)
						stringBuilder.AppendLine($"{indent}...");
					continue;
				}

				if (value == null)
					continue;

				if (value is Transform)
				{
					// TODO: Show position, scale, rotation...
					continue;
				}

				if (propertyInfo.PropertyType.Name == "GameObject" || propertyInfo.PropertyType.Name == "UnityEngine.GameObject")
					continue;

				if (propertyInfo.PropertyType.IsPrimitive)
					continue;
				if (propertyInfo.PropertyType.IsValueType)
					continue;
				if (value is string)
					continue;
				

				if (value == instance)
					continue;

				if (propertyInfo.Name == "SyncRoot")
					continue;

				GetAllProperties(stringBuilder, value, indent + "  ", levelsDeep, maxLevels);
			}
		}

		public static void CallNonPublicMethod(Type type, string methodName, object instance = null, object[] parameters = null)
		{
			if (parameters == null)
				parameters = new object[] { };

			BindingFlags bindingFlags = BindingFlags.NonPublic;
			if (instance == null)
				bindingFlags |= BindingFlags.Static;
			else
				bindingFlags |= BindingFlags.Instance;

			MethodInfo methodInfo = type.GetMethod(methodName, bindingFlags);
			if (methodInfo == null)
			{
				MessageBox.Show($"Non-public method \"{methodName}\" not found in {type.Name}.", "Error!");
				return;
			}

			try
			{
				methodInfo.Invoke(instance, parameters);
			}
			catch (Exception ex)
			{
				Talespire.Log.Exception(ex);
				MessageBox.Show(ex.Message, "Exception calling method!");
			}
		}

		public static T GetNonPublicFieldValue<T>(object instance, string fieldName) where T: class
		{
			return GetNonPublicFieldValue(instance, fieldName) as T;
		}

		public static object GetNonPublicFieldValue(object instance, string fieldName)
		{
			BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
			FieldInfo fieldInfo = instance.GetType().GetField(fieldName, bindingFlags);
			if (fieldInfo == null)
				return null;
			return fieldInfo.GetValue(instance);
		}

		public static void ClonePropertiesFrom(object target, object source)
		{
			if (target == null || source == null)
				return;

			if (!target.GetType().IsAssignableFrom(source.GetType()))
				return;
		}

		public static T GetNonPublicField<T>(object instance, string fieldName) where T : class
		{
			FieldInfo field = instance.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
			if (field == null)
				return default(T);
			return field.GetValue(instance) as T;
		}

		public static T GetPublicField<T>(object instance, string fieldName) where T : class
		{
			FieldInfo field = instance.GetType().GetField(fieldName, BindingFlags.Public | BindingFlags.Instance);
			if (field == null)
				return default(T);
			return field.GetValue(instance) as T;
		}

		public static T GetPublicProperty<T>(object instance, string propertyName) where T : class
		{
			PropertyInfo property = instance.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
			if (property == null)
				return default(T);
			return property.GetValue(instance) as T;
		}

		public static void SetNonPublicFieldValue(object instance, string fieldName, object value)
		{
			try
			{
				FieldInfo field = instance.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
				if (field == null)
					return;
				field.SetValue(instance, value);
			}
			catch (Exception ex)
			{
				Talespire.Log.Exception(ex);
				//MessageBox.Show(ex.Message, "Exception!");
			}
		}

		public static void SetPublicFieldValue(object instance, string fieldName, object value)
		{
			try
			{
				FieldInfo field = instance.GetType().GetField(fieldName, BindingFlags.Public | BindingFlags.Instance);
				if (field == null)
					return;
				field.SetValue(instance, value);
			}
			catch (Exception ex)
			{
				Talespire.Log.Exception(ex);
				//MessageBox.Show(ex.Message, "Exception!");
			}
		}
	}
}
