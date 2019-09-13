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

		public PlayerStateEventArgs(Character player, string key, object oldValue, object newValue) : base(key, oldValue, newValue)
		{
			Player = player;
		}
	}
}
