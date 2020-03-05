using System;
using System.Linq;
using System.Collections.Generic;
using MapCore;

namespace DndMapSpike
{
	public static class Known
	{
		public static List<Type> Interfaces = new List<Type>();
		static Known()
		{
			Interfaces.Add(typeof(IArrangeable));
			Interfaces.Add(typeof(IModifiableColor));
			Interfaces.Add(typeof(IScalable));
		}
	}
}

