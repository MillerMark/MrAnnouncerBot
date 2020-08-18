using System;
using System.Linq;

namespace DndCore
{
	public class CreatureDamagedEventArgs : EventArgs
	{
		public CreatureDamagedEventArgs(Creature creature, double damageAmount)
		{
			Creature = creature;
			DamageAmount = damageAmount;
		}
		public double DamageAmount { get; set; }
		public Creature Creature { get; set; }
	}
}
