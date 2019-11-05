using System;
using System.Linq;
using System.Collections.Generic;

namespace DndCore
{
	// A state-changed event that can report on multiple states changed at once.
	public class StateChangedEventArgs : EventArgs
	{
		public string Key
		{
			get
			{
				if (changeList.Count > 0)
					return changeList[0].Key;
				return null;
			}
		}

		public object OldValue
		{
			get
			{
				if (changeList.Count > 0)
					return changeList[0].OldValue;
				return null;
			}
		}

		public object NewValue
		{
			get
			{
				if (changeList.Count > 0)
					return changeList[0].NewValue;
				return null;
			}
		}

		public bool IsRechargeable
		{
			get
			{
				if (changeList.Count > 0)
					return changeList[0].IsRechargeable;
				return false;
			}
		}

		public bool Contains(string key)
		{
			return GetStateChange(key) != null;
		}

		public StateChangedData GetStateChange(string key)
		{
			return ChangeList.FirstOrDefault(x => x.Key == key);
		}

		public List<StateChangedData> ChangeList { get => changeList; private set => changeList = value; }

		List<StateChangedData> changeList = new List<StateChangedData>();

		public StateChangedEventArgs(string key, object oldValue, object newValue, bool isRechargeable = false)
		{
			Add(key, oldValue, newValue, isRechargeable);
		}

		public void Add(string key, object oldValue, object newValue, bool isRechargeable = false)
		{
			changeList.Add(new StateChangedData(key, oldValue, newValue, isRechargeable));
		}

		public StateChangedEventArgs()
		{

		}
	}
}
