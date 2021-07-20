using System;
using System.Linq;
using System.Collections.Generic;

namespace DndCore
{
	public class ListStateChangedEventArgs : StateChangedEventArgs
	{
		public ListStateChangedEventArgs()
		{

		}
		public ListStateChangedEventArgs(string key, object oldValue, object newValue, bool isRechargeable): base(key, oldValue, newValue, isRechargeable)
		{

		}
		protected void AddList(List<StateChangedData> changeList)
		{
			foreach (StateChangedData stateChangedData in changeList)
				Add(stateChangedData.Key, stateChangedData.OldValue, stateChangedData.NewValue, stateChangedData.IsRechargeable);
		}
	}
}
