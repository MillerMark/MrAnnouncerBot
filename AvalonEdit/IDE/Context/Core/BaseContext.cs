//#define profiling
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodingSeb.ExpressionEvaluator;
using DndCore;
using ICSharpCode.AvalonEdit.Editing;

namespace AvalonEdit
{
	public abstract class BaseContext : DndVariable
	{
		protected abstract bool ContextIsSatisfied(ExpressionEvaluator evaluator, TextArea textArea);

		public override object GetValue(string variableName, ExpressionEvaluator evaluator, Creature player)
		{
			TextArea textArea = Expressions.GetCustomData<TextArea>(evaluator.Variables);
			if (textArea == null)
				return null;

			return ContextIsSatisfied(evaluator, textArea);
		}

		public override bool Handles(string tokenName, Creature player, CastedSpell castedSpell)
		{
			return tokenName == GetType().Name;
		}

		static BaseContext()
		{
		}

		public static void LoadAll()
		{
			Assembly dndCore = typeof(BaseContext).Assembly;

			foreach (Type type in dndCore.GetTypes())
			{
				if (!type.IsClass)
					continue;
				if (type.BaseType == typeof(BaseContext))
				{
					Expressions.AddVariable((DndVariable)Activator.CreateInstance(type));
				}
			}
		}
		public static string TextLeftOfTemplate { get; set; }
		public static string TextRightOfTemplate { get; set; }

		public override List<PropertyCompletionInfo> GetCompletionInfo()
		{
			return null;
		}
	}
}
