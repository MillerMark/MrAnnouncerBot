using System;
using System.Linq;

namespace TaleSpireExplore
{
	public class EffectProperty
	{
		public EffectProperty(string name, Type type, string path)
		{
			Name = name;
			Type = type;
			Path = path;
		}
		public string Path { get; set; }
		public Type Type { get; set; }
		public string Name { get; set; }
		public override string ToString()
		{
			return Name;
		}
	}
}
