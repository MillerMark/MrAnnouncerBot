using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using TaleSpireCore;

namespace TaleSpireExplore
{
	public static class TypeLookup
	{
		static Dictionary<string, Type> typeCache = new Dictionary<string, Type>();

		static TypeLookup()
		{
			typeCache["Color"] = typeof(UnityEngine.Color);
			typeCache["Float"] = typeof(float);
			typeCache["Vector3"] = typeof(UnityEngine.Vector3);
			typeCache["String"] = typeof(string);
			typeCache["Bool"] = typeof(bool);
		}

		public static Type GetType(string typeName)
		{
			if (typeCache.ContainsKey(typeName))
				return typeCache[typeName];
			
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				Type type = assembly.GetType(typeName);
				if (type != null)
				{
					typeCache[typeName] = type;
					return type;
				}
			}
			Talespire.Log.Error($"Type \"{typeName}\" not found!");
			return null;
		}
	}
}
