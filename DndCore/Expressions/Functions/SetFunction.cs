using System;
using System.Collections.Generic;
using System.Reflection;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	/// <summary>
	/// Sets the specified property of the player to the specified value.
	/// </summary>
	public class SetFunction : DndFunction
	{
		public override string Name => "Set";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player)
		{
			ExpectingArguments(args, 2);

			string propertyName = args[0];
			object value = evaluator.Evaluate(Expressions.Clean(args[1]));

			object instance = player;
			Type instanceType = typeof(Character);
			if (KnownQualifiers.IsSpellQualifier(propertyName))
			{
				if (evaluator.Variables.ContainsKey(Expressions.STR_CastedSpell))
				{
					instance = evaluator.Variables[Expressions.STR_CastedSpell];
					instanceType = typeof(CastedSpell);
					propertyName = propertyName.EverythingAfter(KnownQualifiers.Spell);
				}
			}

			FieldInfo field = instanceType.GetField(propertyName);
			if (field != null)
			{
				if (instance != null)
					field.SetValue(instance, value);
				return null;
			}

			PropertyInfo property = instanceType.GetProperty(propertyName);
			if (property != null)
			{
				if (instance != null)
					property.SetValue(instance, value);
				return null;
			}

			player.SetState(propertyName, value);

			return null;
		}
	}
}
