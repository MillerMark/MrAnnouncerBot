using System;
using CodingSeb.ExpressionEvaluator;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace DndCore
{
	public static class Expressions
	{
		static bool debugging;
		public static bool Debugging
		{
			get
			{
				return debugging;
			}
			set
			{
				if (debugging == value)
				{
					return;
				}

				debugging = value;
				if (debugging)
					expressionEvaluator.ExecutionPointerChanged += ExpressionEvaluator_ExecutionPointerChanged;
				else
					expressionEvaluator.ExecutionPointerChanged -= ExpressionEvaluator_ExecutionPointerChanged;
			}
		}

		public static event DndCoreExceptionEventHandler ExceptionThrown;
		public const string STR_Player = "player";
		public const string STR_Target = "target";
		public const string STR_CastedSpell = "castedSpell";
		static List<DndFunction> functions = new List<DndFunction>();
		static List<DndVariable> variables = new List<DndVariable>();
		public static List<string> history = new List<string>();

		public static void OnExceptionThrown(object sender, DndCoreExceptionEventArgs ea)
		{
			ExceptionThrown?.Invoke(sender, ea);
		}

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
		public static object Get(string expression, Character player = null, Creature target = null, CastedSpell spell = null)
		{
			StartEvaluation(player, $"Get({expression})", target, spell);
			try
			{
				object result = expressionEvaluator.Evaluate(Clean(expression));
				Log($"  returns {result};");
				return result;
			}
			finally
			{
				FinishEvaluation(player);
			}
		}

		public static object Get<T>(string expression, Character player = null, Creature target = null, CastedSpell spell = null)
		{
			StartEvaluation(player, $"Get<{typeof(T).ToString()}>({expression})", target, spell);
			try
			{
				try
				{
					T result = (T)expressionEvaluator.Evaluate(Clean(expression));
					Log($"  returns {result};");
					return result;
				}
				catch (Exception ex)
				{
					OnExceptionThrown(null, new DndCoreExceptionEventArgs(ex));
					return null;
				}
			}
			finally
			{
				FinishEvaluation(player);
			}
		}

		public static void Do(string expression, Character player = null, Creature target = null, CastedSpell castedSpell = null)
		{
			if (string.IsNullOrWhiteSpace(expression))
				return;
			StartEvaluation(player, $"Do({expression})", target, castedSpell);
			try
			{
				string script = Clean(expression);
				if (!script.EndsWith(";") && !script.EndsWith("}"))
					script += ";";

				string compactScript = string.Empty;
				string[] splitLines = script.Split('\r', '\n');
				foreach (string line in splitLines)
				{
					if (string.IsNullOrWhiteSpace(line))
						continue;
					if (line.Trim().StartsWith("//"))
						continue;
					compactScript += line + Environment.NewLine;
				}

				if (string.IsNullOrWhiteSpace(compactScript))
					return;
				try
				{
					expressionEvaluator.ScriptEvaluate(compactScript);
				}
				catch (ExpressionEvaluatorSyntaxErrorException ex)
				{
					OnExceptionThrown(null, new DndCoreExceptionEventArgs(ex));
				}
			}
			finally
			{
				FinishEvaluation(player);
			}
		}

		public static int GetInt(string expression, Character player = null, Creature target = null, CastedSpell spell = null)
		{
			StartEvaluation(player, $"GetInt({expression})", target, spell);
			try
			{
				object result = expressionEvaluator.Evaluate(Clean(expression));
				Log($"  returns {result};");

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
				FinishEvaluation(player);
			}
		}

		public static bool GetBool(string expression, Character player = null, Creature target = null, CastedSpell spell = null)
		{
			StartEvaluation(player, $"GetBool({expression})", target, spell);
			try
			{
				object result = expressionEvaluator.Evaluate(Clean(expression));
				bool resultBool;
				if (result is int)
					resultBool = (int)result == 1;
				else if (result is string)
				{
					string compareStr = ((string)result).Trim().ToLower();
					resultBool = compareStr == "x" || compareStr == "true";
				}
				else if (result is bool)
					resultBool = (bool)result;
				else
					resultBool = false;

				Log($"  returns {resultBool};");

				return resultBool;
			}
			finally
			{
				FinishEvaluation(player);
			}
		}

		public static string GetStr(string expression, Character player = null, Creature target = null, CastedSpell spell = null)
		{
			StartEvaluation(player, $"GetStr({expression})", target, spell);
			try
			{
				object result = expressionEvaluator.Evaluate(Clean(expression));
				string resultStr = string.Empty;
				if (result is string)
				{
					resultStr = (string)result;
				}
				else if (result == null)
					resultStr = string.Empty;
				else
					resultStr = result.ToString();

				Log($"  returns \"{resultStr}\";");
				return resultStr;
			}
			finally
			{
				FinishEvaluation(player);
			}
		}

		static Stack<IDictionary<string, object>> variableStack = new Stack<IDictionary<string, object>>();
		private static void StartEvaluation(Character player, string callingProc, Creature target = null, CastedSpell spell = null)
		{
			//historyStack.Push(history);
			//history = new List<string>();
			LogCallingProc(callingProc);
			AddPlayerVariables(player, target, spell);
			if (player != null)
				player.StartingExpressionEvaluation();
			BeginUpdate();
		}

		private static void AddPlayerVariables(Character player, Creature target, CastedSpell spell)
		{
			variableStack.Push(expressionEvaluator.Variables);
			expressionEvaluator.Variables = new Dictionary<string, object>()
			{
				{ STR_Player, player },
				{ STR_Target, target },
				{ STR_CastedSpell, spell }
			};
		}

		static void FinishEvaluation(Character player)
		{
			if (variableStack.Count > 0)
				expressionEvaluator.Variables = variableStack.Pop();
			EndUpdate(player);
		}

		static int callDepth;


		static ExpressionEvaluator expressionEvaluator;
		static Expressions()
		{
			expressionEvaluator = new ExpressionEvaluator();
			expressionEvaluator.EvaluateVariable += ExpressionEvaluator_EvaluateVariable;
			expressionEvaluator.EvaluateFunction += ExpressionEvaluator_EvaluateFunction;
			LoadEvaluatorExtensions();
		}

		private static void ExpressionEvaluator_ExecutionPointerChanged(object sender, ExecutionPointerChangedEventArgs ea)
		{
			if (!Debugging)
				return;

			string thisSection = string.Empty;
			if (ea.InstructionPointer >= ea.OriginalScript.Length)
			{
				System.Diagnostics.Debugger.Break();
			}
			else
				thisSection = ea.OriginalScript.Substring(ea.InstructionPointer);
			// HACK: It's a serious hack kids, but it's for debugging and it solves 99% of my use case needs.
			//! So please don't hate too much.
			if (ea.StackFrames.Count > 0)
				return;
			int instructionPointer = ea.OriginalScript.IndexOf(thisSection);
			//+++ SUPER HACK: Store a previous pointer and search 
			//+++ starting at that!!! From the amazing and one and only
			//+++ Rory!!!
		}

		public static CastedSpell GetCastedSpell(IDictionary<string, object> variables)
		{
			if (variables.ContainsKey(STR_CastedSpell))
				return variables[STR_CastedSpell] as CastedSpell;
			return null;
		}

		public static Creature GetTargetCreature(IDictionary<string, object> variables)
		{
			if (variables.ContainsKey(STR_Target))
				return variables[STR_Target] as Creature;
			return null;
		}

		static Character GetPlayer(IDictionary<string, object> variables)
		{
			if (variables.ContainsKey(STR_Player))
				return variables[STR_Player] as Character;
			return null;
		}

		static string GetArgsStr(List<string> args)
		{
			return string.Join(", ", args);
		}

		private static void ExpressionEvaluator_EvaluateFunction(object sender, FunctionEvaluationEventArg e)
		{
			Character player = GetPlayer(e.Evaluator.Variables);
			CastedSpell castedSpell = GetCastedSpell(e.Evaluator.Variables);
			Creature target = GetTargetCreature(e.Evaluator.Variables);
			DndFunction function = functions.FirstOrDefault(x => x.Handles(e.Name, player, castedSpell));
			if (function != null)
			{
				e.Value = function.Evaluate(e.Args, e.Evaluator, player, target, castedSpell);
				Log($"  {e.Name}({GetArgsStr(e.Args)}) => {GetValueStr(e.Value)}");
			}
		}

		private static string GetValueStr(object value)
		{
			if (value == null)
				return "null";

			return value.ToString();
		}

		private static void ExpressionEvaluator_EvaluateVariable(object sender, VariableEvaluationEventArg e)
		{
			Character player = GetPlayer(e.Evaluator.Variables);
			CastedSpell castedSpell = GetCastedSpell(e.Evaluator.Variables);
			DndVariable variable = variables.FirstOrDefault(x => x.Handles(e.Name, player, castedSpell));
			if (variable != null)
			{
				e.Value = variable.GetValue(e.Name, e.Evaluator, player);
				Log($"  {e.Name} == {e.Value}");
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
		public static bool LogHistory { get; set; }

		static void Log(string str)
		{
			if (!LogHistory)
				return;
			history.Add(str);
		}

		static void LogCallingProc(string callingProc)
		{
			if (!LogHistory)
				return;
			if (history.Count > 0)
				history.Add("");
			history.Add(callingProc);
		}

		public static void ClearHistory()
		{
			history.Clear();
		}
		public static void BeginUpdate()
		{
			callDepth++;
		}
		public static void EndUpdate(Character player = null)
		{
			callDepth--;
			if (player != null && callDepth == 0)
				player.CompletingExpressionEvaluation();
		}

		public static string HistoryLog
		{
			get
			{
				return string.Join(Environment.NewLine, history);
			}
		}
	}
}

