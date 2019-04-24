using System;
using System.Linq;

namespace DndCore
{
	public enum TimeMeasure
	{
		instant = 0,
		actions = 1,
		seconds = 2,
		minutes = 3,
		hours = 4,
		days = 5,
		forever = 6,
		never = 7,
		round = 8
	}
}
