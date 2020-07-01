using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;
using System.ComponentModel;

namespace DndCore
{
	public class DndSpellProperty : DndVariable
	{
		List<string> propertyNames = null;
		List<string> fieldNames = null;

		void GetPropertyNames()
		{
			if (propertyNames != null)
				return;
			propertyNames = new List<string>();
			fieldNames = new List<string>();
			PropertyInfo[] properties = typeof(Spell).GetProperties(BindingFlags.Public | BindingFlags.Instance);
			foreach (PropertyInfo propertyInfo in properties)
			{
				propertyNames.Add(propertyInfo.Name);
			}

			FieldInfo[] fields = typeof(Spell).GetFields(BindingFlags.Public | BindingFlags.Instance);
			foreach (FieldInfo fieldInfo in fields)
			{
				fieldNames.Add(fieldInfo.Name);
			}
		}

		public override bool Handles(string tokenName, Character player, CastedSpell castedSpell)
		{
			if (tokenName.IndexOf(KnownQualifiers.Spell) != 0)
				return false;
			string spellPropToken = tokenName.EverythingAfter(KnownQualifiers.Spell);
			GetPropertyNames();
			if (propertyNames.IndexOf(spellPropToken) >= 0 | fieldNames.IndexOf(spellPropToken) >= 0)
				return true;

			return false;
		}

		public override object GetValue(string variableName, ExpressionEvaluator evaluator, Character player)
		{
			if (variableName.IndexOf(KnownQualifiers.Spell) != 0)
				return null;
			CastedSpell castedSpell = Expressions.GetCastedSpell(evaluator.Variables);
			string spellPropToken = variableName.EverythingAfter(KnownQualifiers.Spell);

			if (castedSpell != null)
			{
				if (fieldNames.IndexOf(spellPropToken) >= 0)
				{
					FieldInfo field = typeof(Spell).GetField(spellPropToken);
					return field?.GetValue(castedSpell.Spell);
				}

				if (propertyNames.IndexOf(spellPropToken) >= 0)
				{
					PropertyInfo property = typeof(Spell).GetProperty(spellPropToken);
					return property?.GetValue(castedSpell.Spell);
				}
			}

			return null;
		}

		public override List<PropertyCompletionInfo> GetCompletionInfo()
		{
			List<PropertyCompletionInfo> result = new List<PropertyCompletionInfo>();
			PropertyInfo[] properties = typeof(Spell).GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				string description = $"Spell Property: {propertyInfo.Name}";
				DescriptionAttribute descriptionAttribute = propertyInfo.GetCustomAttribute<DescriptionAttribute>();
				if (descriptionAttribute != null)
					description = descriptionAttribute.Description;
				TypeHelper.GetTypeDetails(propertyInfo.PropertyType, out string enumTypeName, out ExpressionType expressionType);
				result.Add(new PropertyCompletionInfo() { Name = $"spell_{propertyInfo.Name}", Description = description, EnumTypeName = enumTypeName, Type = expressionType });
			}
			FieldInfo[] fields = typeof(Spell).GetFields();
			foreach (FieldInfo fieldInfo in fields)
			{
				string description = $"Spell Field: {fieldInfo.Name}";
				DescriptionAttribute descriptionAttribute = fieldInfo.GetCustomAttribute<DescriptionAttribute>();
				if (descriptionAttribute != null)
					description = descriptionAttribute.Description;
				TypeHelper.GetTypeDetails(fieldInfo.FieldType, out string enumTypeName, out ExpressionType expressionType);
				result.Add(new PropertyCompletionInfo() { Name = $"spell_{fieldInfo.Name}", Description = description, EnumTypeName = enumTypeName, Type = expressionType });
			}
			return result;
		}
	}
}

