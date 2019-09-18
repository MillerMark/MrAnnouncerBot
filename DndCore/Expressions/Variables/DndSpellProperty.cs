using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class DndSpellProperty : DndVariable
	{
		const string STR_SpellPrefix = "spell_";
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
			if (tokenName.IndexOf(STR_SpellPrefix) != 0)
				return false;
			string spellPropToken = tokenName.EverythingAfter(STR_SpellPrefix);
			GetPropertyNames();
			if (propertyNames.IndexOf(spellPropToken) >= 0 | fieldNames.IndexOf(spellPropToken) >= 0)
				return true;

			return false;
		}

		public override object GetValue(string variableName, ExpressionEvaluator evaluator, Character player)
		{
			if (variableName.IndexOf(STR_SpellPrefix) != 0)
				return null;
			CastedSpell castedSpell = Expressions.GetCastedSpell(evaluator.Variables);
			string spellPropToken = variableName.EverythingAfter(STR_SpellPrefix);

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

			return null;
		}
	}
}

