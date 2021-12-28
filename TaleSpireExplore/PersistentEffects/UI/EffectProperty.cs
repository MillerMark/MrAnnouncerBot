using System;
using System.Linq;
using System.Collections.Generic;
using TaleSpireCore;

namespace TaleSpireExplore
{
	public class EffectProperty
	{
		/// <summary>
		/// Creates a new EffectProperty.
		/// </summary>
		/// <param name="name">The name of the property.</param>
		/// <param name="type">The type of the property.</param>
		/// <param name="paths">The path(s) to the property (to get and set values). 
		/// Separate multiple paths with a semicolon (";").</param>
		public EffectProperty(string name, Type type, string paths)
		{
			Talespire.Log.Debug($"new EffectProperty (Name = \"{name}\", Type = \"{type}\", Path = \"{paths}\")");
			Name = name;
			Type = type;
			Paths = paths;
		}

		/// <summary>
		/// The complete path (or multiple semicolon-separated complete paths) to the properties to change.
		/// </summary>
		public string Paths { get; set; }

		/// <summary>
		/// The type of the property to change.
		/// </summary>
		public Type Type { get; set; }

		/// <summary>
		/// The name of the property (displayed in UI and also used to store and read the property values).
		/// </summary>
		public string Name { get; set; }


		public override string ToString()
		{
			return Name;
		}
	}
}
