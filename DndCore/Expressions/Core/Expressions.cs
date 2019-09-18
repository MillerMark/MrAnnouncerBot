using System;
using CodingSeb.ExpressionEvaluator;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace DndCore
{
	public static class Expressions
	{
		const string STR_Player = "player";
		const string STR_Target = "target";
		const string STR_CastedSpell = "castedSpell";
		static List<DndFunction> functions = new List<DndFunction>();
		static List<DndVariable> variables = new List<DndVariable>();

		public static void AddFunction(DndFunction function)
		{
			functions.Add(function);
		}

		public static void AddVariable(DndVariable variable)
		{
			variables.Add(variable);
		}

		public static string Clean(string expression)
		{
			return expression.Replace("“", "\"").Replace("”", "\"").Trim();

		}
		public static object Get(string expression, Character player = null, Creature target = null)
		{
			AddPlayerVariables(player, target);
			try
			{
				return expressionEvaluator.Evaluate(Clean(expression));
			}
			finally
			{
				FinishedEvaluation(player);
			}
		}

		public static object Get<T>(string expression, Character player = null, Creature target = null)
		{
			AddPlayerVariables(player, target);
			try
			{
				return (T)expressionEvaluator.Evaluate(Clean(expression));
			}
			finally
			{
				FinishedEvaluation(player);
			}
		}

		public static void Do(string expression, Character player = null, Creature target = null, CastedSpell castedSpell = null)
		{
			if (string.IsNullOrWhiteSpace(expression))
				return;
			AddPlayerVariables(player, target, castedSpell);
			try
			{
				string script = Clean(expression);
				if (!script.EndsWith(";") && !script.EndsWith("}"))
					script += ";";

				expressionEvaluator.ScriptEvaluate(script);
			}
			finally
			{
				FinishedEvaluation(player);
			}
		}

		public static int GetInt(string expression, Character player = null, Creature target = null)
		{
			AddPlayerVariables(player, target);
			try
			{
				object result = expressionEvaluator.Evaluate(Clean(expression));
				if (result is int)
					return (int)result;
				if (result is double)
					return (int)Math.Round((double)result);

				if (result is decimal)
					return (int)Math.Round((decimal)result);
				if (result is Enum)
					return (int)result;
				return int.MinValue;
			}
			finally
			{
				FinishedEvaluation(player);
			}
		}

		public static bool GetBool(string expression, Character player = null, Creature target = null)
		{
			AddPlayerVariables(player, target);
			try
			{
				object result = expressionEvaluator.Evaluate(Clean(expression));
				if (result is int)
					return (int)result == 1;
				if (result is string)
				{
					string compareStr = ((string)result).Trim().ToLower();
					return compareStr == "x" || compareStr == "true";
				}

				if (result is bool)
					return (bool)result;

				return false;
			}
			finally
			{
				FinishedEvaluation(player);
			}
		}

		public static string GetStr(string expression, Character player = null, Creature target = null)
		{
			AddPlayerVariables(player, target);
			try
			{
				object result = expressionEvaluator.Evaluate(Clean(expression));
				if (result is string)
				{
					return (string)result;
				}

				if (result == null)
					return string.Empty;

				return result.ToString();
			}
			finally
			{
				FinishedEvaluation(player);
			}
		}

		private static void AddPlayerVariables(Character player, Creature target = null, CastedSpell spell = null)
		{
			expressionEvaluator.Variables = new Dictionary<string, object>()
			{
				{ STR_Player, player },
				{ STR_Target, target },
				{ STR_CastedSpell, spell }
			};
			if (player != null)
				player.StartingExpressionEvaluation();
		}

		static void FinishedEvaluation(Character player)
		{
			if (player != null)
				player.CompletingExpressionEvaluation();
		}



		static ExpressionEvaluator expressionEvaluator;
		static Expressions()
		{
			expressionEvaluator = new ExpressionEvaluator();
			expressionEvaluator.EvaluateVariable += ExpressionEvaluator_EvaluateVariable;
			expressionEvaluator.EvaluateFunction += ExpressionEvaluator_EvaluateFunction;
			LoadEvaluatorExtensions();
		}

		public static CastedSpell GetCastedSpell(IDictionary<string, object> variables)
		{
			if (variables.ContainsKey(STR_CastedSpell))
				return variables[STR_CastedSpell] as CastedSpell;
			return null;
		}

		static Character GetPlayer(IDictionary<string, object> variables)
		{
			if (variables.ContainsKey(STR_Player))
				return variables[STR_Player] as Character;
			return null;
		}

		private static void ExpressionEvaluator_EvaluateFunction(object sender, FunctionEvaluationEventArg e)
		{
			Character player = GetPlayer(e.Evaluator.Variables);
			CastedSpell castedSpell = GetCastedSpell(e.Evaluator.Variables);
			DndFunction function = functions.FirstOrDefault(x => x.Handles(e.Name, player, castedSpell));
			if (function != null)
			{
				e.Value = function.Evaluate(e.Args, e.Evaluator, player);
			}
		}

		private static void ExpressionEvaluator_EvaluateVariable(object sender, VariableEvaluationEventArg e)
		{
			Character player = GetPlayer(e.Evaluator.Variables);
			CastedSpell castedSpell = GetCastedSpell(e.Evaluator.Variables);
			DndVariable variable = variables.FirstOrDefault(x => x.Handles(e.Name, player, castedSpell));
			if (variable != null)
			{
				e.Value = variable.GetValue(e.Name, e.Evaluator, player);
			}
		}

		static void LoadEvaluatorExtensions()
		{
			Assembly dndCore = typeof(Expressions).Assembly;

			foreach (Type type in dndCore.GetTypes())
			{
				if (!type.IsClass)
					continue;
				if (type.BaseType == typeof(DndFunction))
					AddFunction((DndFunction)Activator.CreateInstance(type));
				if (type.BaseType == typeof(DndVariable))
					AddVariable((DndVariable)Activator.CreateInstance(type));
			}
		}
	}
}

