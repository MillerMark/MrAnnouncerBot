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
				try
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
				catch (ReflectionTypeLoadException ex)
				{
					Talespire.Log.Exception(ex);
					Talespire.Log.Indent("LoaderExceptions: ");
					foreach (Exception exception in ex.LoaderExceptions)
					{
						
					}
					Talespire.Log.Unindent("LoaderExceptions");
				}
			}
		}

		public static BasePropertyChanger GetPropertyChanger(Type type)
		{
			if (type == null)
				return null;

			if (type.IsEnum)
				type = typeof(Enum);

			if (!knownPropertyChangers.ContainsKey(type))
			{
				Talespire.Log.Error($"knownPropertyChangers does not Contain Key of type {type}.");
				return null;
			}

			return knownPropertyChangers[type];
		}
	}
}
