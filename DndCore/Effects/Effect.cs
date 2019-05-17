using System;
using System.Collections.Generic;
using System.Linq;

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
