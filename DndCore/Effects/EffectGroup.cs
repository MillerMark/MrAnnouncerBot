using System;
using System.Collections.Generic;
using System.Linq;

namespace DndCore
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
