using System;
using System.Linq;
using System.Collections.Generic;

namespace DndCore.Effects
{
	public class EffectGroup : Effect
	{
		public List<Effect> effects = new List<Effect>();

		public EffectGroup()
		{
			effectKind = EffectKind.GroupEffect;
		}

		public int effectsCount
		{
			get
			{
				return effects.Count;
			}
		}

		public void Add(Effect effect)
		{
			effects.Add(effect);
		}
	}
}
