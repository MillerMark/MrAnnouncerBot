using System;
using System.Linq;
using System.Collections.Generic;

namespace DndCore
{
	public abstract class Effect
	{
		public EffectKind effectKind;
		public int timeOffsetMs = 0;

		public Effect()
		{

		}
	}
}
