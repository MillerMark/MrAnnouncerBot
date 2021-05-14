using BepInEx.Logging;
using System;
using System.Linq;

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
				manualLogSource.Log(BepInEx.Logging.LogLevel.Info, data);
			}

			public static void Error(object data)
			{
				manualLogSource.Log(BepInEx.Logging.LogLevel.Error, data);
			}

			public static void Warning(object data)
			{
				manualLogSource.Log(BepInEx.Logging.LogLevel.Warning, data);
			}

			public static void Debug(object data)
			{
				manualLogSource.Log(BepInEx.Logging.LogLevel.Debug, data);
			}

			public static void Message(object data)
			{
				manualLogSource.Log(BepInEx.Logging.LogLevel.Message, data);
			}
		}
	}
}