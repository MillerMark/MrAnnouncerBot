using System;
using System.Linq;

namespace DndCore
{
	public class StateChangedEventArgs : EventArgs
	{
		public string Key { get; private set; }
		public object OldValue { get; private set; }
		public object NewValue { get; private set; }
		public bool IsRechargeable { get; set; }

		public StateChangedEventArgs(string key, object oldValue, object newValue, bool isRechargeable = false)
		{
			IsRechargeable = isRechargeable;
			Key = key;
			OldValue = oldValue;
			NewValue = newValue;
		}
		public StateChangedEventArgs()
		{

		}
	}
}
