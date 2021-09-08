using System;
using System.Linq;
using System.Collections.Generic;

namespace TaleSpireExplore
{
	public static class ValueEditors
	{
		static Dictionary<string, AllValueEditors> allEditors = new Dictionary<string, AllValueEditors>();

		public static event ValueChangedEventHandler ValueChanged;

		static void OnValueChanged(object sender, ValueChangedEventArgs ea)
		{
			ValueChanged?.Invoke(sender, ea);
		}

		static ValueEditors()
		{
		}

		private static void AllEditors_ValueChanged(object sender, ValueChangedEventArgs ea)
		{
			OnValueChanged(sender, ea);
		}

		public static void Clean(string key)
		{
			if (allEditors.ContainsKey(key))
			{
				allEditors[key].ValueChanged -= AllEditors_ValueChanged;
				allEditors.Remove(key);
			}
		}

		public static void Register(string key)
		{
			if (!allEditors.ContainsKey(key))
			{
				AllValueEditors value = new AllValueEditors();
				allEditors.Add(key, value);
				value.ValueChanged += AllEditors_ValueChanged;
				value.Key = key;
			}

			AllValueEditors allValueEditors = allEditors[key];

			allValueEditors.Register(new EdtFloat());
			allValueEditors.Register(new EdtEnum());
			allValueEditors.Register(new EdtInt());
			allValueEditors.Register(new EdtNGuid());
			allValueEditors.Register(new EdtMaterial());
			allValueEditors.Register(new EdtMesh());
			allValueEditors.Register(new EdtBool());
			allValueEditors.Register(new EdtString());
			allValueEditors.Register(new EdtColor());
			allValueEditors.Register(new EdtVector3());
			allValueEditors.Register(new EdtMinMaxGradient());
		}

		public static List<IValueEditor> GetAll(string key)
		{
			return allEditors[key].Editors.Values.ToList();
		}

		public static IValueEditor Get(string key, Type type)
		{
			return allEditors[key].Get(type);
		}

	}
}
