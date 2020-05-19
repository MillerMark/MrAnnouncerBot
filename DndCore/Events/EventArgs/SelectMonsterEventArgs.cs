using System;

namespace DndCore
{
	public class SelectMonsterEventArgs : EventArgs
	{
		public Monster Monster { get; set; }
		public Character Player { get; private set; }
		public double MaxChallengeRating { get; set; } = double.MaxValue;
		public SelectMonsterEventArgs(Character player)
		{
			Player = player;
		}
	}
}
