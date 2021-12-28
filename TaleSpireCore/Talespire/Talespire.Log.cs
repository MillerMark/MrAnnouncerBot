using BepInEx.Logging;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace TaleSpireCore
{
	public static partial class Talespire
	{
		public static class Log
		{
			static ManualLogSource manualLogSource;

			public static string TotalIndent
			{
				get
				{
					return new string(' ', numSpaces);
				}
			}
			

			static Log()
			{
				manualLogSource = Logger.CreateLogSource("mm");
			}

			public static void Info(object data)
			{
				manualLogSource.Log(LogLevel.Info, TotalIndent + data);
			}

			public static void Error(object data)
			{
				manualLogSource.Log(LogLevel.Error, TotalIndent + data);
			}

			public static void Warning(object data)
			{
				manualLogSource.Log(LogLevel.Warning, TotalIndent + data);
			}

			public static void Debug(object data)
			{
				manualLogSource.Log(LogLevel.Debug, TotalIndent + data);
			}

			public static void Message(object data)
			{
				manualLogSource.Log(LogLevel.Message, TotalIndent + data);
			}

			public static void Exception(Exception ex, [CallerMemberName] string callerName = "")
			{
				string prefix = "";
				string indent = "";
				string suffix = $" in {callerName}";
				while (ex != null)
				{
					manualLogSource.Log(LogLevel.Error, TotalIndent + $"{indent}{prefix}{ex.GetType().Name}{suffix} - \"{ex.Message}\"");
					manualLogSource.Log(LogLevel.Warning, TotalIndent + ex.StackTrace);
					ex = ex.InnerException;
					prefix = "Inner: ";
					suffix = "";
					indent += "  ";
				}
			}

			public static void Vector(string message, UnityEngine.Vector3 vector)
			{
				Debug($"{message} = ({vector.x:N}, {vector.y:N}, {vector.z:N})");
			}

			static Dictionary<string, string> logValueHistory = new Dictionary<string, string>();
			
			public static void ChangeOnly(string label, string value)
			{
				if (logValueHistory.ContainsKey(label))
				{
					if (logValueHistory[label] == value)
						return;
					logValueHistory[label] = value;
				}
				else
					logValueHistory.Add(label, value);
				Warning($"{label}: {value}");
			}

			static object lockHierarchy = new object();
			static List<UnityEngine.GameObject> allChildrenLogged = new List<UnityEngine.GameObject>();

				public static void Hierarchy(UnityEngine.GameObject gameObject, string indent = "")
			{
				if (gameObject == null)
				{
					Error("gameObject is null. Unable to show hierarchy!");
					return;
				}

				lock (lockHierarchy)
				{
					Debug($"Hierarchy:");
					ShowHierarchy(gameObject, indent);
					Debug($"---");
					allChildrenLogged.Clear();
				}
			}

			private static void ShowHierarchy(UnityEngine.GameObject gameObject, string indent)
			{
				if (allChildrenLogged.Contains(gameObject))
					return;

				allChildrenLogged.Add(gameObject);

				Warning(indent + gameObject);
				indent += "  ";
				int childCount = gameObject.transform.childCount;
				for (int i = 0; i < childCount; i++)
				{
					UnityEngine.Transform child = gameObject.transform.GetChild(i);
					if (child.gameObject != null)
						ShowHierarchy(child.gameObject, indent);
				}
			}
			private const int indentSize = 2;
			static int numSpaces;

			public static void Indent([CallerMemberName] string label = "")
			{
				Warning(label + " {");
				numSpaces += indentSize;
			}

			public static void Unindent([CallerMemberName] string label = "")
			{
				numSpaces -= indentSize;
				if (numSpaces < 0)
					numSpaces = 0;
				Debug($"}}  // {label}");
			}
		}
	}
}