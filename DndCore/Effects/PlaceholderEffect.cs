using System;
using System.Linq;

namespace DndCore
{
	public class PlaceholderEffect : Effect
	{
		public PlaceholderType PlaceholderType;
		public string Name;

		public PlaceholderEffect()
		{
			effectKind = EffectKind.Placeholder;
		}

		public PlaceholderEffect(string name, PlaceholderType type) : this()
		{
			Name = name;
			PlaceholderType = type;
		}
	}
}
