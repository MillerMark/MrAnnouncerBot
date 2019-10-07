using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public static class KnownQualifiers
	{
		public const string Spell = "spell_";
		public static bool IsSpellQualifier(string expression)
		{
			return expression.StartsWith(Spell);
		}
		public static bool StartsWithKnownQualifier(string expression)
		{
			if (IsSpellQualifier(expression))
				return true;

			return false;
		}
	}
	public class DndCharacterProperty : DndVariable
	{
		List<string> propertyNames = null;
		List<string> fieldNames = null;
		
		void GetPropertyNames()
		{
			if (propertyNames != null)
				return;
			propertyNames = new List<string>();
			fieldNames = new List<string>();
			PropertyInfo[] properties = typeof(Character).GetProperties(BindingFlags.Public | BindingFlags.Instance);
			foreach (PropertyInfo propertyInfo in properties)
			{
				propertyNames.Add(propertyInfo.Name);
			}

			FieldInfo[] fields = typeof(Character).GetFields(BindingFlags.Public | BindingFlags.Instance);
			foreach (FieldInfo fieldInfo in fields)
			{
				fieldNames.Add(fieldInfo.Name);
			}
		}

		public override bool Handles(string tokenName, Character player, CastedSpell castedSpell)
		{
			GetPropertyNames();
			if (propertyNames.IndexOf(tokenName) >= 0 | fieldNames.IndexOf(tokenName) >= 0)
				return true;

			if (KnownQualifiers.StartsWithKnownQualifier(tokenName))
				return false;

			if (player == null)
				return false;

			return player.HoldsState(tokenName) || tokenName.StartsWith("_"); // ;
		}

		public override object GetValue(string variableName, ExpressionEvaluator evaluator, Character player)
		{
			if (fieldNames.IndexOf(variableName) >= 0)
			{
				FieldInfo field = typeof(Character).GetField(variableName);
				return field?.GetValue(player);
			}

			if (propertyNames.IndexOf(variableName) >= 0)
			{
				PropertyInfo property = typeof(Character).GetProperty(variableName);
				return property?.GetValue(player);
			}

			return player.GetState(variableName);
		}
	}
}

