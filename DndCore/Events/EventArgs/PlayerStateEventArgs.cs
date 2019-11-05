using System;
using System.Linq;
using System.Collections.Generic;

namespace DndCore
{
	public class PlayerStateEventArgs : StateChangedEventArgs
	{
		public Character Player { get; private set; }
		public PlayerStateEventArgs()
		{

		}
		public PlayerStateEventArgs(Character player, string key, object oldValue, object newValue, bool isRechargeable) : base(key, oldValue, newValue, isRechargeable)
		{
			Player = player;
		}

		public PlayerStateEventArgs(Character player, StateChangedEventArgs ea)
		{
			Player = player;
			AddList(ea.ChangeList);
		}

		void AddList(List<StateChangedData> changeList)
		{
			foreach (StateChangedData stateChangedData in changeList)
				Add(stateChangedData.Key, stateChangedData.OldValue, stateChangedData.NewValue, stateChangedData.IsRechargeable);
		}
	}
}
