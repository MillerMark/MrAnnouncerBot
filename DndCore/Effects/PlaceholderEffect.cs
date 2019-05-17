using System;
using System.Linq;

namespace DndCore.Effects
{
	public class PlaceholderEffect : Effect
	{
		public string Name;
		public PlaceholderType PlaceholderType;

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
