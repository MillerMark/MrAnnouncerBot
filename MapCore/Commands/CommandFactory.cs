using System;
using System.Linq;
using System.Reflection;

namespace MapCore
{
	public static class CommandFactory
	{
		const string commandSuffix = "Command";

		public static BaseCommand Create(string commandType, object data)
		{
			if (!commandType.EndsWith(commandSuffix))
				commandType += commandSuffix;
			BaseCommand command = (BaseCommand)(Activator.CreateInstance(null, $"MapCore.{commandType}").Unwrap());
			if (command == null)
				return null;
			command.Data = data;
			return command;
		}

		public static BaseCommand Create<T>(string genericCommandType, object data)
		{
			if (!genericCommandType.EndsWith(commandSuffix))
				genericCommandType += commandSuffix;

			Type genericType = typeof(CommandFactory).Assembly.GetType($"{nameof(MapCore)}.{genericCommandType}`1");
			Type constructedType = genericType.MakeGenericType(typeof(T));
			
			BaseCommand command = (BaseCommand)(Activator.CreateInstance(constructedType, new object[] { }));
			if (command == null)
				return null;
			command.Data = data;
			return command;
		}
	}
}
