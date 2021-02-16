using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;
using System.ComponentModel;

namespace DndCore
{
	public class DndSpellProperty : DndPropertyAccessor
	{
		public DndSpellProperty()
		{
		}

		public override bool Handles(string tokenName, Creature player, CastedSpell castedSpell)
		{
			return Handles<Spell>(tokenName);
		}

		public override object GetValue(string variableName, ExpressionEvaluator evaluator, Creature player)
		{
			CastedSpell castedSpell = Expressions.GetCastedSpell(evaluator.Variables);
			return GetValue<Spell>(variableName, castedSpell.Spell);
		}

		public override List<PropertyCompletionInfo> GetCompletionInfo()
		{
			return AddPropertiesAndFields<Spell>();
		}
	}
}

