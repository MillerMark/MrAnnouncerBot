using System;
using System.Linq;

namespace DndCore
{
	public class PlayerRollRequestEventArgs : RollDiceEventArgs
	{
		public Character Player { get; private set; }

		public PlayerRollRequestEventArgs(Character player, string dieRollStr) : base(dieRollStr)
		{
			Player = player;
		}
	}
}
