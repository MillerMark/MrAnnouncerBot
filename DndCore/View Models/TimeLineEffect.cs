using System;
using System.Linq;
using TimeLineControl;

namespace DndCore.ViewModels
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
