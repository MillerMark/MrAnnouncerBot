using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace TaleSpireCore
{
	public static class PropertyChangerManager
	{
		static Dictionary<Type, BasePropertyChanger> knownPropertyChangers = new Dictionary<Type, BasePropertyChanger>();
		
		static PropertyChangerManager()
		{
			Initialize();
		}
		
		static void Initialize()
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly assembly in assemblies)
			{
				Type[] types = assembly.GetTypes();
				foreach (Type type in types)
				{
					if (typeof(BasePropertyChanger).IsAssignableFrom(type) && type.Name != nameof(BasePropertyChanger))
					{
						Talespire.Log.Warning($"Registering {type.Name}...");
						PropertyTypeAttribute propertyTypeAttribute = type.GetCustomAttribute<PropertyTypeAttribute>(false);
						if (propertyTypeAttribute != null)
						{
							Type key = propertyTypeAttribute.PropertyType;
							if (knownPropertyChangers.ContainsKey(key))
								Talespire.Log.Error($"Error - knownPropertyChangers already contains an entry for {key.Name}!!!");

							knownPropertyChangers[key] = Activator.CreateInstance(type) as BasePropertyChanger;
							if (knownPropertyChangers[key] == null)
								Talespire.Log.Error($"knownPropertyChangers[{key.Name}] == null!!!");
						}
					}
				}
			}
		}

		public static BasePropertyChanger GetPropertyChanger(Type type)
		{
			if (!knownPropertyChangers.ContainsKey(type))
				Talespire.Log.Error($"!knownPropertyChangers.ContainsKey(type)");
			return knownPropertyChangers[type];
		}
	}
}
