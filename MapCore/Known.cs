using System;
using System.Linq;
using System.Collections.Generic;
using MapCore;
using System.Reflection;

namespace MapCore
{
	/// <summary>
	/// Holds a list of all known interfaces with the [Feature] attribute.
	/// </summary>
	public static class Known
	{
		public static List<Type> Interfaces = new List<Type>();
		static Known()
		{
			Type[] types = typeof(Known).Assembly.GetTypes();
			foreach (Type type in types)
				if (type.GetCustomAttributes<FeatureAttribute>().Any())
					Interfaces.Add(type);
		}
	}
}

