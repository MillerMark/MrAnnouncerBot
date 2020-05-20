using System;

namespace DndCore
{
	public class SelectMonsterEventArgs : EventArgs
	{
		public Monster Monster { get; set; }
		public Character Player { get; private set; }
		public int MaxSwimmingSpeed { get; set; } = int.MaxValue;
		public int MaxFlyingSpeed { get; set; } = int.MaxValue;
		public double MaxChallengeRating { get; set; } = double.MaxValue;
		public CreatureKinds CreatureKindFilter { get; set; } = CreatureKinds.AllCreatures;
		public SelectMonsterEventArgs(Character player)
		{
			Player = player;
		}
	}
}
