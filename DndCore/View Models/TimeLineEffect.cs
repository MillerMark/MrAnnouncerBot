using System;
using System.Linq;
using DndCore;
using TimeLineControl;

namespace DndCore
{
	public class TimeLineEffect : TimeLineEntry
	{

		public TimeLineEffect()
		{

		}

		public EffectEntry Effect
		{
			get
			{
				return Data as EffectEntry;
			}
			set
			{
				Data = value;
			}
		}
	}
}
