using System;
using System.Linq;
using System.Collections.ObjectModel;
using DndCore.ViewModels.Support;
using TimeLineControl;

namespace DndCore.ViewModels
{
	public class EffectTimeLines : ListEntry
	{

		public EffectTimeLines()
		{

		}

		public ObservableCollection<TimeLineEffect> Entries { get; set; } = new ObservableCollection<TimeLineEffect>();
	}
}
