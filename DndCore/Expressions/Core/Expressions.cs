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
		public static event ExecutionPointerChangedHandler ExecutionChanged;
		public const string STR_Player = "player";
		public const string STR_Target = "target";
		public const string STR_Dice = "dice";
		public const string STR_CastedSpell = "castedSpell";
		public const string STR_CustomData = "customData";
		public static List<DndFunction> functions = new List<DndFunction>();
		public static List<DndVariable> variables = new List<DndVariable>();
		public static List<string> history = new List<string>();

		public static void OnExecutionChanged(object sender, ExecutionPointerChangedEventArgs ea)
		{
			ExecutionChanged?.Invoke(sender, ea);
		}

		public static void OnExceptionThrown(object sender, DndCoreExceptionEventArgs ea)
		{
			Log($"  Exception: {ea.Ex.Message};");
			Exception innerException = ea.Ex.InnerException;
			string indent = "  ";
			while (innerException != null)
			{
				Log($"{indent}  Inner Exception: {innerException.Message};");
				innerException = innerException.InnerException;
				indent += "  ";
			}
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
		public static object Get(string expression, Character player = null, Target target = null, CastedSpell spell = null, DiceStoppedRollingData dice = null, object customData = null)
		{
			if (string.IsNullOrWhiteSpace(expression))
				return null;

			StartEvaluation(player, $"Get({expression})", target, spell, dice, customData);
			try
			{
				try
				{
					object result = expressionEvaluator.Evaluate(Clean(expression));
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

		public static T Get<T>(string expression, Character player = null, Target target = null, CastedSpell spell = null, DiceStoppedRollingData dice = null, object customData = null)
		{
			if (string.IsNullOrWhiteSpace(expression))
				return default(T);
			StartEvaluation(player, $"Get<{typeof(T).ToString()}>({expression})", target, spell, dice, customData);
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
					return default(T);
				}
			}
			finally
			{
				FinishEvaluation(player);
			}
		}

		public static void Do(string expression, Character player = null, Target target = null, CastedSpell castedSpell = null, DiceStoppedRollingData dice = null, object customData = null)
		{
			if (string.IsNullOrWhiteSpace(expression))
				return;
			StartEvaluation(player, $"Do({expression})", target, castedSpell, dice, customData);
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

		public static int GetInt(string expression, Character player = null, Target target = null, CastedSpell spell = null, DiceStoppedRollingData dice = null, object customData = null)
		{
			if (string.IsNullOrWhiteSpace(expression))
				return 0;
			StartEvaluation(player, $"GetInt({expression})", target, spell, dice, customData);
			try
			{
				object result = 0;
				try
				{
					result = expressionEvaluator.Evaluate(Clean(expression));
					Log($"  returns {result};");
				}
				catch (Exception ex)
				{
					OnExceptionThrown(null, new DndCoreExceptionEventArgs(ex));
					return 0;
				}

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

		public static double GetDouble(string expression, Character player = null, Target target = null, CastedSpell spell = null, DiceStoppedRollingData dice = null, object customData = null)
		{
			if (string.IsNullOrWhiteSpace(expression))
				return 0;
			StartEvaluation(player, $"GetDouble({expression})", target, spell, dice, customData);
			try
			{
				object result = 0.0;

				try
				{
					result = expressionEvaluator.Evaluate(Clean(expression));
					Log($"  returns {result};");
				}
				catch (Exception ex)
				{
					OnExceptionThrown(null, new DndCoreExceptionEventArgs(ex));
					return 0;
				}

				if (result is int)
					return (int)result;

				if (result is decimal)
					return (double)((decimal)result);

				if (result is double)
					return (double)result;

				if (result is string resultStr)
					if (double.TryParse(resultStr, out double resultDbl))
						return resultDbl;
					else  // Support fractions...
					{
						const string divideSymbol = "/";
						if (resultStr.Contains(divideSymbol)) 
						{
							string numeratorStr = resultStr.EverythingBefore(divideSymbol).Trim();
							string denominatorStr = resultStr.EverythingAfter(divideSymbol).Trim();
							if (double.TryParse(numeratorStr, out double numerator) && double.TryParse(denominatorStr, out double denominator) && denominator != 0)
							{
								return numerator/denominator;
							}
						}
					}

				return double.MinValue;
			}
			finally
			{
				FinishEvaluation(player);
			}
		}

		public static bool GetBool(string expression, Character player = null, Target target = null, CastedSpell spell = null, DiceStoppedRollingData dice = null, object customData = null)
		{
			if (string.IsNullOrWhiteSpace(expression))
				return false;
			StartEvaluation(player, $"GetBool({expression})", target, spell, dice, customData);
			try
			{
				object result = false;

				try
				{
					result = expressionEvaluator.Evaluate(Clean(expression));
				}
				catch (Exception ex)
				{
					OnExceptionThrown(null, new DndCoreExceptionEventArgs(ex));
					return false;
				}

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

		public static string GetStr(string expression, Character player = null, Target target = null, CastedSpell spell = null, DiceStoppedRollingData dice = null, object customData = null)
		{
			if (string.IsNullOrWhiteSpace(expression))
				return string.Empty;

			StartEvaluation(player, $"GetStr({expression})", target, spell, dice, customData);
			try
			{
				object result = string.Empty;

				try
				{
					result = expressionEvaluator.Evaluate(Clean(expression));
				}
				catch (Exception ex)
				{
					OnExceptionThrown(null, new DndCoreExceptionEventArgs(ex));
					return string.Empty;
				}

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
		private static void StartEvaluation(Character player, string callingProc, Target target = null, CastedSpell spell = null, DiceStoppedRollingData dice = null, object customData = null)
		{
			//historyStack.Push(history);
			//history = new List<string>();
			LogCallingProc(callingProc);
			AddPlayerVariables(player, target, spell, dice, customData);
			if (player != null)
				player.StartingExpressionEvaluation();
			BeginUpdate();
		}

		private static void AddPlayerVariables(Character player, Target target, CastedSpell spell, DiceStoppedRollingData dice = null, object customData = null)
		{
			variableStack.Push(expressionEvaluator.Variables);
			expressionEvaluator.Variables = new Dictionary<string, object>()
			{
				{ STR_Player, player },
				{ STR_Target, target },
				{ STR_Dice, dice },
				{ STR_CastedSpell, spell },
				{ STR_CustomData, customData }
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

			OnExecutionChanged(sender, ea);
		}

		public static CastedSpell GetCastedSpell(IDictionary<string, object> variables)
		{
			if (variables.ContainsKey(STR_CastedSpell))
				return variables[STR_CastedSpell] as CastedSpell;
			return null;
		}

		public static T GetCustomData<T>(IDictionary<string, object> variables)
		{
			if (variables.ContainsKey(STR_CustomData))
				return (T)variables[STR_CustomData];
			return default(T);
		}

		public static Target GetTargetCreature(IDictionary<string, object> variables)
		{
			if (variables.ContainsKey(STR_Target))
				return variables[STR_Target] as Target;
			return null;
		}

		public static DiceStoppedRollingData GetDiceStoppedRollingData(IDictionary<string, object> variables)
		{
			if (variables.ContainsKey(STR_Dice))
				return variables[STR_Dice] as DiceStoppedRollingData;
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
			Target target = GetTargetCreature(e.Evaluator.Variables);
			DiceStoppedRollingData dice = GetDiceStoppedRollingData(e.Evaluator.Variables);
			DndFunction function = functions.FirstOrDefault(x => x.Handles(e.Name, player, castedSpell));
			if (function != null)
			{
				try
				{
					e.Value = function.Evaluate(e.Args, e.Evaluator, player, target, castedSpell, null);
					Log($"  {e.Name}({GetArgsStr(e.Args)}) => {GetValueStr(e.Value)}");
				}
				catch (Exception ex)
				{
					e.Value = null;
					Log($"  Exception thrown trying to evaluate {e.Name}({GetArgsStr(e.Args)})");
				}
				
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

		static void AddEnumVariables()
		{
			AddVariable(new DndEnumValue<Ability>());
			AddVariable(new DndEnumValue<AttackKind>());
			AddVariable(new DndEnumValue<AttackType>());
			AddVariable(new DndEnumValue<Conditions>());
			AddVariable(new DndEnumValue<CreatureKinds>());
			AddVariable(new DndEnumValue<CreatureSize>());
			AddVariable(new DndEnumValue<DamageType>());
			AddVariable(new DndEnumValue<DiceRollType>());
			AddVariable(new DndEnumValue<ExhaustionLevels>());
			AddVariable(new DndEnumValue<Languages>());
			AddVariable(new DndEnumValue<ModType>());
			AddVariable(new DndEnumValue<PlayerProperty>());
			AddVariable(new DndEnumValue<RecalcOptions>());
			AddVariable(new DndEnumValue<RechargeOdds>());
			AddVariable(new DndEnumValue<Senses>());
			AddVariable(new DndEnumValue<Skills>());
			AddVariable(new DndEnumValue<SpellRangeType>());
			AddVariable(new DndEnumValue<SpellType>());
			AddVariable(new DndEnumValue<TimeMeasure>());
			AddVariable(new DndEnumValue<TimePoint>());
			AddVariable(new DndEnumValue<TurnPart>());
			AddVariable(new DndEnumValue<ValidationAction>());
			AddVariable(new DndEnumValue<VantageKind>());
			AddVariable(new DndEnumValue<WeaponProperties>());
			AddVariable(new DndEnumValue<Weapons>());
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
				if (typeof(DndVariable).IsAssignableFrom(type.BaseType) && !type.Name.StartsWith("DndEnumValue"))
					AddVariable((DndVariable)Activator.CreateInstance(type));
			}
			AddEnumVariables();
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

		public static bool IsUpdating
		{
			get
			{
				return callDepth > 0;
			}
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
		public static List<DndFunction> GetFunctionsStartingWith(string tokenLeftOfCaret)
		{
			if (string.IsNullOrWhiteSpace(tokenLeftOfCaret))
				return functions.ToList();
			string lower = tokenLeftOfCaret.ToLower();
			return functions.Where(x => x.Name != null && x.Name.ToLower().StartsWith(lower)).ToList();
		}
		public static List<DndVariable> GetVariablesStartingWith(string tokenLeftOfCaret)
		{
			if (string.IsNullOrWhiteSpace(tokenLeftOfCaret))
				return variables.ToList();
			string lower = tokenLeftOfCaret.ToLower();
			return variables.Where(x => x.Name != null && x.Name.ToLower().StartsWith(lower)).ToList();
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

