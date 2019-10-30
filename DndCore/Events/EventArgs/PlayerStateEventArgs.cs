using System;
using System.Linq;

namespace DndCore
{
	public class PlayerStateEventArgs : StateChangedEventArgs
	{
		public Character Player { get; private set; }
		public PlayerStateEventArgs()
		{

		}
		public bool IsRechargeable { get; set; }
		public PlayerStateEventArgs(Character player, string key, object oldValue, object newValue, bool isRechargeable) : base(key, oldValue, newValue)
		{
			IsRechargeable = isRechargeable;
			Player = player;
		}
	}
}
