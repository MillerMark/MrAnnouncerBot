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

		public static object Get(string expression, Character player = null)
		{
			AddPlayerVariable(player);
			return expressionEvaluator.Evaluate(expression);
		}

		public static object Get<T>(string expression, Character player = null)
		{
			AddPlayerVariable(player);
			return (T)expressionEvaluator.Evaluate(expression);
		}

		public static void Do(string expression, Character player = null)
		{
			AddPlayerVariable(player);
			expressionEvaluator.Evaluate(expression);
		}

		public static int GetInt(string expression, Character player = null)
		{
			AddPlayerVariable(player);
			object result = expressionEvaluator.Evaluate(expression);
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

		public static bool GetBool(string expression, Character player = null)
		{
			AddPlayerVariable(player);
			object result = expressionEvaluator.Evaluate(expression);
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

		private static void AddPlayerVariable(Character player)
		{
			expressionEvaluator.Variables = new Dictionary<string, object>()
			{
				{ STR_Player, player }
			};
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
			DndFunction function = functions.FirstOrDefault(x => x.Handles(e.Name));
			if (function != null)
			{
				Character player = GetPlayer(e.Evaluator.Variables);
				e.Value = function.Evaluate(e.Args, e.Evaluator, player);
			}
		}

		private static void ExpressionEvaluator_EvaluateVariable(object sender, VariableEvaluationEventArg e)
		{
			DndVariable variable = variables.FirstOrDefault(x => x.Handles(e.Name));
			if (variable != null)
			{
				Character player = GetPlayer(e.Evaluator.Variables);
				e.Value = variable.GetValue(e.Name, player);
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

