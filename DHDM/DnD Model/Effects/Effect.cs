using System;
using System.Linq;

namespace DHDM
{
	public abstract class Effect
	{
		public int timeOffsetMs = 0;
		public EffectKind effectKind;

		public Effect()
		{

		}
	}
}
