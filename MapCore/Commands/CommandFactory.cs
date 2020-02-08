using System;
using System.Linq;

namespace MapCore
{
	public static class CommandFactory
	{

		public static BaseCommand Create(string commandType, object data)
		{
			const string commandSuffix = "Command";
			if (!commandType.EndsWith(commandSuffix))
				commandType += commandSuffix;
			BaseCommand command = (BaseCommand)(Activator.CreateInstance(null, $"MapCore.{commandType}").Unwrap());
			if (command == null)
				return null;
			command.Data = data;
			return command;
		}
	}
}
