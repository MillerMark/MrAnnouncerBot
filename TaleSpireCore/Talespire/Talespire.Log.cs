using BepInEx.Logging;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace TaleSpireCore
{
	public static partial class Talespire
	{
		public static class Log
		{
			static ManualLogSource manualLogSource;

			static Log()
			{
				manualLogSource = Logger.CreateLogSource("Mark");
			}

			public static void Info(object data)
			{
				manualLogSource.Log(LogLevel.Info, data);
			}

			public static void Error(object data)
			{
				manualLogSource.Log(LogLevel.Error, data);
			}

			public static void Warning(object data)
			{
				manualLogSource.Log(LogLevel.Warning, data);
			}

			public static void Debug(object data)
			{
				manualLogSource.Log(LogLevel.Debug, data);
			}

			public static void Message(object data)
			{
				manualLogSource.Log(LogLevel.Message, data);
			}

			public static void Exception(Exception ex, [CallerMemberName] string callerName = "")
			{
				string prefix = "";
				string indent = "";
				string suffix = $" in {callerName}";
				while (ex != null)
				{
					manualLogSource.Log(LogLevel.Error, $"{indent}{prefix}{ex.GetType().Name}{suffix} - \"{ex.Message}\"");
					manualLogSource.Log(LogLevel.Warning, ex.StackTrace);
					ex = ex.InnerException;
					prefix = "Inner: ";
					suffix = "";
					indent += "  ";
				}
			}
		}
	}
}