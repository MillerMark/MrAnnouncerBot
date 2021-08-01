using System;
using System.Linq;

namespace DndCore
{
	public class CreatureHealthChangedEventArgs : EventArgs
	{
		public Creature Creature { get; set; }
		public double PercentChanged { get; set; }
		public CreatureHealthChangedEventArgs(Creature creature, double percentChanged)
		{
			Creature = creature;
			PercentChanged = percentChanged;
		}
		public CreatureHealthChangedEventArgs()
		{

		}
	}
}
