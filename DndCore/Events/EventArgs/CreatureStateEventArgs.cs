using System;
using System.Linq;

namespace DndCore
{
	public class CreatureStateEventArgs : ListStateChangedEventArgs
	{
		public Creature Creature { get; private set; }
		public CreatureStateEventArgs()
		{

		}
		public CreatureStateEventArgs(Creature creature, string key, object oldValue, object newValue, bool isRechargeable) : base(key, oldValue, newValue, isRechargeable)
		{
			Creature = creature;
		}

		public CreatureStateEventArgs(Creature creature, StateChangedEventArgs ea)
		{
			Creature = creature;
			AddList(ea.ChangeList);
		}
	}
}
