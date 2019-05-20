using System;
using System.Linq;
using System.Collections.ObjectModel;
using TimeLineControl;

namespace DndCore
{
	public class EffectTimeLines : ListEntry
	{

		public EffectTimeLines()
		{

		}

		public ObservableCollection<TimeLineEffect> Entries { get; set; } = new ObservableCollection<TimeLineEffect>();
	}
}
