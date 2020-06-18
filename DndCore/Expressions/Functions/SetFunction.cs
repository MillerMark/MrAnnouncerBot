using System;
using System.Collections.Generic;
using System.Reflection;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	/// <summary>
	/// Sets the specified property of the player to the specified value.
	/// </summary>
	[Tooltip("Sets the specified property, field, or state variable for the active player to the specified value.")]
	[Param(1, typeof(string), "variableName", "The field, property, or state variable to change.", ParameterIs.Required)]
	[Param(2, typeof(object), "value", "The value to assign.", ParameterIs.Required)]

	public class SetFunction : DndFunction
	{
		public override string Name => "Set";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Target target, CastedSpell spell, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 2);

			string variableName = args[0];
			// TODO: incorrectValue:
			//object incorrectValue = evaluator.Evaluate(Expressions.Clean(args[1]));
			object value = Expressions.Get(Expressions.Clean(args[1]), player, target, spell);

			object instance = player;
			Type instanceType = typeof(Character);
			if (KnownQualifiers.IsSpellQualifier(variableName))
			{
				if (evaluator.Variables.ContainsKey(Expressions.STR_CastedSpell))
				{
					instance = evaluator.Variables[Expressions.STR_CastedSpell];
					instanceType = typeof(CastedSpell);
					variableName = variableName.EverythingAfter(KnownQualifiers.Spell);
				}
			}

			FieldInfo field = instanceType.GetField(variableName);
			if (field != null)
			{
				if (instance != null)
					field.SetValue(instance, value);
				return null;
			}

			PropertyInfo property = instanceType.GetProperty(variableName);
			if (property != null)
			{
				if (instance != null)
					property.SetValue(instance, value);
				return null;
			}

			player.SetState(variableName, value);

			return null;
		}
	}
}
