using System;
using System.Collections.ObjectModel;
using System.Linq;
using TimeLineControl;

namespace DndUI
{
	public class EffectTimeLines : ListEntry
	{
		private ObservableCollection<TimeLineEffect> entries = new ObservableCollection<TimeLineEffect>();
		public EffectTimeLines()
		{

		}

		public ObservableCollection<TimeLineEffect> Entries { get => entries; set => entries = value; }
	}
}
