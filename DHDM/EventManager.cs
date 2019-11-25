using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using DndCore;

namespace DHDM
{
	public static class EventManager
	{
		static EventManager()
		{
			LoadEvents();
		}

		public static Dictionary<Type, List<string>> knownEvents = new Dictionary<Type, List<string>>();
		static void AddEvent(Type type, string name)
		{
			if (!knownEvents.ContainsKey(type))
				knownEvents.Add(type, new List<string>());
			knownEvents[type].Add(name);
		}

		public static void LoadEvents()
		{
			foreach (Type type in typeof(Feature).Assembly.GetTypes())
			{
				if (type.GetCustomAttributes(typeof(HasDndEventsAttribute), true).Length > 0)
				{
					PropertyInfo[] properties = type.GetProperties();
					foreach (var propertyName in properties.Where(propertyInfo => propertyInfo.GetCustomAttributes(typeof(DndEventAttribute), true).Length > 0).Select(propertyInfo => propertyInfo.Name))
						AddEvent(type, propertyName);
				}
			}
		}
	}
}
