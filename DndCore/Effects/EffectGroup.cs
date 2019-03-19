using System;
using System.Collections.Generic;
using System.Linq;

namespace DndCore
{
	public class EffectGroup : Effect
	{
		public List<Effect> effects = new List<Effect>();
		public int effectsCount
		{
			get
			{
				return effects.Count;
			}
		}

		public EffectGroup()
		{
			effectKind = EffectKind.GroupEffect;
		}
		public void Add(Effect effect)
		{
			effects.Add(effect);
		}
	}
}
