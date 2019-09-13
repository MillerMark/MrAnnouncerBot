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
			return expression.Replace("“", "\"").Replace("”", "\"");

		}
		public static object Get(string expression, Character player = null)
		{
			AddPlayerVariable(player);
			try
			{
				return expressionEvaluator.Evaluate(Clean(expression));
			}
			finally
			{
				FinishedEvaluation(player);
			}
		}

		public static object Get<T>(string expression, Character player = null)
		{
			AddPlayerVariable(player);
			try
			{
				return (T)expressionEvaluator.Evaluate(Clean(expression));
			}
			finally
			{
				FinishedEvaluation(player);
			}
		}

		public static void Do(string expression, Character player = null)
		{
			if (string.IsNullOrWhiteSpace(expression))
				return;
			AddPlayerVariable(player);
			try
			{
				expressionEvaluator.Evaluate(Clean(expression));
			}
			finally
			{
				FinishedEvaluation(player);
			}
		}

		public static int GetInt(string expression, Character player = null)
		{
			AddPlayerVariable(player);
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

		public static bool GetBool(string expression, Character player = null)
		{
			AddPlayerVariable(player);
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

		private static void AddPlayerVariable(Character player)
		{
			expressionEvaluator.Variables = new Dictionary<string, object>()
			{
				{ STR_Player, player }
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

		static Character GetPlayer(IDictionary<string, object> variables)
		{
			if (variables.ContainsKey(STR_Player))
				return variables[STR_Player] as Character;
			return null;
		}

		private static void ExpressionEvaluator_EvaluateFunction(object sender, FunctionEvaluationEventArg e)
		{
			Character player = GetPlayer(e.Evaluator.Variables);
			DndFunction function = functions.FirstOrDefault(x => x.Handles(e.Name, player));
			if (function != null)
			{
				e.Value = function.Evaluate(e.Args, e.Evaluator, player);
			}
		}

		private static void ExpressionEvaluator_EvaluateVariable(object sender, VariableEvaluationEventArg e)
		{
			Character player = GetPlayer(e.Evaluator.Variables);
			DndVariable variable = variables.FirstOrDefault(x => x.Handles(e.Name, player));
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

